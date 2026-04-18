using UnityEngine;
using TMPro;

public class LevelClear : Singleton<LevelClear>
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
