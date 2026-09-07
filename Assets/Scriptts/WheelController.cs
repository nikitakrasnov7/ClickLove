using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WheelController : MonoBehaviour
{
    [SerializeField] Button OpenWindow;
    [SerializeField] Sprite OpenedButton, ClosedButton;

    [SerializeField] Animator PanelAnimator;



    [SerializeField] Button RotateWheelBtn;
    [SerializeField] Transform Wheel;
    [SerializeField] WinnerType[] Sectors;
    // photo history facts gems facts video
    [SerializeField] public VideoWinner VideoWinner;
    [SerializeField] public TextWinner FactsWinner;
    [SerializeField] public TextWinner HistoryWinner;

    [SerializeField] float spinDuration = 3f;
    [SerializeField] int minFullRotations = 5;
    [SerializeField] float accelerationTime = 1;
    [SerializeField] float rotationTime = 3;
    [SerializeField] float decelerationTime = 2;

    bool state = false;
    bool isSpin;

    public enum WinnerType
    {
        Photo,
        Video,
        Facts,
        History,
        Gems
    }
    private void Awake()
    {

        RotateWheelBtn.onClick.AddListener(() =>
        {
            if (isSpin == false)
                StartCoroutine(SpinWheel());
        });
    }
    public void StateButton()
    {
        OpenWindow.interactable =
            (GameManager.Instance.statisticController.levelsCompleted >= 10) ? true : false;

        OpenWindow.GetComponent<Image>().sprite =
            (GameManager.Instance.statisticController.levelsCompleted >= 10) ? OpenedButton : ClosedButton;
    }
    
    IEnumerator SpinWheel()
    {
        isSpin = true;

        int randomSector = Random.Range(0, Sectors.Length);
        float sectorAngle = 360f / Sectors.Length;
        float targetAngle = sectorAngle * randomSector;

        Debug.Log($"Выпало: {Sectors[randomSector]} | Угол: {targetAngle}");

        float currentAngle = Wheel.eulerAngles.z;

        float angleToTarget = currentAngle - targetAngle;

        if (angleToTarget < 0)
            angleToTarget += 360f;

        float totalAngle = minFullRotations * 360f + angleToTarget;

        float startAngle = currentAngle;

        float time = 0f;

        while (time < accelerationTime)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / accelerationTime);
            float smoothT = t * t;
            float angle = startAngle - totalAngle * 0.15f * smoothT;
            Wheel.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }

        time = 0f;

        float rotationStart = startAngle - totalAngle * 0.15f;

        while (time < rotationTime)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / rotationTime);
            float angle = rotationStart - totalAngle * 0.60f * t;
            Wheel.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }

        time = 0f;

        float decelerationStart = startAngle - totalAngle * 0.75f;

        while (time < decelerationTime)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / decelerationTime);
            float smoothT = 1f - Mathf.Pow(1f - t, 2f);
            float angle = decelerationStart - totalAngle * 0.25f * smoothT;
            Wheel.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }

        float finalAngle = startAngle - totalAngle;
        Wheel.rotation = Quaternion.Euler(0, 0, finalAngle);

        yield return new WaitForSeconds(0.5f);

        GetWinning(Sectors[randomSector]);

        isSpin = false;
    }

    public void GetWinning(WinnerType winner)
    {
        float levelsPoints = 100;
        switch (winner)
        {
            case WinnerType.Photo:
                GameManager.Instance.libraryController.BuyingCard();
                levelsPoints = 120;
                break;

            case WinnerType.Video:
                VideoWinner.OpeningPanel();
                levelsPoints = 200;
                break;

            case WinnerType.Facts:
                FactsWinner.OpenWindow();
                levelsPoints = 100;
                break;

            case WinnerType.History:
                HistoryWinner.OpenWindow();
                levelsPoints = 100;
                break;
            case WinnerType.Gems:
                int gems = 100;
                levelsPoints = 30;
                GameManager.Instance.AddGems(gems);
                Debug.Log(gems + " получено");
                break;

        }

        GameManager.Instance.statisticController.AddPlayerPoints(levelsPoints);
    }
}
