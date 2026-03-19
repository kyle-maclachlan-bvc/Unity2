using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineCamera exploreCamera;
    [SerializeField] private CinemachineCamera aimCamera;

    [SerializeField] private PlayerController playerController;
    void OnEnable()
    {
        playerController.OnStateUpdated += SwitchCamera;
    }

    private void OnDestroy()
    {
        playerController.OnStateUpdated -= SwitchCamera;
    }

    private void SwitchCamera(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.EXPLORE:
                // Set up the Explore Camera
                exploreCamera.Prioritize();
                break;
            
            case PlayerState.AIM:
                // Set up the Aim Camera
                aimCamera.Prioritize();
                break;

            default:
                // Nothing to do here
                break;
        }
    }
}

