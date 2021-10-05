using LMM_Movement;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomTileManager", menuName = "ScriptableObjects/GimmickVoix", order = 3)]
public class GimmickVoix : ScriptableObject
{
    public string affichage;
    public AudioClip clip;
    public float volume = 1f;
}

