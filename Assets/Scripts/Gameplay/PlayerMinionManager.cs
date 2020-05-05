using UnityEngine;

using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Player))]
public class PlayerMinionManager : MonoBehaviour, IEnumerable<Minion> {
    readonly List<Minion> _minions = new List<Minion>();

    private Player _player;

    public int ActiveMinionsNum {
        get {
            var res = 0;
            foreach ( var minion in _minions ) {
                if ( minion.IsAlive && (minion.CurState != Minion.State.Wandering) ) {
                    ++res;
                }
            }
            return res;
        }
    }

    public void Init(Player player) {
        _player = player;
    }

    public void AddMinion(Minion minion)
    {
        if ( !minion ) {
            Debug.LogError("Minion is null");
            return;
        }
        if ( _minions.Contains(minion) ) {
            Debug.LogError("Minion already added");
            return;
        }
        minion.Init(_player);
        minion.OnDied += OnMinionDied;
        _minions.Add(minion);
    }

    public void RemoveMinion(Minion minion) {
        if ( !_minions.Contains(minion) ) {
            Debug.LogError("Unexpected minion");
            return;
        }
        minion.OnDied -= OnMinionDied;
        _minions.Remove(minion);
    }

    void OnMinionDied(BaseUnit minionUnit) {
        var minion = minionUnit.GetComponent<Minion>();
        if ( !minion ) {
            Debug.LogError("Unexpected unit");
            return;
        }
        RemoveMinion(minion);
    }

    public IEnumerator<Minion> GetEnumerator() {
        return _minions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
