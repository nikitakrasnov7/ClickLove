using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HUDPanelMenu : MonoBehaviour
{
    [SerializeField] RectTransform WindowContainer;
    [SerializeField] Button nextWindow;
    [SerializeField] Button prevWindow;
    [SerializeField] Image[] iconsWindows;

    [SerializeField] Color colorActiveWindow = Color.green;
    [SerializeField] Color colorDisactiveWindow = Color.white;
    [SerializeField] float moveSpeed = 0.5f;
    [SerializeField] float moveDuration = 1100;
    UnityEvent onClickBtn = new();
    int currentWindow = 0;

    private void Awake()
    {
        nextWindow.onClick.AddListener(MovingNextWindow);
        prevWindow.onClick.AddListener(MovingPrevWindow);
        UpdatingColor();

        onClickBtn.AddListener(() =>
        {
            UpdatingColor();
            CheckingButton();
            MovingWindow();
        });

        for (int i = 0; i < iconsWindows.Length; i++)
        {
            var step = i;
            if (iconsWindows[i].GetComponent<Button>() == null)
                iconsWindows[i].gameObject.AddComponent<Button>();
            iconsWindows[i].GetComponent<Button>().onClick.AddListener(() => MovePointWindow(step));
        }
    }
    public void MovingNextWindow()
    {
        currentWindow++;
        onClickBtn?.Invoke();
    }
    public void MovingPrevWindow()
    {
        currentWindow--;
        onClickBtn?.Invoke();
    }
    public void MovePointWindow(int i)
    {
        int iClamp = Mathf.Clamp(i, 0, iconsWindows.Length - 1);
        currentWindow = iClamp;
        onClickBtn?.Invoke();
    }
    public void MovingWindow()
    {
        WindowContainer.DOAnchorPosX(currentWindow * -moveDuration, moveSpeed);
    }
    public void CheckingButton()
    {
        prevWindow.gameObject.SetActive(currentWindow > 0);
        nextWindow.gameObject.SetActive(currentWindow < iconsWindows.Length - 1);

    }
    public void UpdatingColor()
    {
        for (int i = 0; i < iconsWindows.Length; i++)
        {
            if (i == currentWindow)
            {
                iconsWindows[i].color = colorActiveWindow;
            }
            else
            {
                iconsWindows[i].color = colorDisactiveWindow;
            }

        }
    }
}
