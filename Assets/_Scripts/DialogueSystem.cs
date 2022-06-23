using System.Collections;
using System.Collections.Generic;
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
            Requests.Request q = NPCManager.GetNPCByName(characterName).GetQuest(questIndex);
            q.Init();
            UI.UIManager.Instance.ShowNewRequestView(q);
            if (Player.Instance == null)
                print("No player");
            Player.Instance.QuestLog.AddQuest(q);
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