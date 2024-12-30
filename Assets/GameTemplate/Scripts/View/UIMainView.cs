using deVoid.UIFramework;
using UnityEngine;
using UnityEngine.UI;


public class UIMainView : APanelController
{
    [SerializeField] private Button btnPlay;

    protected override void AddListeners()
    {
        base.AddListeners();
        btnPlay.onClick.AddListener(OnPlayGame);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        btnPlay.onClick.RemoveListener(OnPlayGame);
    }

    private void OnPlayGame()
    {
        Hide();
    }
}