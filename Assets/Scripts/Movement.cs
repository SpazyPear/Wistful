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

    public GameObject target;
    public PopUpManager popUpManager;
    public PlayerCollisions playerCollisions;

    public bool canMove = true;

    public MenuController menuController;



    // Start is called before the first frame update
    void Start()
    {
        height = 2.0f;
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
        checkRespawn();
    }

    void checkRespawn()
    {
        if (transform.position.y < -16)
        {
            Vector3 pos = Vector3.positiveInfinity;
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
            transform.position = pos;
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
        
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        moveX = Input.GetAxis("Mouse X");
        moveY = Input.GetAxis("Mouse Y");
        rotationY -= moveY * sensitivity;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);
       
    }

    private void movement()
    {
        target.transform.Rotate(0, moveX * sensitivity, 0);
        cam.transform.localRotation = Quaternion.Euler(rotationY, 0, 0);
        target.transform.Translate(Vector3.right * moveHorizontal * moveSens * Time.deltaTime, Space.Self);
        target.transform.Translate(Vector3.forward * moveVertical * moveSens * Time.deltaTime, Space.Self);
    }

    private void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (checkGrounded())
            {
                rb.AddForce(new Vector3(0, 2.0f, 0) * jumpForce, ForceMode.Impulse);
            }
            else if (wallJumpCheck)
            {
                rb.AddForce(new Vector3(wallJumpForce * xDiff, 2, wallJumpForce * zDiff) * jumpForce, ForceMode.Impulse);
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

