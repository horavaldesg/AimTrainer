using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    PlayerControls controls;
    public Transform camTransform;
    Vector2 move;
    Vector2 rotate;
    public static int shotCount;
    private void Awake()
    {
        controls = new PlayerControls();
        //controls.Gameplay.Grow.performed += ctx => GrowCube();

        controls.Gameplay.Shoot.started += ctx => Shoot();
        controls.Gameplay.Restart.started += ctx => Restart();
        //controls.Gameplay.Aim.performed += ctx => 

        //controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        //controls.Gameplay.Move.canceled += ctx => move = Vector2.zero;

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
        transform.Rotate(r, Space.Self);
        Quaternion q = transform.rotation;
        q.eulerAngles = new Vector3(q.eulerAngles.x, q.eulerAngles.y, 0);
        transform.rotation = q;
        
    }
    void Shoot()
    {
        shotCount++;
        RaycastHit hit;
        if(Physics.Raycast(camTransform.position, camTransform.forward, out hit))
        {
            if (hit.collider.CompareTag("Target"))
            {
                Debug.Log("Hit target");
                Destroy(hit.collider.gameObject);
            }
        }
    }
    void Restart()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        foreach(GameObject target in targets)
        {
            Destroy(target.gameObject);
        }
        GameObject spawner = GameObject.FindGameObjectWithTag("Spawner");
        spawner.GetComponent<Spawner>().Spawn();
        Debug.Log("Controller REset");
    }
}
