using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextWinner : MonoBehaviour
{
    [SerializeField] TextsSO textData;
    [SerializeField] TextMeshProUGUI Title;
    [SerializeField] string typeTextWindow;
    [SerializeField] TextMeshProUGUI Text;
    [SerializeField] Animator animator;
    [SerializeField] Button closeBtn;
    [SerializeField] List<TextsElement> texts = new();
    [SerializeField] public List<string> textsInLibrary;
    private void Awake()
    {
        closeBtn.onClick.AddListener(CloseWindow);
    }
    public void GetTexts()
    {
        foreach (TextsElement text in textData.fullTexts)
        {
            if (!textsInLibrary.Contains(text.valueText))
            {
                texts.Add(text);
            }
        }
    }
    public void OpenWindow()
    {
        if (texts.Count == 0)
        {
            Debug.Log("кончилось" + gameObject.name);
            return;
        }
        var textEl = texts[Random.Range(0, texts.Count)];

        if (!textsInLibrary.Contains(textEl.valueText))
        {

            Text.text = textEl.valueText ;
            textsInLibrary.Add(textEl.valueText);
            texts.Remove(textEl);
            Title.text = $"{typeTextWindow} {textsInLibrary.Count} из {textData.fullTexts.Count}";
            animator.SetBool("Open", true);
        }

    }
    public void CloseWindow()
    {
        animator.SetBool("Open", false);
    }
}
