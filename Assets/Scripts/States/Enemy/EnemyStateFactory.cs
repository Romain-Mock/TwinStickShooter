using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateFactory : MonoBehaviour
{
    EnemyStateMachine _context;

    public EnemyStateFactory(EnemyStateMachine context)
    {
        _context = context;
    }
}
