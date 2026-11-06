using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanel : MonoBehaviour
{
    [SerializeField] Button closePanel;
    [SerializeField] TextMeshProUGUI hintText;
    public NotificationType NotificationType;
    RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        closePanel.onClick.AddListener(CloseWindow);
    }
    public void UpdateTextHint(string hint)
    {
        hintText.text = hint;
    }

    public void OpenWindow()
    {
        rect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad);
    }
    public void CloseWindow()
    {
        rect.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutQuad);

    }
}
