using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlockBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -20)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(collision.gameObject.transform.position.x - transform.position.x * 10, 0, collision.gameObject.transform.position.z - transform.position.z * 10));
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "ground")
        {
            Destroy(gameObject);
        }
    }
}
