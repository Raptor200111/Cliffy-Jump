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
    public GameObject popUp;
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

    [SerializeField] private ToggleImage onMusicToggle = new ToggleImage();
    [SerializeField] private ToggleImage onSoundToggle = new ToggleImage();

    private GameManager gameManager;
    private SoundManager soundManager;
    public int level;
    void Start()
    {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
        onMusicToggle._toggle.onValueChanged.AddListener(isOnMusic);
        onSoundToggle._toggle.onValueChanged.AddListener(isOnSound);
        volumeMusicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        volumeSoundSlider.onValueChanged.AddListener(ChangeSoundVolume);
        ResetParams();
        level = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelectLevel(int i_level)
    {
        level = i_level;
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
    public void OnGoToConfig()
    {
        popUp.SetActive(true);
    }

    public void OnGoToSelectPlayer()
    {
        gameManager.changeScene(StageName.PLYR_SELECT);
    }
    public void OnExitConfig()
    {
        popUp.SetActive(false);
    }
    public void ChangeMusicVolume(float newVolume)
    {
        soundManager.SetMusicVolume(newVolume);
    }

    public void isOnMusic(bool isOn)
    {
        soundManager.StartStopBackgroundMusic(isOn);
        onMusicToggle.offImage.SetActive(!isOn);
    }
    public void ChangeSoundVolume(float newVolume)
    {
        soundManager.SetSoundVolume(newVolume);
    }
    public void isOnSound(bool isOn)
    {
        soundManager.StartStopSound(isOn);
        onSoundToggle.offImage.SetActive(!isOn);
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
