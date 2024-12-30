using UnityEngine;
using Utils;

public class UserManager : ManualSingletonMono<UserManager>
{
    [SerializeField] private float elapsedTime;
    [SerializeField] private int numberQuestion = 15;

    public float ElapsedTime { get => elapsedTime; set => elapsedTime = value; }
    public int NumberQuestion { get => numberQuestion; set => numberQuestion = value; }
}