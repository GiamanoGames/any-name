using System.Collections.Generic;
using UnityEngine;

public class SongPlayer : MonoBehaviour
{
    public SongData song;

    private List<AudioSource> stemSources = new List<AudioSource>();

    void Start()
    {
        if (song == null || song.stems == null || song.stems.Length == 0)
        {
            Debug.LogError("SongPlayer: no valid SongData/stems assigned.");
            return;
        }

        BuildStemSources();
        Play();
    }

    void Update()
    {
        for (int i = 0; i < stemSources.Count; i++)
        {
            KeyCode key = KeyCode.Alpha1 + i;
            if (Input.GetKeyDown(key))
                ToggleMute(i);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Stop();
    }

    void BuildStemSources()
    {
        foreach (StemTrack stem in song.stems)
        {
            if (stem.audioClip == null) continue;

            GameObject stemObject = new GameObject("Stem_" + stem.trackName);
            stemObject.transform.parent = transform;

            AudioSource source = stemObject.AddComponent<AudioSource>();
            source.clip = stem.audioClip;
            source.volume = stem.defaultVolume;
            source.playOnAwake = false;
            source.loop = true;

            stemSources.Add(source);
        }

        Debug.Log("SongPlayer: built " + stemSources.Count + " looping stem(s). Press 1/2/3 to mute, Space to stop.");
    }

    public void Play()
    {
        if (stemSources.Count == 0) return;
        double startTime = AudioSettings.dspTime + 0.1;
        foreach (AudioSource source in stemSources)
            source.PlayScheduled(startTime);
    }

    public void Stop()
    {
        foreach (AudioSource source in stemSources)
            source.Stop();
    }

    public void ToggleMute(int stemIndex)
    {
        if (stemIndex < 0 || stemIndex >= stemSources.Count) return;
        AudioSource source = stemSources[stemIndex];
        source.mute = !source.mute;
        Debug.Log("SongPlayer: '" + source.gameObject.name + "' " + (source.mute ? "MUTED" : "unmuted"));
    }
}