using UnityEngine;

public abstract class PlayerState
{
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

    protected void SwitchState(PlayerState newState)
    {
        ExitState();

        newState.EnterState();
        //Debug.Log("Enter player state : " + newState.ToString());

        _ctx.CurrentState = newState;
    }
}
