using UnityEngine;

using Spine.Unity;

[RequireComponent(typeof(SkeletonAnimation))]
public sealed class MinionAnim : MonoBehaviour {
    const string StepAudioKey  = "minion_step_solo";
    const string AttackStepKey = "minion_attack_step";
    
    const float MinWalkSpeed = 0.5f;
    
    public Minion            Owner;
    public SkeletonAnimation Animation;
    public SoundPlayer       SoundPlayer;
    [Space]
    [SpineAnimation(dataField = "Animation")]
    public string IdleAnimName;
    [SpineAnimation(dataField = "Animation")]
    public string WalkAnimName;
    [SpineAnimation(dataField = "Animation")]
    public string RunAnimName;
    [SpineAnimation(dataField = "Animation")]
    public string AttackAnimName;
    [SpineAnimation(dataField = "Animation")]
    public string DieAnimName;
    
    bool _isMoving;
    bool _isMovingRight;

    Vector2 _oldPos;

    void Reset() {
        Owner = GetComponentInParent<Minion>();
    }

    void Start() {
        _oldPos = GetPos();
        Owner.OnStateChanged += OnStateChanged;
        OnStateChanged(Owner.CurState);
    }

    void OnDestroy() {
        if ( Owner ) {
            Owner.OnStateChanged -= OnStateChanged;
        }
    }

    void Update() {
        if ( PauseController.IsPaused ) {
            return;
        }
        var newPos = GetPos();
        if ( Vector2.Distance(newPos, _oldPos) > 0.05f ) {
            var isMovingRight = _oldPos.x <= newPos.x;
            Animation.skeleton.ScaleX = isMovingRight ? -1f : 1f;
        }
        switch ( Owner.CurState ) {
            case Minion.State.Following:
            case Minion.State.Wandering: {
                var isMoving = Owner.Speed >= MinWalkSpeed;
                if ( isMoving != _isMoving ) {
                    _isMoving = isMoving;
                    SetMovingAnim(_isMoving);
                }
                break;
            }
        }
        _oldPos = GetPos();
    }

    void SetMovingAnim() {
        var isMoving = Owner.Speed >= MinWalkSpeed;
        SetMovingAnim(isMoving);
    }

    void SetMovingAnim(bool isMoving) {
        var entry = Animation.AnimationState.SetAnimation(0, isMoving ? WalkAnimName : IdleAnimName, true);
        if ( SoundPlayer && isMoving ) {
            entry.Complete += _ => { SoundPlayer.PlayOneShot(StepAudioKey); };
        }
    }

    Vector2 GetPos() {
        var pos = Owner.transform.position;
        return new Vector2(pos.x, pos.z);
    }

    void OnStateChanged(Minion.State newState) {
        switch ( newState ) {
            case Minion.State.Following: {
                SetMovingAnim();
                break;
            }
            case Minion.State.Attacking: {
                var entry = Animation.AnimationState.SetAnimation(0, RunAnimName, true);
                entry.Complete += _ => { SoundPlayer.PlayOneShot(AttackStepKey); };
                break;
            }
            case Minion.State.Fighting: {
                var entry = Animation.AnimationState.SetAnimation(0, AttackAnimName, false);
                entry.Complete += _ => { Owner.MakeAttack(); };
                break;
            }
            case Minion.State.Wandering: {
                SetMovingAnim();
                break;
            }
            case Minion.State.Dying: {
                var entry = Animation.AnimationState.SetAnimation(0, DieAnimName, false);
                entry.Complete += _ => { Owner.FinishDying(); };
                break;
            }
        }
    }
}
