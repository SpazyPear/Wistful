using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Biome
{
    [SerializeField]
    private List<GameObject> forestKeysList;
    [SerializeField]
    private List<int> forestValuesList;
    [SerializeField]
    private List<GameObject> winterKeysList;
    [SerializeField]
    private List<int> winterValuesList;
    [SerializeField]
    private List<GameObject> hellKeysList;
    [SerializeField]
    private List<int> hellValuesList;
    public Dictionary<GameObject, int> ForestArray;
    public Dictionary<GameObject, int> WinterArray;
    public Dictionary<GameObject, int> HellArray;
    public enum biome { Hell = -20, Grass = 0, Ice = 100 };
    public biome currentBiome;

    private Dictionary<GameObject, int> currentBiomeDict;

    public Biome()
    {
        instantiateDataStructures();

    }

    void instantiateDataStructures()
    {
        ForestArray = new Dictionary<GameObject, int>();
        WinterArray = new Dictionary<GameObject, int>();
        HellArray = new Dictionary<GameObject, int>();

        for (int i = 0; i < forestKeysList.Count; i++)
        {
            ForestArray.Add(forestKeysList[i], forestValuesList[i]);
        }
        for (int i = 0; i < winterKeysList.Count; i++)
        {
            WinterArray.Add(winterKeysList[i], winterValuesList[i]);
        }
        for (int i = 0; i < hellKeysList.Count; i++)
        {
            HellArray.Add(hellKeysList[i], hellValuesList[i]);
        }
        currentBiome = biome.Grass;
    }

    void nextBiome()
    {
        currentBiome = Enum.GetValues(typeof(biome)).Cast<biome>().Skip(1).First();
        switch (currentBiome)
        {
            case biome.Grass:
                currentBiomeDict = ForestArray;
                break;
            case biome.Ice:
                break;

        }
    }
}

public class BiomeArgs : EventArgs
{

}
