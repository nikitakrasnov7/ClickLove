using DG.Tweening;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CreateCardsController : MonoBehaviour
{
    [Header("SO DATA")]
    [SerializeField] PhotosDataSO photoData;
    [SerializeField] VideosSO videoData;
    [SerializeField] TextsSO factsData;
    [SerializeField] TextsSO historyData;

    [Header("PHOTO SETTINGS")]
    [SerializeField] Image PreviewPhoto;
    [SerializeField] Sprite spritePhoto;
    [SerializeField] Button getPhoto;

    public TMP_InputField inputNamePhoto;
    public TMP_InputField inputDescriptionPhoto;

    public Toggle commonLevel;
    public Toggle rareLevel;
    public Toggle epicLevel;
    public Toggle legendLevel;

    byte[] bytesActivePhoto;

    [Header("Window Moving")]
    [SerializeField] RectTransform windowContainer;
    [SerializeField] RectTransform videoContainer;
    [SerializeField] float moveStep = -1050f;
    [SerializeField] float speedMove = 0.5f;

    private Sprite defoultSprite;

    [Header("Create cards window")]
    [SerializeField] GameObject prefabPhotoElement;
    [SerializeField] Transform parentForCards;

    [Header("VIDEO SETTINGS")]
    [SerializeField] VideoPlayer previewClip;
    [SerializeField] RenderTexture renderTexture;
    [SerializeField] Button GetVideo;
    [SerializeField] TMP_InputField inputNameVideo;

    [Header("FACTS SETTINGS")]
    [SerializeField] TMP_InputField inputFacts;

    [Header("HISTORY SETTINGS")]
    [SerializeField] TMP_InputField inputHistory;

    [Header("CREATES BUTTON")]
    [SerializeField] Button CreatedCard;
    [SerializeField] Button CreateVideoFile;
    [SerializeField] Button CreateFact;
    [SerializeField] Button CreateHistory;

    [SerializeField] TextMeshProUGUI CreatedCardsCountText, CreatedVideoCountText, CreatedFactsCountText, CreatedHistoryCountText;
    int createdCardsCount;
    int createdVideoCount;
    int createdFactsCount;
    int createdHistoryCount;



    private string tempPathVideo;
    public class TestSaveCards
    {
        public List<TestPhotoElement> photos = new();
        public List<VideoElement> videos = new();
        public List<string> facts = new();
        public List<string> history = new();
    }
    [SerializeField] GameObject Hint;

    private void Awake()
    {
        var data = LoadData();
        if (data != null && data.photos != null)
        {
            this.photoData.photos = data.photos;
          
        }


        if (CreatedCardsCountText != null)
        {

            UpdatinTextCount(CreatedCardsCountText, createdCardsCount);
            UpdatinTextCount(CreatedVideoCountText, createdVideoCount);
            UpdatinTextCount(CreatedFactsCountText, createdFactsCount);
            UpdatinTextCount(CreatedHistoryCountText, createdHistoryCount);
        }

        if (CreatedCard == null)
            return;

        CreatedCard.onClick.AddListener(CreateCard);
        getPhoto.onClick.AddListener(GetPhotoInGallery);

        GetVideo.onClick.AddListener(SelectingVideo);
        CreateVideoFile.onClick.AddListener(CreateVideoCard);
        renderTexture.Release();


        // не понял но пока уберу
        //if (data != null && data.photos.Count > 0)
        //{
        //    foreach (var photo in data.photos)
        //    {
        //        var sprite = SearchPhotoByGuid(photo.GUID);
        //        if (sprite != null)
        //            CreateCard(photo, sprite);
        //    }
        //}


        inputDescriptionPhoto.onSelect.AddListener((path) =>
        {
            Debug.Log($"Выбрали поле описания {path}");

            windowContainer.DOAnchorPosY(900, 0.5f);

        });
        inputDescriptionPhoto.onDeselect.AddListener((str) =>
        {
            Debug.Log($"неВыбираем поле описания {str}");
            windowContainer.DOAnchorPosY(0, 0.5f);
        });

        CreateFact.onClick.AddListener(CreateFactCard);
        CreateHistory.onClick.AddListener(CreateHistoryCard);


    }

    #region настройка и работа с фото карточками
    public Sprite SearchPhotoByGuid(string guid)
    {
        string path = Path.Combine(Application.persistentDataPath, $"{guid}.png");
        Sprite sprite;
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);

            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        }
        else
        {
            sprite = defoultSprite;
        }
        return sprite;
    }
    public void CreateCard()
    {
        PhotoLevel level = GetSelectedLevel();
        if (string.IsNullOrEmpty(inputNamePhoto.text) ||
            string.IsNullOrEmpty(inputDescriptionPhoto.text) || level == PhotoLevel.None)
            return;
        TestPhotoElement photo = new TestPhotoElement();
        photo.NamePhoto = inputNamePhoto.text;
        photo.Description = inputDescriptionPhoto.text;
        photo.Level = level;



        photoData.AddingPhoto(photo);
        CreateCard(photo, spritePhoto);

        createdCardsCount++;
        UpdatinTextCount(CreatedCardsCountText, createdCardsCount);

        inputNamePhoto.text = string.Empty;
        inputDescriptionPhoto.text = string.Empty;
        commonLevel.isOn = true;
        spritePhoto = null;
        PreviewPhoto.sprite = spritePhoto;
        SaveData();
    }
    public void MoveWindows(float posX = 0)
    {
        windowContainer.DOAnchorPosX(posX, speedMove);
    }
    public void GetPhotoInGallery()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                bytesActivePhoto = File.ReadAllBytes(path);

                Texture2D texture = NativeGallery.LoadImageAtPath(path);
                if (texture == null)
                {
                    return;
                }

                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
                spritePhoto = sprite;
                PreviewPhoto.sprite = spritePhoto;
            }



        });
    }
    public void CreateCard(TestPhotoElement photo, Sprite sprite = null)
    {
        if (prefabPhotoElement == null)
            return;


        var card = Instantiate(prefabPhotoElement, parentForCards);
        card.GetComponent<CardElement>().Init(photo, sprite);


    }
    public PhotoLevel GetSelectedLevel()
    {
        var level = PhotoLevel.None;
        if (commonLevel.isOn)
            level = PhotoLevel.Common;
        else if (rareLevel.isOn)
            level = PhotoLevel.Rare;
        else if (epicLevel.isOn)
            level = PhotoLevel.Epic;
        else if (legendLevel.isOn)
            level = PhotoLevel.Legend;
        return level;
    }

    public void SavePhoto(string photoGuid)
    {
        string path = Path.Combine(Application.persistentDataPath, $"{photoGuid}.png");
        File.WriteAllBytes(path, bytesActivePhoto);
    }

    public void SaveData()
    {
        string path = Path.Combine(Application.persistentDataPath, "CARDS_PLAYER.json");

        TestSaveCards data = new TestSaveCards();
        data.photos = photoData.photos;
        data.videos = videoData.FullVideosForWheel;




        List<string> factsTexts = new();
        if (factsData.fullTexts != null)
        {
            foreach (var fact in factsData.fullTexts)
                factsTexts.Add(fact.valueText);
        }
        data.facts = factsTexts;


        List<string> historyTexts = new();
        if (historyData.fullTexts != null)
        {

            foreach (var history in historyData.fullTexts)
                historyTexts.Add(history.valueText);
        }
        data.history = historyTexts;

        foreach (var photo in data.photos)
        {
            string pathTest = Path.Combine(Application.persistentDataPath, $"{photo.GUID}.png");
            if (!File.Exists(pathTest))
            {
                SavePhoto(photo.GUID);
            }
        }
        File.WriteAllText(path, JsonUtility.ToJson(data));
        Debug.Log($"save {data.photos.Count} cards");
        Debug.Log($"save {data.videos.Count} videos");
        Debug.Log($"save {data.facts.Count} facts");
        Debug.Log($"save {data.history.Count} history");

    }
    public TestSaveCards LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, "CARDS_PLAYER.json");

        if (!File.Exists(path))
        {
            Debug.Log("CreateCardsController: CARDS_PLAYER.json не найден");
            return new TestSaveCards();
        }
        try
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
            {
                Debug.Log("CreateController: json == null");
                return new TestSaveCards();
            }
            TestSaveCards data = JsonUtility.FromJson<TestSaveCards>(json);

            if (data == null)
            {
                Debug.Log("CreateCards: json no read");
                return new TestSaveCards();
            }

            if (data.photos == null) data.photos = new();
            if (data.videos == null) data.videos = new();
            if (data.facts == null) data.facts = new();
            if (data.history == null) data.history = new();


            photoData.photos = data.photos;
            videoData.FullVideosForWheel = data.videos;

            List<TextsElement> facts = new List<TextsElement>();
            foreach (var saveFact in data.facts)
            {
                var fact = new TextsElement();
                fact.valueText = saveFact;
                facts.Add(fact);
            }
            factsData.fullTexts = facts;


            List<TextsElement> historys = new();
            foreach (var saveHistory in data.history)
            {
                var history = new TextsElement();
                history.valueText = saveHistory;
                Debug.Log("загружена история " + history.valueText);
                historys.Add(history);

            }
            historyData.fullTexts = historys;

            Debug.Log("CReate Cards: загружено карточек = " + photoData.photos.Count);
            Debug.Log("CReate Cards: загружено видео = " + videoData.FullVideosForWheel.Count);
            Debug.Log("CReate Cards: загружено фактов = " + factsData.fullTexts.Count);
            Debug.Log("CReate Cards: загружено историй = " + historyData.fullTexts.Count);



            createdCardsCount = photoData.photos.Count;
            createdVideoCount = videoData.FullVideosForWheel.Count;
            createdFactsCount = factsData.fullTexts.Count;
            createdHistoryCount = historyData.fullTexts.Count;

            return data;
        }
        catch (System.Exception e)
        {
            Debug.Log("CreatrCards: ошибка загрузки JSON :" + e.ToString());
            return new TestSaveCards();
        }

    }
    public void ImportCards(
    List<TestPhotoElement> importedCards,
    List<VideoElement> importsVideo,
    List<string> importFacts,
    List<string> importHistory)
    {
        int importedCount = 0;
        int skippedCount = 0;

        if (importedCards != null && importedCards.Count > 0)
        {
            if (photoData.photos == null) photoData.photos = new();

            foreach (TestPhotoElement importedCard in importedCards)
            {
                if (importedCard == null) continue;

                if (string.IsNullOrWhiteSpace(importedCard.GUID))
                {
                    skippedCount++;
                    continue;
                }

                bool alreadyExists = false;

                foreach (TestPhotoElement existingCard in photoData.photos)
                {
                    if (existingCard != null && existingCard.GUID == importedCard.GUID)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                if (alreadyExists)
                {
                    skippedCount++;
                    continue;
                }

                photoData.photos.Add(importedCard);
                importedCount++;
            }
        }

        int importedVideo = 0;
        int skippedVideo = 0;

        if (importsVideo != null && importsVideo.Count > 0)
        {
            if (videoData.FullVideosForWheel == null)
                videoData.FullVideosForWheel = new();

            foreach (VideoElement importVideo in importsVideo)
            {
                if (importVideo == null)
                    continue;

                if (string.IsNullOrWhiteSpace(importVideo.GUID))
                {
                    skippedVideo++;
                    continue;
                }

                bool alreadyExists = false;

                foreach (VideoElement existingVideo in videoData.FullVideosForWheel)
                {
                    if (existingVideo != null && existingVideo.GUID == importVideo.GUID)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                if (alreadyExists)
                {
                    skippedVideo++;
                    continue;
                }

                videoData.FullVideosForWheel.Add(importVideo);
                importedVideo++;
            }
        }

        int importedFacts = ImportTexts(importFacts, factsData, "факт");

        int importedHistory = ImportTexts(importHistory, historyData, "история");

        SaveData();

        photoData.isTestSave = true;

        Debug.Log(
            $"ImportCards: карт +{importedCount}, " +
            $"видео +{importedVideo}, " +
            $"фактов +{importedFacts}, " +
            $"историй +{importedHistory}"
        );

        SceneManager.LoadScene("StartMenu");
    }


    public void CheckData()
    {
        string path = Path.Combine(Application.persistentDataPath, "CARDS_PLAYER.json");
        if (File.Exists(path))
        {
            TestSaveCards data = new();
            data = JsonUtility.FromJson<TestSaveCards>(File.ReadAllText(path));

        }
    }


    #endregion

    #region настройкак и работа с видео

    public void MoveVideoContainer(float posX = 0)
    {

        videoContainer.DOAnchorPosX(posX, speedMove);

    }
    public void SelectingVideo()
    {
        Debug.Log("Alo suka");
        NativeGallery.GetVideoFromGallery(OnSelectedVideo, "Выберите видео: ", "video/*");
    }

    public void OnSelectedVideo(string path)
    {
        if (string.IsNullOrEmpty(path))
            Debug.Log("path == null");
        if (!string.IsNullOrEmpty(path))
        {
            tempPathVideo = path;
            //tempPathVideo = Path.Combine(Application.persistentDataPath,
            //    Path.GetFileName(CreateGUIDForVideo()));
            //File.Copy(path, tempPathVideo + ".mp4");

            previewClip.url = path;
            previewClip.time = 0;
            previewClip.Play();
        }
    }

    public void CreateVideoCard()
    {
        if (string.IsNullOrEmpty(inputNameVideo.text))
        {

            return;
        }
        if (string.IsNullOrEmpty(tempPathVideo))
        {
            return;
        }
        VideoElement video = new VideoElement();
        video.Name = inputNameVideo.text;
        string guid = CreateGUIDForVideo();
        video.GUID = guid;

        var newPath = Path.Combine(Application.persistentDataPath, Path.GetFileName(guid));
        if (File.Exists(newPath))
        {
            Debug.Log("newPath exists");
            return;
        }

        File.Copy(tempPathVideo, newPath + ".mp4");


        if (videoData.FullVideosForWheel == null)
        {
            videoData.FullVideosForWheel = new();
        }

        videoData.FullVideosForWheel.Add(video);
        inputNameVideo.text = "";
        renderTexture.Release();
        tempPathVideo = "";

        createdVideoCount++;
        UpdatinTextCount(CreatedVideoCountText, createdVideoCount);

        Debug.Log("видео сохранено");
        SaveData();

    }
    public string CreateGUIDForVideo()
    {
        return System.Guid.NewGuid().ToString();
    }

    #endregion

    private int ImportTexts(List<string> importedTexts, TextsSO targetData, string type)
    {
        if (importedTexts == null || importedTexts.Count == 0)
            return 0;

        if (targetData.fullTexts == null) targetData.fullTexts = new List<TextsElement>();

        int added = 0;

        foreach (string importedText in importedTexts)
        {
            if (string.IsNullOrWhiteSpace(importedText)) continue;

            string value = importedText.Trim();
            bool exists = false;

            foreach (TextsElement existing in targetData.fullTexts)
            {
                if (existing != null && existing.valueText == value)
                {
                    exists = true;
                    break;
                }
            }

            if (exists) continue;

            TextsElement newText = new TextsElement();
            newText.valueText = value;

            targetData.fullTexts.Add(newText);
            added++;

            Debug.Log($"ImportCards: добавлен {type}: {value}");
        }

        return added;
    }
    public void CreateFactCard()
    {
        if (string.IsNullOrEmpty(inputFacts.text)) { return; }

        TextsElement facts = new TextsElement();
        facts.valueText = inputFacts.text;

        factsData.AddNewText(facts);

        createdFactsCount++;
        UpdatinTextCount(CreatedFactsCountText, createdFactsCount);
        inputFacts.text = "";

        Debug.Log("CreateCard: facts Created");

        SaveData();
    }
    public void CreateHistoryCard()
    {
        if (string.IsNullOrEmpty(inputHistory.text)) { return; }

        TextsElement history = new();
        history.valueText = inputHistory.text;

        historyData.AddNewText(history);

        createdHistoryCount++;
        UpdatinTextCount(CreatedHistoryCountText, createdHistoryCount);
        inputHistory.text = "";
        Debug.Log("CreateCard: history created  " + history);
        SaveData();

    }

    public void UpdatinTextCount(TextMeshProUGUI text, int addCount)
    {

        text.text = addCount.ToString();
    }

}
