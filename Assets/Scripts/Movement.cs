using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform pos;
    private Tweener tweener;
    public Camera cam;
    public bool isGrounded;
    public Rigidbody rb;
    public GameObject prefab;
    public float jumpForce;
    public float sensitivity;
    public float moveSens;
    private float moveHorizontal;
    private float moveVertical;
    private float moveX;
    private float moveY;
    private float height;

    public GameObject target;
    public bool canUseJetPack = false;
    public PopUpManager popUpManager;
    public StatManger statManager;
    public PlayerCollisions playerCollisions;

    public event EventHandler nextBiomeEvent;


    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 144;

        height = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        collectInput();
        movement();
        jump();
    }



    void collectInput()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        moveX = Input.GetAxis("Mouse X");
        moveY = Input.GetAxis("Mouse Y");
    }

    public void OnNextBiome()
    {
        nextBiomeEvent?.Invoke(this, EventArgs.Empty);
    }

    private void movement()
    {
        target.transform.Rotate(0, moveX * sensitivity, 0);
        cam.transform.Rotate(-moveY * sensitivity, 0, 0);
        target.transform.Translate(Vector3.right * moveHorizontal * moveSens * Time.deltaTime, Space.Self);
        target.transform.Translate(Vector3.forward * moveVertical * moveSens * Time.deltaTime, Space.Self);
    }

    private void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CheckGrounded())
        {
            rb.AddForce(new Vector3(0, 2.0f, 0) * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "ground")
        { 
            isGrounded = false;
        }
    }

    private bool CheckGrounded()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, height))
        {
            return true;
        }
        return false;
    }

}

