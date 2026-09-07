using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskElement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameTask;
    [SerializeField] TextMeshProUGUI descriptionTask;
    [SerializeField] TextMeshProUGUI textProgressTask;
    [SerializeField] Slider progressTask;
    [SerializeField] Button gemsBtn;
    public bool isCompleted;
    [SerializeField] public Task taskData;


    public void AddStep()
    {
        taskData.currentStep++;
        UpdateData();

        CheckSteps();

    }
    public void CheckSteps()
    {
        if (taskData.currentStep >= taskData.needStep)
        {
            gemsBtn.interactable = true;
            isCompleted = true;
        }
    }
    public void TakeGems()
    {
        GameManager.Instance.AddGems(taskData.gems);
        GameManager.Instance.statisticController.AddPlayerPoints(taskData.LevelPoints);
        GameManager.Instance.taskController.RemoveTask(this);
        Destroy(gameObject);
    }

    public void Init(Task task)
    {
        taskData = task;
        UpdateData();
        gemsBtn.onClick.AddListener(TakeGems);
        gemsBtn.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = taskData.gems.ToString();
        CheckSteps();


    }
    public void UpdateData()
    {
        nameTask.text = taskData.name;
        textProgressTask.text = $"{taskData.currentStep}/{taskData.needStep}";

        progressTask.maxValue = taskData.needStep;
        progressTask.minValue = 0;

        progressTask.value = taskData.currentStep;
    }

}
