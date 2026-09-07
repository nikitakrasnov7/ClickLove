using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatisticController : MonoBehaviour
{
    [SerializeField] Slider sliderLevel;
    [SerializeField] TextMeshProUGUI textLevel;
    [SerializeField] Button test;
    public float needPointsLevels;
    public float CurrentPlayerLevel;
    public int levelsCompleted;

    private void Start()
    {
        test.onClick.AddListener(() => { AddPlayerPoints(150); });
        Init();
    }
    public void Init()
    {
        GetMaxSliderValue();
        textLevel.text = levelsCompleted.ToString();
        sliderLevel.value = CurrentPlayerLevel;
    }
    public void GetMaxSliderValue()
    {
        var maxValue = 200 * levelsCompleted + 100;
        needPointsLevels = maxValue;
        sliderLevel.maxValue = maxValue;
    }
    public void AddPlayerPoints(float points)
    {
        CurrentPlayerLevel += points;

        if(CurrentPlayerLevel >= needPointsLevels)
        {
            levelsCompleted++;
            CurrentPlayerLevel -= needPointsLevels;
            GetMaxSliderValue();
            textLevel.text = levelsCompleted.ToString();

            GameManager.Instance.wheelController.StateButton();

        }

        sliderLevel.value = CurrentPlayerLevel;
    }
    
}
