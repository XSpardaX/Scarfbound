using UnityEngine;
using UnityEngine.AI;

public class BossContext
{
    public BossEnemy Boss;
    public NavMeshAgent Agent;
    public GraphPatrolController Patrol;
    public Animator Animator;
    public Transform Player;
    public Collider BodyCollider;
    public bool BodyColliderWasTrigger;

    public BossBehaviorState State = BossBehaviorState.Active;
    public BossRecoveryStep RecoveryStep;
    public bool FightStarted;
    public bool IsRunAnimPlaying;
    public bool IsAttacking;
    public bool IsDeathStomp;
    public bool IsPhaseTimerPaused;
    public bool Phase3AlternateShot;
    public int StompCount;
    public int CurrentPhase = 1;

    public float AttackEndTime;
    public float RecoveryEndTime;
    public float FireTimer;
    public float PhaseEndTime;
    public float PhasePauseStartTime;
    public float StaggerEndTime;
    public float CurrentProjectileSpeed;
    public float CurrentFireIntervalMultiplier;

    public Vector3 SpawnPosition;
    public Quaternion SpawnRotation;

    public bool IsHarmless
    {
        get
        {
            if (State == BossBehaviorState.Staggered)
            {
                return true;
            }

            if (State == BossBehaviorState.Recovering)
            {
                return true;
            }

            if (State == BossBehaviorState.Dying)
            {
                return true;
            }

            return false;
        }
    }
}
