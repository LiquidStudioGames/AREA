using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    // Scene UI elements
    public Dropdown GraphicsQuality;
    public Toggle MusicOn;
    public Toggle SoundsOn;
    public Slider MusicVolume;
    public Slider SoundsVolume;
    public Toggle AutoReloadOn;
    public Toggle GameStatsOn;

    private Settings Settings;

    private void OnEnable()
    {
        if (Settings == null) Settings = Game.Instance.Settings;
        SetSettings();
    }

    // SetSettings Method :: Sets the scene UI elements
    void SetSettings()
    {
        // Assign all the scene items to the setting files data
        GraphicsQuality.value = Settings.Data.GraphicsIndex;
        MusicOn.isOn = Settings.Data.Music;
        SoundsOn.isOn = Settings.Data.Sounds;
        MusicVolume.value = Settings.Data.MusicVolume;
        SoundsVolume.value = Settings.Data.SoundsVolume;
        AutoReloadOn.isOn = Settings.Data.AutoReload;
        GameStatsOn.isOn = Settings.Data.GameStats;
    }

    public void Cancel()
    {
        Settings.Load();
        SetSettings();
    }

    public void Save()
    {
        Settings.Save();
    }

    public void Defaults()
    {
        Settings.Data = new SettingsData();
        SetSettings();
    }

    private void Update()
    {
        CheckValue(GraphicsQuality.value, "GraphicsIndex", QualitySettings.SetQualityLevel);
        CheckValue(MusicOn.isOn, "Music", null);
        CheckValue(SoundsOn.isOn, "Sounds", null);
        CheckValue(MusicVolume.value, "MusicVolume", null);
        CheckValue(SoundsVolume.value, "SoundsVolume", null);
        CheckValue(AutoReloadOn.isOn, "AutoReload", null);
        CheckValue(GameStatsOn.isOn, "GameStats", null);
    }

    // Did some magic where you can couple a value to a field, the callback is called when a change is detected
    private void CheckValue<T>(T value, string field, Action<T> callback)
    {
        FieldInfo i = Settings.Data.GetType().GetField(field);

        if (!i.GetValue(Settings.Data).Equals(value))
        {
            i.SetValue(Settings.Data, value);
            callback?.Invoke(value);
        }
    }
}
