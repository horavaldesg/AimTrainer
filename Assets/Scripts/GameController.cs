using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    //Controller
    PlayerControls controls;

    //Camera
    Camera mainCamera;
    public Transform camTransform;
    float FOVConst;

    //Scope Camera
    public GameObject scopeCam;

    //Sensitivity
    [SerializeField] float horizontalSens;
    [SerializeField] float verticalSens;
    float horizontalSensConst;
    float verticalSensConst;

    //Jumping/Gravity
    [SerializeField] Transform checkPos;
    public LayerMask groundMask;

    bool grounded;
    float Gravity = -25;
    float verticalSpeed = 0;
    public float jumpSpeed = 9;

    //Movement
    Vector2 move;
    Vector2 rotate;

    public static int shotCount;
    
    [SerializeField] CharacterController cc;

    Vector3 movement;

    [SerializeField] float speed = 3;
    [SerializeField] float speedBoost = 1;


    //Damage
    public float damage;
    private void Awake()
    {
        //Main Camera
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        FOVConst = mainCamera.fieldOfView;

        scopeCam.SetActive(false);

        //Constants
        speedBoost = 1;
        //horizontalSens = 100f;
        //verticalSens = 100f;
        horizontalSensConst = horizontalSens;
        verticalSensConst = verticalSens;
        controls = new PlayerControls();

        //Jump
        controls.Gameplay.Jump.started += tgb => Jump();

        //Shoot
        controls.Gameplay.Shoot.started += tgb => Shoot();

        //Restart
        controls.Gameplay.Restart.started += tgb => Restart();

        //Aim
        controls.Gameplay.Aim.performed += tgb => Aim();
        controls.Gameplay.Aim.canceled += tgb => StopAim();
        

        //Movement
        controls.Gameplay.Move.performed += tgb => move = tgb.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += tgb => move = Vector3.zero;
        controls.Gameplay.Move.canceled += tgb => movement = Vector3.zero;
        
        //Run
        controls.Gameplay.SpeedBoost.performed += tgb => speedBoost = 3;
        controls.Gameplay.SpeedBoost.canceled += tgb => speedBoost = 1;

        //Rotation
        controls.Gameplay.Rotation.performed += tgb => rotate = tgb.ReadValue<Vector2>();
        controls.Gameplay.Rotation.canceled += tgb => rotate = Vector2.zero;

        //Damage
        damage = 30;
       
    }
    void Jump()
    {
        if (grounded)
        {
            verticalSpeed = jumpSpeed;
            grounded = false;
        }
    }
    void Aim()
    {
        mainCamera.fieldOfView = FOVConst - 10;
        //scopeCam.SetActive(true);

    }
    void StopAim()
    {
        mainCamera.fieldOfView = FOVConst;
        //scopeCam.SetActive(false);
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

        //Forward/Backward Movement
        float forwardSpeed = move.y * speed * speedBoost * Time.deltaTime;
        movement += transform.forward * forwardSpeed;

        //Left/Right Movement
        float sideSpeed = move.x * speed * speedBoost * Time.deltaTime;
        movement += transform.right * sideSpeed;

        //Gravity
        verticalSpeed += Gravity * Time.deltaTime;
        movement += transform.up * verticalSpeed * Time.deltaTime;

        //Ground Check
        if (Physics.CheckSphere(checkPos.position,0.5f, groundMask) && verticalSpeed <= 0)
        {
            grounded = true;
            verticalSpeed = 0;
        }
       


        //transform.Translate(m, Space.World);
        
        cc.Move(movement);

        //Player Rotation
        Vector2 r = new Vector2(0, rotate.x) * horizontalSens * Time.deltaTime;
        transform.Rotate(r, Space.Self);
        Quaternion q = transform.rotation;
        q.eulerAngles = new Vector3(q.eulerAngles.x, q.eulerAngles.y, 0);
        transform.rotation = q;

        //Camera Rotation
        Vector2 rC = new Vector2(-rotate.y, 0) * verticalSens * Time.deltaTime;
        camTransform.transform.Rotate(rC, Space.Self);
        Quaternion rQ = camTransform.transform.rotation;
        rQ.eulerAngles = new Vector3(rQ.eulerAngles.x, rQ.eulerAngles.y, 0);
        
        
        camTransform.transform.rotation = rQ;

        RaycastHit hit;
        Debug.DrawRay(camTransform.position, camTransform.forward, Color.green);
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit))
        {
            if (hit.collider.CompareTag("Target"))
            {
                horizontalSens = horizontalSensConst / 2.5f;
                verticalSens = verticalSensConst / 2.5f;
            }
            
        }
        else
        {
            horizontalSens = horizontalSensConst;
            verticalSens = verticalSensConst;
        }
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
            if (hit.collider.CompareTag("MovingTarget"))
            {
                hit.collider.GetComponent<MovingTarget>().health -= damage;
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
        Debug.Log("Controller Reset");
    }
}
