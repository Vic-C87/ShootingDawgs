using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance 
    {
        get
        {
            return instance;
        }  
    }

    static SoundManager instance;

    AudioSource myAudioSource;

    [SerializeField] AudioClip[] myDeathSounds;
    [SerializeField] AudioClip[] myEvilLaughSounds;
    [SerializeField] AudioClip[] myHurtSounds;
    [SerializeField] AudioClip[] myJumpSounds;
    [SerializeField] AudioClip[] myPuncherSounds;

    void Awake()
    {
        if (instance != this && instance != null) 
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        myAudioSource = GetComponent<AudioSource>();
    }

    public void PlayDeathSound()
    {
        myAudioSource.loop = false;
        myAudioSource.volume = 1f;
        myAudioSource.clip = myDeathSounds[0];
        myAudioSource.Play();
    }

    public void PlayEvilLaughSound()
    {
        myAudioSource.loop = false;
        myAudioSource.volume = 1f;
        myAudioSource.clip = myEvilLaughSounds[0];
        myAudioSource.Play();
    }
    public void PlayHurtSound() 
    {
        myAudioSource.loop = false;
        myAudioSource.volume = 1f;
        myAudioSource.clip = myHurtSounds[0];
        myAudioSource.Play();
    }

    public void PlayRopeGuySound() //Place on RopeGuy!!!
    {
        myAudioSource.loop = false;
        myAudioSource.volume = 1f;
    }

    public void PlayJumpSound() 
    {
        myAudioSource.loop = false;
        myAudioSource.volume = 1f;
        myAudioSource.clip = myJumpSounds[0];
        myAudioSource.Play();
    }

    public void PlayPuncherSound() 
    {
        myAudioSource.loop = false;
        myAudioSource.volume = 1f;
        myAudioSource.clip = myPuncherSounds[0];
        myAudioSource.Play();
    }
}
