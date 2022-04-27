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
    private float rotationY = 0.0f;

    public GameObject target;
    public PopUpManager popUpManager;
    public PlayerCollisions playerCollisions;

    public event EventHandler nextBiomeEvent;
    public MenuController menuController;



    // Start is called before the first frame update
    void Start()
    {
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
            transform.position = new Vector3(transform.position.x, popUpManager.levelHeights[popUpManager.currentLevel] + 6, transform.position.z);
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
       
    }

    public void OnNextBiome()
    {
        nextBiomeEvent?.Invoke(this, EventArgs.Empty);
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
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
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

}

