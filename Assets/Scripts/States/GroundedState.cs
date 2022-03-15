using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : PlayerState
{
    public GroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
        InitializeSubState();
    }

    public override void EnterState()
    {
        Debug.Log("Enter Grounded state");
        _ctx.CanMove = true;
    }

    public override void ExitState()
    {

    }

    public override void CheckSwitchStates()
    {
        if (_ctx.IsRolling)
            SwitchState(_factory.Rolling());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void InitializeSubState()
    {
        if (!_ctx.IsMoving && !_ctx.IsAiming)
            SetSubState(_factory.Idle());
        else if (_ctx.IsMoving && !_ctx.IsAiming)
            SetSubState(_factory.Moving());
        else if (_ctx.IsAiming && !_ctx.IsMoving)
            SetSubState(_factory.Shooting());
        else
            SetSubState(_factory.MovingAndShooting());
    }

    
}
