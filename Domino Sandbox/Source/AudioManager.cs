using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public AudioMixer MainAudioMixer;
    public AudioClip[] BGMClips;

    private int curBGM = 0;
    private AudioSource AS;

    void Start()
    {
        AS = GetComponent<AudioSource>();

        if (BGMClips.Length > 0)
            if(!AS.isPlaying)
                PlayBGM();
    }

    void Update()
    {
        if (BGMClips.Length > 0)
            if (!AS.isPlaying)
                PlayBGM();
    }

    private void PlayBGM()
    {
        AS.PlayOneShot(BGMClips[curBGM]);

        curBGM++;
        if (curBGM >= BGMClips.Length)
            curBGM = 0;
    }

    public void SliderToMaster(Slider slider)
    {
        float decibels = -80f + ( 80f * (slider.value / 100f));
        
        MainAudioMixer.SetFloat("masterVol", decibels);
    }
    
    public void SliderToBGM(Slider slider)
    {
        float decibels = -80f + (80f * (slider.value / 100f));

        MainAudioMixer.SetFloat("bgmVol", decibels);
    }
    
    public void SliderToSFX(Slider slider)
    {
        float decibels = -80f + (80f * (slider.value / 100f));

        MainAudioMixer.SetFloat("sfxVol", decibels);
    }
}
