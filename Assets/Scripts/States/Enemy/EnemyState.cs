using UnityEngine;

public abstract class EnemyState
{
    protected EnemyStateMachine _ctx;
    protected EnemyStateFactory _factory;
    
    public EnemyState(EnemyStateMachine currentContext, EnemyStateFactory enemyStateFactory)
    {
        _ctx = currentContext;
        _factory = enemyStateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchStates();

    protected void SwitchState(PlayerState newState)
    {
        ExitState();

        newState.EnterState();

        //_ctx.CurrentState = newState;
    }
}
