using System.Collections.Generic;
using UnityEngine;

public class SongPlayer : MonoBehaviour
{
    public SongData song;

    private List<AudioSource> stemSources = new List<AudioSource>();
    private List<bool> manuallyMuted = new List<bool>();
    private List<bool> soloed = new List<bool>();

    // Solo keys map to tracks in order: Q=track1, W=track2, E=track3, etc.
    // Written out explicitly rather than as an offset from KeyCode.Q -
    // unlike the number row, letters aren't laid out in a way that math
    // on them would reliably give the next one.
    private KeyCode[] soloKeys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P };

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
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                ToggleMute(i);

            if (i < soloKeys.Length && Input.GetKeyDown(soloKeys[i]))
                ToggleSolo(i);
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
            manuallyMuted.Add(false);
            soloed.Add(false);
        }

        Debug.Log("SongPlayer: built " + stemSources.Count + " stem(s). Number keys mute, Q/W/E solo, Space stops.");
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
        manuallyMuted[stemIndex] = !manuallyMuted[stemIndex];
        RefreshAudibleStates();
    }

    public void ToggleSolo(int stemIndex)
    {
        if (stemIndex < 0 || stemIndex >= stemSources.Count) return;
        soloed[stemIndex] = !soloed[stemIndex];
        RefreshAudibleStates();
    }

    // Mute and solo aren't independent - this recalculates what's
    // ACTUALLY audible from both, the same way a real mixing console
    // works: if anything is soloed, every non-soloed track goes silent
    // regardless of its own manual mute state.
    void RefreshAudibleStates()
    {
        bool anySoloed = soloed.Contains(true);

        for (int i = 0; i < stemSources.Count; i++)
        {
            bool shouldBeSilent = anySoloed ? !soloed[i] : manuallyMuted[i];
            stemSources[i].mute = shouldBeSilent;

            Debug.Log("SongPlayer: '" + stemSources[i].gameObject.name + "' now " + (shouldBeSilent ? "SILENT" : "audible") + " (muted=" + manuallyMuted[i] + ", soloed=" + soloed[i] + ")");
        }
    }
}