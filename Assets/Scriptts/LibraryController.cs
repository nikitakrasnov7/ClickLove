using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LibraryController : MonoBehaviour
{

    [SerializeField] Button BuyBtn;
    [SerializeField] Button TakeBtn;

    [SerializeField] GameObject CardPhoto;
    [SerializeField] Image CloseCardImage;
    [SerializeField] Image Background;
    [SerializeField] Image CardImage;
    [SerializeField] TextMeshProUGUI nameCard;

    [SerializeField] Animator CardAnim;
    [SerializeField] Animator BackgroundAnim;

    [Header("Задний фон")]
    [SerializeField] Sprite CommonBack;
    [SerializeField] Sprite RareBack;
    [SerializeField] Sprite EpicBack;
    [SerializeField] Sprite LegendBack;

    [Header("Карты")]
    [SerializeField] Sprite CommonCard;
    [SerializeField] Sprite RareCard;
    [SerializeField] Sprite EpicCard;
    [SerializeField] Sprite LegendCard;

    [SerializeField] Sprite GemsCard;
    int gemsForPlayer = -1;

    [Header("Фотки")]
    [SerializeField] List<PhotoElement> CommonPhotos = new List<PhotoElement>();
    [SerializeField] List<PhotoElement> RarePhotos = new List<PhotoElement>();
    [SerializeField] List<PhotoElement> EpicPhotos = new List<PhotoElement>();
    [SerializeField] List<PhotoElement> LegendPhotos = new List<PhotoElement>();

    private List<PhotoElement> fullCards = new();

    [SerializeField] public PhotosDataSO photoDataSO;
    public List<TestPhotoElement> newFullCards = new();
#if UNITY_EDITOR
    private void OnValidate()
    {
        GenerateId(CommonPhotos);
        GenerateId(RarePhotos);
        GenerateId(EpicPhotos);
        GenerateId(LegendPhotos);
    }

    private void GenerateId(List<PhotoElement> photos)
    {
        foreach (PhotoElement photo in photos)
            photo.GenedateId();
    }

#endif


    [SerializeField] public List<string> CardsPlayer = new List<string>();
    [SerializeField] List<GameObject> CardsElements = new List<GameObject>();
    [SerializeField] List<GameObject> ActiveElements = new List<GameObject>();

    [SerializeField] PhotoElement activePhoto;

    [SerializeField] public float CommonChance = 50f;
    [SerializeField] public float RareChance = 75f;
    [SerializeField] public float EpicChance = 90f;
    [SerializeField] public float LegendChance = 100f;

    private static readonly int startAnim = Animator.StringToHash("StartCard");
    private static readonly int destroyCard = Animator.StringToHash("DestroyCard");
    private static readonly int BackstartAnim = Animator.StringToHash("StartBack");
    private static readonly int OpenCard = Animator.StringToHash("OpenCard");


    [Header("Создание карточек в библиотеке")]
    [SerializeField] GameObject CardPrefab;
    [SerializeField] Transform parentForCard;

    [Header("Кнопки сортировки")]
    [SerializeField] Button FullBtn;
    [SerializeField] Button CommonBtn;
    [SerializeField] Button RareBtn;
    [SerializeField] Button EpicBtn;
    [SerializeField] Button LegendBtn;
    [SerializeField] TextMeshProUGUI cardsPlayerCountText;


    [Header("Карточка с информацией")]
    [SerializeField] Animator InfoPhotoWindow;
    [SerializeField] TextMeshProUGUI NamePhoto;
    [SerializeField] TextMeshProUGUI DescriptionPhoto;
    [SerializeField] Image IconPhoto;
    [SerializeField] Button CloseInfoPanelBtn;

    private void Awake()
    {

        #region тут было старая версия когда фотки храняться в памяти инспектора
        ////////тут было старая версия когда фотки храняться в памяти инспектора

        //foreach (var p in CommonPhotos)
        //    p.levelCard = PhotoLevel.Common;

        //foreach (var p in RarePhotos)
        //    p.levelCard = PhotoLevel.Rare;

        //foreach (var p in EpicPhotos)
        //    p.levelCard = PhotoLevel.Epic;
        //foreach (var p in LegendPhotos)
        //    p.levelCard = PhotoLevel.Legend;

        //fullCards.AddRange(CommonPhotos);
        //fullCards.AddRange(RarePhotos);
        //fullCards.AddRange(EpicPhotos);
        //fullCards.AddRange(LegendPhotos);
        #endregion
        FullBtn.onClick.AddListener(FullCard);

        CommonBtn.onClick.AddListener(() => SearchCards(PhotoLevel.Common));
        RareBtn.onClick.AddListener(() => SearchCards(PhotoLevel.Rare));
        EpicBtn.onClick.AddListener(() => SearchCards(PhotoLevel.Epic));
        LegendBtn.onClick.AddListener(() => SearchCards(PhotoLevel.Legend));

        CloseInfoPanelBtn.onClick.AddListener(CloseInfoPanel);




    }
    public void InitData()
    {

        if (photoDataSO == null)
        {
            Debug.Log("LibraryController: photoDataSo == null");
            return;

        }
        if (photoDataSO.photos == null)
            photoDataSO.photos = new();


        newFullCards = photoDataSO.photos;
        fullCards.Clear();
#if UNITY_ANDROID && !UNITY_EDITOR
        CommonPhotos.Clear();
        RarePhotos.Clear();
        EpicPhotos.Clear();
        LegendPhotos.Clear();
#endif

        foreach (var photo in newFullCards)
        {
            if (photo == null) continue;

            if (string.IsNullOrEmpty(photo.GUID))
            {
                Debug.Log("LibraryCOntroller: photo.GUID == null");
                continue;
            }

            if (CardsPlayer.Contains(photo.GUID))
            {
                NewLoadingCardsCreate(photo);
                Debug.Log($"Создана открытая карта {photo.NamePhoto}");
            }
            else
            {
                AddingCardsInLists(photo);
                Debug.Log($"создана не открытая карта {photo.NamePhoto}");

            }
        }
        Debug.Log($"загружено из подарка {photoDataSO.photos.Count}");

    }
    private void Start()
    {
        BuyBtn.onClick.AddListener(() => BuyingCard());
        CardPhoto.GetComponent<Button>().onClick.AddListener(OpeningCard);
        TakeBtn.onClick.AddListener(ClosePanel);

        ActiveElements = CardsElements;
        UpdateCountCard();
        GameManager.Instance.profileController.UpdateCardCount(CardsPlayer.Count);
    }
    public void AddingCardsInLists(TestPhotoElement photo)
    {
        if (photo == null) return;

        PhotoElement newPhoto = new();
        newPhoto.GetId(photo.GUID);
        newPhoto.Name = photo.NamePhoto;
        newPhoto.Description = photo.Description;
        newPhoto.levelCard = photo.Level;
        newPhoto.Photo = SearchPhotoByGuid(photo.GUID);

        fullCards.Add(newPhoto);

        switch (newPhoto.levelCard)
        {
            case (PhotoLevel.Common):
                CommonPhotos.Add(newPhoto);
                break;

            case PhotoLevel.Rare:
                RarePhotos.Add(newPhoto);
                break;

            case PhotoLevel.Epic:
                EpicPhotos.Add(newPhoto);
                break;
            case PhotoLevel.Legend:
                LegendPhotos.Add(newPhoto);
                break;
        }


    }
    public void BuyingCard(PhotoLevel level = PhotoLevel.None)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("LibraryController: GameManager.Instance == null");
            return;
        }

        PhotoElement photo;

        if (level == PhotoLevel.None)
            photo = RandomPhoto();
        else
            photo = NoRandomPhoto(level);

        // Если подходящей карты нет — ничего не делаем.
        if (photo == null)
        {
            Debug.LogWarning(
                $"LibraryController: нет доступных карт уровня {level}"
            );

            ClosePanel();
            return;
        }

        if (string.IsNullOrEmpty(photo.ID))
        {
            Debug.LogError(
                "LibraryController: у выбранной карты отсутствует ID"
            );

            ClosePanel();
            return;
        }

        if (BackgroundAnim != null)
        {
            BackgroundAnim.gameObject.SetActive(true);
            BackgroundAnim.Play( BackstartAnim,0, 0f);
        }

        if (CardAnim != null)
            CardAnim.gameObject.SetActive(true);

        gemsForPlayer = GetDuplicateReward(photo.levelCard);

        switch (photo.levelCard)
        {
            case PhotoLevel.Common:

                if (Background != null)
                    Background.sprite = CommonBack;

                if (CloseCardImage != null)
                    CloseCardImage.sprite = CommonCard;

                photo.level = "Обычная";

                break;

            case PhotoLevel.Rare:

                if (Background != null)
                    Background.sprite = RareBack;

                if (CloseCardImage != null)
                    CloseCardImage.sprite = RareCard;

                photo.level = "Редкая";

                break;

            case PhotoLevel.Epic:

                if (Background != null)
                    Background.sprite = EpicBack;

                if (CloseCardImage != null)
                    CloseCardImage.sprite = EpicCard;

                photo.level = "Эпическая";

                break;

            case PhotoLevel.Legend:

                if (Background != null)
                    Background.sprite = LegendBack;

                if (CloseCardImage != null)
                    CloseCardImage.sprite = LegendCard;

                photo.level = "Легендарная";

                break;
        }

        if (!CardsPlayer.Contains(photo.ID))
        {
            gemsForPlayer = -1;

            if (CardImage != null)
                CardImage.sprite = photo.Photo;

            if (nameCard != null)
                nameCard.text = photo.Name;

            activePhoto = photo;

            CardsPlayer.Add(photo.ID);

            if (GameManager.Instance.profileController != null)
            {
                GameManager.Instance.profileController.UpdateCardCount(CardsPlayer.Count);
            }

            if (CardsPlayer.Count == 1)
            {
                GameManager.Instance.taskController.TaskCompleted("firstPhoto");
            }

            GameManager.Instance.taskController .TaskCompleted("fullPhoto");
        }
        else
        {
            activePhoto = null;

            if (CardImage != null)
                CardImage.sprite = GemsCard;

            if (nameCard != null)
                nameCard.text = "+" + gemsForPlayer;
        }

        if (CardAnim != null)
        {
            CardAnim.Play(startAnim,0,0f );
        }

    }
    private int GetDuplicateReward(PhotoLevel level)
    {
        switch (level)
        {
            case PhotoLevel.Common:
                return 5;

            case PhotoLevel.Rare:
                return 10;

            case PhotoLevel.Epic:
                return 15;

            case PhotoLevel.Legend:
                return 20;

            default:
                return -1;
        }
    }
    public void OpeningCard()
    {
        CreatingPhotoElement();
        CardAnim.Play(destroyCard, 0, 0f);
    }
    public void CreatingPhotoElement()
    {
        if (activePhoto != null)
        {
            var card = Instantiate(CardPrefab, parentForCard);
            card.GetComponent<CardElement>().Init(activePhoto);

            CardsElements.Add(card);
            FullCard();
            activePhoto = null;

        }
        else if (gemsForPlayer != -1)
        {
            GameManager.Instance.AddGems(gemsForPlayer);
            gemsForPlayer = -1;
        }
        GameManager.Instance.Save();
    }
    public void LoadingCardsCreate(string idCard)
    {

        var findCard = fullCards.FirstOrDefault(c => c.ID == idCard);
        if (findCard != null)
        {
            var card = Instantiate(CardPrefab, parentForCard);
            findCard.level = GetStringLevel(findCard.levelCard);
            card.GetComponent<CardElement>().Init(findCard);
            card.GetComponent<CardElement>().CloseHint();

            CardsElements.Add(card);
            CardsPlayer.Add(idCard);
            FullCard();
        }
    }
    public void NewLoadingCardsCreate(TestPhotoElement data)
    {
        var newCard = Instantiate(CardPrefab, parentForCard);
        newCard.GetComponent<CardElement>().Init(data, SearchPhotoByGuid(data.GUID));
        newCard.GetComponent<CardElement>().CloseHint();
        CardsElements.Add(newCard);
        FullCard();

    }
    public string GetStringLevel(PhotoLevel level)
    {
        string result = string.Empty;
        switch (level)
        {
            case PhotoLevel.Common:
                result = "Обычная";
                break;

            case PhotoLevel.Rare:
                result = "Редкая";
                break;
            case PhotoLevel.Epic:
                result = "Эпическая";
                break;
            case PhotoLevel.Legend:
                result = "Легендарная";
                break;
        }
        return result;
    }
    public void ClosePanel()
    {
        BackgroundAnim.gameObject.SetActive(false);
        CardAnim.gameObject.SetActive(false);

    }
    public PhotoElement RandomPhoto()
    {
        float chance = Random.Range(0f, 100f);

        PhotoElement photo = null;

        if (chance < CommonChance)
        {
            photo = RandomPhotoInList(CommonPhotos);
        }
        else if (chance < RareChance)
        {
            photo = RandomPhotoInList(RarePhotos);
        }
        else if (chance < EpicChance)
        {
            photo = RandomPhotoInList(EpicPhotos);
        }
        else if (chance < LegendChance)
        {
            photo = RandomPhotoInList(LegendPhotos);
        }

        if (photo == null)
        {
            photo = GetAnyAvailablePhoto();
        }

        return photo;
    }
    private PhotoElement GetAnyAvailablePhoto()
    {
        PhotoElement photo;

        photo = RandomPhotoInList(CommonPhotos);

        if (photo != null)
            return photo;

        photo = RandomPhotoInList(RarePhotos);

        if (photo != null)
            return photo;

        photo = RandomPhotoInList(EpicPhotos);

        if (photo != null)
            return photo;

        photo = RandomPhotoInList(LegendPhotos);

        return photo;
    }
    public PhotoElement NoRandomPhoto(PhotoLevel level)
    {
        switch (level)
        {
            case PhotoLevel.Common:
                return RandomPhotoInList(CommonPhotos);
            case PhotoLevel.Rare:
                return RandomPhotoInList(RarePhotos);
            case PhotoLevel.Epic:
                return RandomPhotoInList(EpicPhotos);
            case PhotoLevel.Legend:
                return RandomPhotoInList(LegendPhotos);
            default:
                return GetAnyAvailablePhoto();
        }
    }
    public PhotoElement RandomPhotoInList(List<PhotoElement> list)
    {
        if (list == null || list.Count == 0)
            return null;

        List<PhotoElement> validPhotos =
            list.Where(p => p != null).ToList();

        if (validPhotos.Count == 0)
            return null;

        return validPhotos[
            Random.Range(0, validPhotos.Count)
        ];
    }
    Sprite defoultSprite;
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
    public void UpdateCountCard()
    {
        cardsPlayerCountText.text = $"{CardsPlayer.Count}/{FullCardsCount()}";
    }
    public int FullCardsCount()
    {
        return CommonPhotos.Count + RarePhotos.Count + EpicPhotos.Count + LegendPhotos.Count;
    }
    public int CardsCountLevel(PhotoLevel level)
    {

        switch (level)
        {
            case PhotoLevel.Common:
                return CommonPhotos.Count;
            case PhotoLevel.Rare:
                return RarePhotos.Count;
            case PhotoLevel.Epic:
                return EpicPhotos.Count;
            case PhotoLevel.Legend:
                return LegendPhotos.Count;

        }
        return 0;
    }


    public void FullCard()
    {
        foreach (var photo in CardsElements)
            photo.SetActive(true);

        ActiveElements = CardsElements;
        UpdateCountCard();
    }
    public void SearchCards(PhotoLevel level)
    {
        foreach (var photo in CardsElements)
            photo.SetActive(false);

        var cards = CardsElements.FindAll(
            l => l.GetComponent<CardElement>().data.levelCard == level);

        foreach (var card in cards)
            card.SetActive(true);

        cardsPlayerCountText.text = $"{cards.Count}/{CardsCountLevel(level)}";




    }

    public void OpenInfoImageWindow(PhotoElement photo)
    {
        NamePhoto.text = photo.Name;
        DescriptionPhoto.text = photo.Description;
        IconPhoto.sprite = photo.Photo;
        InfoPhotoWindow.SetBool("Open", true);


    }
    public void CloseInfoPanel()
    {
        InfoPhotoWindow.SetBool("Open", false);
    }
}
public enum PhotoLevel
{
    None, Common, Rare, Epic, Legend
}
[System.Serializable]
public class PhotoElement
{
    [SerializeField] private string id;
    public string ID => id;

    public Sprite Photo;
    public string Name;
    public string Description;
    public PhotoLevel levelCard;
    public string level;
    public void GetId(string newGuid)
    {
        id = newGuid;
    }

#if UNITY_EDITOR
    public void GenedateId()
    {
        if (string.IsNullOrEmpty(ID))
            id = System.Guid.NewGuid().ToString();
        if (Photo.name != null && string.IsNullOrEmpty(Description))
        {
            Description = Photo.name;
            Description = Description.Replace("_0", "");

        }
    }
#endif
}