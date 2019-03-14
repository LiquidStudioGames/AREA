using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{

    public static SettingsUI Instance;

    // Scene UI elements
    public Dropdown GraphicsQuality;
    public Toggle MusicOn;
    public Toggle SoundsOn;
    public Slider MusicVolume;
    public Slider SoundsVolume;
    public Toggle AutoReloadOn;
    public Toggle GameStatsOn;

    void Start()
    {

        SetSettings ();

    }

    // SetSettings Method :: Sets the scene UI elements
    void SetSettings ()
    {

        // Set the variable Data to the settings data
        SettingsData Data = Settings.Instance.Data;

        // Assign all the scene items to the setting files data
        GraphicsQuality.value = Data.GraphicsIndex;
        MusicOn.isOn = Data.Music;
        SoundsOn.isOn = Data.Sounds;
        MusicVolume.value = Data.MusicVolume / 100;
        SoundsVolume.value = Data.SoundsVolume / 100;
        AutoReloadOn.isOn = Data.AutoReload;
        GameStatsOn.isOn = Data.GameStats;

    }

}
