using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JACAMENO
{
    /// <summary>
    /// Manages the main menu scene UI.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        public Button PlayButton;
        public Button SettingsButton;
        public Button QuitButton;

        [Header("Display")]
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI HighScoreText;
        public TextMeshProUGUI VersionText;

        [Header("Settings Panel")]
        public GameObject SettingsPanel;
        public Button CloseSettingsButton;
        public Slider VolumeSlider;
        public Toggle SoundToggle;

        [Header("Animation")]
        public float TitlePulseSpeed = 1f;
        public float TitlePulseAmount = 0.05f;

        private Vector3 originalTitleScale;

        private void Start()
        {
            SetupButtons();
            UpdateHighScoreDisplay();
            SetupSettings();

            if (TitleText != null)
            {
                originalTitleScale = TitleText.transform.localScale;
            }

            if (VersionText != null)
            {
                VersionText.text = $"v{Application.version}";
            }

            // Hide settings panel initially
            if (SettingsPanel != null)
            {
                SettingsPanel.SetActive(false);
            }
        }

        private void Update()
        {
            AnimateTitle();
        }

        private void SetupButtons()
        {
            if (PlayButton != null)
            {
                PlayButton.onClick.AddListener(OnPlayClicked);
            }

            if (SettingsButton != null)
            {
                SettingsButton.onClick.AddListener(OnSettingsClicked);
            }

            if (QuitButton != null)
            {
                QuitButton.onClick.AddListener(OnQuitClicked);
            }

            if (CloseSettingsButton != null)
            {
                CloseSettingsButton.onClick.AddListener(CloseSettings);
            }
        }

        private void SetupSettings()
        {
            // Load saved settings
            float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
            bool soundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;

            if (VolumeSlider != null)
            {
                VolumeSlider.value = savedVolume;
                VolumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            }

            if (SoundToggle != null)
            {
                SoundToggle.isOn = soundEnabled;
                SoundToggle.onValueChanged.AddListener(OnSoundToggled);
            }

            AudioListener.volume = savedVolume;
        }

        private void UpdateHighScoreDisplay()
        {
            if (HighScoreText != null)
            {
                int highScore = PlayerPrefs.GetInt("JACAMENO_HighScore", 0);
                HighScoreText.text = $"HIGH SCORE: {highScore:N0}";
            }
        }

        private void AnimateTitle()
        {
            if (TitleText == null)
                return;

            float pulse = 1f + Mathf.Sin(Time.time * TitlePulseSpeed) * TitlePulseAmount;
            TitleText.transform.localScale = originalTitleScale * pulse;
        }

        private void OnPlayClicked()
        {
            if (GameState.Instance != null)
            {
                GameState.Instance.StartGame();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
            }
        }

        private void OnSettingsClicked()
        {
            if (SettingsPanel != null)
            {
                SettingsPanel.SetActive(true);
            }
        }

        private void CloseSettings()
        {
            if (SettingsPanel != null)
            {
                SettingsPanel.SetActive(false);
            }
        }

        private void OnVolumeChanged(float value)
        {
            AudioListener.volume = value;
            PlayerPrefs.SetFloat("Volume", value);
            PlayerPrefs.Save();
        }

        private void OnSoundToggled(bool enabled)
        {
            AudioListener.volume = enabled ? PlayerPrefs.GetFloat("Volume", 1f) : 0f;
            PlayerPrefs.SetInt("SoundEnabled", enabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void OnQuitClicked()
        {
            if (GameState.Instance != null)
            {
                GameState.Instance.QuitGame();
            }
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}
