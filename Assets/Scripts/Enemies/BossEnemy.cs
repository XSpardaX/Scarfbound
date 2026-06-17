using UnityEngine;
using UnityEngine.AI;

// boss brain, ties all fight parts together
public class BossEnemy : EnemyBase
{
    public float reachDistance = 0.5f;

    public float knockbackForce = 8f;
    public float defendDistance = 4f;
    public float defendReleaseDistance = 5f;
    public float faceTurnSpeed = 12f;

    public GameObject projectilePrefab;
    public Transform firePoint;
    public int projectileDamage = 1;

    public float closeFireInterval = 4f;
    public float farFireInterval = 1f;
    public float minFireRateDistance = 3f;
    public float maxFireRateDistance = 20f;

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

    public Transform headPoint;
    public float headHeightOffset = 2.4f;
    public float headStompRadius = 1.2f;
    public float stompBounceForce = 10f;
    public float staggerDuration = 5f;

    public bool startFightOnAwake = true;
    public string introAttackAnimState = "Attack02";
    public float introAttackAnimDuration = 0.833f;
    public int introAttackLayerIndex = 0;

    public GameObject keyPrefab;
    public Vector3 keySpawnOffset = new Vector3(0f, 1f, 0f);

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

    private BossContext bossContext;
    private BossMovementController movementController;
    private BossAnimationController animationController;
    private BossPhaseController phaseController;
    private BossRangedAttackController rangedAttackController;
    private BossCombatController combatController;

    // start up
    private void Awake()
    {
        bossContext = new BossContext
        {
            Boss = this,
            Agent = GetComponent<NavMeshAgent>(),
            Patrol = GetComponent<GraphPatrolController>(),
            Animator = GetComponentInChildren<Animator>(),
            BodyCollider = GetComponent<Collider>()
        };

        if (bossContext.BodyCollider != null)
        {
            bossContext.BodyColliderWasTrigger = bossContext.BodyCollider.isTrigger;
        }

        CacheHeadPoint();

        bossContext.Patrol.reachDistance = reachDistance;
        bossContext.Patrol.Initialize();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            bossContext.Player = playerObject.transform;
        }

        bossContext.SpawnPosition = transform.position;
        bossContext.SpawnRotation = transform.rotation;

        movementController = new BossMovementController(bossContext);
        animationController = new BossAnimationController(bossContext, this);
        phaseController = new BossPhaseController(bossContext, this, movementController, animationController);
        rangedAttackController = new BossRangedAttackController(bossContext, this, animationController);
        combatController = new BossCombatController(bossContext, this, movementController, animationController, phaseController);

        animationController.Initialize();
        phaseController.ApplySettings();

        if (startFightOnAwake)
        {
            phaseController.StartFight();
        }
        else
        {
            movementController.Stop();
        }
    }

    // each frame fight logic
    private void Update()
    {
        if (bossContext.Patrol.Graph == null || bossContext.Patrol.Graph.NodeCount == 0)
        {
            return;
        }

        phaseController.TickTimedStates();
        if (bossContext.State == BossBehaviorState.Dying)
        {
            return;
        }

        bool isInStaggerOrRecovery =
            bossContext.State == BossBehaviorState.Staggered ||
            bossContext.State == BossBehaviorState.Recovering;

        if (isInStaggerOrRecovery)
        {
            phaseController.TickStagger();
            movementController.KeepStopped();
            return;
        }

        if (!bossContext.FightStarted)
        {
            movementController.KeepStopped();
            return;
        }

        combatController.TickDefend();
        if (bossContext.State == BossBehaviorState.Defending)
        {
            movementController.KeepStopped();
            return;
        }

        phaseController.TickPhaseTimer();

        if (bossContext.IsAttacking && Time.time >= bossContext.AttackEndTime)
        {
            animationController.CancelAttack();
        }

        rangedAttackController.Tick();
        movementController.TickPatrol();
    }

    // face player and walk anim
    private void LateUpdate()
    {
        bool shouldFacePlayer =
            bossContext.State == BossBehaviorState.Defending ||
            (bossContext.FightStarted && bossContext.State == BossBehaviorState.Active);

        if (shouldFacePlayer)
        {
            movementController.FacePlayer(faceTurnSpeed);
        }

        if (!bossContext.IsHarmless)
        {
            animationController.UpdateLocomotion();
        }
    }

    // called by intro and respawn
    public void StartFight()
    {
        phaseController.StartFight();
    }

    public void ResetEncounter()
    {
        phaseController.ResetEncounter();
    }

    public void SuspendForPlayerDeath()
    {
        phaseController.SuspendForPlayerDeath();
    }

    public float PlayIntroAttack()
    {
        movementController.Stop();
        animationController.CancelAttack();
        bossContext.IsRunAnimPlaying = false;
        bossContext.State = BossBehaviorState.Active;
        animationController.PlayIntroAttack();
        return introAttackAnimDuration;
    }

    public override void OnPlayerContact(Player touchingPlayer)
    {
        combatController.OnPlayerContact(touchingPlayer);
    }

    private void OnCollisionEnter(Collision collision)
    {
        combatController.OnCollisionEnter(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        combatController.OnCollisionStay(collision);
    }

    private void CacheHeadPoint()
    {
        if (headPoint != null)
        {
            return;
        }

        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.name == "head")
            {
                headPoint = child;
                return;
            }
        }
    }
}
