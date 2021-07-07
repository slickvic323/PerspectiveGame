using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{

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
            sound.source.clip = sound.clip;
        }
    }

    /**
     * Play Sound that matches given name at given pitch.
     */
    public void Play(string name, float pitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.pitch = pitch;
        s.source.Play();
    }

    /**
     * Plays Button Press Sound
     */
    public void ButtonPress()
    {
        Sound s = Array.Find(sounds, sound => sound.name == "Button_Press");
        s.source.Play();
    }
}
