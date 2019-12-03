using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntesityController : MonoBehaviour
{
    [SerializeField] private EnemyDistanceController _enemyDistanceController;
    [SerializeField] private int _highIntensity = 5;
    [SerializeField] private int _mediumIntensity = 2;
    [SerializeField] private int _lowIntensity = 0;
    public int? Intensity = 0;

    public Tank _tank;
    public Ranger _ranger;

    private GameMaster _gameMaster;

    private void Start() => _gameMaster = FindObjectOfType<GameMaster>();

    private void Update() => CheckIntensity();

    public void CheckIntensity()
    {
        int? dangerIndex = 0;

        foreach (BaseAI baseAI in _enemyDistanceController.engagedAIlist)
        {
            dangerIndex += baseAI?.gameObject.GetComponent<Tank>()?.dangerIndex ?? 0;
            dangerIndex += baseAI?.gameObject.GetComponent<Ranger>()?.dangerIndex ?? 0;
        }

        Intensity = dangerIndex;

        if (Intensity >= 5)
        {
            _gameMaster.MusicController.IntensityStateChange("Intensity", "High");
          //  Debug.Log($"{this.ToString()} | Intensity = {Intensity} | State = High");
        }
        else if (Intensity >= 2 && Intensity < 5)
        {
            _gameMaster.MusicController.IntensityStateChange("Intensity", "Medium");
           // Debug.Log($"{this.ToString()} | Intensity = {Intensity} | State = Medium");
        }
        else if (Intensity < 2)
        {
            _gameMaster.MusicController.IntensityStateChange("Intensity", "Low");
          //  Debug.Log($"{this.ToString()} | Intensity = {Intensity} | State = Low");
        }
    }

    // private void SetMusicIntesity(int intensity)
    // {
    //     if (intensity >= _highIntensity)
    //     {
    //         _gameMaster.MusicController.IntensityStateChange("Intensity", "High");
    //         Debug.Log("high intensity");
    //     }
    //     else if (intensity >= _mediumIntensity)
    //     {
    //         _gameMaster.MusicController.IntensityStateChange("Intensity", "Medium");
    //         Debug.Log("medium intensity");
    //     }
    //     else if (intensity <= 0)
    //     {
    //         _gameMaster.MusicController.IntensityStateChange("Intensity", "Low");
    //         Debug.Log("low intensity");
    //     }
    // }
}
