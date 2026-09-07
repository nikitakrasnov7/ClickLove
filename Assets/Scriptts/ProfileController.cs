using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileController : MonoBehaviour
{
    [SerializeField] Image PhotoProfile;
    [SerializeField] Image MiniPhoto;
    [SerializeField] Button GetPhotoFile;
    [SerializeField] TextMeshProUGUI FullClicksText;
    [SerializeField] TextMeshProUGUI FindCardsText;
    [SerializeField] TextMeshProUGUI FullMonyeCountText;

    private void Awake()
    {
        GetPhotoFile.onClick.AddListener(GetingPhoto);
        var texture = LoadTexture();
        if (texture != null)
        {
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            PhotoProfile.sprite = sprite;
            MiniPhoto.sprite = sprite;
        }
    }
    public void UpdateFullClick()
    {
        FullClicksText.text = GameManager.Instance.fullClicks.ToString();
    }

    public void UpdateCardCount(int cardCount)
    {
        FindCardsText.text = cardCount.ToString();
    }
    public void UpdateMonyeCount()
    {
        FullMonyeCountText.text = GameManager.Instance.FormatingString(GameManager.Instance.fullMonyePlayer);
    }

    public void GetingPhoto()
    {
        if (Application.isEditor)
        {
            GetImageFile();
        }
        else
        {
            GetImageFile();

        }
    }
    public void GetImageFile()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {

                byte[] bytes = File.ReadAllBytes(path);
                string savePath = Path.Combine(Application.persistentDataPath, "profile.png");
                File.WriteAllBytes(savePath, bytes);


                Texture2D texture = NativeGallery.LoadImageAtPath(path);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                PhotoProfile.sprite = sprite;
                MiniPhoto.sprite = sprite;
            }
        });
    }
    
    public Texture2D LoadTexture()
    {
        string path = Path.Combine(Application.persistentDataPath, "profile.png");
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            return texture;
        }
        return null;
    }
}
