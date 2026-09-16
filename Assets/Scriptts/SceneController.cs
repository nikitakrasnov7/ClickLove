using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [SerializeField] PhotosDataSO data;
    [SerializeField] GameObject Hint;
    [SerializeField] GameObject btnStartGame;

    [SerializeField] Button MainGame;
    [SerializeField] Button Create;
    private const string JSON_FILE = "CARDS_PLAYER.json";

    private void Start()
    {
        bool hasGameData = HasSavedGameData();

        if (Hint != null)
        {
            Hint.SetActive(hasGameData);
        }

        if (btnStartGame != null)
        {
            btnStartGame.SetActive(hasGameData);
        }

        if (MainGame != null)
        {
            MainGame.gameObject.SetActive(hasGameData);

            MainGame.onClick.AddListener(() => SceneManager.LoadScene("Main"));
        }

        if (Create != null)
        {
            Create.onClick.AddListener(() => SceneManager.LoadScene("CreateGame"));
        }
    }

    private bool HasSavedGameData()
    {
        string path = Path.Combine( Application.persistentDataPath,JSON_FILE);

        if (!File.Exists(path))
        {
            Debug.Log("SceneController: CARDS_PLAYER.json не найден");
            return false;
        }

        try
        {
            string json = File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.Log("SceneController: CARDS_PLAYER.json пустой");
                return false;
            }

            CreateCardsController.TestSaveCards saveData =
                JsonUtility.FromJson<CreateCardsController.TestSaveCards>(json);

            if (saveData == null)
            {
                Debug.Log("SceneController: не удалось прочитать JSON");
                return false;
            }

            int photos = saveData.photos != null
                ? saveData.photos.Count
                : 0;

            int videos = saveData.videos != null
                ? saveData.videos.Count
                : 0;

            int facts = saveData.facts != null
                ? saveData.facts.Count
                : 0;

            int history = saveData.history != null
                ? saveData.history.Count
                : 0;

            Debug.Log(
                $"SceneController: сохранено — " +
                $"карт: {photos}, " +
                $"видео: {videos}, " +
                $"фактов: {facts}, " +
                $"историй: {history}"
            );

            return photos > 0 ||
                   videos > 0 ||
                   facts > 0 ||
                   history > 0;
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "SceneController: ошибка чтения CARDS_PLAYER.json\n" + e
            );

            return false;
        }
    }
}
