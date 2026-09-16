using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskController : MonoBehaviour
{
    [SerializeField] public List<Task> tasks = new List<Task>();
    [SerializeField] public List<TaskElement> elements = new List<TaskElement>();
    [SerializeField] GameObject prefabTask;
    [SerializeField] Transform parentGrid;


    public void CreatingTask()
    {
        elements.Clear();

        if (tasks == null)
            tasks = new List<Task>();

        if (prefabTask == null)
        {
            Debug.LogError("TaskController: prefabTask не назначен!");
            return;
        }

        if (parentGrid == null)
        {
            Debug.LogError("TaskController: parentGrid не назначен!");
            return;
        }

        for (int i = 0; i < tasks.Count; i++)
        {
            CreateTask(i);
        }

        Debug.Log($"TaskController: создано заданий = {elements.Count}");
    }

    public void CreateTask(int i)
    {
        if (tasks == null)
            return;

        if (i < 0 || i >= tasks.Count)
            return;

        if (tasks[i] == null)
            return;

        GameObject taskObject = Instantiate(
            prefabTask,
            parentGrid
        );

        if (taskObject == null)
            return;

        TaskElement taskElement =
            taskObject.GetComponent<TaskElement>();

        if (taskElement == null)
        {
            Debug.LogError("TaskController: на prefabTask нет TaskElement!");

            Destroy(taskObject);
            return;
        }

        taskElement.Init(tasks[i]);

        elements.Add(taskElement);
    }

    public void TaskCompleted(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        if (elements == null || elements.Count == 0)
            return;

        List<TaskElement> currentElements =
            new List<TaskElement>(elements);

        foreach (TaskElement task in currentElements)
        {
            if (task == null)
                continue;

            if (task.taskData == null)
                continue;

            if (task.taskData.keyForTask != key)
                continue;

            if (task.isCompleted)
                continue;

            task.AddStep();
        }
    }

    public void RemoveTask(TaskElement task)
    {
        if (task == null)
            return;

        if (elements == null)
            return;

        if (elements.Contains(task))
        {
            elements.Remove(task);
        }
    }
}

[Serializable]
public class Task
{
    public string name;
    public string description;
    public string keyForTask;

    public float needStep;
    public float currentStep;

    public int gems;
    public float LevelPoints;
}