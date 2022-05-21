using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateFactory
{
    EnemyStateMachine _context;

    public EnemyStateFactory(EnemyStateMachine context)
    {
        _context = context;
    }

    public EnemyState Chase()
    {
        return new ChaseState(_context, this);
    }

    public EnemyState Attack()
    {
        return new AttackState(_context, this);
    }

    public EnemyState Search()
    {
        return new SearchState(_context, this);
    }

    public EnemyState Patrol()
    {
        return new PatrolState(_context, this);
    }
}
