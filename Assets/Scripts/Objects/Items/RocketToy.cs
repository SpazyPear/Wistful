using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketToy : Item
{
    bool mouseDown = false;
    float rocketFuel;
    UIManager uiManager;
    Rigidbody rb;
    Camera cam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = transform.GetChild(0).GetComponent<Camera>();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        uiManager.toggleRocketBar(true);
    }
    void Update()
    {
        useRocket();
    }

    void useRocket()
    {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                mouseDown = !mouseDown;
            }

            if (mouseDown && rocketFuel > 0)
            {
                rb.AddForce(new Vector3(transform.forward.x * (1 - cam.transform.forward.y), cam.transform.forward.y, transform.forward.z * (1 - cam.transform.forward.y)).normalized * 60 * Time.deltaTime, ForceMode.Impulse);
                rocketFuel -= 1.2f * Time.deltaTime;
            }

            else if (!mouseDown)
            {
                rocketFuel = Mathf.Clamp01(rocketFuel + 0.1f * Time.deltaTime);
            }
            uiManager.UpdateRocketBar(rocketFuel);
        }
    }


