using UnityEngine;

public class MouseBehavior : MonoBehaviour
{
    // This code hides the mouse, and allows the movement of the mouse to always adjust the camera.
    // This code is for Keyboard Play, Controller Play does not utilize this.
    void Start()
    {
        ShowMouse(false);
    }

    public void ShowMouse(bool value)
    {
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        
    }
}
