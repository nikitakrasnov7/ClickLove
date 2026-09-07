using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardElement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI LevelCard;
    [SerializeField] Image PhotoImage;
    [SerializeField] Image LevelIcon;
    [SerializeField] Button OpenInfoPanel;
    [SerializeField] GameObject NewHint;
    [SerializeField] public PhotoElement data;


    public void Init(PhotoElement data)
    {
        this.data = data;


        UpdateUI();
        OpenInfoPanel.onClick.AddListener(() =>
        {
            CloseHint();
            GameManager.Instance.libraryController.OpenInfoImageWindow(data);
        });

    }

    public void Init(TestPhotoElement data,Sprite sprite)
    {
        PhotoElement pe = new();

        pe.GetId(data.GUID);
        pe.Name = data.NamePhoto;
        pe.Description = data.Description;
        pe.levelCard = data.Level;
        pe.Photo = sprite;
        pe.level = GetStringLevel(data.Level);

        this.data = pe;
        UpdateUI();


    }
    public void UpdateUI()
    {
        LevelCard.text = data.level;
        PhotoImage.sprite = data.Photo;
    }
    public string GetStringLevel(PhotoLevel level)
    {
        string result = string.Empty;
        switch (level)
        {
            case PhotoLevel.Common:
                result = "Обычная";
                break;

            case PhotoLevel.Rare:
                result = "Редкая";
                break;
            case PhotoLevel.Epic:
                result = "Эпическая";
                break;
            case PhotoLevel.Legend:
                result = "Легендарная";
                break;
        }
        return result;
    }
    public void CloseHint()
    {
        NewHint.gameObject.SetActive(false);
    }
}


