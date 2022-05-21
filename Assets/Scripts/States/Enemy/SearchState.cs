using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : EnemyState
{
    float timer;
    public SearchState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }

    public override void CheckSwitchStates()
    {
        if (timer <= 0)
        {
            SwitchState(_factory.Patrol());
        }

        if (_ctx.Fov.PlayerInLOS)
            SwitchState(_factory.Chase());
    }

    public override void EnterState()
    {
        _ctx.Agent.stoppingDistance = _ctx.StoppingDistancePatrol;
        _ctx.Mat.color = Color.magenta;
        _ctx.StartCoroutine(_ctx.RandomizeDir());
        timer = 10f;
    }

    public override void ExitState()
    {
        _ctx.StopCoroutine(_ctx.RandomizeDir());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        _ctx.Agent.destination = _ctx.TargetPos;

        timer -= Time.deltaTime;
    }
}
