/*
* Copyright (C) Noah Greaves in Association with VFS
*/

using ObjectPooling;
using UnityEngine;

public class HandsOfDead : MonoBehaviour
{
    public PoolableObject _hand;
    public int _numOfHands = 12;
    
    [SerializeField, Tooltip("The distance that the hands will spawn away from the player in a circle")]
    private float _spawnRadius;

    public float _manaCost;

    private string _handSound = "Play_Undead_Hands";

    private AbilitySelection _selector;

    private void Start()
    {
        _selector = FindObjectOfType<AbilitySelection>();
    }

    public void StartHandsOfDead()
    {
        // _selector.UpdateUIElements();
        SpawnHands();
    }

    private void SpawnHands()
    {
        Vector3 center = transform.position;                                                            // the center of the circle is the player
        center.y += gameObject.transform.localScale.y * 0.5f;
        for(int i = 0; i < _numOfHands; i++)                                                            // for loop to spawn the hands
        {
            Vector3 position = RandCircleSpawn(center, _spawnRadius);                                   // spawn the hands in a cirlce around the player
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, center - position);        // make the hands face away from the player
            PoolManager.GetNext(_hand, position, rotation);
            _hand.GetComponent<HandDamage>().Player = gameObject;
        }                               
        AkSoundEngine.PostEvent(_handSound, gameObject);
    }

    private Vector3 RandCircleSpawn(Vector3 center, float radius)
    {
        // make a random angle between 0 and 360
        float ang = UnityEngine.Random.value * 360;
        Vector3 position;
        position.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        position.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        position.y = center.y;
        
        return position;
    }
}
