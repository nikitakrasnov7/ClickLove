using DG.Tweening;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateCardsController : MonoBehaviour
{
    [SerializeField] PhotosDataSO photoData;
    [SerializeField] Image PreviewPhoto;
    [SerializeField] Sprite spritePhoto;
    [SerializeField] Button getPhoto;

    public TMP_InputField inputNamePhoto;
    public TMP_InputField inputDescriptionPhoto;

    public Toggle commonLevel;
    public Toggle rareLevel;
    public Toggle epicLevel;
    public Toggle legendLevel;

    [SerializeField] Button CreatedCard;
    byte[] bytesActivePhoto;

    [Header("Window Moving")]
    [SerializeField] RectTransform windowContainer;
    [SerializeField] Button forGrids;
    [SerializeField] Button forCreate;
    [SerializeField] float moveStep = -1050f;
    [SerializeField] float speedMove = 0.5f;

    private Sprite defoultSprite;

    [Header("Create cards window")]
    [SerializeField] GameObject prefabPhotoElement;
    [SerializeField] Transform parentForCards;

    public class TestSaveCards
    {
        public List<TestPhotoElement> photos = new();
    }
    private void Awake()
    {
        CreatedCard.onClick.AddListener(CreateCard);
        getPhoto.onClick.AddListener(GetPhotoInGallery);

        forGrids.onClick.AddListener(() => MoveWindows(moveStep));
        forCreate.onClick.AddListener(() => MoveWindows());

        var data = LoadData();
        if (data != null && data.photos.Count > 0)
        {
            foreach (var photo in data.photos)
            {
                var sprite = SearchPhotoByGuid(photo.GUID);
                if (sprite != null)
                    CreateCard(photo, sprite);
            }
        }
    }
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
        if (string.IsNullOrEmpty(inputDescriptionPhoto.text) ||
            string.IsNullOrEmpty(inputDescriptionPhoto.text) || level == PhotoLevel.None)
            return;
        TestPhotoElement photo = new TestPhotoElement();
        photo.NamePhoto = inputNamePhoto.text;
        photo.Description = inputDescriptionPhoto.text;
        photo.Level = level;



        photoData.AddingPhoto(photo);
        CreateCard(photo, spritePhoto);

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
    }
    public TestSaveCards LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, "CARDS_PLAYER.json");
        TestSaveCards photos = new TestSaveCards();
        if (File.Exists(path))
        {
            photos = JsonUtility.FromJson<TestSaveCards>(File.ReadAllText(path));
            if (photos.photos.Count > 0)
            {
                photoData.photos = photos.photos;
                Debug.Log($"loaded {photos.photos.Count} cards");
            }
        }
        return photos;
    }
    public void ImportCards(List<TestPhotoElement> importedCards)
    {
        if (importedCards == null ||
        importedCards.Count == 0)
        {
            Debug.Log( "ImportCards: карточек для импорта нет");
            return;
        }

        int importedCount = 0;
        int skippedCount = 0;

        foreach (TestPhotoElement importedCard in importedCards)
        {
            if (importedCard == null)
                continue;

            if (string.IsNullOrWhiteSpace(importedCard.GUID))
            {
                Debug.LogWarning("ImportCards: карточка без GUID");

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
            Sprite sprite = SearchPhotoByGuid(importedCard.GUID);
            CreateCard(importedCard, sprite);

            importedCount++;
        }

        string path = Path.Combine(Application.persistentDataPath, "CARDS_PLAYER.json");
        TestSaveCards data = new TestSaveCards();
        data.photos = photoData.photos;
        File.WriteAllText(path, JsonUtility.ToJson(data));

        Debug.Log("ImportCards: добавлено = " + importedCount + ", пропущено = " +skippedCount );
    }
}
