// Toony Colors Pro+Mobile 2
// (c) 2014-2021 Jean Moreno

// Terrain BasePass shader:
// This shader is used when the terrain is viewed from the "Base Distance" setting.
// It uses low resolution generated textures from the "BaseGen" shader to draw the terrain entirely,
// thus preventing to perform the full splat map blending code to increase performances.

Shader "Hidden/Toony Colors Pro 2/User/Terrain-BasePass"
{
	Properties
	{
		[TCP2HeaderHelp(Base)]
		_BaseColor ("Color", Color) = (1,1,1,1)
		[TCP2ColorNoAlpha] _HColor ("Highlight Color", Color) = (0.75,0.75,0.75,1)
		[TCP2ColorNoAlpha] _SColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
		[TCP2Separator]

		[TCP2Header(Ramp Shading)]
		
		_RampThreshold ("Threshold", Range(0.01,1)) = 0.5
		_RampSmoothing ("Smoothing", Range(0.001,1)) = 0.5
		[IntRange] _BandsCount ("Bands Count", Range(1,20)) = 4
		_BandsSmoothing ("Bands Smoothing", Range(0.001,1)) = 0.1
		[TCP2Separator]
		[TCP2HeaderHelp(Terrain)]
		_HeightTransition ("Height Smoothing", Range(0, 1.0)) = 0.0
		_Layer0HeightOffset ("Layer 0 Height Offset", Range(-1,1)) = 0
		_Layer1HeightOffset ("Layer 1 Height Offset", Range(-1,1)) = 0
		_Layer2HeightOffset ("Layer 2 Height Offset", Range(-1,1)) = 0
		_Layer3HeightOffset ("Layer 3 Height Offset", Range(-1,1)) = 0
		_Layer4HeightOffset ("Layer 4 Height Offset", Range(-1,1)) = 0
		_Layer5HeightOffset ("Layer 5 Height Offset", Range(-1,1)) = 0
		_Layer6HeightOffset ("Layer 6 Height Offset", Range(-1,1)) = 0
		_Layer7HeightOffset ("Layer 7 Height Offset", Range(-1,1)) = 0
		[HideInInspector] TerrainMeta_maskMapTexture ("Mask Map", 2D) = "white" {}
		[Toggle(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)] _EnableInstancedPerPixelNormal("Enable Instanced per-pixel normal", Float) = 1.0
		[TCP2Separator]
		
		[HideInInspector] _Splat0 ("Layer 0 Albedo", 2D) = "gray" {}
		[HideInInspector] _Splat1 ("Layer 1 Albedo", 2D) = "gray" {}
		[HideInInspector] _Splat2 ("Layer 2 Albedo", 2D) = "gray" {}
		[HideInInspector] _Splat3 ("Layer 3 Albedo", 2D) = "gray" {}
		[HideInInspector] _Splat4 ("Layer 4 Albedo", 2D) = "gray" {}
		[HideInInspector] _Splat5 ("Layer 5 Albedo", 2D) = "gray" {}
		[HideInInspector] _Splat6 ("Layer 6 Albedo", 2D) = "gray" {}
		[HideInInspector] _Splat7 ("Layer 7 Albedo", 2D) = "gray" {}
		[HideInInspector] [NoScaleOffset] _Mask0 ("Layer 0 Mask", 2D) = "gray" {}
		[HideInInspector] [NoScaleOffset] _Mask1 ("Layer 1 Mask", 2D) = "gray" {}
		[HideInInspector] [NoScaleOffset] _Mask2 ("Layer 2 Mask", 2D) = "gray" {}
		[HideInInspector] [NoScaleOffset] _Mask3 ("Layer 3 Mask", 2D) = "gray" {}
		[HideInInspector] [NoScaleOffset] _Mask4 ("Layer 4 Mask", 2D) = "gray" {}
		[HideInInspector] [NoScaleOffset] _Mask5 ("Layer 5 Mask", 2D) = "gray" {}
		[HideInInspector] [NoScaleOffset] _Mask6 ("Layer 6 Mask", 2D) = "gray" {}
		[HideInInspector] [NoScaleOffset] _Mask7 ("Layer 7 Mask", 2D) = "gray" {}

		[ToggleOff(_RECEIVE_SHADOWS_OFF)] _ReceiveShadowsOff ("Receive Shadows", Float) = 1

		// Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
			"RenderType" = "Opaque"
			"Queue"="Geometry-100"
			"TerrainCompatible"="True"
			"SplatCount"="8"
		}

		HLSLINCLUDE
		#define fixed half
		#define fixed2 half2
		#define fixed3 half3
		#define fixed4 half4

		#if UNITY_VERSION >= 202020
			#define URP_10_OR_NEWER
		#endif

		// Texture/Sampler abstraction
		#define TCP2_TEX2D_WITH_SAMPLER(tex)						TEXTURE2D(tex); SAMPLER(sampler##tex)
		#define TCP2_TEX2D_NO_SAMPLER(tex)							TEXTURE2D(tex)
		#define TCP2_TEX2D_SAMPLE(tex, samplertex, coord)			SAMPLE_TEXTURE2D(tex, sampler##samplertex, coord)
		#define TCP2_TEX2D_SAMPLE_LOD(tex, samplertex, coord, lod)	SAMPLE_TEXTURE2D_LOD(tex, sampler##samplertex, coord, lod)

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

		// Terrain
		#define TERRAIN_BASE_PASS

		// Uniforms

		// Shader Properties
		TCP2_TEX2D_WITH_SAMPLER(_Splat0);
		TCP2_TEX2D_NO_SAMPLER(_Splat1);
		TCP2_TEX2D_NO_SAMPLER(_Splat2);
		TCP2_TEX2D_NO_SAMPLER(_Splat3);
		TCP2_TEX2D_WITH_SAMPLER(_Splat4);
		TCP2_TEX2D_NO_SAMPLER(_Splat5);
		TCP2_TEX2D_NO_SAMPLER(_Splat6);
		TCP2_TEX2D_NO_SAMPLER(_Splat7);
		TCP2_TEX2D_WITH_SAMPLER(_Mask0);
		TCP2_TEX2D_NO_SAMPLER(_Mask1);
		TCP2_TEX2D_NO_SAMPLER(_Mask2);
		TCP2_TEX2D_NO_SAMPLER(_Mask3);
		TCP2_TEX2D_WITH_SAMPLER(_Mask4);
		TCP2_TEX2D_NO_SAMPLER(_Mask5);
		TCP2_TEX2D_NO_SAMPLER(_Mask6);
		TCP2_TEX2D_NO_SAMPLER(_Mask7);

		CBUFFER_START(UnityPerMaterial)
			
			// Shader Properties
			float _Layer0HeightOffset;
			float _Layer1HeightOffset;
			float _Layer2HeightOffset;
			float _Layer3HeightOffset;
			float _Layer4HeightOffset;
			float _Layer5HeightOffset;
			float _Layer6HeightOffset;
			float _Layer7HeightOffset;
			float4 _Splat0_ST;
			float4 _Splat1_ST;
			float4 _Splat2_ST;
			float4 _Splat3_ST;
			float4 _Splat4_ST;
			float4 _Splat5_ST;
			float4 _Splat6_ST;
			float4 _Splat7_ST;
			fixed4 _BaseColor;
			float _RampThreshold;
			float _RampSmoothing;
			float _BandsCount;
			float _BandsSmoothing;
			fixed4 _SColor;
			fixed4 _HColor;
		CBUFFER_END

		// Built-in renderer (CG) to SRP (HLSL) bindings
		#define UnityObjectToClipPos TransformObjectToHClip
		#define _WorldSpaceLightPos0 _MainLightPosition
		
		ENDHLSL

		Pass
		{
			Name "Main"
			Tags
			{
				"LightMode"="UniversalForward"
			}

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard SRP library
			// All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 3.0

			// -------------------------------------
			// Material keywords
			#pragma shader_feature_local _ _RECEIVE_SHADOWS_OFF

			// -------------------------------------
			// Universal Render Pipeline keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK

			// -------------------------------------

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd

			#pragma vertex Vertex
			#pragma fragment Fragment

			//--------------------------------------
			// Toony Colors Pro 2 keywords
			#pragma shader_feature_local _TERRAIN_INSTANCED_PERPIXEL_NORMAL

			// vertex input
			struct Attributes
			{
				float4 vertex       : POSITION;
				float3 normal       : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			// vertex output / fragment input
			struct Varyings
			{
				float4 positionCS     : SV_POSITION;
				float3 normal         : NORMAL;
				float4 worldPosAndFog : TEXCOORD0;
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord    : TEXCOORD1; // compute shadow coord per-vertex for the main light
			#endif
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				half3 vertexLights : TEXCOORD2;
			#endif
				float2 pack0 : TEXCOORD3; /* pack0.xy = texcoord0 */
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

		//--------------------------------------
		// Terrain functions

		//================================================================
		// Terrain Shader specific
		
		//----------------------------------------------------------------
		// Per-layer variables
		
		CBUFFER_START(_Terrain)
			float4 _Control_ST;
			float4 _Control_TexelSize;
			half _HeightTransition;
			half _DiffuseHasAlpha0, _DiffuseHasAlpha1, _DiffuseHasAlpha2, _DiffuseHasAlpha3;
			half _LayerHasMask0, _LayerHasMask1, _LayerHasMask2, _LayerHasMask3;
			// half4 _Splat0_ST, _Splat1_ST, _Splat2_ST, _Splat3_ST;
		
			float4 _Control1_ST;
			float4 _Control1_TexelSize;
			half _DiffuseHasAlpha4, _DiffuseHasAlpha5, _DiffuseHasAlpha6, _DiffuseHasAlpha7;
			half _LayerHasMask4, _LayerHasMask5, _LayerHasMask6, _LayerHasMask7;
			// half4 _Splat4_ST, _Splat5_ST, _Splat6_ST, _Splat7_ST;
		
			#ifdef UNITY_INSTANCING_ENABLED
				float4 _TerrainHeightmapRecipSize;   // float4(1.0f/width, 1.0f/height, 1.0f/(width-1), 1.0f/(height-1))
				float4 _TerrainHeightmapScale;       // float4(hmScale.x, hmScale.y / (float)(kMaxHeight), hmScale.z, 0.0f)
			#endif
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif
		CBUFFER_END
		
		//----------------------------------------------------------------
		// Terrain textures
		
		TCP2_TEX2D_WITH_SAMPLER(_Control);
		TCP2_TEX2D_WITH_SAMPLER(_Control1);
		
		#if defined(TERRAIN_BASE_PASS)
			TCP2_TEX2D_WITH_SAMPLER(_MainTex);
		#endif
		
		//----------------------------------------------------------------
		// Terrain Instancing
		
		#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
			#define ENABLE_TERRAIN_PERPIXEL_NORMAL
		#endif
		
		#ifdef UNITY_INSTANCING_ENABLED
			TCP2_TEX2D_NO_SAMPLER(_TerrainHeightmapTexture);
			TCP2_TEX2D_WITH_SAMPLER(_TerrainNormalmapTexture);
		#endif
		
		UNITY_INSTANCING_BUFFER_START(Terrain)
			UNITY_DEFINE_INSTANCED_PROP(float4, _TerrainPatchInstanceData)  // float4(xBase, yBase, skipScale, ~)
		UNITY_INSTANCING_BUFFER_END(Terrain)
		
		void TerrainInstancing(inout float4 positionOS, inout float3 normal, inout float2 uv)
		{
		#ifdef UNITY_INSTANCING_ENABLED
			float2 patchVertex = positionOS.xy;
			float4 instanceData = UNITY_ACCESS_INSTANCED_PROP(Terrain, _TerrainPatchInstanceData);
		
			float2 sampleCoords = (patchVertex.xy + instanceData.xy) * instanceData.z; // (xy + float2(xBase,yBase)) * skipScale
			float height = UnpackHeightmap(_TerrainHeightmapTexture.Load(int3(sampleCoords, 0)));
		
			positionOS.xz = sampleCoords * _TerrainHeightmapScale.xz;
			positionOS.y = height * _TerrainHeightmapScale.y;
		
			#ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
				normal = float3(0, 1, 0);
			#else
				normal = _TerrainNormalmapTexture.Load(int3(sampleCoords, 0)).rgb * 2 - 1;
			#endif
			uv = sampleCoords * _TerrainHeightmapRecipSize.zw;
		#endif
		}
		
		void TerrainInstancing(inout float4 positionOS, inout float3 normal)
		{
			float2 uv = { 0, 0 };
			TerrainInstancing(positionOS, normal, uv);
		}
		
		//----------------------------------------------------------------
		// Terrain Holes
		
		#if defined(_ALPHATEST_ON)
			TCP2_TEX2D_WITH_SAMPLER(_TerrainHolesTexture);
		
			void ClipHoles(float2 uv)
			{
				float hole = TEX2D_SAMPLE(_TerrainHolesTexture, sampler_TerrainHolesTexture, uv).r;
				clip(hole == 0.0f ? -1 : 1);
			}
		#endif
		
		//----------------------------------------------------------------
		// Height-based blending
		
		void HeightBasedSplatModify_8_Layers(inout half4 splatControl, inout half4 splatControl1, in half4 splatHeight, in half4 splatHeight1)
		{
			// We multiply by the splat Control weights to get combined height
			splatHeight *= splatControl.rgba;
			splatHeight1 *= splatControl1.rgba;
				
			half maxHeight = max(splatHeight.r, max(splatHeight.g, max(splatHeight.b, splatHeight.a)));
			half maxHeight1 = max(splatHeight1.r, max(splatHeight1.g, max(splatHeight1.b, splatHeight1.a)));
			maxHeight = max(maxHeight, maxHeight1);
					
			// Ensure that the transition height is not zero.
			half transition = max(_HeightTransition, 1e-5);
					
			// This sets the highest splat to "transition", and everything else to a lower value relative to that
			// Then we clamp this to zero and normalize everything
			half4 weightedHeights = splatHeight + transition - maxHeight.xxxx;
			weightedHeights = max(0, weightedHeights);
			half4 weightedHeights1 = splatHeight1 + transition - maxHeight.xxxx;
			weightedHeights1 = max(0, weightedHeights1);
		
			// We need to add an epsilon here for active layers (hence the blendMask again)
			// so that at least a layer shows up if everything's too low.
			weightedHeights = (weightedHeights + 1e-6) * splatControl;
			weightedHeights1 = (weightedHeights1 + 1e-6) * splatControl1;
					
			// Normalize (and clamp to epsilon to keep from dividing by zero)
			half sumHeight = max(dot(weightedHeights, half4(1, 1, 1, 1)), 1e-6);
			half sumHeight1 = max(dot(weightedHeights1, half4(1, 1, 1, 1)), 1e-6);
			sumHeight = max(sumHeight, sumHeight1);
			splatControl = weightedHeights / sumHeight.xxxx;
			splatControl1 = weightedHeights1 / sumHeight.xxxx;
		}
		
			Varyings Vertex(Attributes input)
			{
				Varyings output = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				TerrainInstancing(input.vertex, input.normal, input.texcoord0.xy);

				// Texture Coordinates
				output.pack0.xy = input.texcoord0.xy;

				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				output.shadowCoord = GetShadowCoord(vertexInput);
			#endif

				float4 vertexTangent = -float4(cross(float3(0, 0, 1), input.normal), 1.0);
				VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normal, vertexTangent);
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				// Vertex lighting
				output.vertexLights = VertexLighting(vertexInput.positionWS, vertexNormalInput.normalWS);
			#endif

				// world position
				output.worldPosAndFog = float4(vertexInput.positionWS.xyz, 0);

				// normal
				output.normal = normalize(vertexNormalInput.normalWS);

				// clip position
				output.positionCS = vertexInput.positionCS;

				return output;
			}

			half4 Fragment(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float3 positionWS = input.worldPosAndFog.xyz;
				float3 normalWS = normalize(input.normal);

				// Shader Properties Sampling
				float4 __layer0Mask = ( TCP2_TEX2D_SAMPLE(_Mask0, _Mask0, input.pack0.xy * _Splat0_ST.xy + _Splat0_ST.zw).rgba );
				float __layer0HeightSource = ( __layer0Mask.b );
				float __layer0HeightOffset = ( _Layer0HeightOffset );
				float4 __layer1Mask = ( TCP2_TEX2D_SAMPLE(_Mask1, _Mask0, input.pack0.xy * _Splat1_ST.xy + _Splat1_ST.zw).rgba );
				float __layer1HeightSource = ( __layer1Mask.b );
				float __layer1HeightOffset = ( _Layer1HeightOffset );
				float4 __layer2Mask = ( TCP2_TEX2D_SAMPLE(_Mask2, _Mask0, input.pack0.xy * _Splat2_ST.xy + _Splat2_ST.zw).rgba );
				float __layer2HeightSource = ( __layer2Mask.b );
				float __layer2HeightOffset = ( _Layer2HeightOffset );
				float4 __layer3Mask = ( TCP2_TEX2D_SAMPLE(_Mask3, _Mask0, input.pack0.xy * _Splat3_ST.xy + _Splat3_ST.zw).rgba );
				float __layer3HeightSource = ( __layer3Mask.b );
				float __layer3HeightOffset = ( _Layer3HeightOffset );
				float4 __layer4Mask = ( TCP2_TEX2D_SAMPLE(_Mask4, _Mask4, input.pack0.xy * _Splat4_ST.xy + _Splat4_ST.zw).rgba );
				float __layer4HeightSource = ( __layer4Mask.b );
				float __layer4HeightOffset = ( _Layer4HeightOffset );
				float4 __layer5Mask = ( TCP2_TEX2D_SAMPLE(_Mask5, _Mask4, input.pack0.xy * _Splat5_ST.xy + _Splat5_ST.zw).rgba );
				float __layer5HeightSource = ( __layer5Mask.b );
				float __layer5HeightOffset = ( _Layer5HeightOffset );
				float4 __layer6Mask = ( TCP2_TEX2D_SAMPLE(_Mask6, _Mask4, input.pack0.xy * _Splat6_ST.xy + _Splat6_ST.zw).rgba );
				float __layer6HeightSource = ( __layer6Mask.b );
				float __layer6HeightOffset = ( _Layer6HeightOffset );
				float4 __layer7Mask = ( TCP2_TEX2D_SAMPLE(_Mask7, _Mask4, input.pack0.xy * _Splat7_ST.xy + _Splat7_ST.zw).rgba );
				float __layer7HeightSource = ( __layer7Mask.b );
				float __layer7HeightOffset = ( _Layer7HeightOffset );
				float4 __layer0Albedo = ( TCP2_TEX2D_SAMPLE(_Splat0, _Splat0, input.pack0.xy * _Splat0_ST.xy + _Splat0_ST.zw).rgba );
				float4 __layer1Albedo = ( TCP2_TEX2D_SAMPLE(_Splat1, _Splat0, input.pack0.xy * _Splat1_ST.xy + _Splat1_ST.zw).rgba );
				float4 __layer2Albedo = ( TCP2_TEX2D_SAMPLE(_Splat2, _Splat0, input.pack0.xy * _Splat2_ST.xy + _Splat2_ST.zw).rgba );
				float4 __layer3Albedo = ( TCP2_TEX2D_SAMPLE(_Splat3, _Splat0, input.pack0.xy * _Splat3_ST.xy + _Splat3_ST.zw).rgba );
				float4 __layer4Albedo = ( TCP2_TEX2D_SAMPLE(_Splat4, _Splat4, input.pack0.xy * _Splat4_ST.xy + _Splat4_ST.zw).rgba );
				float4 __layer5Albedo = ( TCP2_TEX2D_SAMPLE(_Splat5, _Splat4, input.pack0.xy * _Splat5_ST.xy + _Splat5_ST.zw).rgba );
				float4 __layer6Albedo = ( TCP2_TEX2D_SAMPLE(_Splat6, _Splat4, input.pack0.xy * _Splat6_ST.xy + _Splat6_ST.zw).rgba );
				float4 __layer7Albedo = ( TCP2_TEX2D_SAMPLE(_Splat7, _Splat4, input.pack0.xy * _Splat7_ST.xy + _Splat7_ST.zw).rgba );
				float4 __mainColor = ( _BaseColor.rgba );
				float __ambientIntensity = ( 1.0 );
				float __rampThreshold = ( _RampThreshold );
				float __rampSmoothing = ( _RampSmoothing );
				float __bandsCount = ( _BandsCount );
				float __bandsSmoothing = ( _BandsSmoothing );
				float3 __shadowColor = ( _SColor.rgb );
				float3 __highlightColor = ( _HColor.rgb );

				// Terrain
				
				float2 terrainTexcoord0 = input.pack0.xy.xy;
				
				#if defined(_ALPHATEST_ON)
					ClipHoles(terrainTexcoord0.xy);
				#endif
				
				#if defined(TERRAIN_BASE_PASS)
				
					half4 terrain_mixedDiffuse = TCP2_TEX2D_SAMPLE(_MainTex, _MainTex, terrainTexcoord0.xy).rgba;
					half3 normalTS = half3(0.0h, 0.0h, 1.0h);
				
				#else
				
					// Sample the splat control texture generated by the terrain
					// adjust splat UVs so the edges of the terrain tile lie on pixel centers
					float2 terrainSplatUV = (terrainTexcoord0.xy * (_Control_TexelSize.zw - 1.0f) + 0.5f) * _Control_TexelSize.xy;
					half4 terrain_splat_control_0 = TCP2_TEX2D_SAMPLE(_Control, _Control, terrainSplatUV);
					terrainSplatUV = (terrainTexcoord0.xy * (_Control1_TexelSize.zw - 1.0f) + 0.5f) * _Control1_TexelSize.xy;
					half4 terrain_splat_control_1 = TCP2_TEX2D_SAMPLE(_Control1, _Control1, terrainSplatUV);
					half height0 = __layer0HeightSource + __layer0HeightOffset;
					half height1 = __layer1HeightSource + __layer1HeightOffset;
					half height2 = __layer2HeightSource + __layer2HeightOffset;
					half height3 = __layer3HeightSource + __layer3HeightOffset;
					half height4 = __layer4HeightSource + __layer4HeightOffset;
					half height5 = __layer5HeightSource + __layer5HeightOffset;
					half height6 = __layer6HeightSource + __layer6HeightOffset;
					half height7 = __layer7HeightSource + __layer7HeightOffset;
					HeightBasedSplatModify_8_Layers(terrain_splat_control_0, terrain_splat_control_1, half4(height0, height1, height2, height3), half4(height4, height5, height6, height7));
				
					// Calculate weights and perform the texture blending
					half terrain_weight = dot(terrain_splat_control_0, half4(1,1,1,1));
					half terrain_weight_1 = dot(terrain_splat_control_1, half4(1,1,1,1));
				
					#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
						clip(terrain_weight == 0.0f ? -1 : 1);
						clip(terrain_weight_1 == 0.0f ? -1 : 1);
					#endif
				
					// Normalize weights before lighting and restore afterwards so that the overall lighting result can be correctly weighted
					terrain_splat_control_0 /= (terrain_weight + terrain_weight_1 + 1e-3f);
					terrain_splat_control_1 /= (terrain_weight + terrain_weight_1 + 1e-3f);
				
				#endif // TERRAIN_BASE_PASS
				
				// Terrain normal, if using instancing and per-pixel normal map
				#if defined(UNITY_INSTANCING_ENABLED) && !defined(SHADER_API_D3D11_9X) && defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 terrainNormalCoords = (terrainTexcoord0.xy / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					normalWS = normalize(TCP2_TEX2D_SAMPLE(_TerrainNormalmapTexture, _TerrainNormalmapTexture, terrainNormalCoords.xy).rgb * 2 - 1);
					normalWS = mul(float4(normalWS, 0), unity_ObjectToWorld).xyz;
				#endif

				// main texture
				half3 albedo = half3(1,1,1);
				half alpha = 1;

				#if !defined(TERRAIN_BASE_PASS)
					// Sample textures that will be blended based on the terrain splat map
					half4 splat0 = __layer0Albedo;
					half4 splat1 = __layer1Albedo;
					half4 splat2 = __layer2Albedo;
					half4 splat3 = __layer3Albedo;
					half4 splat4 = __layer4Albedo;
					half4 splat5 = __layer5Albedo;
					half4 splat6 = __layer6Albedo;
					half4 splat7 = __layer7Albedo;
				
					#define BLEND_TERRAIN_HALF4(outVariable, sourceVariable) \
						half4 outVariable = terrain_splat_control_0.r * sourceVariable##0; \
						outVariable += terrain_splat_control_0.g * sourceVariable##1; \
						outVariable += terrain_splat_control_0.b * sourceVariable##2; \
						outVariable += terrain_splat_control_0.a * sourceVariable##3; \
						outVariable += terrain_splat_control_1.r * sourceVariable##4; \
						outVariable += terrain_splat_control_1.g * sourceVariable##5; \
						outVariable += terrain_splat_control_1.b * sourceVariable##6; \
						outVariable += terrain_splat_control_1.a * sourceVariable##7;
					#define BLEND_TERRAIN_HALF(outVariable, sourceVariable) \
						half4 outVariable = dot(terrain_splat_control_0, half4(sourceVariable##0, sourceVariable##1, sourceVariable##2, sourceVariable##3)); \
						outVariable += dot(terrain_splat_control_1, half4(sourceVariable##4, sourceVariable##5, sourceVariable##6, sourceVariable##7));
				
					BLEND_TERRAIN_HALF4(terrain_mixedDiffuse, splat)
				
				#endif // !TERRAIN_BASE_PASS
				
				albedo = terrain_mixedDiffuse.rgb;
				alpha = terrain_mixedDiffuse.a;
				
				half3 emission = half3(0,0,0);
				
				albedo *= __mainColor.rgb;

				// main light: direction, color, distanceAttenuation, shadowAttenuation
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord = input.shadowCoord;
			#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
				float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
			#else
				float4 shadowCoord = float4(0, 0, 0, 0);
			#endif

			#if defined(URP_10_OR_NEWER)
				#if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
					half4 shadowMask = SAMPLE_SHADOWMASK(input.uvLM);
				#elif !defined (LIGHTMAP_ON)
					half4 shadowMask = unity_ProbesOcclusion;
				#else
					half4 shadowMask = half4(1, 1, 1, 1);
				#endif

				Light mainLight = GetMainLight(shadowCoord, positionWS, shadowMask);
			#else
				Light mainLight = GetMainLight(shadowCoord);
			#endif

				// ambient or lightmap
				// Samples SH fully per-pixel. SampleSHVertex and SampleSHPixel functions
				// are also defined in case you want to sample some terms per-vertex.
				half3 bakedGI = SampleSH(normalWS);
				half occlusion = 1;

				half3 indirectDiffuse = bakedGI;
				indirectDiffuse *= occlusion * albedo * __ambientIntensity;

				half3 lightDir = mainLight.direction;
				half3 lightColor = mainLight.color.rgb;

				half atten = mainLight.shadowAttenuation * mainLight.distanceAttenuation;

				half ndl = dot(normalWS, lightDir);
				half3 ramp;
				
				half rampThreshold = __rampThreshold;
				half rampSmooth = __rampSmoothing * 0.5;
				half bandsCount = __bandsCount;
				half bandsSmoothing = __bandsSmoothing;
				ndl = saturate(ndl);
				half bandsNdl = smoothstep(rampThreshold - rampSmooth, rampThreshold + rampSmooth, ndl);
				half bandsSmooth = bandsSmoothing * 0.5;
				ramp = saturate((smoothstep(0.5 - bandsSmooth, 0.5 + bandsSmooth, frac(bandsNdl * bandsCount)) + floor(bandsNdl * bandsCount)) / bandsCount).xxx;

				// apply attenuation
				ramp *= atten;

				half3 color = half3(0,0,0);
				half3 accumulatedRamp = ramp * max(lightColor.r, max(lightColor.g, lightColor.b));
				half3 accumulatedColors = ramp * lightColor.rgb;

				// Additional lights loop
			#ifdef _ADDITIONAL_LIGHTS
				uint additionalLightsCount = GetAdditionalLightsCount();
				for (uint lightIndex = 0u; lightIndex < additionalLightsCount; ++lightIndex)
				{
					#if defined(URP_10_OR_NEWER)
						Light light = GetAdditionalLight(lightIndex, positionWS, shadowMask);
					#else
						Light light = GetAdditionalLight(lightIndex, positionWS);
					#endif
					half atten = light.shadowAttenuation * light.distanceAttenuation;
					half3 lightDir = light.direction;
					half3 lightColor = light.color.rgb;

					half ndl = dot(normalWS, lightDir);
					half3 ramp;
					
					ndl = saturate(ndl);
					half bandsNdl = smoothstep(rampThreshold - rampSmooth, rampThreshold + rampSmooth, ndl);
					half bandsSmooth = bandsSmoothing * 0.5;
					ramp = saturate((smoothstep(0.5 - bandsSmooth, 0.5 + bandsSmooth, frac(bandsNdl * bandsCount)) + floor(bandsNdl * bandsCount)) / bandsCount).xxx;

					// apply attenuation (shadowmaps & point/spot lights attenuation)
					ramp *= atten;

					accumulatedRamp += ramp * max(lightColor.r, max(lightColor.g, lightColor.b));
					accumulatedColors += ramp * lightColor.rgb;

				}
			#endif
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				color += input.vertexLights * albedo;
			#endif

				accumulatedRamp = saturate(accumulatedRamp);
				half3 shadowColor = (1 - accumulatedRamp.rgb) * __shadowColor;
				accumulatedRamp = accumulatedColors.rgb * __highlightColor + shadowColor;
				color += albedo * accumulatedRamp;

				// apply ambient
				color += indirectDiffuse;

				color += emission;

				#if !defined(TERRAIN_BASE_PASS)
					color.rgb *= saturate(terrain_weight + terrain_weight_1);
				#endif
				
				return half4(color, alpha);
			}
			ENDHLSL
		}

		// Depth & Shadow Caster Passes
		HLSLINCLUDE

		#if defined(SHADOW_CASTER_PASS) || defined(DEPTH_ONLY_PASS)

			#define fixed half
			#define fixed2 half2
			#define fixed3 half3
			#define fixed4 half4

			float3 _LightDirection;
			float3 _LightPosition;

			struct Attributes
			{
				float4 vertex   : POSITION;
				float3 normal   : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS     : SV_POSITION;
			#if defined(DEPTH_ONLY_PASS)
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			#endif
			};

			float4 GetShadowPositionHClip(Attributes input)
			{
				float3 positionWS = TransformObjectToWorld(input.vertex.xyz);
				float3 normalWS = TransformObjectToWorldNormal(input.normal);

				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

				#if UNITY_REVERSED_Z
					positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
				#else
					positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
				#endif

				return positionCS;
			}

			Varyings ShadowDepthPassVertex(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				#if defined(DEPTH_ONLY_PASS)
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				#endif

				#if defined(DEPTH_ONLY_PASS)
					output.positionCS = TransformObjectToHClip(input.vertex.xyz);
				#elif defined(SHADOW_CASTER_PASS)
					output.positionCS = GetShadowPositionHClip(input);
				#else
					output.positionCS = float4(0,0,0,0);
				#endif

				return output;
			}

			half4 ShadowDepthPassFragment(Varyings input) : SV_TARGET
			{
				#if defined(DEPTH_ONLY_PASS)
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				#endif

				half3 albedo = half3(1,1,1);
				half alpha = 1;
				half3 emission = half3(0,0,0);

				return 0;
			}

		#endif
		ENDHLSL

		Pass
		{
			Name "ShadowCaster"
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0

			// using simple #define doesn't work, we have to use this instead
			#pragma multi_compile SHADOW_CASTER_PASS

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd

			#pragma vertex ShadowDepthPassVertex
			#pragma fragment ShadowDepthPassFragment

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

			ENDHLSL
		}

		Pass
		{
			Name "DepthOnly"
			Tags
			{
				"LightMode" = "DepthOnly"
			}

			ZWrite On
			ColorMask 0

			HLSLPROGRAM

			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd

			// using simple #define doesn't work, we have to use this instead
			#pragma multi_compile DEPTH_ONLY_PASS

			#pragma vertex ShadowDepthPassVertex
			#pragma fragment ShadowDepthPassFragment

			ENDHLSL
		}

		// Scene picking for terrain shader
		UsePass "Hidden/Nature/Terrain/Utilities/PICKING"

	}

	FallBack "Hidden/InternalErrorShader"
	CustomEditor "ToonyColorsPro.ShaderGenerator.MaterialInspector_SG2"
}

