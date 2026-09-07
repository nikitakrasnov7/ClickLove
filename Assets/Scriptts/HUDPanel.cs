using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HUDPanel : MonoBehaviour
{
    [SerializeField] Button Main;
    [SerializeField] Button Library;
    [SerializeField] Button Shop;
    [SerializeField] Button Task;
    [SerializeField] Button Profile;
    [SerializeField] Button Wheel;

    [SerializeField] RectTransform windowContainer;
    [SerializeField] RectTransform profileRect;
    [SerializeField] RectTransform wheelRect;

    [SerializeField] GameObject MainWindoww;
    [SerializeField] GameObject LibraryWindoww;
    [SerializeField] GameObject ShopWindoww;
    [SerializeField] GameObject ProfileWindoww;
    [SerializeField] GameObject TaskWindoww;

    [SerializeField] private float swipeThreshold = 100f;
    [SerializeField] private float windowWidth = 1000f;
    [SerializeField] private float moveDuration = 0.5f;
    private int currentWindow = 0;

    private Vector2 swipeStartPosition;
    private bool isSwiping;
    public bool isOpenProfile;
    public bool isOpenWheel;
    private void Awake()
    {
        Main.onClick.AddListener(() => MoveWindow(0));
        Library.onClick.AddListener(() => MoveWindow(1));
        Shop.onClick.AddListener(() => MoveWindow(2));
        Task.onClick.AddListener(() => MoveWindow(3));

        Profile.onClick.AddListener(() => MoveProfileWindow());
        Wheel.onClick.AddListener(() => MoveWheelWindow());


    }

    private void Update()
    {
        HandleSwipe();
    }

    private void HandleSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipeStartPosition = Input.mousePosition;
            isSwiping = true;
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            Vector2 swipeEndPosition = Input.mousePosition;

            Vector2 swipeDelta = swipeEndPosition - swipeStartPosition;

            isSwiping = false;

            if (Mathf.Abs(swipeDelta.x) < swipeThreshold)
                return;

            if (swipeDelta.x < 0)
            {
                MoveWindow(currentWindow + 1);
            }
            else
            {
                MoveWindow(currentWindow - 1);
            }
        }
    }

    public void MoveWindow(int index)
    {
        if (index < 0 || index > 3)
            return;

        currentWindow = index;

        windowContainer.DOKill();

        float targetX = -index * windowWidth;

        if (isOpenProfile)
            CloseProfileWindow();

        windowContainer
            .DOAnchorPosX(targetX, moveDuration)
            .SetEase(Ease.OutCubic);
    }

    public void MoveProfileWindow()
    {
        isOpenProfile = true;
        profileRect.DOAnchorPosY(0, moveDuration).SetEase(Ease.OutCubic);
    }
    public void CloseProfileWindow()
    {
        isOpenProfile = false;
        profileRect.DOAnchorPosY(2100, moveDuration).SetEase(Ease.OutCubic);
    }

    public void MoveWheelWindow()
    {
        isOpenWheel = !isOpenWheel;

        wheelRect.DOAnchorPosX((isOpenWheel? 0 : 2025), moveDuration);
    }
}
