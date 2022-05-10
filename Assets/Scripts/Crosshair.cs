using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField]
    private Image image;
    Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.TransformDirection(Vector3.forward), out hit, 3.0f))
        {
            Debug.Log("Item" + hit.transform.gameObject.GetComponent(typeof(Item)));
            Debug.Log("Door" + hit.transform.gameObject.GetComponent(typeof(Door)));
            if (hit.transform.gameObject.GetComponent(typeof(Item)) || hit.transform.gameObject.GetComponent(typeof(Door)))
            {
                image.color = Color.green;
            }
            else
            {
                image.color = Color.white;
            }
        }
    }
}
