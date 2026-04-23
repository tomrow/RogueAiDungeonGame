using System.Collections.Generic;
using UnityEngine;

public class LootTables : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> lootDropsForThisStage;
    public static List<GameObject> currentLootDrops;
    void Start()
    {

        LootTables.currentLootDrops.Add(Resources.Load<GameObject>("Coin"));
        LootTables.currentLootDrops.Add(Resources.Load<GameObject>("HealthPack"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
