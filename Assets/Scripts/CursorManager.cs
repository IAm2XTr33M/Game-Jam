using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public bool canRightClick = true;

    [SerializeField] private Texture2D mainCursor;
    [SerializeField] private Texture2D grabCursor;

    Vector2 cursorHotspot;


    void Start()
    {
        cursorHotspot = new Vector2(mainCursor.width / 2, mainCursor.height / 2);
        Cursor.SetCursor(mainCursor, cursorHotspot, CursorMode.Auto);
    }

    void Update()
    {
        if(canRightClick && Input.GetMouseButton(1) || Input.GetMouseButton(0))
        {
            Cursor.SetCursor(grabCursor, cursorHotspot, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(mainCursor, cursorHotspot, CursorMode.Auto);
        }
    }
}
