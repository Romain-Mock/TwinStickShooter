using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour
{
    //Move between multiple points infinitely (patrol state)
    //If it spots the player, switches to attack state and follow and shoot at the player if is in attack range
    //Notifies all other enemies to switch to attack state and chase the player as well, attacking it when in range
    //If the enemy dies or loses sight of the player, go to the last known player's position and switches to search state
    //Notifies all other enemies to go the the same point and switch to search state
    //wander around that point until it finds the player (switch to attack state again) or until a certain timer expires and return to patrol state

    private EnemyStateFactory _states;

    [SerializeField]
    private float _speedPatrol = 8f;
    [SerializeField]
    private float _speedAttack = 5f;
    [SerializeField]
    private float _stoppingDistPatrol;
    [SerializeField]
    private float _AttackRange;
    [SerializeField]
    private float _timeToLooseAggro;
    [SerializeField]
    private float _aimOffset;
    public Weapon weapon;
    private WeaponData data;

    private bool _isPlayerInLOS;
    private bool _isPlayerInRange;

    public Material Mat { get; set; }
    public Transform anchorWeapon;

    //Getters and setters
    public EnemyState CurrentState { get; set; }
    public NavMeshAgent Agent { get; private set; }
    public GameObject[] NavPoints { get; set; }
    public int NextPoint { get; set; } = 0;
    public FieldOfView Fov { get; private set; }
    public Vector3 TargetPos { get; set; }
    public float StoppingDistancePatrol { get { return _stoppingDistPatrol; } }
    public float AttackRange { get { return _AttackRange; } }
    public float SpeedPatrol { get { return _speedPatrol; } }
    public float SpeedAttack { get { return _speedAttack; } }
    public float TimeToLooseAggro { get { return _timeToLooseAggro; } }
    public float LastPlayerPos { get; set; }
    public float AimOffset { get { return _aimOffset; } }

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Fov = GetComponent<FieldOfView>();

        data = weapon.weaponData;

        Mat = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        Mat.color = Color.black;

        _states = new EnemyStateFactory(this);
        CurrentState = _states.Patrol();
        CurrentState.EnterState();

    }

    // Start is called before the first frame update
    void Start()
    {
        weapon = Instantiate(weapon, anchorWeapon.position, anchorWeapon.rotation, anchorWeapon);
        weapon.IsEquipped = true;
        weapon.GetComponent<Rigidbody>().isKinematic = true;
        _AttackRange = weapon.weaponData.range;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CurrentState.UpdateState();
        Fov.FindVisibleTargets();

        if (Fov.Target)
            TargetPos = Fov.Target.position;
    }

    public void Shoot()
    {   
        //RaycastHit hitInfo;
        //if (Physics.Raycast(transform.position, transform.forward, out hitInfo, data.range))
        //{
        //    weapon.TrailEffect(data, transform.position, hitInfo.point, hitInfo.normal);
        //}
    }

    public IEnumerator RandomizeDir()
    {
        Debug.Log("Update searchPoint coroutine");
        TargetPos += new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
        Agent.destination = TargetPos;

        yield return new WaitForSeconds(1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(TargetPos, 1);
    }
}
