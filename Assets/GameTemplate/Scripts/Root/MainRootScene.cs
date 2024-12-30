using Utils;

public class MainRootScene : ManualSingletonMono<MainRootScene>
{
    private StateManager gameStateManager;

    public override void Awake()
    {
        base.Awake();
        gameStateManager = new StateManager();
    }

    private void Start()
    {
        
    }

    public async void ChangeState(IState state)
    {
        await gameStateManager.ChangeState(state);
    }
}