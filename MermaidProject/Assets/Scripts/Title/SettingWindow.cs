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



	private void Start()
    {
        MasterVolumeSize.onValueChanged.AddListener(MasterVolumeControl);
        BGMSize.onValueChanged.AddListener(BGMControl);
        SoundEffectSize.onValueChanged.AddListener(SoundEffectControl);
        MouseSensitivity.onValueChanged.AddListener(MouseSensitivityControl);

	}

    private void MasterVolumeControl(float volume)
    {
        volume = MasterVolumeSize.value;

        if (volume == -40f) audioMixer.SetFloat("Master", -80);
        else
        {
            audioMixer.SetFloat("Master", volume);
            print(volume);
        }
    }

    private void BGMControl(float volume)
    {
        volume = BGMSize.value;

        if (volume == -40f) audioMixer.SetFloat("BGM", -80);
        else audioMixer.SetFloat("BGM", volume);
    }

    private void SoundEffectControl(float volume)
    {
        volume = SoundEffectSize.value;

        if (volume == -40f) audioMixer.SetFloat("SFX", -80);
        else audioMixer.SetFloat("SFX", volume);
    }

    public void MouseSensitivityControl(float sensitivity) {
        sensitivity = MouseSensitivity.value;
    }
}
