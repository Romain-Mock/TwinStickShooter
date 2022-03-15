using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingState : PlayerState
{
    public RollingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        Debug.Log("Enter rolling state");

        MonoBehaviour.FindObjectOfType<WeaponManager>().enabled = false;
        _ctx.IsRolling = true;
        _ctx.Animator.SetTrigger("Roll");
        _ctx.Rigidbody.AddForce(_ctx.transform.forward * _ctx.RollSpeed, ForceMode.VelocityChange);
    }

    public override void ExitState()
    {
        MonoBehaviour.FindObjectOfType<WeaponManager>().enabled = true;
        _ctx.CanMove = false;
        _ctx.Rigidbody.velocity = Vector3.zero;
    }

    public override void CheckSwitchStates()
    {
        if(!_ctx.IsRolling)
        {
            SwitchState(_factory.Grounded());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void InitializeSubState()
    {

    }

   
}
