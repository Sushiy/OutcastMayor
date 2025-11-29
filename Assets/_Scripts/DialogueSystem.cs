using System.Collections;
using System.Collections.Generic;
using OutcastMayor.Requests;
using UnityEngine;
using Yarn.Unity;

namespace OutcastMayor.Dialogue
{
    public class DialogueSystem : MonoBehaviour
    {
        private static DialogueRunner dialogueRunner;

        private void Awake()
        {
            dialogueRunner = GetComponent<DialogueRunner>();
            dialogueRunner.AddCommandHandler<string, int>("NewQuest", OnNewQuest);
            dialogueRunner.AddCommandHandler<string, int>("CheckQuest", OnCheckQuest);
        }

        public static void StartDialogue(string node)
        {
            dialogueRunner.StartDialogue(node);
            CameraController.ChangeToDialogueCamera();
            dialogueRunner.onDialogueComplete.AddListener(EndDialogue);
        }

        private static void EndDialogue()
        {
            CameraController.ChangeToStandardCamera();
        }
        
        public void OnNewQuest(string characterName, int questIndex)
        {
            Request request = new Request(NPCManager.GetNPCByName(characterName).NPCData.GetQuest(questIndex));
            UI.UIManager.Instance.ShowNewRequestView(request);
            if (Player.Instance == null)
                print("No player");
            Player.Instance.QuestLog.AddQuest(request);
        }
        public void OnCheckQuest(string characterName, int questIndex)
        {
            Request request = new Request(NPCManager.GetNPCByName(characterName).NPCData.GetQuest(questIndex));
            if (Player.Instance == null)
                print("No player");
            Player.Instance.QuestLog.CheckActiveQuest(request);
        }

        public static void SetDialogueValue(string valueName, bool boolValue)
        {
            dialogueRunner.variableStorage.SetValue(valueName, boolValue);
        }
        public static void SetDialogueValue(string valueName, int intValue)
        {
            dialogueRunner.variableStorage.SetValue(valueName, intValue);
        }
    }
}