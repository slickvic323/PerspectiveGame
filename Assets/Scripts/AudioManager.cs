using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{

    private bool soundEffectsEnabled;

    private bool musicEnabled;


    /**
     * Stores array of available sounds to play.
     */
    public Sound[] sounds;

    void Awake()
    {
        // To help keep sound continuing between scenes
        GameObject[] objs = GameObject.FindGameObjectsWithTag("AudioManager");

        // Ensures that there is only one audio manager object in the game
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        // Audio Manager object stays open between Scenes in the Game
        DontDestroyOnLoad(this.gameObject);

        // Initializes all sounds available
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.clip = sound.clip;
        }

        RefreshSoundEffectsEnabled();
        RefreshMusicEnabled();
    }

    /**
     * Play Sound that matches given name at given pitch.
     */
    public void Play(string name, float pitch)
    {
        bool playSound = true;
        // If a sound effect noise, check if sound effects are enabled
        if (name.Equals("Button_Press")
            || name.Equals("Pattern_Single_Note"))
        {
            if (!soundEffectsEnabled)
            {
                playSound = false;
            }
        }
        else if (name.Equals("Menu_Music"))
        {
            if (!musicEnabled)
            {
                playSound = false;
            }
        }

        if (playSound)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            s.source.pitch = pitch;
            s.source.Play();
        }
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

    /**
     * Plays Button Press Sound
     */
    public void ButtonPress()
    {
        if (soundEffectsEnabled)
        {
            Sound s = Array.Find(sounds, sound => sound.name == "Button_Press");
            s.source.Play();
        }
    }

    public void RefreshSoundEffectsEnabled()
    {
        soundEffectsEnabled = (PlayerPrefs.GetInt("SoundEffectsEnabled", 1) == 1);
    }

    public void RefreshMusicEnabled()
    {
        musicEnabled = (PlayerPrefs.GetInt("MusicEnabled", 1) == 1);
    }

    public void SetVolume(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.volume = volume;
    }
}
