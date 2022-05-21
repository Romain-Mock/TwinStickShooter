using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFactory
{
    PlayerStateMachine _context;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
    }

    public PlayerState Idle()
    {
        return new IdleState(_context, this);
    }

    public PlayerState Moving()
    {
        return new MovingState(_context, this);
    }

    public PlayerState Shooting()
    {
        return new ShootingState(_context, this);
    }

    public PlayerState MovingAndShooting()
    {
        return new MovingAndShootingState(_context, this);
    }

    public PlayerState Rolling()
    {
        return new RollingState(_context, this);
    }
}
