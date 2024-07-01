using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("References")]

    public Camera cam;
    private CharacterController characterController;
    public GameObject skin;

    [Header("Cam Settings")]
    
    
    public float mouseSensitivity;
    public Vector3 slidePos;
    public Vector3 crouchPos;
    public Vector3 normalPos;
    Vector3 desiredPosition = new Vector3();
    private float cameraPosY = 0;

    [Header("Walk Settings")]
    public float walkSpeed;
    public bool isRunning = true;


    [Header("Gravity Settings")]

    public LayerMask groundLayer;
    private float groundRadius = 0.4f;
    private Vector3 velocity;

    public GameObject groundCheck;
    public bool isGrounded = true;
    public float gravity;

    [Header("Jump Settings")]
    public float jumpForce = 1.5f;


    [Header("Crouch Settings")]
    public bool canCrouch = true;
    public bool isCrouch = true;
    public GameObject topCheck;

    [Header("Slide Settings")]

    public bool isSliding = true;
    public bool coroutineLock = true;
    public float slideSpeed;

    [Header("Animations")]

    public Animator animator;



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        coroutineLock = false;
        isRunning = false;
        isSliding = false;
        isCrouch = false;
        velocity.y = -2;
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Controller();
        CameraLook();
        Gravity();
        Animations();
    }

    void Controller()
    {
        Movement();
        Jump();
        CrouchPosition();
        Sprint();
        Slide();
    }

    void Movement()
    {
        //Get WASD input values
        Vector2 moveAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 moveInput = new Vector3(moveAxis.x, 0, moveAxis.y).normalized;

        if (!isSliding)
        {
            characterController.Move(transform.TransformDirection(moveInput * walkSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D) && !isCrouch && !isSliding)
        {
            Vector3 diagonalPos = new Vector3(0, 45, 0);
            skin.transform.localRotation = Quaternion.Euler(diagonalPos);
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A) && !isCrouch && !isSliding)
        {
            Vector3 diagonalPos = new Vector3(0, -45, 0);
            skin.transform.localRotation = Quaternion.Euler(diagonalPos);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D) && !isCrouch && !isSliding)
        {
            Vector3 diagonalPos = new Vector3(0, -45, 0);
            skin.transform.localRotation = Quaternion.Euler(diagonalPos);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A) && !isCrouch && !isSliding)
        {
            Vector3 diagonalPos = new Vector3(0, 45, 0);
            skin.transform.localRotation = Quaternion.Euler(diagonalPos);
        }
        else
        {
            skin.transform.localRotation = Quaternion.identity;
        }


    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouch && !isSliding)
        {
            animator.SetBool("Jump", true);
            animator.CrossFade("Jump", 0.1f);
            velocity.y = Mathf.Sqrt(jumpForce * -2 * gravity); 
        }

        if (!isGrounded && animator.GetBool("Jump") == true)
        {
            animator.SetBool("Jump", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Walking", false);
            animator.SetBool("Walking Backward", false);
            animator.SetBool("Running", false);
            animator.SetBool("Right Strafe", false);
            animator.SetBool("Left Strafe", false);
        }
        else if (isGrounded && !isCrouch && !isSliding)
        {
            desiredPosition = normalPos;
        }
    }

    void CrouchPosition()
    {
        //Check if a collider is above my head
        if (canCrouch = Physics.CheckSphere(topCheck.transform.position, -0.4f) && isCrouch)
        {
            canCrouch = false;
        }
        else
        {
            canCrouch = true;
        }
        if (isCrouch)
        {

            topCheck.transform.localPosition = new Vector3(0, 0.55f, 0);
        }
        else
        {
            topCheck.transform.localPosition = new Vector3(0, 1.55f, 0);
        }
        
        
        
        
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isCrouch && canCrouch && isGrounded)
        {
            characterController.height = 1.6f;
            characterController.center = new Vector3(0, -0.65f, 0);
            desiredPosition = crouchPos;
            isCrouch = true;
            isRunning = false;
            walkSpeed = 2;
            animator.SetBool("Crouch", true);
            animator.CrossFade("Crouch Idle", 0.05f);
            Debug.Log("crouched");
        }
        else if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.Space)) && isCrouch && canCrouch)
        {
            characterController.height = 3f;
            characterController.center = new Vector3(0, 0, 0);
            desiredPosition = normalPos;
            isSliding = false;
            isCrouch = false;
            walkSpeed = 5;
            animator.SetBool("Crouch", false);
            animator.CrossFade("Idle", 0.05f);
            Debug.Log("not crouch");
        }

        //Idle
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.LeftShift) && animator.GetBool("Crouch") == false && isCrouch)
        {
            animator.SetBool("Crouch Walking", false);
            animator.SetBool("Crouch", true);
            animator.CrossFade("Crouch Idle", 0.2f);
        }

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && isCrouch && animator.GetBool("Crouch Walking") == false)
        {
            animator.SetBool("Crouch Walking", true);
        }
        else if ((Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D) && isCrouch) && animator.GetBool("Crouch Walking") == true)
        {
            animator.SetBool("Crouch Walking", false);
            animator.CrossFade("Crouch Idle", 0.05f);
        }
        if (animator.GetBool("Crouch Walking") == true && animator.GetBool("Crouch") == true && isCrouch && !isSliding)
        {
            animator.SetBool("Crouch", false);
            
            animator.CrossFade("Crouch Walking", 0.15f);
        }

        if (isCrouch && !isSliding)
        {
            desiredPosition = crouchPos;

            if (animator.GetBool("Crouch Walking") == false)
            animator.SetBool("Crouch", true);
            
            animator.SetBool("Idle", false);
            animator.SetBool("Walking", false);
            animator.SetBool("Walking Backward", false);
            animator.SetBool("Running", false);
            animator.SetBool("Right Strafe", false);
            animator.SetBool("Left Strafe", false);
        }
        else if (!isCrouch && !isSliding)
        {
            desiredPosition = normalPos;
            animator.SetBool("Crouch", false);
            animator.SetBool("Idle", true);
        }
    }

    void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouch && Input.GetKey(KeyCode.W) && !isRunning)
        {
            walkSpeed = 7;
            isRunning = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && !isCrouch && isRunning || Input.GetKeyUp(KeyCode.W) && !isCrouch && isRunning)
        {
            walkSpeed = 4;
            isRunning = false;
        }
    }

    void Slide()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isSliding && isRunning)
        {
            isSliding = true;
            transform.Rotate(Vector3.forward * 5);
            animator.SetBool("Sliding", true);
            animator.CrossFade("Sliding", 0.15f);
        }


        if (isSliding)
            characterController.Move(transform.TransformDirection(Vector3.forward * slideSpeed * Time.deltaTime));

        if (isSliding && !isCrouch && !coroutineLock && isGrounded)
        {
            desiredPosition = slidePos;
            StartCoroutine(SlideDuration());
            characterController.height = 1.6f;
            characterController.center = new Vector3(0, -0.65f, 0);
            walkSpeed = 2;
            Debug.Log("crouched");

        }


        IEnumerator SlideDuration()
        {
            coroutineLock = true;
            Debug.Log("Coroutine");
            yield return new WaitForSeconds(1f);
            float currentXRotation = transform.localRotation.eulerAngles.x;
            float currentYRotation = transform.localRotation.eulerAngles.y;
            float currentZRotation = transform.localRotation.eulerAngles.z;
            transform.localRotation = Quaternion.Euler(0, currentYRotation, 0);
            walkSpeed = 2;
            coroutineLock = false;
            isRunning = false;
            isCrouch = true;
            isSliding = false;
            animator.SetBool("Sliding", false);
            animator.CrossFade("Crouch Walking", 0.05f);

        }
    }
    void CameraLook()
    {
        //Get mouse axis values
        Vector2 mouseAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        //rotate parent on X axis
        transform.Rotate(Vector3.up, mouseAxis.x * mouseSensitivity);

        //rotate camera on Y axis
        cameraPosY -= mouseAxis.y * mouseSensitivity;
        cameraPosY = Mathf.Clamp(cameraPosY, -90, 90);
        cam.transform.localEulerAngles = Vector3.right * cameraPosY;


        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, desiredPosition, 4 * Time.deltaTime);
    }

    void Gravity()
    {
        //check if player is on ground
        isGrounded = Physics.CheckSphere(groundCheck.transform.position, groundRadius, groundLayer);

        velocity.y += gravity * Time.deltaTime;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }

        //if player is not grounded then apply gravity
        characterController.Move(velocity * Time.deltaTime);
    }
    void Animations()
    {
        //Not Crouched
        if (isGrounded && !isCrouch && !isSliding)
        {
            //Idle
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && animator.GetBool("Idle") == false)
            {
                animator.SetBool("Walking", false);
                animator.SetBool("Walking Backward", false);
                animator.SetBool("Running", false);
                animator.SetBool("Right Strafe", false);
                animator.SetBool("Left Strafe", false);


                animator.SetBool("Idle", true);
                animator.CrossFade("Idle", 0.2f);

            }
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                animator.SetBool("Idle", false);
            }


            //Walking
            if (Input.GetKey(KeyCode.W) && animator.GetBool("Walking") == false && animator.GetBool("Walking Backward") == false)
            {
                animator.SetBool("Walking", true);
                animator.CrossFade("Walking", 0.1f);

            }
            else if (Input.GetKeyUp(KeyCode.W) && animator.GetBool("Walking") == true)
            {
                animator.SetBool("Walking", false);
                animator.CrossFade("Idle", 0.2f);
            }

            //Walking Backward
            if (Input.GetKey(KeyCode.S) && animator.GetBool("Walking Backward") == false && animator.GetBool("Walking") == false)
            {
                animator.SetBool("Walking Backward", true);
                animator.CrossFade("Walking Backward", 0.1f);
            }
            else if (Input.GetKeyUp(KeyCode.S) && animator.GetBool("Walking Backward") == true)
            {
                animator.SetBool("Walking Backward", false);
                animator.CrossFade("Idle", 0.2f);
            }

            //Running
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift) && animator.GetBool("Walking") == true)
            {
                animator.SetBool("Running", true);
            }
            else if (Input.GetKeyUp(KeyCode.W) && (animator.GetBool("Running") == true || animator.GetBool("Walking") == true))
            {
                animator.SetBool("Running", false);
                animator.CrossFade("Walking", 0.2f);
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift) && (animator.GetBool("Running") == true || animator.GetBool("Walking") == true))
            {
                animator.SetBool("Running", false);
                animator.CrossFade("Walking", 0.2f);
            }


            //Right Strafe
            if (Input.GetKey(KeyCode.D) && animator.GetBool("Left Strafe") == false)
            {
                animator.SetBool("Right Strafe", true);
            }
            else if (!Input.GetKey(KeyCode.D) && animator.GetBool("Right Strafe") == true && animator.GetBool("Walking Backward") == false && animator.GetBool("Walking") == false)
            {
                animator.SetBool("Right Strafe", false);
                animator.CrossFade("Idle", 0.2f);
            }

            //Left Strafe
            if (Input.GetKey(KeyCode.A) && animator.GetBool("Right Strafe") == false)
            {
                animator.SetBool("Left Strafe", true);
            }
            else if (!Input.GetKey(KeyCode.A) && animator.GetBool("Left Strafe") == true && animator.GetBool("Walking Backward") == false && animator.GetBool("Walking") == false)
            {
                animator.SetBool("Left Strafe", false);
                animator.CrossFade("Idle", 0.2f);
            }


        }

    }
}
