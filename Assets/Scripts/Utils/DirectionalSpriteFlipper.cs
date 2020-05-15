using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class DirectionalSpriteFlipper : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;

    Vector3 _oldPos;

    void Start() 
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _oldPos         = transform.position;
    }

    void Update() 
    {
        var newPos = transform.position;
        if ( Mathf.Abs(newPos.x - _oldPos.x) > .1f ) 
        {
            _spriteRenderer.flipX = (_oldPos.x < newPos.x);
            _oldPos = newPos;
        }
    }
}
