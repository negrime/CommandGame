using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursor : MonoBehaviour
{
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    private Vector2 _hotSpot = Vector2.zero;
    void Start()
    {
        Cursor.SetCursor(cursorTexture, _hotSpot, cursorMode);
    }
}
