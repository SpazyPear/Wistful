using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextRoomTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject newRoom;
    void Start()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        bool negX = false;
        bool negZ = false;
        if (other.tag == "Player")
        {
            if (other.transform.rotation.x < 0)
            {

            }
            //Instantiate(newRoom, )
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
