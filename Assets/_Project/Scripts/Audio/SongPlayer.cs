using System.Collections.Generic;
using UnityEngine;

public class SongPlayer : MonoBehaviour
{
    public SongData song;

    private List<AudioSource> stemSources = new List<AudioSource>();

    void Start()
    {
        Debug.Log("SongPlayer Start() running.");

        if (song == null)
        {
            Debug.LogError("SongPlayer: 'Song' field is not assigned in the Inspector.");
            return;
        }

        if (song.stems == null || song.stems.Length == 0)
        {
            Debug.LogError("SongPlayer: SongData '" + song.name + "' has no stems assigned.");
            return;
        }

        Debug.Log("SongPlayer: found " + song.stems.Length + " stem(s) on '" + song.name + "'.");

        BuildStemSources();
        Play();
    }

    void BuildStemSources()
    {
        foreach (StemTrack stem in song.stems)
        {
            if (stem.audioClip == null)
            {
                Debug.LogWarning("SongPlayer: stem '" + stem.trackName + "' has no Audio Clip assigned — skipping.");
                continue;
            }

            GameObject stemObject = new GameObject("Stem_" + stem.trackName);
            stemObject.transform.parent = transform;

            AudioSource source = stemObject.AddComponent<AudioSource>();
            source.clip = stem.audioClip;
            source.volume = stem.defaultVolume;
            source.playOnAwake = false;

            stemSources.Add(source);
            Debug.Log("SongPlayer: built AudioSource for '" + stem.trackName + "' (clip: " + stem.audioClip.name + ", volume: " + stem.defaultVolume + ").");
        }

        Debug.Log("SongPlayer: total playable stem sources built: " + stemSources.Count);
    }

    public void Play()
    {
        if (stemSources.Count == 0)
        {
            Debug.LogError("SongPlayer: Play() called but there are zero stem sources to play.");
            return;
        }

        double startTime = AudioSettings.dspTime + 0.1;
        foreach (AudioSource source in stemSources)
        {
            source.PlayScheduled(startTime);
            Debug.Log("SongPlayer: scheduled '" + source.gameObject.name + "' at dspTime " + startTime);
        }
    }

    public void Stop()
    {
        foreach (AudioSource source in stemSources)
            source.Stop();
    }
}