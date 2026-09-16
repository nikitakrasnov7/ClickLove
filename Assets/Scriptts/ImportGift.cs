using System;
using System.Collections;
using System.IO;
using System.IO.Compression;

using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;

#endif
public class ImportGift : MonoBehaviour
{
    private const string GIFT_EXTENSION = ".gamegift";
    private const string JSON_FILE = "CARDS_PLAYER.json";
    private const string IMPORT_FOLDER = "GiftImportTemp";

    [SerializeField] GameObject buttonStartGameTest;
    [SerializeField] CreateCardsController controller;

    private string lastProcessedUri = null;
    bool isImporting = false;

    private void OpenMainScene()
    {
        SceneManager.LoadScene("Main");
    }

    private void Start()
    {
        if (buttonStartGameTest != null)
            buttonStartGameTest.GetComponent<Button>().onClick.AddListener(OpenMainScene);
#if UNITY_ANDROID && !UNITY_EDITOR
            StartCoroutine(CheckIncomingFile());
#endif
    }


    private void OnApplicationFocus(bool focus)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            if(focus && !isImporting)
            {
                StartCoroutine(CheckIncomingFile());
            }
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR

    IEnumerator CheckIncomingFile()
    {
        yield return new WaitForSeconds(0.5f);
        CheckIncomingIntent();
    }
    void CheckIncomingIntent()
    {
        if (isImporting)
            return;

        try
        {
            AndroidJavaObject activity = UnityEngine.Android.AndroidApplication.currentActivity;
            if (activity == null)
                return;

            AndroidJavaObject intent = activity.Call<AndroidJavaObject>("getIntent");

            if (intent == null)
                return;

            string action = intent.Call<string>("getAction");

            Debug.Log("ImportGift: Intent action = " + action);

            if (action != "android.intent.action.VIEW")
                return;

            var uri = intent.Call<AndroidJavaObject>("getData");
            if (uri == null) return;

            string uriString = uri.Call<string>("toString");

            string mimeType = intent.Call<string>("getType");

            Debug.Log("ImportGift: URI = " + uriString);
            Debug.Log("ImportGift: MIME = " + mimeType);

            if (uriString == lastProcessedUri)
                return;

            lastProcessedUri = uriString;

            StartCoroutine(ImportGiftFile(uriString));


        }
        catch (Exception e)
        {
            Debug.LogError("ImportGift: ошибка получения Intent:\n" + e);
        }


    }
    IEnumerator ImportGiftFile(string uriString)
    {
        isImporting = true;

        string importFolder = Path.Combine(Application.persistentDataPath, IMPORT_FOLDER);

        string giftFile = Path.Combine(importFolder, "received.gamegift");

        string extractedFolder = Path.Combine(importFolder, "Extracted");

        try
        {
            Debug.Log("ImportGift: начинаем импорт");

            if (Directory.Exists(importFolder))
            {
                Directory.Delete(importFolder, true);
            }

            Directory.CreateDirectory(importFolder);

            AndroidJavaObject activity = AndroidApplication.currentActivity;

            if (activity == null)
            {
                throw new Exception("Android currentActivity == null");
            }

            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

            if (context == null)
            {
                throw new Exception("Не удалось получить Android Context");
            }

            AndroidJavaClass importer = new AndroidJavaClass("com.clicklove.gamegift.GameGiftImporter");

            Debug.Log("ImportGift: копирование .gamegift...");

            bool copied = importer.CallStatic<bool>("copyUriToFile", context, uriString, giftFile);

            if (!copied)
            {
                throw new Exception("Android не смог скопировать gamegift");
            }

            if (!File.Exists(giftFile))
            {
                throw new Exception("received.gamegift не создан");
            }

            Debug.Log("ImportGift: файл успешно скопирован: " + giftFile);

            Directory.CreateDirectory(extractedFolder);

            Debug.Log("ImportGift: распаковка...");

            ExtractZipSafely(giftFile, extractedFolder);

            string jsonPath = Path.Combine(extractedFolder, JSON_FILE);

            if (!File.Exists(jsonPath))
            {
                throw new Exception("В gamegift отсутствует " + JSON_FILE);
            }
            Debug.Log("ImportGift: JSON найден");

            string json = File.ReadAllText(jsonPath);
            CreateCardsController.TestSaveCards saveData = JsonUtility.FromJson<CreateCardsController.TestSaveCards>(json);

            if (saveData == null)
            {
                throw new Exception("Не удалось прочитать CARDS_PLAYER.json");
            }

            if (saveData.photos == null)
                saveData.photos = new System.Collections.Generic.List<TestPhotoElement>();

            if (saveData.videos == null)
                saveData.videos = new System.Collections.Generic.List<VideoElement>();
            if (saveData.facts == null)
                saveData.facts = new();
            if (saveData.history == null)
                saveData.history = new();

            Debug.Log("ImportGift: карточек в файле = " + saveData.photos.Count);
            Debug.Log("ImportGift: видео в файле = " + saveData.videos.Count);
            Debug.Log("ImportGift: фактов в файле = " + saveData.facts.Count);
            Debug.Log("ImportGift: историй в файле = " + saveData.history.Count);


            foreach (var card in saveData.photos)
            {
                if (card == null) continue;

                string sourcePhoto = Path.Combine(extractedFolder, card.GUID + ".png");
                string destinationPhoto = Path.Combine(Application.persistentDataPath, card.GUID + ".png");

                if (!File.Exists(sourcePhoto))
                {
                    Debug.LogWarning("ImportGift: нет картинки для GUID " + card.GUID);
                    continue;
                }

                File.Copy(sourcePhoto, destinationPhoto, true);

            }
            foreach (var video in saveData.videos)
            {
                if (video == null) continue;
                string sourceVideo = Path.Combine(extractedFolder, video.GUID + ".mp4");
                string destinationVideo = Path.Combine(Application.persistentDataPath, video.GUID + ".mp4");

                if (!File.Exists(sourceVideo))
                {
                    Debug.Log("ImportGift: нет видео для GUID");
                    continue;
                }
                File.Copy(sourceVideo, destinationVideo, true);
            }

            if (controller == null)
            {
                controller = FindFirstObjectByType<CreateCardsController>();
            }
            if (controller != null)
            {
                controller.ImportCards(saveData.photos, saveData.videos, saveData.facts, saveData.history);
                Debug.Log("ImportGift: ИМПОРТ УСПЕШНО ЗАВЕРШЁН");

                if (buttonStartGameTest != null)
                    buttonStartGameTest.SetActive(true);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ImportGift: ОШИБКА ИМПОРТА:\n" + e);
        }
        finally
        {

            try
            {
                if (Directory.Exists(importFolder))
                {
                    Directory.Delete(importFolder, true);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("ImportGift: не удалось удалить " + "временные файлы:\n" + ex);
            }
            isImporting = false;

        }
        yield return null;
    }

    void CopyAndroidInputStreamToFile(AndroidJavaObject contentResolver, AndroidJavaObject uri, string destinationPath)
    {
        var inputStream = contentResolver.Call<AndroidJavaObject>("openInputStream", uri);
        if (inputStream == null)
        {
            throw new Exception("Не удалось открыть InputStream.");
        }
        var outputStream = new AndroidJavaObject("java.io.FileOutputStream", destinationPath);

        AndroidJavaObject buffer = new AndroidJavaObject("[B", 81920);
        try
        {
            while (true)
            {
                int bytesRead = inputStream.Call<int>("read", buffer);
                if (bytesRead == -1)
                    break;
                if (bytesRead == 0)
                    continue;

                outputStream.Call("write", buffer, 0, bytesRead);


            }

        }
        finally
        {
            inputStream.Call("close");
            outputStream.Call("close");
        }
    }

    void ExtractZipSafely(string zipPath, string destinationDirectory)
    {
        using (ZipArchive archive = ZipFile.OpenRead(zipPath))
        {
            string fullDestination = Path.GetFullPath(destinationDirectory);
            if (!fullDestination.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                fullDestination += Path.DirectorySeparatorChar;
            }

            foreach (var entry in archive.Entries)
            {
                string destinationPath = Path.GetFullPath(
                    Path.Combine(destinationDirectory, entry.FullName));

                if (!destinationPath.StartsWith(fullDestination, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Небезопасный путь в ZIP: " + entry.FullName);
                }

                if (string.IsNullOrEmpty(entry.Name))
                {
                    Directory.CreateDirectory(destinationDirectory);
                    continue;
                }
                string parentDirectory = Path.GetDirectoryName(destinationPath);

                if (!string.IsNullOrEmpty(parentDirectory))
                {
                    Directory.CreateDirectory(parentDirectory);
                }
                entry.ExtractToFile(destinationPath, true);

            }
        }
    }
#endif
}