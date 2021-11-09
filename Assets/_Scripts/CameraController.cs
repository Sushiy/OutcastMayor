using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera camStandard;
    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera camTopDown;
    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera camDialogue;

    public enum CameraType
    {
        Standard,
        TopDown,
        Dialogue
    }

    private CameraType activeCamera = CameraType.Standard;
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
