using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingState : PlayerState
{
    public RollingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        _ctx.WeaponManager.enabled = false;
        _ctx.IsRolling = true;
        _ctx.Animator.Play("Roll");
    }

    public override void ExitState()
    {
        _ctx.WeaponManager.enabled = true;
        _ctx.Rigidbody.velocity = Vector3.zero;
    }

    public override void CheckSwitchStates()
    {
        SwitchState(_factory.Idle());
    }

    public override void UpdateState()
    {
        _ctx.Rigidbody.AddForce(_ctx.transform.forward * _ctx.RollSpeed, ForceMode.VelocityChange);
    }
}
