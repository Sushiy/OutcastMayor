using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

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
        Quest q = NPCManager.GetNPCByName(characterName).GetQuest(questIndex);
        UIManager.Instance.ShowNewRequestView(q.title);
        if (Player.Instance == null)
            print("No player");
        Player.Instance.QuestLog.AddQuest(q);
    }
}
