using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextsSO", menuName = "Scriptable Objects/TextsSO")]
public class TextsSO : ScriptableObject
{
    public List<TextsElement> fullTexts;
    public void AddNewText(TextsElement text)
    {
        if (fullTexts == null)
            fullTexts = new List<TextsElement>();
        fullTexts.Add(text);
    }
}

[System.Serializable]
public class TextsElement
{
    public string valueText;
}
