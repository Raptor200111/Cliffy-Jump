using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
    JUMP
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
    private MusicSoundParams musicsoundparams;

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

            if (bgMusics != null)
            {
                musicSource.clip = bgMusics[0].sound;
                musicSource.Play(); // Start playing the music
            }
        }
    }
   
    public static void PlaySound(SoundType sound, AudioSource source = null)
    {
        SoundList sL = Instance.soundList[(int)sound];
        AudioClip randomClip = sL.sound;

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
    }
    public void SetSoundVolume(float volume)
    {
        Instance.musicsoundparams.soundVolume = volume;
    }
    public void StartStopSound(bool is_on)
    {
        Instance.musicsoundparams.onSound = is_on;
    }



#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        bool differentSize = names.Length != soundList.Length;

        Dictionary<string, SoundList> sounds = new();

        if (differentSize)
        {
            for (int i = 0; i < soundList.Length; ++i)
            {
                sounds.Add(soundList[i].name, soundList[i]);
            }
        }

        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            string currentName = names[i];
            soundList[i].name = currentName;
            if (soundList[i].volume == 0) soundList[i].volume = 1;

            if (differentSize)
            {
                if (sounds.ContainsKey(currentName))
                {
                    SoundList current = sounds[currentName];
                    UpdateElement(ref soundList[i], current.volume, current.sound, current.mixer);
                }
                else
                    UpdateElement(ref soundList[i], 1, null, null);

                static void UpdateElement(ref SoundList element, float volume, AudioClip sound, AudioMixerGroup mixer)
                {
                    element.volume = volume;
                    element.sound = sound;
                    element.mixer = mixer;
                }
            }
        }
    }
#endif
}

[Serializable]
public struct SoundList
{
    [ReadOnly] public string name;
    [Range(0, 1)] public float volume;
    public AudioMixerGroup mixer;
    public AudioClip sound;
}
