using UnityEngine;
using TMPro;

public class LevelClear : Singleton<LevelClear>
{
    [SerializeField] private UIMessageBase ui;

    public void ShowLevelClear(string message)
    {
        ui.Show(message);
    }

    public void HideLevelClear()
    {
        ui.Hide();
    }

}
