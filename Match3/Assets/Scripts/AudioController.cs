using System;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;

    public static AudioController instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(this);

        foreach (Sound audio in sounds)
        {
           audio.source = gameObject.AddComponent<AudioSource>();
           audio.source.clip = audio.audio;
           audio.source.volume = audio.volume;
           audio.source.loop = audio.loop;
        }
    }

    void Start() 
    {
        Play("Background Music");
    } 

    //Находит клип по имени и проигрывает его
    public void Play(string name)
    {
        Sound audio = Array.Find(sounds, Sound => Sound.name == name);
        audio.source.Play();
    }
}

//Класс для настройки аудио клипов
[System.Serializable]
public class Sound{

    public string name;

    [Range(0f, 1f)]
    public float volume;
    public bool loop;
    public AudioClip audio;

    [HideInInspector]
    public AudioSource source;
}