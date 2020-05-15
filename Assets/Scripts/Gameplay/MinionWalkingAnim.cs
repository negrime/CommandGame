using UnityEngine;

using DG.Tweening;

public sealed class MinionWalkingAnim : MonoBehaviour {
    const string AudioKey = "minion_step_solo";
    
    static MinionWalkingAnim()
    {
        DOTween.SetTweensCapacity(500, 200);
    }

    public SoundPlayer SoundPlayer;
    
    Sequence _sequence;

    bool _stop;

    void Start()
    {
        TryCreateAnim();
    }

    public void Play()
    {
        TryCreateAnim();
        if ( _sequence.IsPlaying() )
        {
            return;
        }
        _sequence.Restart();
    }

    public void Stop()
    {
        _stop = true;
    }

    public void Kill()
    {
        _sequence?.Kill();
    }

    void TryCreateAnim()
    {
        if ( _sequence != null )
        {
            return;
        }
        
        _sequence = DOTween.Sequence()
            .Append(transform.DOLocalJump(Vector3.zero, 0.5f, 1, 0.4f))
            .SetEase(Ease.InOutSine)
            .InsertCallback(0.2f, () => {
                if ( SoundPlayer ) {
                    SoundPlayer.PlayOneShot(SoundsManager.Instance.GetClip(AudioKey));
                }
            })
            .AppendCallback(() => {
                if ( _stop ) {
                    _sequence.Pause();
                }
            })
            .SetLoops(-1);
        _sequence.SetUpdate(UpdateType.Manual);
        _sequence.SetAutoKill(false);
        _sequence.Pause();
    }
}
