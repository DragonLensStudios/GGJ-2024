using System;
using DLS.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MenuSettingsController : MonoBehaviour
{
    [field: SerializeField] public TMP_Text MasterVolumeText { get; set; }
    [field: SerializeField] public Scrollbar MasterVolumeScrollbar { get; set; }
    [field: SerializeField] public TMP_Text MusicVolumeText { get; set; }
    [field: SerializeField] public Scrollbar MusicVolumeScrollbar { get; set; }
    [field: SerializeField] public TMP_Text SFXVolumeText { get; set; }
    [field: SerializeField] public Scrollbar SFXVolumeScrollbar { get; set; }
    
    [field: SerializeField] public TMP_Dropdown ResolutionDropdown { get; set; }
    [field: SerializeField] public TMP_Dropdown QualityDropdown { get; set; }
    [field: SerializeField] public TMP_Dropdown FullScreenMode { get; set; }

    private void OnEnable()
    {
        var masterVolume = PlayerPrefs.GetFloat("masterVolume", 1);
        var musicVolume = PlayerPrefs.GetFloat("musicVolume", 1);
        var sfxVolume = PlayerPrefs.GetFloat("effectsVolume", 1);
        
        MasterVolumeScrollbar.value = Mathf.Pow(10, masterVolume / 20f);
        MusicVolumeScrollbar.value = Mathf.Pow(10, musicVolume / 20f);
        SFXVolumeScrollbar.value = Mathf.Pow(10, sfxVolume / 20f);
        UpdateMasterVolume(MasterVolumeScrollbar.value);
        UpdateMusicVolume(MusicVolumeScrollbar.value);
        UpdateSfxVolume(SFXVolumeScrollbar.value);
        
        ResolutionDropdown.options.Clear();
        foreach (var resolution in Screen.resolutions)
        {
            if(ResolutionDropdown.options.Exists(x=>x.text == $"{resolution.width}x{resolution.height}")) continue;
            ResolutionDropdown.options.Add(new TMP_Dropdown.OptionData($"{resolution.width}x{resolution.height}"));
        }
        
        ResolutionDropdown.value = PlayerPrefs.GetInt("resolution", 0);
        
        QualityDropdown.options.Clear();
        foreach (var quality in QualitySettings.names)
        {
            QualityDropdown.options.Add(new TMP_Dropdown.OptionData(quality));
        }
        
        QualityDropdown.value = PlayerPrefs.GetInt("quality", 0);
        
        FullScreenMode.options.Clear();
        FullScreenMode.options.Add(new TMP_Dropdown.OptionData("Fullscreen"));
        FullScreenMode.options.Add(new TMP_Dropdown.OptionData("Fullscreen Window"));
        FullScreenMode.options.Add(new TMP_Dropdown.OptionData("Borderless"));
        FullScreenMode.options.Add(new TMP_Dropdown.OptionData("Windowed"));

        FullScreenMode.value = PlayerPrefs.GetInt("fullscreen", 0);
    }

    public void UpdateMasterVolume(float val)
    {
        AudioManager.Instance.SetMasterVolume(val);
        MasterVolumeText.text = $"Master: {val:P0}";
    }
    
    public void UpdateMusicVolume(float val)
    {
        AudioManager.Instance.SetMusicVolume(val);
        MusicVolumeText.text = $"Music: {val:P0}";
    }
    
    public void UpdateSfxVolume(float val)
    {
        AudioManager.Instance.SetSoundEffectsVolume(val);
        SFXVolumeText.text = $"SFX: {val:P0}";
    }
    
    public void SetResolution(int resolutionIndex)
    {
        Resolution selectedResolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode);
        PlayerPrefs.SetInt("resolution", resolutionIndex);
    }
    
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("quality", qualityIndex);
    }
    
    public void SetFullScreenMode(int modeIndex)
    {
        Screen.fullScreenMode = (FullScreenMode)modeIndex;
        PlayerPrefs.SetInt("fullscreen", modeIndex);
    }



    
}
