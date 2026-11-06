using TMPro;
using UnityEngine;

public class LanguageText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text;
    [TextArea]
    [SerializeField]
    string russianText;
    [TextArea]
    [SerializeField]
    string englishText;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (gameObject.GetComponent<TextMeshProUGUI>() != null)
        {
            Text = gameObject.GetComponent<TextMeshProUGUI>();
            russianText = Text.text;
        }
    }
#endif
    public void SetText(string text)
    {
        if (Text == null) return;
        switch (text)
        {
            case "ru":
                Text.text = russianText;
                break;
            case "en":
                Text.text = englishText;
                break;

            default:
                Text.text = russianText;
                break;
        }


    }
}
