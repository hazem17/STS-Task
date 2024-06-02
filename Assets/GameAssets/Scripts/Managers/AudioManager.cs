using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct AudioProperty
{
    public string ID;
    public AudioClip clip;
}
public class AudioManager : Singleton<AudioManager>
{
    [Header("SFX")]
    [SerializeField] private AudioProperty[] SFXArray;
    private List<AudioSource> SFXs;
    private int audioSourceIndex;

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    // Start is called before the first frame update
    void Start()
    {
        musicSource.Play();
        SFXs = new List<AudioSource>();
        for (int i = 0; i < 5; i++)
        {
            SFXs.Add(gameObject.AddComponent(typeof(AudioSource)) as AudioSource);
        }
    }

    public void SwitchMusic(string musicID)
    {
        for (int i = 0; i < SFXArray.Length; i++)
        {
            if (SFXArray[i].ID == musicID)
            {
                musicSource.clip = SFXArray[i].clip;
                musicSource.Play();
                break;
            }
        }
    }

    public void PlaySFX(string clipID)
    {

        for (int i = 0; i < SFXArray.Length; i++)
        {
            if (SFXArray[i].ID == clipID)
            {
                SFXs[audioSourceIndex].clip = SFXArray[i].clip;
                SFXs[audioSourceIndex].Play();
                audioSourceIndex++;
                if (audioSourceIndex >= SFXs.Count)
                {
                    audioSourceIndex = 0;
                }
                break;
            }
        }

    }
}
