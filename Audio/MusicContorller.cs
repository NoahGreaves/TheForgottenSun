using UnityEngine;

// TODO: Make this class control the music, call the below functions to change the music state/track
public class MusicContorller : MonoBehaviour
{
    [Header("Event Names")]
    [SerializeField] private string _playMusic = "Play_Music";
    [SerializeField] private string _stopMusic = "Stop_Music";

    private void Awake() => DontDestroyOnLoad(gameObject);
    private void Start() => PlayMusic();
    private void Update() => EvaluateInput();

    private void PlayMusic()
    {
        AkSoundEngine.SetSwitch("Location", "MainTheme", gameObject);
        AkSoundEngine.PostEvent(_playMusic, gameObject);
    }

    private void EvaluateInput()
    {
        if (Input.GetKeyDown("-"))
        {
            AkSoundEngine.PostEvent(_stopMusic, gameObject);
        }

        if (Input.GetKeyDown("="))
        {
            AkSoundEngine.PostEvent(_playMusic, gameObject);
        }
    }

    public void SwitchToRainForest()
    {
        AkSoundEngine.SetSwitch("Location", "Rainforest", gameObject);
        AkSoundEngine.SetState("Intensity",  "Low");
    }

    public void SwitchToCrypt()
    {
        AkSoundEngine.SetSwitch("Location", "Boss", gameObject);
        AkSoundEngine.SetSwitch("Gods", "Ah_Puch", gameObject);
        AkSoundEngine.SetState("BossHealth", "High"); 
    }

    // Note: uint will store number between 0 and 4 billion whereas an int will store number from negative 2 billion to positive 2 billion
    public void BossHealthSwitchChange(string stateGroupName, string stateState)
    {
        // AkSoundEngine.SetSwitch(switchGroupName, switchState, gameObject);
        AkSoundEngine.SetState(stateGroupName, stateState);
    }

    public void GameStateChange(string stateGroup, string state)
    {
        AkSoundEngine.SetState(stateGroup, state);
    }

    public void IntensityStateChange(string stateGroup, string state)
    {
        AkSoundEngine.SetState(stateGroup, state);
    }

    public void SetTriggers(GameObject gameObject, string triggerName)
    {
        AkSoundEngine.PostTrigger(triggerName, gameObject);
    }
}