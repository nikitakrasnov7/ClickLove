
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class MoveWindowController : MonoBehaviour
{
    [SerializeField] private RectTransform windowContainer;
    [SerializeField] private RectTransform profileWindow;
    [SerializeField] private RectTransform WheelRect;

    [SerializeField] private float windowWidth = 1000f;
    [SerializeField] private float moveDuration = 0.5f;

    [SerializeField] private Button MainWindow;
    [SerializeField] private Button LibraryWindow;
    [SerializeField] private Button ShopWindow;
    [SerializeField] private Button TaskWindow;

    [SerializeField] private Button btnOpenProfile;
    [SerializeField] private Button WheelWindow;

    [SerializeField] private float swipeThreshold = 100f;

    private int currentWindow = 0;

    private Vector2 swipeStartPosition;
    private bool isSwiping;
    public bool isOpenProfile;
    private bool isStateWheelBtn;
    private void Awake()
    {
        MainWindow.onClick.AddListener(() => MoveWindow(0));
        LibraryWindow.onClick.AddListener(() => MoveWindow(1));
        ShopWindow.onClick.AddListener(() => MoveWindow(2));
        TaskWindow.onClick.AddListener(() => MoveWindow(3));

        btnOpenProfile.onClick.AddListener(() => MoveProfileWindow());
        WheelWindow.onClick.AddListener(() => MoveWheelWindow());

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
        profileWindow.DOAnchorPosY(0, moveDuration).SetEase(Ease.OutCubic);
    }
    public void CloseProfileWindow()
    {
        isOpenProfile = false;
        profileWindow.DOAnchorPosY(2100, moveDuration).SetEase(Ease.OutCubic);
    }
    public void MoveWheelWindow()
    {
        isStateWheelBtn = !isStateWheelBtn;

        WheelRect.DOAnchorPosX((isStateWheelBtn ? 0 : 2025), moveDuration);
    }
}
