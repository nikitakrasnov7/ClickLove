using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class OfflineBonusController : MonoBehaviour
{
    [SerializeField] GameObject BonusPanel;
    [SerializeField] TextMeshProUGUI TextBonus;
    [SerializeField] Button GetedBonus;

    DateTime exitTime;
    DateTime enterTime;
    int bonus;
    private const string SAVE_FILE_NAME = "SAVE_TIME.JSON";
    //private void Update()
    //{
    //    //if (Input.GetKeyDown(KeyCode.Alpha1))

    //    //{
    //    //    exitTime = DateTime.Now;
    //    //}
    //    //else if (Input.GetKeyDown(KeyCode.Alpha2))
    //    //{
    //    //    enterTime = DateTime.Now;
    //    //}

    //    //else if (Input.GetKeyDown(KeyCode.Alpha3))
    //    //{

    //    //    var result2 = enterTime - exitTime;

    //    //    Debug.Log(Math.Truncate(result2.TotalSeconds).ToString());


    //    //}
    //}
    //private void Awake()
    //{
    //    //var data = LoadData();

    //    //if (data.firstSave)
    //    //{
    //    //    Debug.Log("сохранений нет");
    //    //}
    //    //else
    //    //{
    //    //    Debug.Log("data");

    //    //    exitTime = DateTime.Parse(data.exitTime);
    //    //    Debug.Log(exitTime.ToString() + " время выхода");

    //    //    enterTime = DateTime.Now;
    //    //    Debug.Log(enterTime.ToString() + "время входа");

    //    //    var result = enterTime - exitTime;
    //    //    Debug.Log(result.ToString() + "разница времени");

    //    //    var resFloat = Math.Truncate(result.TotalSeconds);
    //    //    Debug.Log(resFloat.ToString() + "разница в секундах");

    //    //    var getBonus = resFloat * step;
    //    //    Debug.Log(getBonus.ToString() + "бонус");

    //    //}

    //}
    //public void OnApplicationPause(bool pause)
    //{
    //    if (pause == true)
    //    {
    //        Save();
    //    }
    //}

    //private void OnApplicationQuit()
    //{
    //    Save();
    //}

    //public void Save()
    //{
    //    exitTime = DateTime.Now;
    //    SaveData(exitTime);

    //}

    private void Start()
    {
        GetedBonus.onClick.AddListener(GetingBonus);
    }
    public string GetExitTime()
    {
        exitTime = DateTime.Now;
        return exitTime.ToString("F");
    }
    public void SetExitTime(string saveExitTime)
    {
        exitTime = DateTime.Parse(saveExitTime);
        enterTime = DateTime.Now;

        var result = enterTime - exitTime;
        var intResult = Math.Truncate(result.TotalSeconds);

        OpenPanelBonus(((int)intResult));
    }

    public void OpenPanelBonus(int bonusTime)
    {

        this.bonus = bonusTime * GameManager.Instance.AutoClickCount;
        string bon = GameManager.Instance.FormatingString(bonus);
        TextBonus.text = "+" + bon;

        if (bonus == 0)
            return;
        BonusPanel.GetComponent<RectTransform>().DOScale(Vector2.one, 1);


    }

    public void GetingBonus()
    {
        if (bonus == 0)
            return;

        GameManager.Instance.AddPLayerClick(bonus);
        BonusPanel.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f);
        bonus = 0;
    }
    //public void SaveData(DateTime time)
    //{
    //    SaveDataTime saveDataTime = new SaveDataTime();

    //    var strTime = time.ToString("F");
    //    saveDataTime.exitTime = strTime;
    //    saveDataTime.firstSave = false;
    //    string path = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
    //    string tempPath = path + ".tmp";

    //    File.WriteAllText(path, JsonUtility.ToJson(saveDataTime));
    //}
    //public SaveDataTime LoadData()
    //{
    //    string path = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
    //    SaveDataTime data = new();

    //    if (File.Exists(path))
    //    {
    //        data = JsonUtility.FromJson<SaveDataTime>(File.ReadAllText(path));
    //        return data;

    //    }
    //    return data;
    //}
}
//public class SaveDataTime
//{
//    public bool firstSave = true;
//    public string exitTime;
//}
