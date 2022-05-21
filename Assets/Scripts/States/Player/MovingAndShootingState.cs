using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAndShootingState : PlayerState
{
    public MovingAndShootingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }

    public override void EnterState()
    {
       
    }

    public override void ExitState()
    {

    }

    public override void CheckSwitchStates()
    {
        if (!_ctx.IsMoving && !_ctx.IsAiming)
            SwitchState(_factory.Idle());
        else if (!_ctx.IsMoving && _ctx.IsAiming)
            SwitchState(_factory.Shooting());
        else if (_ctx.IsMoving && !_ctx.IsAiming)
            SwitchState(_factory.Moving());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        _ctx.WantedDirection = _ctx.AimDirection;
        _ctx.Rigidbody.position += _ctx.MoveDirection * _ctx.MoveSpeed * Time.fixedDeltaTime;

        _ctx._currentWeapon.StartShoot(
            _ctx._currentWeapon.transform.forward, _ctx._currentWeapon.weaponData.range, _ctx._currentWeapon.weaponData.fireRate,
            _ctx._currentWeapon.weaponData.aimAssist);
    }
}
