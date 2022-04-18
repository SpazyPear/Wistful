using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    float halfLength;
    bool isOpen;
    bool isTurning;
    public GameObject prefab;
    Vector3 pivotPoint;
    public bool isLocked;

    // Start is called before the first frame update
    void Start()
    {
        try 
        { 
            halfLength = gameObject.GetComponent<MeshRenderer>().bounds.size.x / 2f;
        }
        catch (MissingComponentException e)
        {
            halfLength = transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size.x / 2f;
        }
        pivotPoint = new Vector3(transform.position.x - halfLength, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggleDoor()
    {
        if (!isTurning)
        {
            
            if (!isOpen)
            {
                StartCoroutine(spinDoor(true));
            }
            else
            {
                StartCoroutine(spinDoor(false));
            }
            isOpen = !isOpen;
        }
    }

    IEnumerator spinDoor(bool open)
    {
        isTurning = true;

        float rotation = open ? -90f : 90f;
        float progress = 0;
        while (progress < 1f)
        {
            progress += Time.fixedDeltaTime;
            float angle = rotation * Time.fixedDeltaTime;
            transform.RotateAround(pivotPoint, Vector3.up, angle);
            yield return new WaitForFixedUpdate();
        }
        transform.eulerAngles = open ? new Vector3(0, -90, 0) : new Vector3(0, 0, 0);
        isTurning = false;
    }

}
