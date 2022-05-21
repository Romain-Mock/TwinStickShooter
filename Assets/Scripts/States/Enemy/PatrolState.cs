using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : EnemyState
{
    public PatrolState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }

    public override void CheckSwitchStates()
    {
        if (_ctx.Fov.PlayerInLOS && _ctx.Fov.PlayerInRange)
            SwitchState(_factory.Chase());
    }

    public override void EnterState()
    {
        _ctx.NavPoints = GameObject.FindGameObjectsWithTag("Navigation");
        _ctx.Agent.stoppingDistance = _ctx.StoppingDistancePatrol;
        _ctx.Agent.speed = _ctx.SpeedPatrol;
        _ctx.Mat.color = Color.green;
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        if (!_ctx.Agent.pathPending && _ctx.Agent.remainingDistance < _ctx.StoppingDistancePatrol)
            GoToNextPoint();
    }

    void GoToNextPoint()
    {
        if (_ctx.NavPoints.Length == 0)
            return;

        _ctx.Agent.SetDestination(_ctx.NavPoints[_ctx.NextPoint].transform.position);

        _ctx.NextPoint = (_ctx.NextPoint + 1) % _ctx.NavPoints.Length;
    }
}
