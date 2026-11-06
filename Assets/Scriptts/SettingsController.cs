using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] Toggle backgroundMusicToggle;
    [SerializeField] Toggle soundEffectsToggle;
    [SerializeField] Toggle notificationToggle;

    [SerializeField] AudioMixer audioMixer;

    [SerializeField] Button nextLanguageButton;
    [SerializeField] Button previousLanguageButton;
    [SerializeField] TextMeshProUGUI languageText;
    [SerializeField]
    Language[] languages = {

        new Language("Русский", "ru"),
        new Language("English", "en"),
    };
    [System.Serializable]
    public class Language
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public Language(string name, string code)
        {
            Name = name;
            Code = code;
        }


    }

    [SerializeField] Button SupportButton;
    [SerializeField] Button PrivacyPolicyButton;
    List<LanguageText> languageTexts;
    int currentLanguageIndex = 0;

    bool isMusic = true;
    bool isSound = true;
    bool isNotification = true;

    public bool IsNotificationEnabled => isNotification;

    private void Awake()
    {
        backgroundMusicToggle.onValueChanged.AddListener(MusicControlling);
        soundEffectsToggle.onValueChanged.AddListener(SoundControlling);
        notificationToggle.onValueChanged.AddListener(NotificationControlling);

        nextLanguageButton.onClick.AddListener(() => NextLanguage(1));
        previousLanguageButton.onClick.AddListener(() => NextLanguage(-1));

        SupportButton.onClick.AddListener(OpenSupport);
        PrivacyPolicyButton.onClick.AddListener(OpenPrivacyPolicy);


        if (languageTexts == null)
            languageTexts = new List<LanguageText>(FindObjectsByType<LanguageText>(FindObjectsSortMode.None));

    }
    private void Start()
    {
        NextLanguage(0);
    }
    private void MusicControlling(bool state)
    {
        isMusic = !isMusic;
        audioMixer.SetFloat("MusicVolume", isMusic ? 80f : 0f);
    }

    private void SoundControlling(bool state)
    {
        isSound = !isSound;
        audioMixer.SetFloat("SoundVolume", isSound ? 80f : 0f);
    }
    private void NotificationControlling(bool state)
    {
        isNotification = !isNotification;
    }

    void NextLanguage(int step = 1)
    {
        currentLanguageIndex += step;
        ClampIndex();
        UpdateBtnState();
        UpdateLanguage();
    }

    void ClampIndex()
    {
        currentLanguageIndex = Mathf.Clamp(currentLanguageIndex, 0, languages.Length - 1);

    }
    void UpdateLanguage()
    {
        Language language = languages[currentLanguageIndex];
        languageText.text = language.Name;
        LocalizatingText(language.Code);

    }
    void UpdateBtnState()
    {
        nextLanguageButton.gameObject.SetActive(!(currentLanguageIndex >= languages.Length - 1));
        previousLanguageButton.gameObject.SetActive(!(currentLanguageIndex <= 0));
    }
    void LocalizatingText(string code)
    {
        foreach (var text in languageTexts)
        {
            text.SetText(code);
        }
    }
    void OpenSupport()
    {
        //Application.OpenURL("https://t.me/your_support_link");
    }
    void OpenPrivacyPolicy()
    {
        //Application.OpenURL("https://your_privacy_policy_link");
    }
}
