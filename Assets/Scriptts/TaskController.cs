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
        for (int i = 0; i < tasks.Count; i++)
        {
            CreateTask(i);
        }
    }

    public void CreateTask(int i)
    {
        if (tasks[i] != null) 
        {
            var task = Instantiate(prefabTask, parentGrid);
            task.GetComponent<TaskElement>().Init(tasks[i]);
                
            elements.Add(task.GetComponent<TaskElement>());

        }
    }

    public void TaskCompleted(string key)
    {
        var task = elements.FirstOrDefault(t=> t.taskData.keyForTask == key);
        if (task != null)
        {
            task.AddStep();
            //if (task.isCompleted)
            //{
            //    elements.Remove(task);
            //}

        }
    }
    public void RemoveTask(TaskElement task)
    {
        if (elements.Contains(task))
            elements.Remove(task);
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