using UnityEngine;

using Spine.Unity;

[RequireComponent(typeof(SkeletonAnimation))]
public sealed class EnemyMinionAnim : MonoBehaviour {
    const string StepAudioKey = "enemy_step_solo";
    
    const float MinWalkSpeed = 0.5f;
    
    public EnemyMinion Owner;
    public SkeletonAnimation Animation;
    public SoundPlayer SoundPlayer;
    [Space]
    [SpineAnimation(dataField = "Animation")]
    public string IdleAnimName;
    [SpineAnimation(dataField = "Animation")]
    public string RunAnimName;
    [SpineAnimation(dataField = "Animation")]
    public string AttackAnimName;
    [SpineAnimation(dataField = "Animation")]
    public string DieAnimName;
    
    bool _isMoving;
    bool _isMovingRight;

    Vector2 _oldPos;

    void Reset() 
    {
        Owner = GetComponentInParent<EnemyMinion>();
    }

    void Start() 
    {
        _oldPos = GetPos();
        Owner.OnStateChanged += OnStateChanged;
        OnStateChanged(Owner.CurState);
    }

    void OnDestroy() 
    {
        if ( Owner ) 
        {
            Owner.OnStateChanged -= OnStateChanged;
        }
    }

    void Update() {
        if ( PauseController.IsPaused )
        {
            return;
        }
        var newPos = GetPos();
        if ( Vector2.Distance(newPos, _oldPos) > 0.05f ) 
        {
            var isMovingRight = _oldPos.x <= newPos.x;
            Animation.skeleton.ScaleX = isMovingRight ? -1f : 1f;
        }
        if ( Owner.CurState == EnemyMinion.State.Guarding ) 
        {
            var isMoving = Owner.Speed >= MinWalkSpeed;
            if ( isMoving != _isMoving ) 
            {
                _isMoving = isMoving;
                SetMovingAnim(_isMoving);
            }
        }
        _oldPos = GetPos();
    }

    void SetMovingAnim() 
    {
        var isMoving = Owner.Speed >= MinWalkSpeed;
        SetMovingAnim(isMoving);
    }

    void SetMovingAnim(bool isMoving) 
    {
        var entry = Animation.AnimationState.SetAnimation(0, isMoving ? RunAnimName : IdleAnimName, true);
        if ( SoundPlayer && isMoving ) 
        {
            entry.Complete += _ => { SoundPlayer.PlayOneShot(StepAudioKey); };
        }
    }

    Vector2 GetPos()
    {
        var pos = Owner.transform.position;
        return new Vector2(pos.x, pos.z);
    }

    void OnStateChanged(EnemyMinion.State newState)
    {
        switch ( newState ) 
        {
            case EnemyMinion.State.Guarding: 
            {
                SetMovingAnim();
                break;
            }
            case EnemyMinion.State.Pursuing: 
            {
                Animation.AnimationState.SetAnimation(0, RunAnimName, true);
                break;
            }
            case EnemyMinion.State.Fighting:
            {
                var entry = Animation.AnimationState.SetAnimation(0, AttackAnimName, false);
                entry.Complete += _ => { Owner.MakeAttack(); };
                break;
            }
            case EnemyMinion.State.Dying: 
            {
                var entry = Animation.AnimationState.SetAnimation(0, DieAnimName, false);
                entry.Complete += _ => 
                {
                    Owner.FinishDying();
                };
                break;
            }
        }
    }
}
