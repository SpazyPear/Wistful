using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Tweener : MonoBehaviour
{
   // private Tween activeTween;
    public List<Tween> activeTweens;
    private List<Tween> toBeRemoved;
    public PopUpManager popUpManager;
    public AudioClip[] blockChimes;

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
                    if (Vector3.Distance(activeTween.Target.position, activeTween.EndPos) > 0.04f)
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
                       /* AudioSource audioSource = activeTween.Target.transform.GetChild(0).GetComponent<AudioSource>();
                        audioSource.clip = blockChimes[UnityEngine.Random.Range(0, blockChimes.Length - 1)];
                        int reverse = UnityEngine.Random.Range(0, 8);  
                        if (reverse == 7)
                        {
                            audioSource.time = audioSource.clip.length - 0.01f;
                            audioSource.pitch = -1;
                        }
                        activeTween.Target.transform.GetChild(0).GetComponent<AudioSource>().Play();*/
                    }
                }
            }
            


            catch (Exception e)
            {

            }
        }

        for (int i = toBeRemoved.Count - 1; i > 0; i--)
        {
            GameObject obj = toBeRemoved[i].Target.gameObject;
            activeTweens.Remove(toBeRemoved.ElementAt(i));

            toBeRemoved.RemoveAt(i);

            if (obj.transform.position.y <= -8)
            {
                popUpManager.pastPlatforms.Remove(obj);
                Destroy(obj);
            }

        } 
    }

    public void AddTween(Transform targetObject, Vector3 startPos, Vector3 endPos, float duration)
    {
       
        activeTweens.Add(new Tween(targetObject, startPos, endPos, Time.time, duration - UnityEngine.Random.Range(-9f, 0.5f)));
        
    }

    public async Task waitForComplete()
    {
        while (activeTweens.Count > 0)
        {
            await Task.Yield();
        }
    }

   

}
