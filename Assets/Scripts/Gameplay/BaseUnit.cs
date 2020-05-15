using UnityEngine;

using System;

public abstract class BaseUnit : MonoBehaviour
{
    const float RegenTime = 3f;

    public event Action<BaseUnit> OnDied;
    
    int _curHp = int.MinValue;

    GameTimer _regenTimer;

    protected virtual int BaseHp { get; } = 3;
    protected virtual int Damage { get; } = 1;
    protected virtual bool CanRegen { get; } = true;
    
    public abstract Team Team { get; }

    public bool IsAlive { get; private set; } = true;

    int CurHp 
    {
        get 
        {
            if ( _curHp == int.MinValue ) 
            {
                _curHp = BaseHp;
            }
            return _curHp;
        }
        set 
        {
            _curHp = value;
            TryStartRegen();
        }
    }

    public void Die() 
    {
        CurHp = 0;
        IsAlive = false;
        OnDied?.Invoke(this);
        DieSpecific();
    }

    void TryStartRegen() 
    {
        if ( !CanRegen ) {
            return;
        }
        if ( _regenTimer != null ) {
            return;
        }
        if ( (CurHp <= 0) || (CurHp >= BaseHp) ) {
            return;
        }
        _regenTimer = new GameTimer(RegenTime, TryRegenHp);
    }

    void TryRegenHp() 
    {
        if ( !IsAlive ) 
        {
            return;
        }
        CurHp = Mathf.Min(CurHp + 1, BaseHp);
        _regenTimer = null;
        TryStartRegen();
    }

    protected abstract void DieSpecific();

    protected void Fight(BaseUnit enemyUnit) 
    {
        enemyUnit.CurHp -= Damage; 
        if ( enemyUnit.CurHp <= 0 ) 
        {
            enemyUnit.Die();
        }
    }

    protected BaseUnit GetEnemy(GameObject other) 
    {
        var baseUnit = other.GetComponent<BaseUnit>();
        if ( !baseUnit || !baseUnit.IsAlive || (baseUnit.Team == Team) ) 
        {
            return null;
        }
        return baseUnit;
    }
}
