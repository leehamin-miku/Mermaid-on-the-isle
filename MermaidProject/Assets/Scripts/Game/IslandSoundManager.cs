using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class IslandSoundManager : MonoBehaviour
{
    public AudioClip tsunamiClip;
    public AudioClip islandBGMClip;
    public AudioMixerGroup BGMMixer;
    public AudioMixerGroup SFXMixer;

    private AudioSource islandBGM;
    private List<AudioSource> audioSources = new List<AudioSource>();
    private float lastPlayTime = 0f;

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.spatialBlend = 0f;
            audioSources.Add(source);
            source.outputAudioMixerGroup = SFXMixer;
        }
    }
    public void TsunamiSoundEffectStart()
    {
        if(Time.time - lastPlayTime >= 0.5f)
        {
            for (int i = 0; i < 5; i++)
            {
                if (!audioSources[i].isPlaying)
                {
                    audioSources[i].clip = tsunamiClip;
                    audioSources[i].time = 0.9f;
                    audioSources[i].Play();
                    lastPlayTime = Time.time;
                    break;
                }
            }
        }
    }

    public void islandBGMStart()
    {
        islandBGM = gameObject.AddComponent<AudioSource>();
        islandBGM.outputAudioMixerGroup = BGMMixer;
        islandBGM.clip = islandBGMClip;
        islandBGM.spatialBlend = 0f;
        islandBGM.loop = true;
        islandBGM.Play();
    }

    public void islandBGMEnd()
    {
        StartCoroutine(islandBGMStop());
    }

    public IEnumerator islandBGMStop()
    {

        float timer = 0;
        while (timer <= 1)
        {
            timer += Time.deltaTime / 1.5f;
            islandBGM.volume = Mathf.Lerp(1, 0, timer);
            yield return null;
        }
        islandBGM.Stop();
    }
}
