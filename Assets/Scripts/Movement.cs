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
    private Vector3 normalizedVector;
    private float moveX;
    private float moveY;
    private float rotationY = 0.0f;
    private float height;
    private bool sprint;
    public Vector3 lastGroundedPos;
    public GameObject target;
    public PopUpManager popUpManager;
    public PlayerCollisions playerCollisions;

    public bool canMove = true;

    public MenuController menuController;

    bool isGrounded = false;



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
        if (canMove)
        {
            collectInput();
            movement();
            jump();
        }
        moveX = Input.GetAxis("Mouse X");
        moveY = Input.GetAxis("Mouse Y");
        rotationY -= moveY * sensitivity;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);
        target.transform.Rotate(0, moveX * sensitivity, 0);
        cam.transform.localRotation = Quaternion.Euler(rotationY, 0, 0);
        checkRespawn();
    }

    void checkRespawn()
    {
        if (transform.position.y < -16)
        {
            /*Vector3 pos = Vector3.positiveInfinity;
            for (int x = -12; x <= 12; x += 4)
            {
                for (int y = -12; y <= 12; y += 4)
                {
                    if (Vector3.Distance(new Vector3(transform.position.x, popUpManager.levelHeight, transform.position.z), VectorUtil.roundVector3(new Vector3(transform.position.x + x, popUpManager.levelHeight, transform.position.z + y))) < Vector3.Distance(new Vector3(transform.position.x, popUpManager.levelHeight, transform.position.z), pos) && Physics.CheckBox(VectorUtil.roundVector3(new Vector3(transform.position.x + x, popUpManager.levelHeight, transform.position.z + y)), new Vector3(1, 1, 1)))
                    {
                        pos = VectorUtil.roundVector3(new Vector3(transform.position.x + x - 3, popUpManager.levelHeight + 4, transform.position.z + y - 3));
                    }
                }
            }
            transform.position = pos;*/
            transform.position = new Vector3(lastGroundedPos.x, lastGroundedPos.y + 4, lastGroundedPos.z);
        }
        
    }

    void createDebugSphere(Vector3 pos, Vector3 scale)
    {
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        obj.transform.localScale = scale;
        //Destroy(obj, 2f);
    }

    void collectInput()
    {
        normalizedVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveHorizontal = normalizedVector.x;
        moveVertical = normalizedVector.z;

        //sprint = (Input.GetKey(KeyCode.LeftShift));
    }

    private void movement()
    {
        
        target.transform.Translate(Vector3.right * moveHorizontal * moveSens * ((sprint) ? sprintMultiplier : 1) * Time.deltaTime, Space.Self);
        target.transform.Translate(Vector3.forward * moveVertical * moveSens * ((sprint) ? sprintMultiplier : 1) * Time.deltaTime, Space.Self);
    }

    private void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (checkGrounded())
            {
                rb.AddForce(new Vector3(0, 3.0f, 0) * jumpForce, ForceMode.Impulse);
            }
            else if (wallJumpCheck)
            {
                //rb.AddForce(new Vector3(wallJumpForce * xDiff, 3.0f, wallJumpForce * zDiff) * jumpForce, ForceMode.Impulse);
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
            VectorUtil.roundVector3(transform.position);
        }
        if (collision.gameObject.tag == "ground" && checkGrounded())
        {
            lastGroundedPos = transform.position;
            //isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            //isGrounded = false;
        }
        wallJumpCheck = false;
    }

}

