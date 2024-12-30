using Cysharp.Threading.Tasks;

public interface IState
{
    UniTask Enter();

    UniTask Exit();
}

public class StateManager
{
    private IState currentState;

    public async UniTask ChangeState(IState state)
    {
        if (state == currentState) return;

        if (currentState != null)
        {
            await currentState.Exit();
        }
        currentState = state;
        await currentState.Enter();
    }
}

public class PrePlayState : IState
{
    public UniTask Enter()
    {
        throw new System.NotImplementedException();
    }

    public UniTask Exit()
    {
        throw new System.NotImplementedException();
    }
}

public class PlayingState : IState
{
    public UniTask Enter()
    {
        throw new System.NotImplementedException();
    }

    public UniTask Exit()
    {
        throw new System.NotImplementedException();
    }
}

public class WinState : IState
{
    public UniTask Enter()
    {
        throw new System.NotImplementedException();
    }

    public UniTask Exit()
    {
        throw new System.NotImplementedException();
    }
}

public class LoseState : IState
{
    public UniTask Enter()
    {
        throw new System.NotImplementedException();
    }

    public UniTask Exit()
    {
        throw new System.NotImplementedException();
    }
}