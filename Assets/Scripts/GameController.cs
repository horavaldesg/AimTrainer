using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    PlayerControls controls;
    public Transform camTransform;
    [SerializeField] float horizontalSens;
    [SerializeField] float verticalSens;
    [SerializeField] Transform checkPos;
    public LayerMask groundMask;
    bool grounded;
    float Gravity = -25;
    float verticalSpeed = 0;
    public float jumpSpeed = 9;

    Vector2 move;
    Vector2 rotate;
    public static int shotCount;
    
    [SerializeField] CharacterController cc;
    Vector3 movement;
    [SerializeField] float speed = 3;
    [SerializeField] float speedBoost = 1;
    private void Awake()
    {
        speedBoost = 1;
        horizontalSens = 100f;
        verticalSens = 100f;
        controls = new PlayerControls();
        controls.Gameplay.Jump.started += ctx => Jump();

        controls.Gameplay.Shoot.started += ctx => Shoot();
        controls.Gameplay.Restart.started += ctx => Restart();
        //controls.Gameplay.Aim.performed += ctx => 

        controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => move = Vector3.zero;
        controls.Gameplay.Move.canceled += ctx => movement = Vector3.zero;
        controls.Gameplay.SpeedBoost.performed += ctx => speedBoost = 3;
        controls.Gameplay.SpeedBoost.canceled += ctx => speedBoost = 1;

       controls.Gameplay.Rotation.performed += ctx => rotate = ctx.ReadValue<Vector2>();
        controls.Gameplay.Rotation.canceled += ctx => rotate = Vector2.zero;
       
    }
    void Jump()
    {
        if (grounded)
        {
            verticalSpeed = jumpSpeed;
            grounded = false;
        }
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
        movement = Vector3.zero;
        float forwardSpeed = move.y * speed * speedBoost * Time.deltaTime;
        movement += transform.forward * forwardSpeed;
        float sideSpeed = move.x * speed * speedBoost * Time.deltaTime;
        movement += transform.right * sideSpeed;
        verticalSpeed += Gravity * Time.deltaTime;
        movement += transform.up * verticalSpeed * Time.deltaTime;

        if (Physics.CheckSphere(checkPos.position,0.5f, groundMask) && verticalSpeed <= 0)
        {
            grounded = true;
            verticalSpeed = 0;
        }
       


        //transform.Translate(m, Space.World);

        cc.Move(movement);
        Vector2 r = new Vector2(0, rotate.x) * horizontalSens * Time.deltaTime;
        Vector2 rC = new Vector2(-rotate.y, 0) * verticalSens * Time.deltaTime;

        transform.Rotate(r, Space.Self);
        Quaternion q = transform.rotation;
        q.eulerAngles = new Vector3(q.eulerAngles.x, q.eulerAngles.y, 0);
        transform.rotation = q;
        camTransform.transform.Rotate(rC, Space.Self);
        Quaternion rQ = camTransform.transform.rotation;
        rQ.eulerAngles = new Vector3(rQ.eulerAngles.x, rQ.eulerAngles.y, 0);
        camTransform.transform.rotation = rQ;
    }
    void Shoot()
    {
        shotCount++;
        RaycastHit hit;
        if(Physics.Raycast(camTransform.position, camTransform.forward, out hit))
        {
            if (hit.collider.CompareTag("Target"))
            {
                ShotCount.shotsHit++;
                Debug.Log("Hit target");
                Destroy(hit.collider.gameObject);
            }
        }
    }
    void Restart()
    {
        shotCount = 0;
        ShotCount.shotsHit = 0;
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
