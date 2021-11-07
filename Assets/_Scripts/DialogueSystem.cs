using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueSystem : MonoBehaviour
{
    private static DialogueRunner dialogueRunner;

    private DialogueSystem instance;

    private void Awake()
    {
        dialogueRunner = GetComponent<DialogueRunner>();
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
}
