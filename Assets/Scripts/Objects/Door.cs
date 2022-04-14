using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    float halfLength;
    bool isOpen;
    bool isTurning;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        halfLength = gameObject.GetComponent<MeshRenderer>().bounds.size.x/2f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator toggleDoor()
    {
        if (!isTurning)
        {
            float startAngle = transform.rotation.eulerAngles.y;
            isTurning = true;
            float progress = 0;
            if (!isOpen)
            {
                while (progress < 0.98f)
                {
                    progress += Time.deltaTime;
                    transform.RotateAround(new Vector3(transform.position.x - halfLength, transform.position.y, transform.position.z), Vector3.up, Mathf.Clamp(Mathf.Lerp(startAngle, 90, progress), 0, 90));
                    yield return null;
                }
                isOpen = true;
            }
            else
            {

            }
            isTurning = false;
        }
    }

}
