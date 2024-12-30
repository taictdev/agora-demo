using deVoid.UIFramework;
using UnityEngine;
using UnityEngine.UI;

public class UILoseView : APanelController
{
    [SerializeField] private Button btnPlayAgain;

    protected override void Awake()
    {
        base.Awake();
        btnPlayAgain.onClick.AddListener(OnPlayAgain);
    }

    private void OnPlayAgain()
    {
        Hide();
    }
}