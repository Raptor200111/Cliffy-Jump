using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;


public class ScriptMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject configPopUp;
    public GameObject selectCharPopUp;
    public TMP_Text numCoins;
    public TMP_Text progressLvl1;
    public TMP_Text progressLvl2;
    public GameObject playButton;
    public GameObject lvl1Button;
    public GameObject lvl2Button;
    [SerializeField] private TMP_Text character_name;
    [SerializeField] private Slider volumeMusicSlider;
    [SerializeField] private Slider volumeSoundSlider;
    [SerializeField] private Slider speedSlider;
    [System.Serializable]
    private class ToggleImage
    {
        public Toggle _toggle;
        public GameObject offImage;
    }

    [SerializeField] private ToggleImage onMusicToggle;
    [SerializeField] private ToggleImage onSoundToggle;

    private GameManager gameManager;
    private SoundManager soundManager;
    public int level;
    void Start()
    {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
        level = 1;
        if (onMusicToggle == null)
        {
            Debug.LogWarning("onMusicToggle not assigned");
        }
        else
        {
            onMusicToggle._toggle.onValueChanged.AddListener(isOnMusic);
        }
        if (onSoundToggle == null)
        {
            Debug.LogWarning("onMusicToggle not assigned");
        }
        else
        {
            onSoundToggle._toggle.onValueChanged.AddListener(isOnSound);
        }
        if (volumeMusicSlider == null)
        {
            Debug.LogWarning("onMusicToggle not assigned");
        }
        else
        {
            volumeMusicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        }
        if (volumeSoundSlider == null)
        {
            Debug.LogWarning("onMusicToggle not assigned");
        }
        else
        {
            volumeSoundSlider.onValueChanged.AddListener(ChangeSoundVolume);
        }
        numCoins.text = $"{GameManager.Instance.CoinsCollected}";
        progressLvl1.text = $"{GameManager.Instance.MaxLevelProgress[0]} %";
        progressLvl2.text = $"{GameManager.Instance.MaxLevelProgress[1]} %";
        if(PlayerPrefs.GetInt("PlayerSelected", -1)  == -1)
        {
            selectCharPopUp.SetActive(true);
        }
        else { selectCharPopUp.SetActive(false); }
        playButton.SetActive(false);
        lvl1Button.SetActive(true);
        lvl1Button.SetActive(true);

        ResetParams();         
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelectLevel(int i_level)
    {
        level = i_level;
        playButton.SetActive(true);
        lvl1Button.SetActive(false);
        lvl2Button.SetActive(false);
    }

    public void OnPlay()
    {
        if(level == 1)
        {
            gameManager.changeScene(StageName.LVL_1);
        }
        else
        {
            gameManager.changeScene(StageName.LVL_2);
        }
    }

    public void OnGoToCredits()
    {
        gameManager.changeScene(StageName.CREDITS);
    }
    public void OnGoToConfig()
    {
        configPopUp.SetActive(true);
    }

    public void OnGoToSelectPlayer()
    {
        selectCharPopUp.SetActive(true);
    }
    public void OnExitConfig()
    {
        configPopUp.SetActive(false);
    }

    public void OnExitSelectChar()
    {
        selectCharPopUp.SetActive(false);
    }
    public void ChangeMusicVolume(float newVolume)
    {
        soundManager.SetMusicVolume(newVolume);
    }

    public void isOnMusic(bool isOn)
    {
        soundManager.StartStopBackgroundMusic(isOn);
        onMusicToggle.offImage.GetComponent<Image>().enabled = !isOn;
    }
    public void ChangeSoundVolume(float newVolume)
    {
        soundManager.SetSoundVolume(newVolume);
    }
    public void isOnSound(bool isOn)
    {
        soundManager.StartStopSound(isOn);
        onSoundToggle.offImage.GetComponent<Image>().enabled = !isOn;
    }

    public void ResetParams()
    {
        volumeMusicSlider.value = 0.6f;
        volumeSoundSlider.value = 0.9f;
        onMusicToggle._toggle.isOn = true;
        onSoundToggle._toggle.isOn = true;
        ChangeMusicVolume(volumeMusicSlider.value);        
        isOnMusic(onMusicToggle._toggle.isOn);
        ChangeSoundVolume(volumeSoundSlider.value);
        isOnSound(onSoundToggle._toggle.isOn);
    }

    private void OnDestroy()
    {
        onMusicToggle._toggle.onValueChanged.RemoveListener(isOnMusic);
        onSoundToggle._toggle.onValueChanged.RemoveListener(isOnSound);
        volumeMusicSlider.onValueChanged.RemoveListener(ChangeMusicVolume);
        volumeSoundSlider.onValueChanged.RemoveListener(ChangeSoundVolume);
    }

}
