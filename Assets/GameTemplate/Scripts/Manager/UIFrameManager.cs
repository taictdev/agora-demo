using System.Collections.Generic;
using deVoid.UIFramework;
using UnityEngine;
using Utils;

public class UIFrameManager : ManualSingletonMono<UIFrameManager>
{
    [SerializeField] private List<UISettings> uISettings;
    public UIFrame uIFrame { get; private set; }
    public override void Awake()
    {
        base.Awake();
        foreach (UISettings uiSetting in uISettings)
        {
            InitUISettings(uiSetting);
        }

    }
    private void InitUISettings(UISettings uiSettings)
    {
        uIFrame = uiSettings.CreateUIInstance(true);
    }
}