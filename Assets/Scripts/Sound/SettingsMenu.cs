using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer = null;

    FMOD.Studio.Bus _master;
    FMOD.Studio.Bus _music;
    FMOD.Studio.Bus _SFX;
    FMOD.Studio.Bus _readAloud;

    private float _masterVolume = 1f;
    private float _musicVolume = 1f;

    private float _lastVolume;
    private bool _isActive = true;

    private void Awake()
    {
        _master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        _music = FMODUnity.RuntimeManager.GetBus("bus:/Master/TestMusic");
    }

    public void SetVolume(float pVolume)
    {
        _music.setVolume(pVolume);
        //masterMixer.SetFloat("Volume", pVolume);
        //lastVolume = pVolume;
        Debug.Log(pVolume);
    }

    public void SetMusicActive(Image pButton)
    {
        if (pButton.gameObject.activeInHierarchy)
        {
            pButton.gameObject.SetActive(false);
            _music.setMute(false);
        }
        else
        {
            pButton.gameObject.SetActive(true);
            _music.setMute(true);
        }
    }

    public void SetSFXActive(Image pButton)
    {
        if (pButton.gameObject.activeInHierarchy)
        {
            pButton.gameObject.SetActive(false);
            _SFX.setMute(false);
        }
        else
        {
            pButton.gameObject.SetActive(true);
            _SFX.setMute(true);
        }
    }
}