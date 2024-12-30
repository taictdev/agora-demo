using deVoid.UIFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWinView : APanelController
{
    [SerializeField] private Button btnOk;

    protected override void Awake()
    {
        base.Awake();
        btnOk.onClick.AddListener(OnClick);
    }

    protected override void OnPropertiesSet()
    {
        base.OnPropertiesSet();
    }

    private void OnClick()
    {
    }
}