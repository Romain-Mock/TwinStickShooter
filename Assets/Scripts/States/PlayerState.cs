using UnityEngine;

public abstract class PlayerState
{
    protected bool _isRootState = false;
    protected PlayerStateMachine _ctx;
    protected PlayerStateFactory _factory;

    protected PlayerState _currentSubState;
    protected PlayerState _currentSuperState;

    public PlayerState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchStates();

    public abstract void InitializeSubState();

    protected void SwitchState(PlayerState newState)
    {
        ExitState();

        newState.EnterState();

        if (_isRootState)
            _ctx.CurrentState = newState;
        else if (_currentSuperState != null)
            _currentSuperState.SetSubState(newState);
    }

    protected void SetSuperState(PlayerState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(PlayerState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }

    public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState != null)
            _currentSubState.UpdateStates();
    }

}
