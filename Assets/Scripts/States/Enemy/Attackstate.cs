using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyState
{
    Vector3 dest;
    float timer;

    public AttackState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }

    public override void CheckSwitchStates()
    {
        Vector3 directionToTarget = _ctx.TargetPos - _ctx.transform.position;
        float dSqrToTarget = directionToTarget.sqrMagnitude;

        if (dSqrToTarget >= Mathf.Pow(_ctx.AttackRange, 2) && _ctx.Fov.PlayerInLOS)
        {
            SwitchState(_factory.Chase());
        }

        if (!_ctx.Fov.PlayerInLOS || !_ctx.Fov.PlayerInRange)
        {
            SwitchState(_factory.Search());
        }
    }

    public override void EnterState()
    {
        _ctx.Agent.stoppingDistance = _ctx.AttackRange;
        _ctx.Agent.speed = _ctx.SpeedAttack;
        _ctx.Mat.color = Color.red;

        //_ctx.StartCoroutine(EnemyShoot());
    }

    public override void ExitState()
    {
        //_ctx.StopCoroutine(EnemyShoot());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        _ctx.Agent.destination = _ctx.TargetPos;
        FaceTarget(_ctx.TargetPos);

        _ctx.weapon.StartShoot(AddRandomOffset(), _ctx.weapon.weaponData.range, _ctx.weapon.weaponData.fireRate, 0f);
    }

    private void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - _ctx.transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        _ctx.transform.rotation = Quaternion.Slerp(_ctx.transform.rotation, rotation, 1f);
        lookPos = destination - _ctx.anchorWeapon.position;
        rotation = Quaternion.LookRotation(lookPos);
        _ctx.anchorWeapon.rotation = Quaternion.Slerp(_ctx.anchorWeapon.rotation, rotation, 1f);
    }

    IEnumerator EnemyShoot()
    {
        while (true)
        {
            Vector3 dirToTarget = _ctx.TargetPos - _ctx.transform.position;
            dirToTarget += new Vector3(Random.Range(-_ctx.AimOffset, _ctx.AimOffset), 0, Random.Range(-_ctx.AimOffset, _ctx.AimOffset));
            dirToTarget.Normalize();
            //_ctx.weapon.TrailEffect(_ctx.weapon.weaponData, _ctx.anchorWeapon.position, dirToTarget * _ctx.weapon.weaponData.range, Vector3.zero);
            //_ctx.weapon.Shoot();

            yield return new WaitForSeconds(0.5f);
        }
    }

    Vector3 AddRandomOffset()
    {
        Vector3 dirToTarget = _ctx.TargetPos - _ctx.transform.position;
        dirToTarget.y = 0;
        dirToTarget += new Vector3(0, 0, Random.Range(-_ctx.AimOffset, _ctx.AimOffset));
        dirToTarget.Normalize();

        return dirToTarget;
    }
}
