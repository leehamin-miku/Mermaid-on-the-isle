using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class VNSoundManager : MonoBehaviour {
    public static VNSoundManager instance;

    public AudioClip[] BGMClips;
    [SerializeField] AudioSource BGMPlayer;
    [SerializeField] AudioMixerGroup BGMMixer;

    public AudioClip[] SEClips;
    [SerializeField] AudioSource[] SEPlayer;
    [SerializeField] AudioMixerGroup SFXMixer;
    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(instance);
        }
    }

    int loofnum = 0;
    void PlayBGM(string name) {
		print(BGMClips.Length);
        for (int i = 0; i < BGMClips.Length; i++) {
            if (name == BGMClips[i].name) {
                BGMPlayer.clip = BGMClips[i];
                BGMPlayer.outputAudioMixerGroup = BGMMixer;
                BGMPlayer.Play();
                if (loofnum++ > 10) {
                    print(BGMClips.Length);
                    break;
                }
                return;
            }
        }
        Debug.LogError(name + "����!");
    }

    void PlaySE(string name) {
        for (int i = 0; i < SEClips.Length; i++) {
            if (name == SEClips[i].name) {
                for (int j = 0; j < SEPlayer.Length; j++) {
                    if (!SEPlayer[j].isPlaying) {
                        SEPlayer[j].clip = SEClips[i];
                        SEPlayer[j].outputAudioMixerGroup = SFXMixer;
                        SEPlayer[j].Play();
                        return;
                    }
                }
                Debug.LogError("��� SEPlayer �����!");
            }
        }
        Debug.LogError(name + "����!");
    }

    // name = ���� �̸�, type == 0 -> BGM ���, == 1 -> SE ���
    public void PlaySound(string name, int type) {
        if (type == 0) PlayBGM(name);
        else if (type == 1) PlaySE(name);
    }
    public IEnumerator StopSound() {

        float timer = 0;
        while (timer <= 1) {
            timer += Time.deltaTime/1.5f;
            BGMPlayer.volume = Mathf.Lerp(1, 0, timer);
            yield return null;
        }
		BGMPlayer.Stop();
	}

}

