using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] public int PlayerClikcCount;
    public int fullMonyePlayer;
    [SerializeField] public int fullClicks;
    [SerializeField] public int StepClick = 1;
    [SerializeField] public TextMeshProUGUI PlayerClicksText;
    [SerializeField] TextMeshProUGUI StepClickText;

    // улучшение случайного бонусного клика
    public float ChanceBonusClick = 0;
    public int bonusStep = 8;

    // автополучение алмазов
    public float timeForGems = 10;
    private float timerGems;
    public int gemsAvtoCount = 0;

    public int bonusBust = 1;

    [Space]
    [Space]
    [Space]
    // кнопка улучшения ручного клика
    [SerializeField] public int PriceForUpgrade = 100;
    [SerializeField] Button UpgradeButton;
    [SerializeField] TextMeshProUGUI PriceForUpgradeText;
    [Space]
    [Space]
    [Space]
    // автоклик
    [SerializeField] public int PriceForAutoClick = 150;
    [SerializeField] Button UpgradeAutoClick;
    [SerializeField] public float CooldownAutoClick = 1;
    [SerializeField] public int AutoClickCount = 0;
    [SerializeField] TextMeshProUGUI PriceAutoClickText;
    [SerializeField] TextMeshProUGUI AutoClickCountText;
    private float currentTime;
    [Space]
    [Space]
    [Space]
    [SerializeField] public int GemsBalance;
    [SerializeField] TextMeshProUGUI GemsBalanceText;
    public UnityAction OnClick;

    [Header("Save")]
    [SerializeField] Button SaveBtn;
    [SerializeField] Button LoadBtn;

    [SerializeField] public TaskController taskController;
    [SerializeField] public ShopController shopController;
    [SerializeField] public LibraryController libraryController;
    [SerializeField] public ProfileController profileController;
    [SerializeField] public SaveController saveController;
    [SerializeField] public PlayerStatisticController statisticController;
    [SerializeField] public WheelController wheelController;
    [SerializeField] public OfflineBonusController offlineBonusController;

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<GameManager>();

            return instance;
        }
    }
    private void Awake()
    {
        //if (Application.platform == RuntimePlatform.Android)
        Load();



        SaveBtn.onClick.AddListener(Save);
        LoadBtn.onClick.AddListener(Load);

        OnClick += ClickPlayer;
        OnClick += UpdatePlayerClickCount;
        OnClick += CheckingBalancePlayerForUpgrade;
        OnClick += UpdatePriceForUpgrade;
        OnClick += profileController.UpdateFullClick;
        OnClick += profileController.UpdateMonyeCount;


        UpgradeButton.onClick.AddListener(UpgradeStepClick);
        UpgradeAutoClick.onClick.AddListener(UpgradingAutoClick);

        taskController.CreatingTask();
        shopController.Init();

        wheelController.StateButton();
    }
    private void Start()
    {
        bonusBust = 1;
        FullUpdate();
    }
    private void Update()
    {
        AutoClicking();
        AutoGems();
    }
    public void FullUpdate()
    {
        UpdatePlayerClickCount();
        UpdateStepText();
        UpdateAutoClick();
        UpdatePriceForUpgrade();
        CheckingBalancePlayerForUpgrade();
        UpdatingGemsText();

        shopController.UpdateHint();
        shopController.UpdateFullTextLevel();
        shopController.UpdateText();

        profileController.UpdateMonyeCount();
        profileController.UpdateFullClick();
    }
    public void UpdatePlayerClickCount()
    {


        PlayerClicksText.text = FormatingString(PlayerClikcCount);
    }
    public string FormatingString(int number)
    {
        string[] suffixes = { "", "K", "M", "B", "T" };
        int index = 0;
        double numPlayer = number;
        string result;

        while (numPlayer >= 1000 && index < suffixes.Length - 1)
        {
            numPlayer /= 1000;
            index++;
        }
        if (numPlayer % 1 == 0)
            result = $"{numPlayer:0}{suffixes[index]}";
        else result = $"{numPlayer:0.0}{suffixes[index]}";

        return result;
    }
    public void UpdateStepText()
    {
        StepClickText.text = "+" + FormatingString(StepClick);
    }
    public void UpdateAutoClick()
    {
        AutoClickCountText.text = AutoClickCount.ToString();
    }
    public void ClickPlayer()
    {
        fullClicks++;
        if (RandomClickBonus())
        {
            Debug.Log("BONUS");
        }
        else
        {
            PlayerClikcCount += StepClick * bonusBust;
            fullMonyePlayer += StepClick * bonusBust;
        }


        taskController.TaskCompleted("100Cliks");


    }
    public void UpdatingGemsText()
    {
        GemsBalanceText.text = FormatingString(GemsBalance);
    }
    public void CheckingBalancePlayerForUpgrade()
    {
        UpgradeButton.interactable = (PlayerClikcCount >= PriceForUpgrade) ? true : false;
        UpgradeAutoClick.interactable = (PlayerClikcCount >= PriceForAutoClick) ? true : false;
    }
    public void UpdatePriceForUpgrade()
    {
        PriceForUpgradeText.text = FormatingString(PriceForUpgrade);
        PriceAutoClickText.text = FormatingString(PriceForAutoClick);
    }
    public void UpgradeStepClick()
    {
        PlayerClickCountMinus(PriceForUpgrade);
        AddStepClick();
        PriceForUpgrade = PriceForUpgrade * 2;

        FullUpdate();


    }

    public void UpgradingAutoClick()
    {
        PlayerClickCountMinus(PriceForAutoClick);
        AutoClickCount++;
        PriceForAutoClick = PriceForAutoClick * 2;
        taskController.TaskCompleted("4avtoClick");
        FullUpdate();
    }
    public void PlayerClickCountMinus(int count)
    {
        PlayerClikcCount -= count;
    }
    public void AutoClicking()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= CooldownAutoClick)
        {
            PlayerClikcCount += AutoClickCount * bonusBust;
            fullMonyePlayer += AutoClickCount * bonusBust;
            currentTime = 0;
            FullUpdate();
        }
    }
    public void AddStepClick(int step = 1, bool add = true)
    {
        if (add)
            StepClick += step;
        else StepClick *= step;
    }
    public void AddGems(int gems)
    {
        GemsBalance += gems * bonusBust;
        FullUpdate();
    }
    public bool RandomClickBonus()
    {
        float rnd = Random.Range(0f, 100f);
        if (rnd < ChanceBonusClick)
        {
            PlayerClikcCount += StepClick * bonusStep;
            fullMonyePlayer += StepClick * bonusStep;
            return true;
        }
        else
            return false;
    }

    public void AutoGems()
    {
        timerGems += Time.deltaTime;
        if (timerGems >= timeForGems)
        {
            timerGems = 0f;
            AddGems(gemsAvtoCount);

        }
    }
    public void Save()
    {
        saveController.Save();
    }
    public void Load()
    {
        SaveSystem.SaveData data = saveController.LoadingData();

        if (data.saveVersion == 1) return;

        PlayerClikcCount = data.PlayerClickBalance;
        GemsBalance = data.PlayerGemsBalance;
        fullClicks = data.fullClicksCount;
        fullMonyePlayer = data.fullPlayerClicksCount;

        StepClick = data.stepClick;

        PriceForUpgrade = data.priceStepClick;
        PriceForAutoClick = data.PriceAvtoClick;
        AutoClickCount = data.AvtoClickStep;

        var cards = data.playerCards;

        if (data.tasks.Count != 0)
        {
            taskController.tasks = data.tasks;
        }
        foreach (var card in cards)
        {
            libraryController.LoadingCardsCreate(card);
        }

        shopController.levelForce = data.levelForce;
        shopController.levelCrit = data.levelCrit;
        shopController.levelDohod = data.levelDohod;

        shopController.priceForce = data.forcePrice;
        shopController.priceCrit = data.CritPrice;
        shopController.priceDohod = data.DohodPrice;

        ChanceBonusClick = shopController.crits[shopController.levelCrit - 1];
        gemsAvtoCount = shopController.dohods[shopController.levelDohod - 1];

        statisticController.levelsCompleted = data.levelsCompleted;
        statisticController.CurrentPlayerLevel = data.playerPointsLevels;

        offlineBonusController.SetExitTime(data.exitTime);


        wheelController.VideoWinner.videosInLibrary = data.videosInLibrary;
        wheelController.VideoWinner.GetVideos();

        wheelController.FactsWinner.textsInLibrary = data.factsInLibrary;
        wheelController.FactsWinner.GetTexts();

        wheelController.HistoryWinner.textsInLibrary = data.historyInLibrary;
        wheelController.HistoryWinner.GetTexts();


        FullUpdate();
    }

    public void AddPLayerClick(int clicks)
    {
        PlayerClikcCount += clicks;
        FullUpdate();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause == true) Save();
    }
    private void OnApplicationQuit()
    {
        Save();
    }
}
