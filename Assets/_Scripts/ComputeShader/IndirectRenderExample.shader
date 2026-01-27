Shader "IndirectRender"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #define UNITY_INDIRECT_DRAW_ARGS IndirectDrawIndexedArgs
            #include "UnityIndirect.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR0;
            };

            uniform float4 _MinPositionWS;
            uniform float4 _ShingleRotation;
            uniform float2 _ShingleOffset;
            uniform uint _WidthCount;
            uniform uint _HeightCount;
            uniform float4 _WidthVec;
            uniform float4 _HeightVec;
            uniform float _Offset;

            float4x4 QuatToMatrix(float4 q)
            {
                float4x4 rotMat = float4x4
                (
                    float4(1 - 2 * q.y * q.y - 2 * q.z * q.z, 2 * q.x * q.y + 2 * q.w * q.z, 2 * q.x * q.z - 2 * q.w * q.y, 0),
                    float4(2 * q.x * q.y - 2 * q.w * q.z, 1 - 2 * q.x * q.x - 2 * q.z * q.z, 2 * q.y * q.z + 2 * q.w * q.x, 0),
                    float4(2 * q.x * q.z + 2 * q.w * q.y, 2 * q.y * q.z - 2 * q.w * q.x, 1 - 2 * q.x * q.x - 2 * q.y * q.y, 0),
                    float4(0, 0, 0, 1)
                );
                return rotMat;
            }

            float4x4 MakeTRSMatrix(float3 pos, float4 rotQuat, float3 scale)
            {
                float4x4 rotPart = QuatToMatrix(rotQuat);
                float4x4 trPart = float4x4(float4(scale.x, 0, 0, 0), float4(0, scale.y, 0, 0), float4(0, 0, scale.z, 0), float4(pos, 1));
                return mul(rotPart, trPart);
            }

            v2f vert(appdata_base v, uint svInstanceID : SV_InstanceID)
            {
                InitIndirectDrawArgs(0);
                v2f o;
                uint cmdID = GetCommandID(0);
                uint instanceID = GetIndirectInstanceID(svInstanceID);
                uint x = instanceID % _WidthCount;
                uint y = (instanceID-x)/ _WidthCount;
                float offset = 0.5f * _Offset;
                if(y % 2 != 0)
                    offset = 1.0f * _Offset;
                float3 pos = _MinPositionWS + (x * _ShingleOffset.x + offset) * _WidthVec + y * _ShingleOffset.y * _HeightVec;
                float4x4 oTw = transpose(MakeTRSMatrix(pos, _ShingleRotation, float3(1,1,1)));
                float4 wpos = mul(oTw, v.vertex);
                o.pos = mul(UNITY_MATRIX_VP, wpos);
                o.color = float4(1,0,0,1);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}