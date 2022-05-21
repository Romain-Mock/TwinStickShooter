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

    protected void SwitchState(EnemyState newState)
    {
        ExitState();

        newState.EnterState();
        //Debug.Log("Enemy state : " + newState.ToString());

        _ctx.CurrentState = newState;
    }
}
