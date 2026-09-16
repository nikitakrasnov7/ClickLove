using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoWinner : MonoBehaviour
{
    [SerializeField] VideosSO videoData;
    public VideosSO VideoData => videoData;

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
        videos.Clear();

        foreach (VideoElement videoElement in videoData.FullVideosForWheel)
        {
            if (!videosInLibrary.Contains(videoElement.GUID))
            {
                videos.Add(videoElement);
                Debug.Log("добавлено видео " + videoElement.Name);
            }
        }

        Debug.Log("Всего доступно видео для открытия: " + videos.Count);
    }

    public void OpeningPanel()
    {
        if (videos.Count == 0)
        {
            Debug.Log("Видео 0");
            return;
        }

        var rndVideo = videos[Random.Range(0, videos.Count)];

        foreach (var video in videos)
        {
            Debug.Log("video : " + video.Name);
        }

        if (videosInLibrary.Contains(rndVideo.GUID))
        {
            Debug.Log("Видео уже было открыто: " + rndVideo.Name);
            return;
        }

        videoPlayer.Stop();
        videoPlayer.clip = null;

        if (rndVideo.clip != null)
        {
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = rndVideo.clip;

            Debug.Log("Запускаем VideoClip: " + rndVideo.Name);
        }
        else if (!string.IsNullOrEmpty(rndVideo.GUID))
        {
            string videoPath = Path.Combine(
                Application.persistentDataPath,
                rndVideo.GUID + ".mp4"
            );

            if (!File.Exists(videoPath))
            {
                Debug.LogError("Файл видео не найден: " + videoPath);
                return;
            }

            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = videoPath;

            Debug.Log("Запускаем MP4: " + videoPath);
        }
        else
        {
            Debug.LogError("У видео нет ни VideoClip, ни GUID: " + rndVideo.Name);
            return;
        }

        AddVideoInLibrary(rndVideo);

        NameVideoText.text =
            $"{rndVideo.Name} ({videosInLibrary.Count}/{videoData.FullVideosForWheel.Count})";

        videoPlayer.time = 0;

        playVideo.interactable = true;
        AnimatorWindow.SetBool("Open", true);

    }
    public void AddVideoInLibrary(VideoElement video)
    {
        if (!videosInLibrary.Contains(video.GUID))
        {
            videosInLibrary.Add(video.GUID);
        }

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
