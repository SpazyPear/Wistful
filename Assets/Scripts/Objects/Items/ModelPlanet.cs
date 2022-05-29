using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelPlanet : Item
{
    List<GameObject> planets = new List<GameObject>();
    List<int> planetPositions = new List<int>();
    GameObject modelObj;
    SkyManager skyManager;
    UIManager uiManager;
    int currentPlanetIndex = 0;
    bool beingHeld;
    bool turning;
    Movement movement;
    // Start is called before the first frame update
    void Start()
    {
        transform.eulerAngles = Vector3.zero;
        modelObj = Instantiate((Resources.Load("planetInHand") as GameObject), Vector3.zero, Quaternion.identity);
        modelObj.transform.eulerAngles += new Vector3(0, 90, -90);
        modelObj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        modelObj.transform.SetParent(transform, true);
        modelObj.transform.localPosition = new Vector3(0, 1f, 3);

        modelObj.SetActive(false);

        for (int x = 1; x < modelObj.transform.childCount - 1; x++)
        {
            planets.Add(modelObj.transform.GetChild(x).gameObject);
            planetPositions.Add(0);
        }

        movement = GetComponent<Movement>();
        skyManager = GameObject.FindGameObjectWithTag("SkyManager").GetComponent<SkyManager>();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        uiManager.updateHoldPrompt("Press 1 or 2 To Switch Items");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            modelObj.SetActive(!beingHeld);
            beingHeld = !beingHeld;
            if (beingHeld)
            {
                StartCoroutine(flashSelectedPlanet());
                movement.canMove = false;
                uiManager.updateHoldPrompt("W-S To Select Planet, A-D To Rotate, E To Confirm (Watch The Sky), R to Reset");
            }
            else
            {
                uiManager.updateHoldPrompt("Press 1 or 2 To Switch Items");
                movement.canMove = true;
            }

        }
        if (beingHeld && !turning)
        {
            if (Input.GetKeyDown(KeyCode.W) && currentPlanetIndex != planets.Count - 1)
            {
                currentPlanetIndex++;
            }
            if (Input.GetKeyDown(KeyCode.S) && currentPlanetIndex != 0)
            {
                currentPlanetIndex--;
            }

            
            if (Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(rotatePlanet(planets[currentPlanetIndex].transform, 1));
                updatePlanetPosition(1);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(rotatePlanet(planets[currentPlanetIndex].transform, -1));
                updatePlanetPosition(-1);
            }

            if (Input.GetKey(KeyCode.E))
            {
                StartCoroutine(skyManager.spin(8f, isRotsCorrect()));
            }

            if (Input.GetKey(KeyCode.R))
            {
                int temp = currentPlanetIndex;
                for (currentPlanetIndex = 0; currentPlanetIndex < planetPositions.Count; currentPlanetIndex++)
                {
                    StartCoroutine(rotatePlanet(planets[currentPlanetIndex].transform, -planetPositions[currentPlanetIndex]));
                    updatePlanetPosition(-planetPositions[currentPlanetIndex]);
                }
                currentPlanetIndex = temp;
            }
        }
    }

    void updatePlanetPosition(int direction)
    {
        planetPositions[currentPlanetIndex] += direction;
        if (planetPositions[currentPlanetIndex] == 5)
        {
            planetPositions[currentPlanetIndex] = 0;
        }
        else if (planetPositions[currentPlanetIndex] == -1)
        {
            planetPositions[currentPlanetIndex] = 4;
        }
    }

    bool isRotsCorrect()
    {
        return planetPositions.SequenceEqual(skyManager.correctRots);
    }

    IEnumerator flashSelectedPlanet()
    {
        int startPlanet = currentPlanetIndex;
        MeshRenderer curRend = planets[currentPlanetIndex].transform.GetChild(0).GetComponent<MeshRenderer>();
        float alpha = 0;
        while (true)
        {
            if (beingHeld)
            {
                setRecurringMaterialAlpha(curRend, (Mathf.Sin(alpha += 0.1f) + 1) / 2f);
            }
            else
            {
                setRecurringMaterialAlpha(curRend, 1);
                yield break;
            }
            if (startPlanet != currentPlanetIndex)
            {
                setRecurringMaterialAlpha(curRend, 1);
                break;
            }
            yield return null;
        }
        StartCoroutine(flashSelectedPlanet());
    }

    void setRecurringMaterialAlpha(MeshRenderer curMat, float alpha)
    {
        foreach (Material mat in curMat.materials)
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);

    }

    IEnumerator rotatePlanet(Transform planet, int direction)
    {
        turning = true;
        float progress = 0;
        Vector3 startRot = planet.localEulerAngles;
        while (progress < 1)
        {
            progress += Time.deltaTime / 0.5f;
            planet.localEulerAngles = new Vector3(planet.localEulerAngles.x, Mathf.Lerp(startRot.y, startRot.y + (direction * 72f), progress), planet.transform.localEulerAngles.z);
            yield return null;
        }
        planet.localEulerAngles = new Vector3(planet.localEulerAngles.x, startRot.y + (direction * 72f), planet.localEulerAngles.z);
        turning = false;
    }
}
