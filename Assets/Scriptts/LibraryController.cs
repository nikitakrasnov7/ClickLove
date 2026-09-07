using System.Collections.Generic;
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
        foreach (var p in CommonPhotos)
            p.levelCard = PhotoLevel.Common;

        foreach (var p in RarePhotos)
            p.levelCard = PhotoLevel.Rare;

        foreach (var p in EpicPhotos)
            p.levelCard = PhotoLevel.Epic;
        foreach (var p in LegendPhotos)
            p.levelCard = PhotoLevel.Legend;

        fullCards.AddRange(CommonPhotos);
        fullCards.AddRange(RarePhotos);
        fullCards.AddRange(EpicPhotos);
        fullCards.AddRange(LegendPhotos);

        FullBtn.onClick.AddListener(FullCard);

        CommonBtn.onClick.AddListener(() => SearchCards(PhotoLevel.Common));
        RareBtn.onClick.AddListener(() => SearchCards(PhotoLevel.Rare));
        EpicBtn.onClick.AddListener(() => SearchCards(PhotoLevel.Epic));
        LegendBtn.onClick.AddListener(() => SearchCards(PhotoLevel.Legend));

        CloseInfoPanelBtn.onClick.AddListener(CloseInfoPanel);
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
    public void BuyingCard(PhotoLevel level = PhotoLevel.None)
    {
        BackgroundAnim.gameObject.SetActive(true);
        CardAnim.gameObject.SetActive(true);

        BackgroundAnim.Play(BackstartAnim, 0, 0f);
        var photo = (level == PhotoLevel.None) ? RandomPhoto() : NoRandomPhoto(level);
        gemsForPlayer = -1;
        switch (photo.levelCard)
        {
            case PhotoLevel.Common:
                Background.sprite = CommonBack;
                CloseCardImage.sprite = CommonCard;
                photo.level = "Обычная";
                gemsForPlayer = 5;
                break;

            case PhotoLevel.Rare:
                Background.sprite = RareBack;
                CloseCardImage.sprite = RareCard;
                photo.level = "Редкая";
                gemsForPlayer = 10;
                break;

            case PhotoLevel.Epic:
                Background.sprite = EpicBack;
                CloseCardImage.sprite = EpicCard;
                photo.level = "Эпическая";
                gemsForPlayer = 15;
                break;

            case PhotoLevel.Legend:
                Background.sprite = LegendBack;
                CloseCardImage.sprite = LegendCard;
                photo.level = "Легендарная";
                gemsForPlayer = 20;
                break;
        }
        if (!CardsPlayer.Contains(photo.ID))
        {
            gemsForPlayer = -1;
            CardImage.sprite = photo.Photo;
            nameCard.text = photo.Name;
            CardAnim.Play(startAnim, 0, 0f);

            activePhoto = photo;
            CardsPlayer.Add(activePhoto.ID);

            GameManager.Instance.profileController.UpdateCardCount(CardsPlayer.Count);


            if (CardsPlayer.Count == 1)
                GameManager.Instance.taskController.TaskCompleted("firstPhoto");


            GameManager.Instance.taskController.TaskCompleted("fullPhoto");


        }
        else
        {
            activePhoto = null;
            CardImage.sprite = GemsCard;
            nameCard.text = "+" + gemsForPlayer;
            CardAnim.Play(startAnim, 0, 0f);

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
        PhotoElement photo = new();
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
        return photo;
    }
    public PhotoElement NoRandomPhoto(PhotoLevel level)
    {
        PhotoElement photo = new();
        switch (level)
        {
            case PhotoLevel.Common:
                photo = RandomPhotoInList(CommonPhotos);
                break;
            case PhotoLevel.Rare:
                photo = RandomPhotoInList(RarePhotos);
                break;
            case PhotoLevel.Epic:
                photo = RandomPhotoInList(EpicPhotos);
                break;
            case PhotoLevel.Legend:
                photo = RandomPhotoInList(LegendPhotos);
                break;
        }
        return photo;
    }
    public PhotoElement RandomPhotoInList(List<PhotoElement> list)
    {
        return list[Random.Range(0, list.Count)];
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
        if(Photo.name!=null && string.IsNullOrEmpty(Description))
        {
            Description = Photo.name;
            Description = Description.Replace("_0","");

        }
    }
#endif
}