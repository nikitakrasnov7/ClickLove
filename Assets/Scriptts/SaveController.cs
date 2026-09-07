using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI debug;

    public void Save()
    {

        SaveSystem.SaveData data = new SaveSystem.SaveData();


        data.PlayerClickBalance = GameManager.Instance.PlayerClikcCount;
        data.PlayerGemsBalance = GameManager.Instance.GemsBalance;
        data.fullClicksCount = GameManager.Instance.fullClicks;
        data.fullPlayerClicksCount = GameManager.Instance.fullMonyePlayer;

        data.stepClick = GameManager.Instance.StepClick;

        data.priceStepClick = GameManager.Instance.PriceForUpgrade;
        data.PriceAvtoClick = GameManager.Instance.PriceForAutoClick;
        data.AvtoClickStep = GameManager.Instance.AutoClickCount;

        data.playerCards = GameManager.Instance.libraryController.CardsPlayer;

        var tasksPlayer = GameManager.Instance.taskController.elements;
        var tasks = new List<Task>();
        foreach (var task in tasksPlayer)
            tasks.Add(task.taskData);

        data.tasks = tasks;

        data.levelForce = GameManager.Instance.shopController.levelForce;
        data.levelCrit = GameManager.Instance.shopController.levelCrit;
        data.levelDohod = GameManager.Instance.shopController.levelDohod;

        data.forcePrice = GameManager.Instance.shopController.priceForce;
        data.CritPrice = GameManager.Instance.shopController.priceCrit;
        data.DohodPrice = GameManager.Instance.shopController.priceDohod;

        data.levelsCompleted = GameManager.Instance.statisticController.levelsCompleted;
        data.playerPointsLevels = GameManager.Instance.statisticController.CurrentPlayerLevel;

        data.exitTime = GameManager.Instance.offlineBonusController.GetExitTime();

        data.videosInLibrary  = GameManager.Instance.wheelController.VideoWinner.videosInLibrary;
        data.factsInLibrary = GameManager.Instance.wheelController.FactsWinner.textsInLibrary;
        data.historyInLibrary = GameManager.Instance.wheelController.HistoryWinner.textsInLibrary;

        data.saveVersion = 2;
        SaveSystem.SavingData(data);

        //debug.text = $"PlayerClickCount = {data.PlayerClickBalance}" +
        //    $"PlayerGems = {data.PlayerGemsBalance}" +
        //    $"fullClicks = {data.fullClicksCount}" +
        //    $"fullPlayerClicks = {data.fullPlayerClicksCount}" +
        //    $"cards = {data.playerCards.Count}" +
        //    $"tasks = {data.tasks.Count}";

    }

    public SaveSystem.SaveData LoadingData()
    {
        SaveSystem.SaveData data = SaveSystem.LoadingData();
        if (data != null)
        {
            debug.text = data.ToString();
        }
        else
        {
            Debug.Log("error");
        }
        return data;
    }
}
