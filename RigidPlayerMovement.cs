using System.Collections.Specialized;
using UnityEngine;

public class RigidPlayerMovement : MonoBehaviour
{

    public Transform cam;
    public Rigidbody rb;
    public float camRotationSpeed = 5f;
    public float cameraMinY = -75f;
    public float cameraMaxY = 80f;
    public float rotationSmoothSpeed = 10f;

    float speed;
    public float walkSpeed = 9f;
    public float runSpeed = 14f;
    public float maxSpeed = 20f;
    public float jumpPower = 30f;

    public float extraGravity = 45f;

    float bodyRotationX;
    float camRotationY;
    Vector3 directionIntentX;
    Vector3 directionIntentY;

    public bool grounded;

    public int jumpCounter;

    void Update()
    {
        LookRotation();
        Movement();
        ExtraGravity();
        GroundCheck();
        Jump();
        
    }

    void LookRotation()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //Get camera and body rotational values
        bodyRotationX += Input.GetAxis("Mouse X") * camRotationSpeed;
        camRotationY += Input.GetAxis("Mouse Y") * camRotationSpeed;

        //Stop our camera from rotating 360 degrees
        camRotationY = Mathf.Clamp(camRotationY, cameraMinY, cameraMaxY);

        //create rotation targets and handle rotations of the body and camera
        Quaternion camTargetRotation = Quaternion.Euler(-camRotationY, 0, 0);
        Quaternion bodyTargetRotation = Quaternion.Euler(0, bodyRotationX, 0);

        //handle rotations
        transform.rotation = Quaternion.Lerp(transform.rotation, bodyTargetRotation, Time.deltaTime * rotationSmoothSpeed);

        cam.localRotation = Quaternion.Lerp(cam.localRotation, camTargetRotation, Time.deltaTime * rotationSmoothSpeed);
    }

    void Movement()
    {
        //Direction must match camera direction
        directionIntentX = cam.right;
        directionIntentX.y = 0;
        directionIntentX.Normalize();

        directionIntentY = cam.forward;
        directionIntentY.y = 0;
        directionIntentY.Normalize();

        //change our character's velocity in this direction
        rb.velocity = directionIntentY * Input.GetAxis("Vertical") * speed + directionIntentX * Input.GetAxis("Horizontal") * speed + Vector3.up * rb.velocity.y;
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        //control speed based on movement state

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = runSpeed;
        }
        else
        {
            speed = walkSpeed;
        }
    }

    void ExtraGravity()
    {
        rb.AddForce(Vector3.down * extraGravity);
    }

    void GroundCheck()
    {
        RaycastHit groundHit;
        grounded = Physics.Raycast(transform.position, -transform.up, out groundHit, 1.25f);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && grounded)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            jumpCounter = 1;
        }

        //double jump
        if (Input.GetButtonDown("Jump") && !grounded && jumpCounter == 1)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            jumpCounter -= 1;
        }

    }
}
