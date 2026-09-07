using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem
{
    public class SaveData
    {

        public int saveVersion = 1;

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


        public int levelForce;
        public int levelCrit;
        public int levelDohod;

        public int forcePrice;
        public int CritPrice;
        public int DohodPrice;

        public int levelsCompleted;
        public float playerPointsLevels;

        public string exitTime;

        public List<string> videosInLibrary = new();
        public List<string> factsInLibrary = new();
        public List<string> historyInLibrary = new();
        public SaveData()
        {

        }
        public SaveData(int clicks, int fullClicks, int fullPlayerClicks, int gems, List<string> cards)
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
        string path = Path.Combine(Application.persistentDataPath, SAVE_NAME);
        string tempPath = path + ".tmp";


        File.WriteAllText(path, JsonUtility.ToJson(data));
    }
    public static SaveData LoadingData()
    {
        string path = Path.Combine(Application.persistentDataPath, SAVE_NAME);

        SaveData data = new();

        if (File.Exists(path))
        {
            data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
            return data;
        }
        else return data;


    }
}
