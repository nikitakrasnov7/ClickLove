using UnityEngine;

[CreateAssetMenu(fileName = "TestPhotoSO", menuName = "Scriptable Objects/TestPhotoSO")]
public class PhotosDataSO : ScriptableObject
{
    public System.Collections.Generic.List<TestPhotoElement> photos = new();
    public bool isTestSave = false;
    public void AddingPhoto(TestPhotoElement photo)
    {
        if(photo != null)
        {
            photo.GUID = System.Guid.NewGuid().ToString();
            photos.Add(photo);
        }
    }
    

}
[System.Serializable]
public class TestPhotoElement
{
    public string GUID;
    public string NamePhoto;
    public string Description;
    public PhotoLevel Level;
}