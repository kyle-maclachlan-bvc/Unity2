using UnityEngine;

public class Toast : Singleton<Toast>
{
    [SerializeField] private UIMessageBase ui;

    public void ShowToast(string message)
    {
        ui.Show(message);
    }

    public void HideToast()
    {
        ui.Hide();
    }
}
