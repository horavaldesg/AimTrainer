using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cube : MonoBehaviour
{
    /*
    PlayerControls controls;
    Vector2 move;
    Vector2 rotate;
    private void Awake()
    {
        controls = new PlayerControls();
        controls.Gameplay.Grow.performed += ctx => GrowCube();

        controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => move = Vector2.zero;

        controls.Gameplay.Rotation.performed += ctx => rotate = ctx.ReadValue<Vector2>();
        controls.Gameplay.Rotation.canceled += ctx => rotate = Vector2.zero;
    }
    void GrowCube()
    {
        transform.localScale *= 1.1f;
    }
    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }
    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void Update()
    {
        Vector2 m = new Vector2(-move.x, move.y) * Time.deltaTime;
        transform.Translate(m, Space.World);

        Vector2 r = new Vector2(-rotate.y, rotate.x) * 100f * Time.deltaTime;
        transform.Rotate(r, Space.World);
    }
    */
}
