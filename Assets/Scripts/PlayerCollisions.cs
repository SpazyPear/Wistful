using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    [HideInInspector]
    public List<string> itemsHeld = new List<string>();

    public UIManager uiManager;
    public InventoryManager inventoryManager;
    public GameObject prefab;
    Movement movement;

    private void Start()
    {
        movement = GetComponent<Movement>();
    }

    private void Update()
    {
    }

    private void OnTriggerStay(Collider collider)
    {
        if (Input.GetKeyDown(KeyCode.E) && collider.gameObject.GetComponents(typeof(Item)).Length > 0)
        {
            inventoryManager.pickUpItem((collider.gameObject.GetComponent(typeof(Item))).GetType());
            Destroy(collider.gameObject);
        }
       
    }
    public float PosNegAngle(Vector3 a1, Vector3 a2, Vector3 normal)
    {
        float angle = Vector3.Angle(a1, a2);
        float sign = Mathf.Sign(Vector3.Dot(normal, Vector3.Cross(a1, a2)));
        return angle * sign;
    }


    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.CompareTag("Ladder"))
        {
            float halfLength = Mathf.Sqrt(Mathf.Pow(collider.gameObject.GetComponent<MeshRenderer>().bounds.size.y, 2) + Mathf.Pow(collider.gameObject.GetComponent<MeshRenderer>().bounds.size.z, 2)) * 0.9f; 
            float angle = collider.gameObject.transform.rotation.eulerAngles.x * Mathf.Deg2Rad;
            movement.lastLadderAngle = collider.transform.position - new Vector3(halfLength * Mathf.Cos(angle), halfLength * Mathf.Sin(angle), 0);
           
            movement.onLadder = true;

            // movement.lastLadderAngle = collider.gameObject.transform.forward;
        }
    }

    void OnCollisionExit(Collision collider)
    {
        if (collider.gameObject.CompareTag("Ladder"))
        {
            movement.onLadder = false;
        }
    }

    /*Vector3 findMostForwardPoint(MeshFilter filter)
    {
        Vector3 vertices = filter.mesh.
        Matrix 4x4 localToWorld = transform.localToWorldMatrix;

        for (int i = 0; i < mf.mesh.vertices.Length; ++i)
        {
            Vector3 world_v = localToWorld.MultiplyPoint3x4(mf.mesh.vertices[i]);
        }
    }*/

    float easeInOutElastic(float x) {
            const float c5 = (2f * Mathf.PI) / 4.5f;

            return x == 0
            ? 0
            : x == 1
            ? 1
            : x< 0.5
            ? -(Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((20f * x - 11.125f) * c5)) / 2
            : (Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((20f * x - 11.125f) * c5)) / 2 + 1;
    }

}
