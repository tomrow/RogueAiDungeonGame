using UnityEngine;

public class LootTables : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] lootDropsForThisStage;
    public static GameObject[] currentLootDrops;
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
