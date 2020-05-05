using UnityEngine;

using System;
using System.Collections.Generic;

public class UnityContext : MonoBehaviour {
    sealed class StoredAction {
        public readonly Action Action;
        public          bool   ToRemove;

        public StoredAction(Action action) {
            Action = action;
        }
    }
    
    static UnityContext _instance;
    static UnityContext Instance {
        get {
            if ( !_instance ) {
                var go = new GameObject("[UnityContext]");
                _instance = go.AddComponent<UnityContext>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    readonly List<StoredAction> _updateActions = new List<StoredAction>();
    readonly List<StoredAction> _toAdd         = new List<StoredAction>(1);

    void Update() {
        if ( _toAdd.Count > 0 ) {
            _updateActions.AddRange(_toAdd);
            _toAdd.Clear();
        }
        var hasToRemove = false;
        foreach ( var action in _updateActions ) {
            if ( action.ToRemove ) {
                hasToRemove = true;
            } else {
                action.Action?.Invoke();
            }
        }
        if ( hasToRemove ) {
            for ( var i = _updateActions.Count - 1; i >= 0; i-- ) {
                var action = _updateActions[i];
                if ( action.ToRemove ) {
                    _updateActions.RemoveAt(i);
                }
            }
        }
    }

    void AddUpdateActionInternal(Action action) {
        if ( action == null ) {
            return;
        }
        _toAdd.Add(new StoredAction(action));
    }

    void RemoveUpdateActionInternal(Action action) {
        if ( action == null ) {
            return;
        }
        foreach ( var tmpAction in _toAdd ) {
            if ( tmpAction.Action == action ) {
                tmpAction.ToRemove = true;
                break;
            }
        }
        foreach ( var tmpAction in _updateActions ) {
            if ( tmpAction.Action == action ) {
                tmpAction.ToRemove = true;
                break;
            }
        }
    }

    public static void AddUpdateAction(Action action) {
        Instance.AddUpdateActionInternal(action);
    }

    public static void RemoveUpdateAction(Action action) {
        Instance.RemoveUpdateActionInternal(action);
    }
}
