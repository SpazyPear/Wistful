using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKPickup : MonoBehaviour
{
    Animator anim;
    public GameObject target;
    public GameObject hand;
    public float IK_weight = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        IK_weight = anim.GetFloat("IKPickup");
        if (IK_weight > 0.95)
        {
            target.transform.parent = hand.transform;
            target.transform.localPosition = new Vector3(-2.55f, 4.488f, 11.482f);
        }
        anim.SetIKPosition(AvatarIKGoal.RightHand, target.transform.position);
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, IK_weight);
        anim.SetLookAtPosition(target.transform.position);
        anim.SetLookAtWeight(IK_weight);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetTrigger("pickUp");
        }
    }
}
