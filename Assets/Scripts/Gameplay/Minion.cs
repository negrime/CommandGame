using UnityEngine;
using UnityEngine.AI;

using System;

using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class Minion : BaseMinion 
{
    const string SoundSoloAttackKey = "minion_attack_solo";
    const string SoundSoloBattleCryKey  = "minion_battlecry_solo";
    const string SoundGroupBattleCryKey = "minion_battlecry_group";
    
    const float FollowDist = 40f;
    const float FollowSpeed = 7f;
    const float WanderSpeed = 3f;
    const float RunSpeed = 9f;
    
    public enum State {
        Following,
        Attacking,
        Fighting,
        Wandering,
        Dying,
    }

    public Player Player;
    public SoundPlayer SoundPlayer;

    BaseUnit _target;

    bool _canAttack = true;

    State _curState = State.Following;

    public State CurState 
    {
        get => _curState;
        private set 
        {
            if ( _curState == value ) 
            {
                return;
            }
            _curState = value;
            OnStateChanged?.Invoke(_curState);
        }
    }

    public event Action<State> OnStateChanged;

    public override Team Team => Team.Player;

    protected new void Start()
    {
        base.Start();
        NavMeshAgent.updateRotation = false;
        NavMeshAgent.speed = FollowSpeed;

        AttackCollider.Init(TryInitFight, TryInitFight);
    }

    void Update() 
    {
        if ( PauseController.IsPaused ) 
        {
            return;
        }
        switch ( CurState )
        {
            case State.Wandering: 
            {
                if ( GetPlayerDistance() < FollowDist )
                {
                    CurState = State.Following;
                } else 
                {
                    if ( (NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance) ||
                         (NavMeshAgent.pathStatus != NavMeshPathStatus.PathComplete) ) 
                    {
                        var rand = Random.insideUnitCircle * (NavMeshAgent.stoppingDistance * 2f);
                        NavMeshAgent.SetDestination(transform.position + new Vector3(rand.x, 0, rand.y));
                    }
                }
                break;
            }
            case State.Following:
            {
                NavMeshAgent.stoppingDistance = Player.MinionManager.ActiveMinionsNum / 3f;
                if ( GetPlayerDistance() > FollowDist )
                {
                    NavMeshAgent.SetDestination(transform.position);
                    NavMeshAgent.speed = WanderSpeed;
                    CurState = State.Wandering;
                } else
                {
                    NavMeshAgent.SetDestination(Player.transform.position);
                    NavMeshAgent.speed = FollowSpeed;
                }
                break;
            }
            case State.Attacking:
            {
                if ( _target )
                {
                    NavMeshAgent.SetDestination(_target.transform.position);
                } else
                {
                    if ( CurEnemies.Count == 0 )
                    {
                        CurState = State.Following;
                        NavMeshAgent.stoppingDistance = 5f;
                        break;
                    }
                    BaseUnit nextEnemy = null;
                    var minDist = float.MaxValue;
                    var myPosRaw = transform.position;
                    var myPos = new Vector2(myPosRaw.x, myPosRaw.z);
                    for ( var i = CurEnemies.Count - 1; i >= 0; i-- ) {
                        var enemy = CurEnemies[i];
                        if ( !enemy ) 
                        {
                            CurEnemies.RemoveAt(i);
                            continue;
                        }
                        var enemyPosRaw = enemy.transform.position;
                        var enemyPos    = new Vector2(enemyPosRaw.x, enemyPosRaw.z);
                        var tmpDist     = Vector2.Distance(enemyPos, myPos);
                        if ( tmpDist < minDist ) 
                        {
                            minDist   = tmpDist;
                            nextEnemy = enemy;
                        }
                    }
                    if ( nextEnemy )
                    {
                        _target = nextEnemy;
                        _target.OnDied += OnTargetDied;
                    }
                }
                break;
            }
            case State.Fighting: 
            {
                break;
            }
        }
    }

    public void Init(Player player) 
    {
        Player = player;
    }

    public void Attack(BaseUnit unit) 
    {
        if ( CurState != State.Following ) 
        {
            return;
        }
        CurState = State.Attacking;
        NavMeshAgent.speed = RunSpeed;
        NavMeshAgent.stoppingDistance = 0f;
        _target         = unit;
        _target.OnDied += OnTargetDied;

        if ( SoundPlayer ) 
        {
            if ( Player.MinionManager.ActiveMinionsNum < 4 ) 
            {
                SoundPlayer.PlayOneShot(SoundsManager.Instance.GetClip(SoundSoloBattleCryKey));
            } else
            {
                SoundPlayer.PlayOneShot(SoundsManager.Instance.GetClip(SoundGroupBattleCryKey), 0.5f);
            }
        }
    }

    public void FinishDying() 
    {
        Destroy(gameObject);
    }

    protected override void DieSpecific() 
    {
        CurState = State.Dying;
    }

    float GetPlayerDistance()
    {
        var myPosRaw     = transform.position;
        var myPos        = new Vector2(myPosRaw.x, myPosRaw.z);
        var playerPosRaw = Player.transform.position;
        var playerPos    = new Vector2(playerPosRaw.x, playerPosRaw.z);
        return Vector2.Distance(myPos, playerPos);
    }

    void OnTargetDied(BaseUnit enemy) 
    {
        if ( _target != enemy )
        {
            Debug.LogError("Unexpected enemy");
            return;
        }
        _target.OnDied -= OnTargetDied;
        _target         = null;
        _canAttack = true;
    }

    void TryInitFight(GameObject other)
    {
        if ( PauseController.IsPaused ) 
        {
            return;
        }
        var enemy = GetEnemy(other);
        if ( enemy )
        {
            TryAttack(enemy);
        }
    }
    
    void TryAttack(BaseUnit enemyUnit) 
    {
        if ( CurState != State.Attacking ) 
        {
            return;
        }
        if ( !_canAttack )
        {
            return;
        }
        if ( _target && (enemyUnit != _target) )
        {
            _target.OnDied -= OnTargetDied;
            _target = enemyUnit;
            _target.OnDied += OnTargetDied;
        }
        CurState   = State.Fighting;
        _canAttack = false;
    }

    public void MakeAttack() 
    {
        if ( IsAlive ) {
            if ( _target && _target.IsAlive ) 
            {
                Fight(_target);
                if ( SoundPlayer )
                {
                    SoundPlayer.PlayOneShot(SoundsManager.Instance.GetClip(SoundSoloAttackKey));
                }
                _canAttack = true;
            }
            CurState = State.Attacking;
        }
    }
}
