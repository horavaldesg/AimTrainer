using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
public class GameController : MonoBehaviour
{
    
    
    //Animator
    Animator animator;

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
    float rotY;
    [SerializeField] float adsSensHorizontal;
    [SerializeField] float adsSensVertical;
    bool ads;

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

    //Full auto
    public float fireRate;

    //LightBar
    DualShockGamepad gamepad;

    //Rumbles
    [SerializeField] bool Rumbles;
    private void Awake()
    {
        gamepad = (DualShockGamepad)Gamepad.all[0];
        gamepad.SetLightBarColor(Color.yellow);

        

        //Animator
        animator = GameObject.FindGameObjectWithTag("PlayerAnim").GetComponent<Animator>();

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

        //Shoot Single Fire
        controls.Gameplay.Shoot.started += tgb => SingleShoot();

        //Rumbles
        if (Rumbles)
        {
            controls.Gameplay.Shoot.started += tgb => StartRumbles();
            controls.Gameplay.Shoot.canceled += tgb => StopRumble();
        }


        //Shoot Full Auto
        //controls.Gameplay.Shoot.performed += tgb => AutoMaticShoot();


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
        horizontalSens = adsSensHorizontal;
        verticalSens = adsSensVertical;
        ads = true;

    }
    void StopAim()
    {
        mainCamera.fieldOfView = FOVConst;
        //scopeCam.SetActive(false);
        horizontalSens = horizontalSensConst;
        verticalSens = verticalSensConst;
        ads = false;
    }
    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }
    private void OnDisable()
    {
        gamepad.ResetHaptics();
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

        //Movement Animator

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
        
        rotY += -rotate.y * verticalSens * Time.deltaTime;
        rotY = Mathf.Clamp(rotY, -90, 90);
        camTransform.transform.localRotation = Quaternion.Euler(rotY, 0, 0);

        

        RaycastHit hit;
        Debug.DrawRay(camTransform.position, camTransform.forward, Color.green);
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit))
        {
            if (hit.collider.GetComponent<MeshRenderer>())
            {
                gamepad.SetLightBarColor(hit.collider.GetComponent<MeshRenderer>().material.color);

            }
            if (hit.collider.CompareTag("Target") || hit.collider.CompareTag("MovingTarget"))
            {
                horizontalSens = horizontalSensConst / 2.5f;
                verticalSens = verticalSensConst / 2.5f;
            }
            
        }
        else if (!ads)
        {
            horizontalSens = horizontalSensConst;
            verticalSens = verticalSensConst;
        }
        
    }
    void SingleShoot()
    {
        shotCount++;
        RaycastHit hit;
        if(Physics.Raycast(camTransform.position, camTransform.forward, out hit))
        {
            if (hit.collider.CompareTag("Target"))
            {
                ShotCount.shotsHit++;
                //Debug.Log("Hit target");
                Destroy(hit.collider.gameObject);
            }
            if (hit.collider.CompareTag("MovingTarget"))
            {
                ShotCount.shotsHit++;
                hit.collider.GetComponent<MovingTarget>().health -= damage;
            }
        }
        
    }
    void StartRumbles()
    {
        gamepad.SetMotorSpeeds(0.25f, 0.75f);
    }
    void StopRumble()
    {
        gamepad.ResetHaptics();
    }
    void AutoMaticShoot()
    {
        //
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
        //Debug.Log("Controller Reset");
    }
}
