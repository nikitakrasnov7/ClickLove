using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextsSO", menuName = "Scriptable Objects/TextsSO")]
public class TextsSO : ScriptableObject
{
    public List<TextsElement> fullTexts;
}

[System.Serializable]
public class TextsElement
{   
    public string valueText;
}
