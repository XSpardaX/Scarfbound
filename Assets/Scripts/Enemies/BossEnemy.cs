using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(GraphPatrolController))]
public class BossEnemy : EnemyBase
{
    public float reachDistance = 0.5f;

    [Header("Combat")]
    public float knockbackForce = 8f;
    public float defendDistance = 4f;
    public float defendReleaseDistance = 5f;
    public float faceTurnSpeed = 12f;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int projectileDamage = 1;

    [Header("Distance Scaling")]
    public float closeFireInterval = 4f;
    public float farFireInterval = 1f;
    public float minFireRateDistance = 3f;
    public float maxFireRateDistance = 20f;

    [Header("Phases")]
    public float phase1Duration = 10f;
    public float phase2Duration = 20f;
    public float phase3Duration = 30f;
    public float phase1MoveSpeed = 4f;
    public float phase2MoveSpeed = 4f;
    public float phase3MoveSpeed = 6f;
    public float phase1ProjectileSpeed = 12f;
    public float phase2ProjectileSpeed = 12f;
    public float phase3ProjectileSpeed = 16f;
    public float phase1FireIntervalMultiplier = 1f;
    public float phase2FireIntervalMultiplier = 0.85f;
    public float phase3FireIntervalMultiplier = 0.7f;
    public float phase2ProjectileYawOffset = 25f;
    public float phase2ConvergeStrength = 1f;
    public float phase2ConvergeTurnRate = 100f;
    public float phase3SideProjectileYawOffset = 45f;

    [Header("Stagger")]
    public Transform headPoint;
    public float headHeightOffset = 2.4f;
    public float headStompRadius = 1.2f;
    public float stompBounceForce = 10f;
    public float staggerDuration = 5f;

    [Header("Fight Start")]
    public bool startFightOnAwake = true;
    public string introAttackAnimState = "Attack02";
    public float introAttackAnimDuration = 0.833f;
    public int introAttackLayerIndex = 0;

    [Header("Death")]
    public GameObject keyPrefab;
    public Vector3 keySpawnOffset = new Vector3(0f, 1f, 0f);

    [Header("Animation")]
    public string idleAnimState = "Idle_Battle";
    public string runAnimState = "RunForwardBattle";
    public string attackAnimState = "Attack01";
    public string defendAnimState = "Defend";
    public string dizzyAnimState = "Dizzy";
    public string getHitAnimState = "GetHit";
    public string dieRecoverAnimState = "DieRecover";
    public string dieAnimState = "Die";
    public float animBlendDuration = 0.15f;
    public float moveAnimThreshold = 0.1f;
    public float attackAnimDuration = 0.85f;
    public float getHitAnimDuration = 0.833f;
    public float dieRecoverAnimDuration = 1.167f;
    public float dieAnimDuration = 1.167f;
    public int upperBodyLayerIndex = 1;

    private BossContext context;
    private BossMovementController movement;
    private BossAnimationController animation;
    private BossPhaseController phases;
    private BossRangedAttackController rangedAttack;
    private BossCombatController combat;

    private void Awake()
    {
        context = new BossContext
        {
            Boss = this,
            Agent = GetComponent<NavMeshAgent>(),
            Patrol = GetComponent<GraphPatrolController>(),
            Animator = GetComponentInChildren<Animator>(),
            BodyCollider = GetComponent<Collider>()
        };

        if (context.BodyCollider != null)
            context.BodyColliderWasTrigger = context.BodyCollider.isTrigger;

        CacheHeadPoint();

        context.Patrol.reachDistance = reachDistance;
        context.Patrol.branchStrategy = PatrolBranchStrategy.Random;
        context.Patrol.Initialize();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) context.Player = playerObject.transform;

        context.SpawnPosition = transform.position;
        context.SpawnRotation = transform.rotation;

        movement = new BossMovementController(context);
        animation = new BossAnimationController(context, this);
        phases = new BossPhaseController(context, this, movement, animation);
        rangedAttack = new BossRangedAttackController(context, this, animation);
        combat = new BossCombatController(context, this, movement, animation, phases);

        animation.Initialize();
        phases.ApplySettings();

        if (startFightOnAwake) phases.StartFight();
        else movement.Stop();
    }

    private void Update()
    {
        if (context.Patrol.Graph == null || context.Patrol.Graph.NodeCount == 0) return;

        phases.TickTimedStates();
        if (context.State == BossBehaviorState.Dying) return;

        if (context.State == BossBehaviorState.Staggered || context.State == BossBehaviorState.Recovering)
        {
            phases.TickStagger();
            movement.KeepStopped();
            return;
        }

        if (!context.FightStarted)
        {
            movement.KeepStopped();
            return;
        }

        combat.TickDefend();
        if (context.State == BossBehaviorState.Defending)
        {
            movement.KeepStopped();
            return;
        }

        phases.TickPhaseTimer();
        if (context.IsAttacking && Time.time >= context.AttackEndTime) animation.CancelAttack();
        rangedAttack.Tick();
        movement.TickPatrol();
    }

    private void LateUpdate()
    {
        if (context.State == BossBehaviorState.Defending || (context.FightStarted && context.State == BossBehaviorState.Active))
            movement.FacePlayer(faceTurnSpeed);

        if (!context.IsHarmless) animation.UpdateLocomotion();
    }

    public void StartFight() => phases.StartFight();

    public void ResetEncounter() => phases.ResetEncounter();

    public float PlayIntroAttack()
    {
        movement.Stop();
        animation.CancelAttack();
        context.IsRunAnimPlaying = false;
        context.State = BossBehaviorState.Active;
        animation.PlayIntroAttack();
        return introAttackAnimDuration;
    }

    public override void OnPlayerContact(Player touchingPlayer) => combat.OnPlayerContact(touchingPlayer);

    private void OnCollisionEnter(Collision collision) => combat.OnCollisionEnter(collision);

    private void OnCollisionStay(Collision collision) => combat.OnCollisionStay(collision);

    private void CacheHeadPoint()
    {
        if (headPoint != null) return;
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.name == "head") { headPoint = child; return; }
        }
    }
}
