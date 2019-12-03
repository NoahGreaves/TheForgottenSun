using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeMixer : MonoBehaviour
{
    [Header("Master")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private string _masterRTPC = "Master_Volume";

    [Header("SFX")]
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private string _sfxRTPC = "SFX_Volume";

    [Header("Ambience")]
    [SerializeField] private Slider _ambienceSlider;
    [SerializeField] private string _ambienceRTPC = "Ambience_Volume";

    [Header("Music")]
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private string _musicRTPC = "Music_Volume";

    [Header("Dialogue")]
    [SerializeField] private Slider _dialogueSlider;
    [SerializeField] private string _dialogueRTPC = "Dialogue_Volume";
    [SerializeField, Space(10)] private GameObject firstSelected;

    private void Start() => SubcribeEvents();

    public void UpdateMasterVolume()    => AkSoundEngine.SetRTPCValue(_masterRTPC, _masterSlider.value);
    public void UpdateSFXVolume()       => AkSoundEngine.SetRTPCValue(_sfxRTPC, _sfxSlider.value);  
    public void UpdateMusicVolume()     => AkSoundEngine.SetRTPCValue(_musicRTPC, _musicSlider.value);  
    public void UpdateDialogueVolume()  => AkSoundEngine.SetRTPCValue(_dialogueRTPC, _dialogueSlider.value);
    public void UpdateAmbienceVolume()  => AkSoundEngine.SetRTPCValue(_ambienceRTPC, _ambienceSlider.value);

    public void SubcribeEvents()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
        _masterSlider.onValueChanged.AddListener(   delegate {   UpdateMasterVolume();   });
        _sfxSlider.onValueChanged.AddListener(      delegate {   UpdateSFXVolume();      });
        _musicSlider.onValueChanged.AddListener(    delegate {   UpdateMusicVolume();    });
        _dialogueSlider.onValueChanged.AddListener( delegate {   UpdateDialogueVolume(); });
        _ambienceSlider.onValueChanged.AddListener( delegate {   UpdateAmbienceVolume(); });
    }
}
