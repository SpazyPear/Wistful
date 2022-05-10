using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform pos;
    public Camera cam;
    public Rigidbody rb;
    public GameObject prefab;
    public float jumpForce;
    public float sensitivity;
    public float moveSens;
    public float sprintMultiplier;
    public float wallJumpForce;
    private bool wallJumpCheck;
    private Vector3 wallPos;
    private float xDiff;
    private float zDiff;
    private float moveHorizontal;
    private float moveVertical;
    private float moveX;
    private float moveY;
    private float rotationY = 0.0f;
    private float height;
    private bool sprint;

    public GameObject target;
    public PopUpManager popUpManager;
    public PlayerCollisions playerCollisions;

    public MenuController menuController;



    // Start is called before the first frame update
    void Start()
    {
        height = 2.0f;
        sprintMultiplier = 1.5f;
        //Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 144;
    }

    // Update is called once per frame
    void Update()
    {
        collectInput();
        movement();
        jump();
        checkRespawn();
    }

    void checkRespawn()
    {
        if (transform.position.y < -12)
        {
            rb.GetComponent<Rigidbody>().isKinematic = true;
            transform.position = new Vector3(transform.position.x, popUpManager.levelHeight + 6, transform.position.z);
            rb.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    void collectInput()
    {
        
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        moveX = Input.GetAxis("Mouse X");
        moveY = Input.GetAxis("Mouse Y");
        rotationY -= moveY * sensitivity;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

        sprint = (Input.GetKey(KeyCode.LeftShift));
    }

    private void movement()
    {
        target.transform.Rotate(0, moveX * sensitivity, 0);
        cam.transform.localRotation = Quaternion.Euler(rotationY, 0, 0);
        target.transform.Translate(Vector3.right * moveHorizontal * moveSens * ((sprint) ? sprintMultiplier : 1) * Time.deltaTime, Space.Self);
        target.transform.Translate(Vector3.forward * moveVertical * moveSens * ((sprint) ? sprintMultiplier : 1) * Time.deltaTime, Space.Self);
    }

    private void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (checkGrounded())
            {
                rb.AddForce(new Vector3(0, 4.0f, 0) * jumpForce, ForceMode.Impulse);
            }
            else if (wallJumpCheck)
            {
                rb.AddForce(new Vector3(wallJumpForce * xDiff, 3.5f, wallJumpForce * zDiff) * jumpForce, ForceMode.Impulse);
            }
        }
    }

    private bool checkGrounded()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, height))
        {
            return true;
        }
        return false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!checkGrounded())
        {
            wallJumpCheck = true;
            var collider = collision.transform.GetComponent<Collider>();
            wallPos = collider.ClosestPoint(target.transform.position);
            xDiff = target.transform.position.x - wallPos.x;
            zDiff = target.transform.position.z - wallPos.z;
        }
        else
        {
            wallJumpCheck = false;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        wallJumpCheck = false;
    }

}

