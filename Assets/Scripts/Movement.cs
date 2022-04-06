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
    private float timer = 0.4f;
    private bool isTimerRunning = false;
    private Collider other;
    public GameObject target;
    public bool canUseJetPack = false;
    public PopUpManager popUpManager;
    public StatManger statManager;
    public PlayerCollisions playerCollisions;
    public UIManager uiManager;
    bool mouseDown = false;
    float rocketFuel = 1f;
    

    // Start is called before the first frame update
    void Start()
    {
        //camMove.handleCamMove(target);
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 144;
    }

    // Update is called once per frame
    void Update()
    {
 
        collectInput();
        movement();
        jump();
        jetpackUse();
        useRocket();

    }

    private void collectInput()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
        Transform[] ts = target.GetComponentsInChildren<Transform>();
        foreach (Transform com in ts)
        {
        }
        if (ts.Length > 1)
        {

            moveX = Input.GetAxis("Mouse X");
            moveY = Input.GetAxis("Mouse Y");
        }
    }

    void jetpackUse()
    {
        if (Input.GetKeyDown(KeyCode.E) && canUseJetPack)
        {
            rb.AddForce(Vector3.up * 45, ForceMode.Impulse);
            canUseJetPack = false;
            Physics.gravity += new Vector3(0, 1.2f, 0);
            OnNextBiome();
        }
    }

    void useRocket()
    {
        if (playerCollisions.itemsHeld.Contains("RocketToy")) {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                mouseDown = !mouseDown;
            }

            if (mouseDown)
            {
                rb.AddForce(new Vector3(transform.forward.normalized.x, cam.transform.forward.y, transform.forward.normalized.z) * 60 * Time.deltaTime, ForceMode.Impulse);
                rocketFuel -= 1.2f * Time.deltaTime;
            }

            else if (!mouseDown)
            {
                rocketFuel = Mathf.Clamp01(rocketFuel + 0.1f * Time.deltaTime);
            }
            uiManager.UpdateRocketBar(rocketFuel);
        }
    }


    public void OnNextBiome()
    {
        nextBiomeEvent?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler nextBiomeEvent;

    private void movement()
    {
        target.transform.Rotate(0, moveX * sensitivity, 0);
        cam.transform.Rotate(-moveY * sensitivity, 0, 0);
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



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "box")
        {
            if (timer == 0)
            {
                timer = 0.4f;
                this.other = other;
            }
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ground")
        { 
            isGrounded = false;
        }
    }






}

