using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pointer : MonoBehaviour
{

    List<Transform> currentItemObjects = new List<Transform>();
    Material material;
    public AnimationCurve curve;
    Color32 c = new Color32(172, 222, 94, 255);

    // Start is called before the first frame update
    void Start()
    {
        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentItemObjects.Count != 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);

            Transform closestItem = currentItemObjects[0];
            float minDist = Mathf.Infinity;
            for (int x = currentItemObjects.Count - 1; x <= 0; x++)
            {
                try
                {
                    Transform item = currentItemObjects[x];

                    if (Vector3.Distance(transform.position, item.position) < minDist)
                    {
                        closestItem = item;
                        minDist = Vector3.Distance(transform.position, item.position);
                    }
                }
                catch (Exception e)
                {
                    currentItemObjects.RemoveAt(x);
                    continue;
                }
            }

            try
            {

                Vector3 _direction = (closestItem.position - transform.position).normalized;

                Vector3 _lookRotation = Quaternion.LookRotation(_direction).eulerAngles;

                transform.rotation = Quaternion.Euler(_lookRotation.x, _lookRotation.y, 0);

                Debug.Log(Remap(Mathf.Clamp(Vector3.Distance(transform.position, closestItem.position), 1, 50), 1, 50, 0, 1));

                material.color = Color.Lerp(c, Color.red, curve.Evaluate(Remap(Mathf.Clamp(Vector3.Distance(transform.position, closestItem.position), 5, 50), 5, 50, 0, 1)));
            }

            catch (Exception e)
            {
                currentItemObjects.Remove(closestItem);
            }

        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

  

    public void addItemInstance(GameObject item)
    {
        recursiveChildSearch(item.transform);
    }

    void printList()
    {
        foreach(Transform currentItemObject in currentItemObjects)
        {
            Debug.Log(currentItemObject.gameObject.name);
        }
    }

    void recursiveChildSearch(Transform searchObj)
    {
        if (searchObj.CompareTag("Trackable"))
        {
            currentItemObjects.Add(searchObj);
        }

        for (int x = 0; x < searchObj.transform.childCount; x++)
        {
            if (searchObj.transform.GetChild(x).CompareTag("Trackable")) {
                currentItemObjects.Add(searchObj.transform.GetChild(x));
            }

            recursiveChildSearch(searchObj.transform.GetChild(x));

        }
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
