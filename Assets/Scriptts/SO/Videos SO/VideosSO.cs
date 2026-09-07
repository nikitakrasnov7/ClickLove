using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "VideosSO", menuName = "Scriptable Objects/VideosSO")]
public class VideosSO : ScriptableObject
{
#if UNITY_EDITOR
    private void OnValidate()
    {
        GenerateName();
    }
    private void GenerateName()
    {
        foreach (var v in FullVideosForWheel)
            v.GenerateName();
    }
#endif
    public List<VideoElement> FullVideosForWheel = new();
}
[System.Serializable]
public class VideoElement
{
    public string Name;
    public VideoClip clip;

#if UNITY_EDITOR

    public void GenerateName()
    {
        if (string.IsNullOrEmpty(Name) && clip != null)
        {
            Name = clip.name;
        }
    }

#endif

}
