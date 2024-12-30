using UnityEngine;
using Utils;

public class SoundManager : ManualSingletonMono<SoundManager>
{
    private AudioSource audioBG;

    public void PlayBG(AudioClip clipBG)
    {
        if (audioBG == null)
        {
            audioBG = new GameObject("audioBG").AddComponent<AudioSource>();
            audioBG.loop = true;
        }

        audioBG.clip = clipBG;
        audioBG.Play();
    }

    public void StopBG()
    {
        audioBG.Stop();
    }

    public AudioSource PlaySFX(AudioClip clip, float duration = -1)
    {
        var audioSFX = new GameObject(clip.name).AddComponent<AudioSource>();
        audioSFX.clip = clip;
        audioSFX.Play();
        Destroy(audioSFX, duration > 0 ? duration : clip.length);
        return audioSFX;
    }
}