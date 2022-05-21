using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingState : PlayerState
{
    public ShootingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
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
        if (!_ctx.IsAiming)
            SwitchState(_factory.Idle());
        else if (_ctx.IsAiming && _ctx.IsMoving)
            SwitchState(_factory.MovingAndShooting());
        else if (!_ctx.IsAiming && _ctx.IsMoving)
            SwitchState(_factory.Moving());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        _ctx.WantedDirection = _ctx.AimDirection;

        _ctx._currentWeapon.StartShoot(
            _ctx.transform.forward, _ctx._currentWeapon.weaponData.range, _ctx._currentWeapon.weaponData.fireRate, 
            _ctx._currentWeapon.weaponData.aimAssist);
    }
}
