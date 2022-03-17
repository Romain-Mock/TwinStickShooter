using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticState : EnemyState
{
    public StaticState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }

    public override void CheckSwitchStates()
    {
        throw new System.NotImplementedException();
    }

    public override void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}