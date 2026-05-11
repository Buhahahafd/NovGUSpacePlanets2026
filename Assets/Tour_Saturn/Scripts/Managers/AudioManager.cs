using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource voiceSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip startClip;
    [SerializeField] private AudioClip atmosphereClip;
    [SerializeField] private AudioClip observationClip;
    [SerializeField] private AudioClip researchClip;
    [SerializeField] private AudioClip questClip;
    [SerializeField] private AudioClip returnClip;
    [SerializeField] private AudioClip endClip;

    private void PlayClip(AudioClip clip)
    {
        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    public void PlayStartAudio()
    {
        PlayClip(startClip);
    }

    public void PlayAtmosphereAudio()
    {
        PlayClip(atmosphereClip);
    }

    public void PlayObservationAudio()
    {
        PlayClip(observationClip);
    }

    public void PlayResearchAudio()
    {
        PlayClip(researchClip);
    }

    public void PlayQuestAudio()
    {
        PlayClip(questClip);
    }

    public void PlayReturnAudio()
    {
        PlayClip(returnClip);
    }

    public void PlayEndAudio()
    {
        PlayClip(endClip);
    }
}
