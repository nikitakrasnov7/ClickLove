using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoWinner : MonoBehaviour
{
    [SerializeField] VideosSO videoData;

    [SerializeField] TextMeshProUGUI NameVideoText;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] RawImage rawImage;
    [SerializeField] Button playVideo;
    [SerializeField] Button closePanelBtn;
    [SerializeField] Animator AnimatorWindow;

    [SerializeField] List<VideoElement> videos = new();
    [SerializeField] public List<string> videosInLibrary = new();

    private void Awake()
    {     

        playVideo.onClick.AddListener(PlayerVideo);
        closePanelBtn.onClick.AddListener(ClosePanel);
    }

    public void GetVideos()
    {
        foreach (VideoElement videoElement in videoData.FullVideosForWheel)
        {
            if (!videosInLibrary.Contains(videoElement.Name))
            {
                videos.Add(videoElement);
            }
        }
    }

    public void OpeningPanel()
    {
        if (videos.Count == 0)
        {

            Debug.Log("Видео кончились");
            return;
        }
        var rndVideo = videos[Random.Range(0, videos.Count)];

        if (!videosInLibrary.Contains(rndVideo.Name))
        {
            AddVideoInLibrary(rndVideo);

            NameVideoText.text = $"{rndVideo.Name} ({videosInLibrary.Count}/{videoData.FullVideosForWheel.Count})";
            videoPlayer.time = 0;
            videoPlayer.clip = rndVideo.clip;
            playVideo.interactable = true;
            AnimatorWindow.SetBool("Open", true);
        }
        else
        {
            Debug.Log("Видео кончились");
        }


    }
    public void AddVideoInLibrary(VideoElement video)
    {
        videosInLibrary.Add(video.Name);
        videos.Remove(video);
    }
    public void PlayerVideo()
    {
        rawImage.gameObject.SetActive(true);
        videoPlayer.Play();

        playVideo.interactable = false;
    }

    public void ClosePanel()
    {
        videoPlayer.Stop();
        videoPlayer.time = 0;
        rawImage.gameObject.SetActive(false);

        AnimatorWindow.SetBool("Open", false);

    }


}
