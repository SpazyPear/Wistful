using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tweener : MonoBehaviour
{
   // private Tween activeTween;
    private List<Tween> activeTweens;
    private List<Tween> toBeRemoved;

    // Start is called before the first frame update
    void Start()
    {
        activeTweens = new List<Tween>();
        toBeRemoved = new List<Tween>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Tween activeTween in activeTweens)
        {
            //Debug.Log("0");
            try
            {
                if (activeTween.Target != null)
                {
                   // Debug.Log("1");
                    if (Vector3.Distance(activeTween.Target.position, activeTween.EndPos) > 0.1f)
                    {
                        //Debug.Log("2");
                        float timeFraction = (Time.time - activeTween.StartTime) / activeTween.Duration;
                        float newTime = Mathf.Pow(timeFraction, 2);
                        activeTween.Target.position = Vector3.Lerp(activeTween.Target.position, activeTween.EndPos, timeFraction);

                    }
                    else
                    {
                       // Debug.Log("3");
                        activeTween.Target.position = activeTween.EndPos;
                        toBeRemoved.Add(activeTween);
                        //Cursor.lockState = CursorLockMode.Confined;
                    }
                }
            }
            


            catch (Exception e)
            {

            }
        }

        for (int i = toBeRemoved.Count - 1; i > 0; i--)
        {
            activeTweens.Remove(toBeRemoved.ElementAt(i));
            if (toBeRemoved.ElementAt(i).EndPos.y == -20f && toBeRemoved.ElementAt(i).Target.gameObject.tag != "puzzle")
            {
                Destroy(toBeRemoved.ElementAt(i).Target.gameObject);
            }
            toBeRemoved.RemoveAt(i);
        } 
    }

    public void AddTween(Transform targetObject, Vector3 startPos, Vector3 endPos, float duration)
    {
       
            activeTweens.Add(new Tween(targetObject, startPos, endPos, Time.time, duration));
        
    }

   

}
