using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    [SerializeField] Slider progressSlider;
    [SerializeField] TextMeshProUGUI FactsText;

    [SerializeField] string[] facts;
    private float currentProgress;
    [SerializeField] float speed = 1.5f;

    [SerializeField] Animator animator;
    [SerializeField] Animator PerehodPanel;

    [SerializeField] Button StartGame;
    bool loading;
    private void Awake()
    {
        StartGame.onClick.AddListener(StartMainScene);
        loading = true;
        FactsText.text = $"ิเ๊๒:{facts[Random.Range(0, facts.Length)]}";
        progressSlider.value = 0;
        progressSlider.maxValue = 100;
    }

    private void Update()
    {
        if (loading)
        {

            currentProgress += Time.deltaTime * speed;
            currentProgress = Mathf.Clamp(currentProgress, 0, 100);
            progressSlider.value = currentProgress;

            if (currentProgress == 100)
            {
                animator.SetTrigger("Play");
                StartGame.interactable = true;
                loading = false;
            }
        }
    }
    public void StartMainScene()
    {
        PerehodPanel.SetTrigger("Play");
        Debug.Log("suka");
    }
    public static void StartGameScene()
    {
        SceneManager.LoadScene("Main");
    }
}
