using System.Collections;
using UnityEngine;

using System.Collections.Generic;

using Random = UnityEngine.Random;

public class Cell : MonoBehaviour {
    const string OpenAudioKey = "cell_open";
    
    public List<BaseUnit> Guards = new List<BaseUnit>();
    public int MinionCount;
    public SoundPlayer SoundPlayer;

    public Animator Animator;

    private Player _player;

    bool IsOpen { get; set; }

    bool CanOpen => !IsOpen && _player && (Guards.Count == 0);

    void Start() 
    {
        foreach ( var guard in Guards ) 
        {
            guard.OnDied += OnGuardDied;
        }
    }

    void Update() 
    {
        if ( CanOpen )
        {
            if ( Input.GetKeyDown(KeyCode.E) ) 
            {
                IsOpen = true;

                
                Animator.SetTrigger("Crash");
                GetComponent<BoxCollider>().enabled = false;
                

                if ( SoundPlayer ) 
                {
                    SoundPlayer.PlayOneShot(SoundsManager.Instance.GetClip(OpenAudioKey));
                }

                StartCoroutine(SpawnUnits(_player));
            }
        }
    }

    void OnGuardDied(BaseUnit guard) 
    {
        if ( !Guards.Contains(guard) ) 
        {
            Debug.Log("Unexpected guard died");
        }
        Guards.Remove(guard);
        guard.OnDied -= OnGuardDied;
    }

    private void OnTriggerEnter(Collider other) 
    {
        var player = other.GetComponent<Player>();
        if ( player ) {
            _player = player;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        var otherPlayer = other.GetComponent<Player>();
        if ( otherPlayer && (otherPlayer == _player) ) 
        {
            _player = null;
        }
    }


    private IEnumerator SpawnUnits(Player player)
    {
        yield return new WaitForSeconds(0.6f);
        var minionPrefab = Resources.Load<GameObject>("Minion");
        if ( !minionPrefab ) 
        {
            Debug.LogError("Minion prefab is null");
            yield break;
        }
        for ( var i = 0; i < MinionCount; i++ ) 
        {
            var rand = Random.insideUnitCircle * 7;
            var minionGo = Instantiate(minionPrefab, transform.position + new Vector3(rand.x, 0, rand.y),
                Quaternion.Euler(45, 0, 0));
            var minion = minionGo.GetComponent<Minion>();
            if ( !minion ) {
                Debug.LogError("Minion is null");
                yield break;
            }
            player.MinionManager.AddMinion(minion);
        }
    }
}
