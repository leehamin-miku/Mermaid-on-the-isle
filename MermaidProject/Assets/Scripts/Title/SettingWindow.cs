using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingWindow : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider MasterVolumeSize;
    public Slider BGMSize;
    public Slider SoundEffectSize;
    public Slider MouseSensitivity;

    public float Sensitivity = 4;


    private void Start()
    {
        MasterVolumeSize.onValueChanged.AddListener(MasterVolumeControl);
        BGMSize.onValueChanged.AddListener(BGMControl);
        SoundEffectSize.onValueChanged.AddListener(SoundEffectControl);
        MouseSensitivity.onValueChanged.AddListener(MouseSensitivityControl);

	}

    private void MasterVolumeControl(float volume)
    {
        DataManager.Instance.data.MV = MasterVolumeSize.value;

        //if (volume == -40f) audioMixer.SetFloat("Master", -80);
        //else
        //{
        //    audioMixer.SetFloat("Master", volume);
        //    print(volume);
        //}
    }

    private void BGMControl(float volume)
    {
        DataManager.Instance.data.BV = BGMSize.value;

        //if (volume == -40f) audioMixer.SetFloat("BGM", -80);
        //else audioMixer.SetFloat("BGM", volume);
    }

    private void SoundEffectControl(float volume)
    {
        DataManager.Instance.data.SV = SoundEffectSize.value;

        //if (volume == -40f) audioMixer.SetFloat("SFX", -80);
        //else audioMixer.SetFloat("SFX", volume);
    }

    public static float MS = 4;
    public void MouseSensitivityControl(float sensitivity) {
        DataManager.Instance.data.MS = MouseSensitivity.value;
    }

}
