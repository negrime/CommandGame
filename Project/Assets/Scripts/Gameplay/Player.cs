using UnityEngine;

using System.Collections.Generic;

public class Player : BaseUnit 
{
    const string DeathKey = "king_death";
    
     readonly int Run = Animator.StringToHash("Run");
     readonly int Roar = Animator.StringToHash("Roar");
     readonly int Death = Animator.StringToHash("Death");

    public PlayerDeathDetector DeathDetector;
    public Animator Animator;
    public List<Minion> StartMinions;
    public PlayerMinionManager MinionManager;

    [Header("Player Stats")]
    public float Speed;

    private SoundPlayer _soundPlayer;

    private Vector3 _input;

    private Rigidbody _rigidbody;
    
    private readonly RaycastHit[] _hits = new RaycastHit[5];

    public override Team Team => Team.Player;

    protected override int BaseHp => 1;

    private void Start() 
    {
        _soundPlayer = GetComponent<SoundPlayer>();
        _rigidbody = GetComponent<Rigidbody>();
        MinionManager.Init(this);
        foreach ( var minion in StartMinions )
        {
            MinionManager.AddMinion(minion);
        }
        StartMinions.Clear();
    }

    private void Update()
    {
        HandleMovementInput();
        HandleCombatInput();
        UpdateRunAnim();
    }

    private void HandleMovementInput()
    {
        if ( !IsAlive || PauseController.IsPaused )
        {
            return;
        }
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
    }

    private void HandleCombatInput() 
    {
        if ( !IsAlive || PauseController.IsPaused )
        {
            return;
        }
        if ( Input.GetMouseButtonDown(0) ) 
        {
            var ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
            var size = Physics.RaycastNonAlloc(ray, _hits, Mathf.Infinity, 1 << 10);
            for ( var i = 0; i < size; i++ ) 
            {
                var hit   = _hits[i];
                var enemy = GetEnemy(hit.collider.gameObject);
                if ( enemy ) {
                    _soundPlayer.PlayOneShot(SoundsManager.Instance.GetClip("king_scream"));
                    Animator.SetTrigger(Roar);
                    foreach ( var minion in MinionManager )
                    {
                        minion.Attack(enemy);
                    }
                    break;
                }
            }
        }
    }

    private void UpdateRunAnim() 
    {
        if ( !IsAlive || PauseController.IsPaused )
        {
            return;
        }

        if (_input.magnitude > 0 && !_soundPlayer.AudioSource.isPlaying)
        {
            _soundPlayer.PlayOneShot(SoundsManager.Instance.GetClip("king_steps"));
        }
        Animator.SetFloat(Run, _input.magnitude);
    }

    private void FixedUpdate() 
    {
        KeyBoardMovement();
    }

    protected override void DieSpecific() 
    {
        _soundPlayer.PlayOneShot(DeathKey);
        Animator.SetTrigger(Death);
    }

    private void KeyBoardMovement()
    {
        if ( _input != Vector3.zero ) 
        {
            if ( Mathf.Abs(_input.x) > float.Epsilon ) 
            {
                Animator.transform.localScale = !(_input.x < 0f) ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
            }

            _rigidbody.position += _input * (Speed * Time.fixedDeltaTime);

            _input = Vector2.zero;
        }
    }
}
