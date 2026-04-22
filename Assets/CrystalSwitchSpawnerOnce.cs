using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSwitchSpawnerOnce : MonoBehaviour
{
    public List<CrystalDestroySwitchChecker> crystals;
    public GameObject prefab;
    public Transform location;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int count = 0;
        foreach (CrystalDestroySwitchChecker crystal in crystals)
        {
            if (crystal.Check() == false) { count++; }
        }
        if (count == crystals.Count) { DoTrigger(); }
    }

    private void DoTrigger()
    {
        Instantiate(prefab, location.position, location.rotation); Destroy(gameObject);
    }
}
