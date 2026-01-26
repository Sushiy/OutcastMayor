using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera camStandard;
    [SerializeField]
    private CinemachineCamera camTopDown;
    [SerializeField]
    private CinemachineCamera camDialogue;

    public enum CameraType
    {
        Standard,
        TopDown,
        Dialogue
    }

    public CameraType activeCamera = CameraType.Standard;
    public static CameraType ActiveCamera
    {
        get
        {
            return instance.activeCamera;
        }
    }

    private static CameraController instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Two CameraControllers");
            Destroy(instance);
        }
        ChangeToStandardCamera();
    }

    public static void ChangeToStandardCamera()
    {
        instance.camStandard.Priority = 10;
        instance.camTopDown.Priority = 0;
        instance.camDialogue.Priority = 0;
        instance.activeCamera = CameraType.Standard;
    }

    public static void ChangeToTopDownCamera()
    {
        instance.camStandard.Priority = 0;
        instance.camTopDown.Priority = 10;
        instance.camDialogue.Priority = 0;
        instance.activeCamera = CameraType.TopDown;
    }

    public static void ChangeToDialogueCamera()
    {
        instance.camStandard.Priority = 0;
        instance.camTopDown.Priority = 0;
        instance.camDialogue.Priority = 10;
        instance.activeCamera = CameraType.Dialogue;
    }
}
