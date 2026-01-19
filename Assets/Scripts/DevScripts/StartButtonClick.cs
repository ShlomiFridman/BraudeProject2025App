using TMPro;
using UnityEngine;

public class ButtonClickHandler : MonoBehaviour
{
    public TMP_Text targetTMPText;

    private int cntr;

    public void Start()
    {
        Debug.Log("Started");
    }

    public void OnButtonClick()
    {
        if (targetTMPText != null)
            targetTMPText.text = $"INFO: Button Clicked {++cntr} time{(cntr == 1 ? "" : "s")}";
        else
            Debug.LogWarning("TMP_Text is not assigned!");
    }
}