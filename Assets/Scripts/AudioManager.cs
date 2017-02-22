using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioClip[] clips;
    private List<AudioSource> sources;

    public static void Play(AudioClip clip, float volume = 1, float pitch = 1, float delay = 0f)
    {
        Instance.InstancePlay(clip, volume, pitch, delay);
    }

    public static void Play(string name, float volume = 1, float pitch = 1, float delay = 0f)
    {
        Play(Instance.GetClip(name), volume, pitch, delay);
    }

    void Awake ()
    {
        if (Instance != null) Destroy(this);
        Instance = this;
        sources = new List<AudioSource>();
	}

    private void InstancePlay(AudioClip clip, float volume, float pitch, float delay)
    {
        if (clip == null) Debug.LogError("Audio clip not found.");
        var source = GetSource();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.PlayDelayed(delay);
    }
    
    private AudioClip GetClip(string name)
    {
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] != null && clips[i].name == name)
                return clips[i];
        }

        return null;
    }

    private AudioSource GetSource()
    {
        for (int i = 0; i < sources.Count; i++)
        {
            if (!sources[i].isPlaying)
                return sources[i];
        }

        var source = this.gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 0f;
        sources.Add(source);
        return source;
    }
}
