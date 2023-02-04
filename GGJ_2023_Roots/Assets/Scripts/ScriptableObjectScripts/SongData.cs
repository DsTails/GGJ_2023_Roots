using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Song Data", menuName = "Scriptable Objects/Song Data")]
public class SongData : ScriptableObject
{
    public AudioClip songClip;
    public string tempoMapLocation;

    public string songName;
    public string songDescription;
}
