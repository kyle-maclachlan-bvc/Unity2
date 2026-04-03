using UnityEngine;

public class Toast : UIMessageBase
{
    public static Toast Instance;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);

        Instance = this;
    }

    public void ShowToast(string message)
    {
        Show(message);
    }

    public void HideToast()
    {
        Hide();
    }
}
