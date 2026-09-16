using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public int saveVersion = 2;

        public int PlayerClickBalance;
        public int fullClicksCount;
        public int fullPlayerClicksCount;

        public int stepClick = 1;

        public int priceStepClick;
        public int PriceAvtoClick;
        public int AvtoClickStep;

        public int PlayerGemsBalance;

        public List<string> playerCards = new();
        public List<Task> tasks = new();

        public int levelForce = 1;
        public int levelCrit = 1;
        public int levelDohod = 1;

        public int forcePrice = 10;
        public int CritPrice = 15;
        public int DohodPrice = 20;

        public int levelsCompleted;
        public float playerPointsLevels;

        public string exitTime;

        public List<string> videosInLibrary = new();
        public List<string> factsInLibrary = new();
        public List<string> historyInLibrary = new();

        public SaveData()
        {
        }

        public SaveData(
            int clicks,
            int fullClicks,
            int fullPlayerClicks,
            int gems,
            List<string> cards)
        {
            PlayerClickBalance = clicks;
            fullClicksCount = fullClicks;
            fullPlayerClicksCount = fullPlayerClicks;
            PlayerGemsBalance = gems;
            playerCards = cards;
        }
    }

    private const string SAVE_NAME = "SAVE.json";

    public static void SavingData(SaveData data)
    {
        if (data == null)
        {
            Debug.LogError("SaveSystem: попытка сохранить null data");
            return;
        }

        string path = Path.Combine( Application.persistentDataPath,SAVE_NAME);

        string tempPath = path + ".tmp";

        try
        {
            string json = JsonUtility.ToJson(data, true);

            File.WriteAllText(tempPath, json);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.Move(tempPath, path);
            Debug.Log("SaveSystem: сохранение выполнено");
        }
        catch (Exception e)
        {
            Debug.LogError("SaveSystem: ошибка сохранения:\n" + e);

            try
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            catch
            {
                // Ничего не делаем.
            }
        }
    }

    public static SaveData LoadingData()
    {
        string path = Path.Combine(Application.persistentDataPath,SAVE_NAME);

        if (!File.Exists(path))
        {
            Debug.Log("SaveSystem: SAVE.json не найден. Создаём новое сохранение.");
            return new SaveData();
        }

        try
        {
            string json = File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogWarning("SaveSystem: SAVE.json пустой. Создаём новое сохранение.");
                return new SaveData();
            }

            SaveData data = JsonUtility.FromJson<SaveData>(json);

            if (data == null)
            {
                Debug.LogWarning( "SaveSystem: не удалось прочитать SAVE.json." );
                return new SaveData();
            }

            if (data.playerCards == null)
                data.playerCards = new List<string>();

            if (data.tasks == null)
                data.tasks = new List<Task>();

            if (data.videosInLibrary == null)
                data.videosInLibrary = new List<string>();

            if (data.factsInLibrary == null)
                data.factsInLibrary = new List<string>();

            if (data.historyInLibrary == null)
                data.historyInLibrary = new List<string>();

            Debug.Log($"SaveSystem: сохранение загружено. Version = {data.saveVersion}");

            return data;
        }
        catch (Exception e)
        {
            Debug.LogError( "SaveSystem: ошибка загрузки SAVE.json:\n" + e);
            return new SaveData();
        }
    }
}
