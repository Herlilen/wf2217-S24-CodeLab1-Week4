using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class CharacterControl : MonoBehaviour
{
    public static CharacterControl instance;

    [Header("Game Start")] 
    [SerializeField] private bool gameStart;
    
    [Header("HUD")] 
    [SerializeField] private LayerMask button;
    [SerializeField] private TextMeshProUGUI popupText;

    [Header("Gravity")]
    [SerializeField] private float gravityValue;
    [SerializeField] private float acceleration;
    [SerializeField] private float gravityValueMax;
    
    [Header("Camera Control")] 
    [SerializeField] private Camera _camera;
    [SerializeField] private float camSpeed;
    private float xRotation;    //camera yaw rotation

    [Header("Character Controller")] 
    private CharacterController _characterController;
    private Vector3 playerVelocity;
    [SerializeField] private bool grounded;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float jumpPower;
    
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        //get controller
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //get game start value
        gameStart = GameManager.instance.gameStart;
        //grounded bool for jump & gravity
        grounded = IsGrounded();
        //player functions
        Gravity();
        CameraControl();
        PlayerMovementControl();
        Jump();
        Button();
        Shoot();
    }

    void Gravity()
    {
        if (!grounded)
        {
            //accelerate
            if (gravityValue < gravityValueMax)
            {
                gravityValue += acceleration * Time.deltaTime;
            }
            if (gravityValue >= gravityValueMax)
            {
                gravityValue = gravityValueMax;
            }
            
            //falling down
            playerVelocity.y -= gravityValue * Time.deltaTime;
            _characterController.Move(playerVelocity * Time.deltaTime);
        }
        else
        {
            gravityValue = 0;
        }
    }
    
    void CameraControl()
    {
        //get mouse with input system
        float mouseX = Input.GetAxis("Mouse X") * camSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * camSpeed * Time.deltaTime;
        
        //set x ration according to mouse Y
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        //rotate player according to mouse x asis
        transform.Rotate(Vector3.up * mouseX);
        //rotate yaw of camera
        _camera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    void PlayerMovementControl()
    {
        //get wasd axis value
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        //set the direction to move
        Vector3 move = transform.right * x + transform.forward * y;
        
        _characterController.Move(move * playerSpeed * Time.deltaTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            playerVelocity.y = jumpPower;
            _characterController.Move(playerVelocity * Time.deltaTime);
        }
    }

    Boolean IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void Button()
    {
        //deal with hud
        Color color = new Color(255,255,255,1);
        Color noColor = new Color(0, 0, 0, 0);
        RaycastHit hit;
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, 1.5f, button) && !GameManager.instance.gameStart)
        {
            //set color
            popupText.color = color;
            if (Input.GetKeyDown(KeyCode.E))
            {
                //game start
                GameManager.instance.gameStart = true;
                GameManager.instance.startedOnce = true;
            }
        }
        else
        {
            popupText.color = noColor;
        }
    }

    void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, 1000f))
            {
                if (hit.transform.tag == "targets")
                {
                    //score goes up
                    GameManager.instance.Score++;
                    //relocate targets
                    hit.transform.position = new Vector3(
                        UnityEngine.Random.Range(-14.5f, 14.5f),
                        UnityEngine.Random.Range(.5f, 5.5f),
                        UnityEngine.Random.Range(-14.5f, 14.5f)
                    );
                }
            }
        }
    }
}
