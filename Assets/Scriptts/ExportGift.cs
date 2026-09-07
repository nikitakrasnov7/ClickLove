using System;
using System.Collections;
using System.Data.Common;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.UI;
using NativeShareNamespace;

public class ExportGift : MonoBehaviour
{
    private const string FILE_NAME = "CARDS_PLAYER.json";
    [SerializeField] Button createExport;


    bool isExport = false;
    private void Awake()
    {
        createExport.onClick.AddListener(CreateExport);
    }
    public void CreateExport()
    {
        Debug.Log("click");
        if (isExport == false)
        {
            Debug.Log("start");
            StartCoroutine(ExportingGift());
        }
    }
    public IEnumerator ExportingGift()
    {

        isExport = true;

        string path = Application.persistentDataPath;
        string jsonPath = Path.Combine(path, FILE_NAME);

        Debug.Log("начало экспорта");
        if (File.Exists(jsonPath))
        {
            string tempFolder = Path.Combine(path, "GiftTemp");
            if (Directory.Exists(tempFolder))
                Directory.Delete(tempFolder, true);

            Directory.CreateDirectory(tempFolder);

            File.Copy(jsonPath, Path.Combine(tempFolder, FILE_NAME));

            string json = File.ReadAllText(jsonPath);
            CreateCardsController.TestSaveCards data = JsonUtility.FromJson<CreateCardsController.TestSaveCards>(json);
            Debug.Log("копируем фотки");
            foreach (var card in data.photos)
            {
                string photoPath = Path.Combine(path, $"{card.GUID}.png");
                if (File.Exists(photoPath))
                {
                    File.Copy(
                        photoPath,
                        Path.Combine(tempFolder, $"{card.GUID}.png"));
                }
                else
                {
                    Debug.Log("проблема нет фотки");
                }
            }
            string zipPath = Path.Combine(path, "MyGift.zip");
            if (File.Exists(zipPath))
                File.Delete(zipPath);

            ZipFile.CreateFromDirectory(
                tempFolder, zipPath, System.IO.Compression.CompressionLevel.Fastest, false
                );

            string giftPath = Path.Combine(path, "MyGift.gamegift");

            if (File.Exists(giftPath))
                File.Delete(giftPath);

            File.Move(zipPath, giftPath);

            Directory.Delete(tempFolder, true);
            yield return null;
            Debug.Log("подарок создан");

            new NativeShare().
                AddFile(giftPath, "application/x-gamegift")
                .SetSubject("Подарок")
                .SetText("Это подарок для тебя")
                .Share();

        }
        else
        {
            Debug.Log("проблема 1 нет сохранения");
            yield return null;

        }
        isExport = false;
    }
}
