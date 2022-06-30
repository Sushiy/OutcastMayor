
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

namespace OutcastMayor.UtilityAI
{
    public class UtilityCharacterWindow : OdinEditorWindow
    {
        UtilityAICharacter character;

        [MenuItem("Tools/Outcast Mayor/UtilityCharacter")]
        private static void Open()
        {
            var window = GetWindow<UtilityCharacterWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }

        protected override void OnBeginDrawEditors()
        {
            if(Selection.activeGameObject == null)
            {
                GUILayout.Label("No Gameobject Selected");
                return;
            }
            character = Selection.activeGameObject.GetComponent<UtilityAICharacter>();
            if(character == null)
            {
                GUILayout.Label("Object is not a UtilityAICharacter");
                return;
            }

            //Okay we have a character!
            characterName = character.name;

            currentState = character.currentStateName;
            IsPerforming = character.currentState == character.PerformingState;
        }

        //[TitleGroup("$characterName")]
        [HorizontalGroup("Character")]
        [VerticalGroup("Character/Left")]
        public string characterName;

        [VerticalGroup("Character/Right")]
        [DisplayAsString]
        public string currentState;
        
        bool IsPerforming = false;
        [VerticalGroup("Character/Right")]
        [ShowIf("$IsPerforming"), DisplayAsString]
        public string currentAction;
    }

}
