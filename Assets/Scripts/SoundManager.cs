using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Audio;


// TO DO: add sounds && MusicSoundParams=> playerprefs
public enum SoundType
{
    JUMP,
    REVIVE,
    DIE,
    WIN, 
    COIN
// DIE
//    BIRD_MOVE
//    HAMMER

}

public struct MusicSoundParams
{
    public float musicVolume;
    public float soundVolume;
    public bool onSound;
    public bool onMusic;
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private SoundList[] soundList;
    [SerializeField] private SoundList[] bgMusics; // Add a field for the background music clip
    [SerializeField] private float backgroundMusicVolume = 0.5f; // Adjustable music volume
    private AudioSource soundEffectSource; // For sound effects
    private AudioSource musicSource; // For background music
    private MusicSoundParams musicsoundparams = new MusicSoundParams
    {
        musicVolume = 1f,
        soundVolume = 1f,
        onMusic = false,
        onSound = false,

    };

    private void Awake()
    {
        if (SoundManager.Instance == null)
        {
            Instance = this;
            AudioSource[] sources = GetComponents<AudioSource>();

            // Use the first AudioSource for sound effects, add or configure a second one for music
            soundEffectSource = sources.Length > 0 ? sources[0] : gameObject.AddComponent<AudioSource>();
            musicSource = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();

            // Configure the music source
            musicSource.loop = true; // Ensures the background music loops
            musicSource.volume = Instance.musicsoundparams.soundVolume* backgroundMusicVolume;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    private void Start()
    {
        if (bgMusics != null && musicsoundparams.onMusic)
        {
            musicSource.clip = bgMusics[0].sound;
            musicSource.Play(); // Start playing the music
        }
    }
    public static void PlaySound(SoundType sound, AudioSource source = null)
    {
        SoundList sL = Instance.soundList[(int)sound];
        AudioClip randomClip = sL.sound;
        source= Instance.soundEffectSource;

        if (Instance.musicsoundparams.onSound != true) return;
        if (source)
        {
            source.outputAudioMixerGroup = sL.mixer;
            source.clip = randomClip;
            source.volume = Instance.musicsoundparams.soundVolume * sL.volume;
            source.Play();
        }
        else
        {
            Instance.soundEffectSource.PlayOneShot(randomClip, Instance.musicsoundparams.soundVolume);
        }
    }

    public void SetBackgroundMusic(StageName stageName)
    {
        Instance.musicSource.clip = Instance.bgMusics[(int)stageName].sound;
        Instance.musicSource.volume = Instance.bgMusics[(int)stageName].volume * Instance.musicsoundparams.musicVolume;
        if (Instance.musicsoundparams.onMusic)
        {
            Instance.musicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        Instance.musicSource.Stop();
    }

    public void StartStopBackgroundMusic(bool isOn)
    {
        if(isOn && !Instance.musicsoundparams.onMusic)
        {
            Instance.musicSource.Play();
        }
        else if (!isOn && Instance.musicsoundparams.onMusic) 
        { Instance.musicSource.Stop(); }
        Instance.musicsoundparams.onMusic = isOn;
    }

    public void SetMusicVolume(float volume)
    {
        Instance.musicsoundparams.musicVolume = volume;
        Instance.musicSource.volume = bgMusics[(int)GameManager.Instance.stageName].volume * volume;
    }
    public void SetSoundVolume(float volume)
    {
        Instance.musicsoundparams.soundVolume = volume;
    }
    public void StartStopSound(bool isOn)
    {
        if (!isOn && Instance.musicsoundparams.onMusic)
        { Instance.soundEffectSource.Stop(); }
        Instance.musicsoundparams.onSound = isOn;
    }


    private void PopulateSongList(string[] names, SoundList[] list)
    {
        bool differentSize = names.Length != list.Length;

        Dictionary<string, SoundList> sounds = new();

        if (differentSize)
        {
            for (int i = 0; i < list.Length; ++i)
            {
                sounds.Add(list[i].name, list[i]);
            }
        }

        int oldLength = list.Length;
        Array.Resize(ref list, names.Length);

        // Initialize new elements
        for (int i = oldLength; i < list.Length; i++)
        {
            list[i] = new SoundList(); 
        }

        for (int i = 0; i < list.Length; i++)
        {
            string currentName = names[i];
            list[i].name = currentName;
            if (list[i].volume == 0) list[i].volume = 1;

            if (differentSize)
            {
                if (sounds.ContainsKey(currentName))
                {
                    SoundList current = sounds[currentName];
                    UpdateElement(ref list[i], current.volume, current.sound, current.mixer);
                }
                else
                    UpdateElement(ref list[i], 1, null, null);

                static void UpdateElement(ref SoundList element, float volume, AudioClip sound, AudioMixerGroup mixer)
                {
                    element.volume = volume;
                    element.sound = sound;
                    element.mixer = mixer;
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        PopulateSongList(names, soundList);
        string[] bgnames = Enum.GetNames(typeof(StageName));
        PopulateSongList(bgnames, bgMusics);        
    }
#endif
}


[Serializable]
public struct SoundList
{
    [ReadOnly, SerializeField] public string name;
    [Range(0, 1)] public float volume;
    public AudioMixerGroup mixer;
    public AudioClip sound;
}
