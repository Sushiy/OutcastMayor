using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueSystem : MonoBehaviour
{
    private static DialogueRunner dialogueRunner;

    private DialogueSystem instance;

    public GameObject newRequestPanel;

    public LineView view;

    private void Awake()
    {
        dialogueRunner = GetComponent<DialogueRunner>();
        dialogueRunner.AddCommandHandler("NewQuest", OnNewQuest);
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

    public void OnNewQuest()
    {
        newRequestPanel.SetActive(true);
        //view.ReadyForNextLine();
    }
}
