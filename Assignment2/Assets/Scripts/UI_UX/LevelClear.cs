using UnityEngine;
using TMPro;

public class LevelClear : UIMessageBase
{
    public static LevelClear Instance;
    
void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);

        Instance = this;
    }

    public void ShowLevelClear(string message)
    {
        Show(message);
    }

    public void HideLevelClear()
    {
        Hide();
    }

}
