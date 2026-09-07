using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ClicksText;
    [SerializeField] TextMeshProUGUI GemsText;

    [Space]
    [Space]
    [SerializeField] GameObject UpgradeWindow;
    [SerializeField] GameObject BustWindow;
    [SerializeField] GameObject CaseWindow;

    [SerializeField] Button UpgradeBtn;
    [SerializeField] Button BustBtn;
    [SerializeField] Button CaseBtn;

    [Space]
    [Space]
    [Space]
    [Header("Улучшения")]
    [SerializeField] Button ForceClickBtn;
    [SerializeField] TextMeshProUGUI ForceLevelText;
    [SerializeField] TextMeshProUGUI ForceHint;
    public int levelForce = 1;
    public int priceForce = 10;
    private int[] forces = { 0, 1, 3, 5, 10, 2, 3, 4, 5, 0 };

    [Space]
    [SerializeField] Button CritBtn;
    [SerializeField] TextMeshProUGUI CritLevelText;
    [SerializeField] TextMeshProUGUI CritHint;
    public int levelCrit = 1;
    public int priceCrit = 15;
    public int[] crits = { 0, 10, 15, 20, 30, 50, 70, 80, 100, 0 };

    [Space]
    [SerializeField] Button DohodBtn;
    [SerializeField] TextMeshProUGUI DohodLevelText;
    [SerializeField] TextMeshProUGUI DohodHint;
    public int levelDohod = 1;
    public int priceDohod = 20;
    public  int[] dohods = { 0, 1, 3, 5, 10, 20, 60, 100, 0 };


    [Header("Бустеры")]
    [SerializeField] Button SpeedBustBtn;
    [Space]
    [SerializeField] GameObject BustText;
    private int priceSpeedBust = 100;

    [Header("Кейсы")]
    [SerializeField] Button RandomBtn;
    float priceRandomCase = 10;

    [SerializeField] Button CommonBtn;
    float priceCommonCase = 20;

    [SerializeField] Button RareBtn;
    float priceRareCase = 50;

    [SerializeField] Button EpicBtn;
    float priceEpicCase = 100;

    [SerializeField] Button LegendBtn;
    float priceLegendCase = 150;



    private float timeForBust = 30;
    private float timerBust;
    private int bust = 2;



    private void Start()
    {
       
    }

    public void Init()
    {
        UpgradeBtnInit();
        BustBtnInit();
        CaseBtnInit();

        WindowBtnInit();
        UpdateFullTextLevel();

      



        timerBust = timeForBust + 1;
    }
    private void Update()
    {
        TimerForBust();
    }
    public void WindowBtnInit()
    {
        UpgradeBtn.onClick.AddListener(() => OpenWindow(UpgradeWindow));
        BustBtn.onClick.AddListener(() => OpenWindow(BustWindow));
        CaseBtn.onClick.AddListener(() => OpenWindow(CaseWindow));
    }
    public void BustBtnInit()
    {
        SpeedBustBtn.onClick.AddListener(StartBust);
    }
    public void CaseBtnInit()
    {
        PriceInit(RandomBtn, priceRandomCase);
        RandomBtn.onClick.AddListener(() =>
        {
            PlayerClickMinus(priceRandomCase);
            RandomBuyingPhoto();

        });

        PriceInit(CommonBtn, priceCommonCase);
        CommonBtn.onClick.AddListener(() =>
        {
            PlayerClickMinus(priceCommonCase);
            RandomBuyingPhoto(PhotoLevel.Common);

        }
        );

        PriceInit(RareBtn, priceRareCase);
        RareBtn.onClick.AddListener(() =>
        {
            PlayerClickMinus(priceRareCase);
            RandomBuyingPhoto(PhotoLevel.Rare);
        });

        PriceInit(EpicBtn, priceEpicCase);
        EpicBtn.onClick.AddListener(() =>
        {
            PlayerClickMinus(priceEpicCase);
            RandomBuyingPhoto(PhotoLevel.Epic);
        });

        PriceInit(LegendBtn, priceLegendCase);
        LegendBtn.onClick.AddListener(() =>
        {
            PlayerClickMinus(priceLegendCase);

            RandomBuyingPhoto(PhotoLevel.Legend);
        });

    }
    public void UpgradeBtnInit()
    {
        ForceClickBtn.onClick.AddListener(
                    () =>
                    {
                        if (GameManager.Instance.PlayerClikcCount >= priceForce)
                        {
                            GameManager.Instance.PlayerClickCountMinus(priceForce);
                            priceForce += 50;
                            if (levelForce >= 5)
                                GameManager.Instance.AddStepClick(forces[levelForce], false);

                            else
                                GameManager.Instance.AddStepClick(forces[levelForce]);

                            levelForce++;
                            GameManager.Instance.FullUpdate();
                            if (levelForce == forces.Length - 1)
                            {
                                ForceHint.text = "макс.";
                                ForceClickBtn.gameObject.SetActive(false);
                            }
                        }
                    }
                    );

        CritBtn.onClick.AddListener(
            () =>
            {
                if (GameManager.Instance.PlayerClikcCount >= priceCrit)
                {
                    GameManager.Instance.PlayerClickCountMinus(priceCrit);
                    priceCrit += 50;
                    GameManager.Instance.ChanceBonusClick = crits[levelCrit];
                    levelCrit++;
                    GameManager.Instance.FullUpdate();
                    if (levelCrit == crits.Length - 1)
                    {
                        CritHint.text = "макс.";
                        CritBtn.gameObject.SetActive(false);
                    }
                }

            }
            );
        DohodBtn.onClick.AddListener(
            () =>
            {
                if (GameManager.Instance.PlayerClikcCount >= priceDohod)
                {
                    GameManager.Instance.PlayerClickCountMinus(priceDohod);
                    priceDohod += 100;
                    GameManager.Instance.gemsAvtoCount = dohods[levelDohod];
                    levelDohod++;
                    GameManager.Instance.FullUpdate();
                    if (levelDohod == dohods.Length - 1)
                    {
                        DohodHint.text = "макс.";
                        DohodBtn.gameObject.SetActive(false);
                    }
                }
            }
        );

    }
    public void UpdateText()
    {
        ClicksText.text = GameManager.Instance.PlayerClicksText.text.ToString();
        GemsText.text = GameManager.Instance.GemsBalance.ToString();

    }



    public void UpdateFullTextLevel()
    {

        UpdateText(ForceLevelText, ForceClickBtn, levelForce, priceForce);
        UpdateText(CritLevelText, CritBtn, levelCrit, priceCrit);
        UpdateText(DohodLevelText, DohodBtn, levelDohod, priceDohod);

        UpdateBustBtn();
        UpdateCaseBtn();


    }

    public void UpdateText(TextMeshProUGUI text, Button button, int level, int price)
    {
        text.text = $"Уровень {level}";
        button.GetComponentInChildren<TextMeshProUGUI>().text = price.ToString();
        button.interactable = CheckInteractable(price);
    }
    public void UpdateBustBtn()
    {
        SpeedBustBtn.GetComponentInChildren<TextMeshProUGUI>().text = priceSpeedBust.ToString();
        SpeedBustBtn.interactable = CheckInteractable(priceSpeedBust);
    }
    public void UpdateCaseBtn()
    {
        RandomBtn.interactable = CheckInteractable(priceRandomCase,true);
        CommonBtn.interactable = CheckInteractable(priceCommonCase, true);
        RareBtn.interactable = CheckInteractable(priceRareCase, true    );
        EpicBtn.interactable = CheckInteractable(priceEpicCase, true);
        LegendBtn.interactable = CheckInteractable(priceLegendCase, true);
    }
    public bool CheckInteractable(float price, bool gems = false)
    {
        if (gems == false)
            return (GameManager.Instance.PlayerClikcCount >= price) ? true : false;
        else return (GameManager.Instance.GemsBalance >= price) ? true : false;
    }
    public void UpdateHint()
    {
        char c = levelForce >= 5 ? 'x' : '+';
        if (levelForce < forces.Length - 1)
            ForceHint.text = $"{c}{forces[levelForce]} за клик";
        if (levelCrit < crits.Length - 1)
            CritHint.text = $"шанс {crits[levelCrit]}% на удар х8";
        if (levelDohod < dohods.Length - 1)
            DohodHint.text = $"+{dohods[levelDohod]} алмазов в 10 секунд";
    }

    public void OpenWindow(GameObject window)
    {
        UpgradeWindow.SetActive(false);
        BustWindow.SetActive(false);
        CaseWindow.SetActive(false);

        window.SetActive(true);
    }

    public void StartBust()
    {


        GameManager.Instance.bonusBust = bust;
        timerBust = 0;
        GameManager.Instance.PlayerClickCountMinus(priceSpeedBust);
        BustText.SetActive(true);
        GameManager.Instance.FullUpdate();
    }
    public void TimerForBust()
    {
        if (timerBust != timeForBust + 1)
        {

            timerBust += Time.deltaTime;
            if (timerBust >= timeForBust)
            {
                timerBust = timeForBust + 1;
                GameManager.Instance.bonusBust = 1;
                BustText.SetActive(false);
                GameManager.Instance.FullUpdate();
                SpeedBustBtn.interactable = true;
            }
            else
            {

                SpeedBustBtn.interactable = false;
                SpeedBustBtn.GetComponent<Image>().fillAmount = timerBust / timeForBust;

            }
        }

    }
    public void RandomBuyingPhoto(PhotoLevel level = PhotoLevel.None)
    {
        GameManager.Instance.libraryController.BuyingCard(level);
    }
    public void PriceInit(Button btn, float price)
    {
        btn.GetComponentInChildren<TextMeshProUGUI>().text = price.ToString();
    }
    public void PlayerClickMinus(float price)
    {
        GameManager.Instance.AddGems(-(int)price);
    }
}
