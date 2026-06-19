using UnityEngine;

/// <summary>
/// Singleton that plays sound effects. Generates all SFX procedurally with
/// AudioSource.PlayOneShot + AudioClip.Create so you don't need any audio files.
/// Drop real clips into the Inspector slots later if you want.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Optional: drag real audio clips here (otherwise generated)")]
    public AudioClip clickClip;
    public AudioClip successClip;
    public AudioClip errorClip;
    public AudioClip completeClip;
    public AudioClip beepClip;

    private AudioSource _src;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _src = gameObject.AddComponent<AudioSource>();
        _src.playOnAwake = false;
    }

    public void PlayClick()    { Play(clickClip ?? GenTone(800, 0.05f, 0.3f)); }
    public void PlaySuccess()  { Play(successClip ?? GenChord(new[] { 523, 659, 784 }, 0.3f)); }
    public void PlayError()    { Play(errorClip ?? GenTone(200, 0.2f, 0.4f)); }
    public void PlayComplete() { Play(completeClip ?? GenChord(new[] { 523, 659, 784, 1047 }, 0.6f)); }
    public void PlayBeep()     { Play(beepClip ?? GenTone(1000, 0.03f, 0.2f)); }

    void Play(AudioClip clip)
    {
        if (clip != null && _src != null) _src.PlayOneShot(clip, 0.7f);
    }

    // --- Procedural clip generators (no files needed) ---
    AudioClip GenTone(int freq, float dur, float vol)
    {
        int sr = 44100;
        int samples = Mathf.RoundToInt(sr * dur);
        var clip = AudioClip.Create("tone", samples, 1, sr, false);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sr;
            data[i] = Mathf.Sin(2 * Mathf.PI * freq * t) * vol * (1 - t / dur);
        }
        clip.SetData(data, 0);
        return clip;
    }

    AudioClip GenChord(int[] freqs, float dur)
    {
        int sr = 44100;
        int samples = Mathf.RoundToInt(sr * dur);
        var clip = AudioClip.Create("chord", samples, 1, sr, false);
        float[] data = new float[samples];
        float vol = 0.2f / freqs.Length;
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sr;
            float v = 0;
            foreach (int f in freqs) v += Mathf.Sin(2 * Mathf.PI * f * t);
            data[i] = v * vol * (1 - t / dur);
        }
        clip.SetData(data, 0);
        return clip;
    }
}
