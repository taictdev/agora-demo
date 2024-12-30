using UnityEngine;
using Utils;

public class AudioClipManager : ManualSingletonMono<AudioClipManager>
{
    [SerializeField] private AudioClip clipBG;
    [SerializeField] private AudioClip clipCorrectAnwser;
    [SerializeField] private AudioClip clipInCorrectAnwser;
    [SerializeField] private AudioClip clipStartQuestion;
    [SerializeField] private AudioClip clipTimeOut;
    [SerializeField] private AudioClip clipClock;

    public AudioClip ClipBG { get => clipBG; set => clipBG = value; }
    public AudioClip ClipCorrectAnwser { get => clipCorrectAnwser; set => clipCorrectAnwser = value; }
    public AudioClip ClipInCorrectAnwser { get => clipInCorrectAnwser; set => clipInCorrectAnwser = value; }
    public AudioClip ClipStartQuestion { get => clipStartQuestion; set => clipStartQuestion = value; }
    public AudioClip ClipClock { get => clipClock; set => clipClock = value; }
    public AudioClip ClipTimeOut { get => clipTimeOut; set => clipTimeOut = value; }
}