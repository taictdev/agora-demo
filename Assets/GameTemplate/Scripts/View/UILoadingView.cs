using deVoid.UIFramework;
using UnityEngine;
using UnityEngine.UI;

public class LoadingViewProperties : PanelProperties
{
    public bool HasBackground = true;
}

public class UILoadingView : APanelController<LoadingViewProperties>
{
    [SerializeField] private Image imgLoading;
    [SerializeField] private Image imgBG;

    protected override void OnPropertiesSet()
    {
        base.OnPropertiesSet();
        imgBG.gameObject.SetActive(Properties != null && Properties.HasBackground);
    }

    private void Update()
    {
        imgLoading.transform.Rotate(0, 0, 1);
    }
}