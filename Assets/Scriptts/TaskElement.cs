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
    
    private bool rewardTaken = false;

    public void AddStep()
    {
        if (taskData == null)
            return;

        if (isCompleted)
            return;

        if (taskData.currentStep >= taskData.needStep)
        {
            CheckSteps();
            return;
        }

        taskData.currentStep++;

        if (taskData.currentStep > taskData.needStep)
        {
            taskData.currentStep = taskData.needStep;
        }

        UpdateData();
        CheckSteps();
    }

    public void CheckSteps()
    {
        if (taskData == null)
            return;

        if (taskData.currentStep >= taskData.needStep)
        {
            taskData.currentStep = taskData.needStep;

            isCompleted = true;

            if (gemsBtn != null)
            {
                gemsBtn.interactable = true;
            }

            UpdateData();
        }
        else
        {
            isCompleted = false;

            if (gemsBtn != null)
            {
                gemsBtn.interactable = false;
            }
        }
    }

    public void TakeGems()
    {
        if (rewardTaken)
            return;

        if (!isCompleted)
            return;

        if (taskData == null)
            return;

        rewardTaken = true;

        // Выдаём награду.
        if (taskData.gems > 0)
        {
            GameManager.Instance.AddGems(taskData.gems);
        }

        if (taskData.LevelPoints > 0)
        {
            GameManager.Instance.statisticController
                .AddPlayerPoints(taskData.LevelPoints);
        }

        // Убираем задание из списка активных.
        if (GameManager.Instance.taskController != null)
        {
            GameManager.Instance.taskController.RemoveTask(this);
        }

        // СРАЗУ сохраняем.
        GameManager.Instance.Save();

        Destroy(gameObject);
    }

    public void Init(Task task)
    {
        taskData = task;

        rewardTaken = false;

        if (taskData == null)
        {
            Debug.LogError(
                "TaskElement: Init получил null Task"
            );

            return;
        }

        if (gemsBtn != null)
        {
            gemsBtn.onClick.RemoveListener(TakeGems);
            gemsBtn.onClick.AddListener(TakeGems);

            TextMeshProUGUI buttonText =
                gemsBtn.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                buttonText.text = taskData.gems.ToString();
            }
        }

        UpdateData();
        CheckSteps();
    }

    public void UpdateData()
    {
        if (taskData == null)
            return;

        if (nameTask != null)
        {
            nameTask.text = taskData.name;
        }

        if (descriptionTask != null)
        {
            descriptionTask.text = taskData.description;
        }

        if (textProgressTask != null)
        {
            textProgressTask.text =
                $"{taskData.currentStep}/{taskData.needStep}";
        }

        if (progressTask != null)
        {
            progressTask.minValue = 0;
            progressTask.maxValue = taskData.needStep;
            progressTask.value = taskData.currentStep;
        }
    }
}
