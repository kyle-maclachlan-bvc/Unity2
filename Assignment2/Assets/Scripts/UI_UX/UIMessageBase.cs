using TMPro;
using UnityEngine;

public class UIMessageBase : MonoBehaviour
{
    [SerializeField] private GameObject uiObject;
    [SerializeField] private TMP_Text messageText;

    private void Start()
    {
        Hide();
    }

    public void Show(string message)
    {
        uiObject.SetActive(true);
        messageText.SetText(message);
    }

    public void Hide()
    {
        uiObject.SetActive(false);
    }
}
