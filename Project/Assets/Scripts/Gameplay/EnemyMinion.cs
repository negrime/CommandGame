using UnityEngine;
using UnityEngine.AI;

using System;

using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
public class EnemyMinion : BaseMinion 
{
    const string AttackKey = "minion_attack_solo";
    const string BattleCryKey = "enemy_battlecry";
    const string DeathKey  = "enemy_death";
    const string BossDeathKey = "boss_death";
    const string BossBattleCryKey = "boss_battlecry";
    
    const float WalkSpeed = 11f;

    public enum State 
    {
        Guarding,
        Pursuing,
        Fighting,
        Dying,
    }

    public BossDeathDetector DeathDetector;
    public bool IsBoss = false;
    public Transform GuardZoneCenter;
    public float GuardZoneRadius;
    public SoundPlayer 
        SoundPlayer;
    
    State _curState = State.Guarding;

    BaseUnit _target;

    bool _canAttack = true;

    public event Action<State> OnStateChanged;
    
    public State CurState 
    {
        get => _curState;
        private set {
            if ( _curState == value ) 
            {
                return;
            }
            _curState = value;
            OnStateChanged?.Invoke(_curState);
        }
    }

    public override Team Team => Team.Enemy;

    protected override bool CanRegen => !IsBoss;

    protected new void Start()
    {
        base.Start();
        NavMeshAgent.speed = WalkSpeed;
        AttackCollider.Init(TryAttackSomething, TryAttackSomething);
        var rand = Random.insideUnitCircle;
        transform.position += new Vector3(rand.x, 0, rand.y);
    }

    void Update() 
    {
        if ( PauseController.IsPaused || !IsAlive )
        {
            return;
        }
        switch ( CurState ) 
        {
            case State.Guarding: 
            {
                if ( CurEnemies.Count > 0 ) 
                {
                    var enemy = FindNextEnemy();
                    if ( enemy ) 
                    {
                        _target        =  enemy;
                        _target.OnDied += OnTargetDied;
                        CurState      =  State.Pursuing;
                        SoundPlayer.PlayOneShot(IsBoss ? BossBattleCryKey : BattleCryKey);
                    }
                }
                break;
            }
            case State.Pursuing:
            {
                if ( !CheckGuardZoneDistance(_target) )
                {
                    // too far
                    CurState = State.Guarding;
                    var rand = Random.insideUnitSphere * (GuardZoneRadius / 2f);
                    rand                     = new Vector3(rand.x, GuardZoneCenter.hierarchyCapacity, rand.z);
                    NavMeshAgent.destination = GuardZoneCenter.position + rand;
                    break;
                }
                NavMeshAgent.enabled     = true;
                NavMeshAgent.destination = _target.transform.position;
                break;
            }
            case State.Fighting: 
            {
                break;
            }
        }
    }

    protected override void DieSpecific() 
    {
        CurState = State.Dying;
        _canAttack = false;

        if ( SoundPlayer ) 
        {
            SoundPlayer.PlayOneShot(IsBoss ? BossDeathKey : DeathKey);
        }
    }

    public void FinishDying() 
    {
        NavMeshAgent.enabled = false;
        if ( IsBoss ) 
        {
            DeathDetector.Win();
        } else 
        {
            Destroy(gameObject);
        }
    }

    BaseUnit FindNextEnemy() 
    {
        BaseUnit nextEnemy = null;
        var      minDist   = float.MaxValue;
        var      myPosRaw  = transform.position;
        var      myPos     = new Vector2(myPosRaw.x, myPosRaw.z);
        for ( var i = CurEnemies.Count - 1; i >= 0; i-- ) 
        {
            var enemy = CurEnemies[i];
            if ( !enemy ) 
            {
                CurEnemies.RemoveAt(i);
                continue;
            }
            var enemyPosRaw = enemy.transform.position;
            var enemyPos    = new Vector2(enemyPosRaw.x, enemyPosRaw.z);
            if ( !CheckGuardZoneDistance(enemy) ) {
                continue;
            }
            var tmpDist = Vector2.Distance(enemyPos, myPos);
            if ( tmpDist < minDist ) {
                minDist   = tmpDist;
                nextEnemy = enemy;
            }
        }
        return nextEnemy;
    }

    bool CheckGuardZoneDistance(BaseUnit enemy) 
    {
        if ( !GuardZoneCenter ) 
        {
            return true;
        }
        var enemyPosRaw = enemy.transform.position;
        var enemyPos    = new Vector2(enemyPosRaw.x, enemyPosRaw.z);
        var gzcPosRaw   = GuardZoneCenter.position;
        var gzcPos      = new Vector2(gzcPosRaw.x, gzcPosRaw.z);
        return (Vector2.Distance(gzcPos, enemyPos) < GuardZoneRadius);
    }

    void OnTargetDied(BaseUnit enemy)
    {
        if ( _target != enemy ) 
        {
            Debug.LogError("Unexpected enemy");
            return;
        }
        _target.OnDied -= OnTargetDied;
        _target = null;
        if ( IsAlive ) 
        {
            CurState = State.Guarding;
            _canAttack = true;
        }
    }

    void TryAttackSomething(GameObject other)
    {
        if ( PauseController.IsPaused ) 
        {
            return;
        }
        var enemy = GetEnemy(other);
        if ( enemy ) 
        {
            TryAttackEnemy(enemy);
        }
    }

    void TryAttackEnemy(BaseUnit enemyUnit) 
    {
        if ( !_canAttack ) 
        {
            return;
        }
        if ( CurState == State.Fighting ) 
        {
            return;
        }
        if ( _target && (enemyUnit != _target) ) 
        {
            _target.OnDied -= OnTargetDied;
            _target        =  enemyUnit;
            _target.OnDied += OnTargetDied;
        }
        CurState = State.Fighting;
        _canAttack = false;
    }

    public void MakeAttack() 
    {
        if ( IsAlive ) 
        {
            if ( _target && _target.IsAlive ) 
            {
                Fight(_target);
                if ( SoundPlayer ) 
                {
                    SoundPlayer.PlayOneShot(AttackKey);
                }
                _canAttack = true;
            }
            if ( _target ) 
            {
                _target.OnDied -= OnTargetDied;
            }
            _target = null;
            CurState = State.Guarding;
        }
    }
}
