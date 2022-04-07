using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Vector2 MovementAxis { get; set; }
    public static Vector2 MousePosition => Mouse.current.position.ReadValue();
    public bool IsShootPressed { get; set; }

	public void OnMove(InputAction.CallbackContext ctx) => MovementAxis = ctx.ReadValue<Vector2>();
    public void OnShoot(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            IsShootPressed = true;
        else if (ctx.canceled)
            IsShootPressed = false;
    }
}
