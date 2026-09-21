using UnityEngine;

[CreateAssetMenu(fileName = "NewSong", menuName = "Music Executive/Song Data")]
public class SongData : ScriptableObject
{
    [Header("Identity")]
    public string songTitle = "Untitled";
    public string artistName = "Unknown Artist";

    [Header("Musical Info")]
    public float bpm = 120f;
    public string musicalKey = "C Major";

    [Header("Stems")]
    public StemTrack[] stems;
}

[System.Serializable]
public class StemTrack
{
    public string trackName;      // e.g. "Vocals", "Drums", "Bass"
    public AudioClip audioClip;
    [Range(0f, 1f)]
    public float defaultVolume = 1f;
}