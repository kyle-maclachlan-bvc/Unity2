using System;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    // This Code is used on the UI Canvas designed for the Crosshairs to show.
    // Crosshairs should appear when in "Aim Mode".
    
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
