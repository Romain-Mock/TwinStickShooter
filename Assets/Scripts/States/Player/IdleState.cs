using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }

    public override void EnterState()
    {
        _ctx.Animator.Play("Idle");
    }

    public override void ExitState()
    {

    }

    public override void CheckSwitchStates()
    {
        if (_ctx.IsMoving && _ctx.IsAiming)
            SwitchState(_factory.MovingAndShooting());
        if (_ctx.IsAiming && !_ctx.IsMoving)
            SwitchState(_factory.Shooting());
        if (_ctx.IsMoving && !_ctx.IsAiming)
            SwitchState(_factory.Moving());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}
