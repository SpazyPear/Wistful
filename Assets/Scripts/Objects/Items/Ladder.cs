using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : Item
{

    public bool onLadder = false;
    public Vector3 endOfLadderPos;
    bool interactDown = false;

    void Update()
    {
        checkClimbLadder();
    }

    public void checkClimbLadder()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyUp(KeyCode.E))
        {
            interactDown = !interactDown;
        }
        if (interactDown && onLadder)
            StartCoroutine(climbLadder());

    }

    IEnumerator climbLadder()
    {
        float progress = 0;
        Vector3 startPos = transform.position;
        while (interactDown && progress < 0.98f)
        {
            transform.position = Vector3.Lerp(startPos, endOfLadderPos, progress);
            progress += Time.deltaTime;
            yield return null;
        }
    }
    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("LadderPlacement") && Input.GetKeyDown(KeyCode.E) && GetComponent<PlayerCollisions>().itemsHeld.Contains("Ladder"))
        {
            collider.gameObject.GetComponent<LadderPlacementTrigger>().ladder.SetActive(true);
        }
    }
    public float PosNegAngle(Vector3 a1, Vector3 a2, Vector3 normal)
    {
        float angle = Vector3.Angle(a1, a2);
        float sign = Mathf.Sign(Vector3.Dot(normal, Vector3.Cross(a1, a2)));
        return angle * sign;
    }


    void OnCollisionStay(Collision collider)
    {
        if (collider.gameObject.CompareTag("Ladder"))
        {
            float halfLength = Mathf.Sqrt(Mathf.Pow(collider.gameObject.GetComponent<MeshRenderer>().bounds.size.y, 2) + Mathf.Pow(collider.gameObject.GetComponent<MeshRenderer>().bounds.size.z, 2)) * 0.9f;
            float angle = collider.gameObject.transform.rotation.eulerAngles.x * Mathf.Deg2Rad;
            endOfLadderPos = collider.transform.position - new Vector3(halfLength * Mathf.Cos(angle), halfLength * Mathf.Sin(angle), 0);

            onLadder = true;
        }
        else if (collider.gameObject.CompareTag("VerticalLadder"))
        {
            float halfLength = collider.gameObject.GetComponent<MeshRenderer>().bounds.size.y;
            endOfLadderPos = collider.transform.position + new Vector3(0, halfLength, 0);
            onLadder = true;
        }
    }

    private void OnCollisionExit(Collision collider)
    {
        if (collider.gameObject.CompareTag("Ladder") || collider.gameObject.CompareTag("VerticalLadder"))
            onLadder = false;
    }
}
