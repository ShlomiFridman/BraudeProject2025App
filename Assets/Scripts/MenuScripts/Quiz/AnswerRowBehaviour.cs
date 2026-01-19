using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using RTLTMPro;

public class AnswerRowBehaviour : MonoBehaviour
{

    public RTLTextMeshPro answerText;
    public Button answerButton;
    public TMP_Text answerButtonText;

    private bool isCorrect;

    public void setAnswer(int answerIndex, string answerText, Action<int> onClickAction)
    {
        this.answerText.text = answerText;
        this.answerButtonText.text = $"{answerIndex + 1}";

        this.answerButton.onClick.RemoveAllListeners();
        this.answerButton.onClick.AddListener(()=>
        {
            // Debug.Log($"Answer {answerIndex + 1} was clicked");
            onClickAction(answerIndex);
        });
    }

    public void disableButton()
    {
        answerButton.interactable = false;
    }

    public void setBtnColor(Color color)
    {
        answerButton.GetComponent<Image>().color = color;
    }

}
