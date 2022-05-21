using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : PlayerState
{
    public MovingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory)
    {

    }

    public override void EnterState()
    {
        //_ctx.Animator.Play("Running");
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        if (!_ctx.IsMoving)
            SwitchState(_factory.Idle());
        else if (_ctx.IsMoving && _ctx.IsAiming)
            SwitchState(_factory.MovingAndShooting());
        else if (!_ctx.IsMoving && _ctx.IsAiming)
            SwitchState(_factory.Shooting());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        _ctx.WantedDirection = _ctx.MoveDirection;
        _ctx.Rigidbody.position += _ctx.MoveDirection * _ctx.MoveSpeed * Time.fixedDeltaTime;
    }
}
