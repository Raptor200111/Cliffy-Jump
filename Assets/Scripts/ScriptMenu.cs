using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
//using UnityEngine.UIElements;

public class ScriptMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject popUp;
    public int indexCharacter;
    [SerializeField] private TMP_Text character_name;
    [SerializeField] private Slider volumeMusicSlider;
    [SerializeField] private Slider volumeSoundSlider;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Toggle onSoundToggle;
    [SerializeField] private Toggle onMusicToggle;
    [SerializeField] private Sprite offImage;

    private GameManager gameManager;
    private SoundManager soundManager;
    public int level;
    void Start()
    {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
        onMusicToggle.onValueChanged.AddListener(StartStopMusic);
        onSoundToggle.onValueChanged.AddListener(StartStopSound);
        volumeMusicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        volumeSoundSlider.onValueChanged.AddListener(ChangeSoundVolume);
        ResetParams();
        level = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void selectLevel(int i_level)
    {
        level = i_level;
    }

    public void playLevel()
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
    public void ShowConfigPopUp()
    {
        popUp.SetActive(true);
    }

    public void showSelectPlayer()
    {
        gameManager.changeScene(StageName.PLYR_SELECT);
    }
    public void ExitConfigPopUp()
    {
        popUp.SetActive(false);
    }
    public void ChangeMusicVolume(float newVolume)
    {
        soundManager.SetMusicVolume(newVolume);
    }
    private void SetToggleImage(Toggle toggle, bool isOn)
    {
        Image toggleBgImage = toggle.transform.Find("Background").GetComponent<Image>();
        if (isOn && toggleBgImage != null)
        {
            toggleBgImage.sprite = null;
        }
        else if (!isOn && toggleBgImage != null)
        {
            toggleBgImage.sprite = offImage;
        }
    }

    public void StartStopMusic(bool isOn)
    {
        soundManager.StartStopBackgroundMusic(isOn);
        SetToggleImage(onMusicToggle, isOn); 
    }
    public void ChangeSoundVolume(float newVolume)
    {
        soundManager.SetSoundVolume(newVolume);
    }
    public void StartStopSound(bool isOn)
    {
        soundManager.StartStopSound(isOn);
        SetToggleImage(onSoundToggle, isOn);
    }

    public void ResetParams()
    {
        volumeMusicSlider.value = 0.6f;
        volumeSoundSlider.value = 0.9f;
        onMusicToggle.isOn = true;
        onSoundToggle.isOn = true;
        ChangeMusicVolume(volumeMusicSlider.value);        
        StartStopMusic(onMusicToggle.isOn);
        ChangeSoundVolume(volumeSoundSlider.value);
        StartStopSound(onSoundToggle.isOn);
    }

    private void OnDestroy()
    {
        onMusicToggle.onValueChanged.RemoveListener(StartStopMusic);
        onSoundToggle.onValueChanged.RemoveListener(StartStopSound);
        volumeMusicSlider.onValueChanged.RemoveListener(ChangeMusicVolume);
        volumeSoundSlider.onValueChanged.RemoveListener(ChangeSoundVolume);
    }

}
