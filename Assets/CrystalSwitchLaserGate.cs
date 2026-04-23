using System.Collections.Generic;
using UnityEngine;

public class CrystalSwitchLaserGate : MonoBehaviour
{
    [SerializeField, Tooltip("This is the switch's accompanying gate.")] GameObject MyDoor;
    LaserDoor laserDoorReference;
    CrystalDestroySwitchChecker[] crystals;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        crystals = transform.GetComponentsInChildren<CrystalDestroySwitchChecker>();
        Debug.Log("Crystals in switch puzzle: " + crystals.Length.ToString());
    }

    // Update is called once per frame
    void Update()
    {
            int count = 0;
            foreach (CrystalDestroySwitchChecker crystal in crystals)
            {
                if (crystal.Check() == false) { count++; }
            }
            if (count == crystals.Length) { Debug.Log("Puzzle Destroyed"); DoTrigger(); }
        
    }

    void DoTrigger()
    {
        laserDoorReference = MyDoor.GetComponentInChildren<LaserDoor>(); // pull from the connected object's children to find the LaserDoor script.
        laserDoorReference.GateHasBeenOpened = true; //open the door.
    }
}
