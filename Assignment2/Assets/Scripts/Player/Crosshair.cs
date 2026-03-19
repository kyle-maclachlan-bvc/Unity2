using System;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Canvas crosshairCanvas;

    
    private void OnEnable()
    {
        playerController.OnStateUpdated += StateUpdate;
    }

    private void OnDestroy()
    {
        playerController.OnStateUpdated -= StateUpdate;
    }

    void State()
    {
        crosshairCanvas.enabled = true;
    }

    void StateUpdate(PlayerState state)
    {
        crosshairCanvas.enabled = state == PlayerState.AIM;
    }
}
