using UnityEngine;
using UnityEngine.AI;

using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class BaseMinion : BaseUnit {
    public int OverrideBaseHp = -1;
    public int OverrideDamage = -1;
    public ColliderAction VisibilityCollider;
    public ColliderAction AttackCollider;

    protected readonly List<BaseUnit> CurEnemies = new List<BaseUnit>();

    protected NavMeshAgent NavMeshAgent;

    float   _pausedSpeed;
    Vector3 _pausedVelocity;
    float   _pausedRadius;

    bool _isInit;

    protected override int BaseHp => (OverrideBaseHp > 0) ? OverrideBaseHp : base.BaseHp;
    protected override int Damage => (OverrideDamage > 0) ? OverrideDamage : base.Damage;

    public float Speed 
    {
        get 
        {
            TryInit();
            return NavMeshAgent.velocity.magnitude;
        }
    }

    protected void Start() 
    {
        TryInit();
    }

    void TryInit() 
    {
        if ( _isInit ) 
        {
            return;
        }
        
        NavMeshAgent = GetComponent<NavMeshAgent>();
        
        VisibilityCollider.Init(OnNoticeSomething, null, OnLoseSomething);

        OnDied += OnDeath;

        PauseController.OnPauseChanged += OnPauseChanged;
        
        _isInit = true;
    }

    protected void OnDestroy() 
    {
        PauseController.OnPauseChanged -= OnPauseChanged;
    }

    void OnPauseChanged(bool isPaused) 
    {
        if ( isPaused ) 
        {
            _pausedSpeed    = NavMeshAgent.speed;
            _pausedVelocity = NavMeshAgent.velocity;
            _pausedRadius   = NavMeshAgent.radius;
            NavMeshAgent.speed    = 0f;
            NavMeshAgent.velocity = Vector3.zero;
            NavMeshAgent.radius   = 0f;
        } else 
        {
            NavMeshAgent.speed    = _pausedSpeed;
            NavMeshAgent.velocity = _pausedVelocity;
            NavMeshAgent.radius   = _pausedRadius;
        }
    }

    void OnDeath(BaseUnit unit) 
    {
        if ( unit != this ) 
        {
            return;
        }
        OnDied -= OnDeath;

        PauseController.OnPauseChanged -= OnPauseChanged;
    }
    
    void OnNoticeSomething(GameObject other) 
    {
        var enemy = GetEnemy(other);
        if ( enemy ) 
        {
            OnNoticeEnemy(enemy);
        }
    }

    void OnNoticeEnemy(BaseUnit enemy) 
    {
        CurEnemies.Add(enemy);
        enemy.OnDied += OnEnemyDied;
    }

    void OnLoseSomething(GameObject other)
    {
        var enemy = GetEnemy(other);
        if ( enemy ) 
        {
            OnLoseEnemy(enemy);
        }
    }

    void OnLoseEnemy(BaseUnit enemy)
    {
        enemy.OnDied -= OnEnemyDied;
        CurEnemies.Remove(enemy);
    }

    void OnEnemyDied(BaseUnit enemy) 
    {
        OnLoseEnemy(enemy);
    }
}
