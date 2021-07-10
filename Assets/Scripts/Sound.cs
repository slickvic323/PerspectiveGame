using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    /**
     * Stores Audio Clip
     */
    public AudioClip clip;

    /**
     * Stores Name of Sound
     */
    public string name;

    /**
     * Stores volume float value between 0.0f and 1.0f
     */
    [Range(0f, 1f)]
    public float volume;

    /**
     * Stores pitch float value between 0.1f and 4.0f
     */
    [Range(0.1f, 4f)]
    public float pitch;

    public bool loop;

    /**
     * Stores Audio Source
     */
    [HideInInspector]
    public AudioSource source;
}
