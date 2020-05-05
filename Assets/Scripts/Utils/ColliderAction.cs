using UnityEngine;

using System;

[RequireComponent(typeof(Collider))]
public sealed class ColliderAction : MonoBehaviour {
    public bool IsTrigger;

    Collider _collider;

    Action<GameObject> _enterAction;
    Action<GameObject> _stayAction;
    Action<GameObject> _exitAction;

    void Start() {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = IsTrigger;
    }
    
    public void Init(Action<GameObject> enterAction, Action<GameObject> stayAction,
        Action<GameObject> exitAction = null) {
        _enterAction = enterAction;
        _stayAction  = stayAction;
        _exitAction  = exitAction;
    }
    
    public void Deinit() {
        _enterAction = null;
        _stayAction  = null;
        _exitAction  = null;
    }
    
     void OnTriggerEnter(Collider other) {
         if ( !IsTrigger ) {
             return;
         }
         _enterAction?.Invoke(other.gameObject);
     }
     
     void OnTriggerStay(Collider other) {
         if ( !IsTrigger ) {
             return;
         }
         _stayAction?.Invoke(other.gameObject);
     }
 
     void OnTriggerExit(Collider other) {
         if ( !IsTrigger ) {
             return;
         }
         _exitAction?.Invoke(other.gameObject);
     }
 
     void OnCollisionEnter(Collision other) {
         if ( IsTrigger ) {
             return;
         }
         _enterAction?.Invoke(other.gameObject);
     }
 
     void OnCollisionStay(Collision other) {
         if ( IsTrigger ) {
             return;
         }
         _stayAction?.Invoke(other.gameObject);
     }
 
     void OnCollisionExit(Collision other) {
        if ( IsTrigger ) {
            return;
        }
        _exitAction?.Invoke(other.gameObject);
    }
}
