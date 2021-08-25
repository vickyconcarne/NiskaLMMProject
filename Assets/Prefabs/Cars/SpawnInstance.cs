using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMM_Movement;

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnInstance", order = 1)]
public class SpawnInstance : ScriptableObject
{
    public string ressourceID;
    public bool fromTop; //Spawn from top of screen or bottom
    public lane laneToPlace;
}