using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }

    public override void CheckSwitchStates()
    {
        Vector3 directionToTarget = _ctx.TargetPos - _ctx.transform.position;
        float dSqrToTarget = directionToTarget.sqrMagnitude;

        if (dSqrToTarget <= Mathf.Pow(_ctx.AttackRange, 2) && _ctx.Fov.PlayerInLOS)
            SwitchState(_factory.Attack());
        else if (!_ctx.Fov.PlayerInRange || !_ctx.Fov.PlayerInLOS)
            SwitchState(_factory.Search());
    }

    public override void EnterState()
    {
        _ctx.Mat.color = Color.yellow;
        //_ctx.Agent.stoppingDistance = _ctx.AttackRange;
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        Debug.DrawLine(_ctx.transform.position, _ctx.TargetPos, Color.white);
        _ctx.Agent.SetDestination(_ctx.TargetPos);
    }
}