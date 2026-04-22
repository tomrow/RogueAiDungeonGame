using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class StageGoal : MonoBehaviour
{
    PlayerCtl playerCtl;
    public string nextStage;
    float walkTimer;
    Vector3 playerContactPos;
    public bool revertInventory;
    public List<PlayerHealth.Item> oldInv = new List<PlayerHealth.Item>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        oldInv = PlayerHealth.inventory;
        foreach(PlayerHealth.Item item in PlayerHealth.inventory)
        { oldInv.Add(item); }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCtl != null)
        {
            walkTimer += Time.deltaTime;
            playerCtl.transform.position = Vector3.Lerp(playerContactPos, transform.position + (Vector3.up * 0.2f), Mathf.Clamp01(walkTimer));
            if (walkTimer >= 1) 
            { 
                playerCtl.state = PlayerCtl.States.StandOnGoal;
                if (walkTimer <= 4)
                { transform.position += (Vector3.up * Time.deltaTime); }
                else 
                { 
                    FadeOverlay f = Instantiate(Resources.Load<GameObject>("FadeOverlay"), transform.Find("/Canvas")).GetComponent<FadeOverlay>();
                    f.sceneForTransfer = nextStage;
                    f.transform.localPosition = Vector3.zero;
                    f.mode = FadeOverlay.Transitions.FadeOut;
                    if (PlayerHealth.thisPlayer.weapon != null) { WeaponClass.Collect(PlayerHealth.thisPlayer.weapon); PlayerHealth.thisPlayer.weapon = null; }
                    if (revertInventory)
                    {
                        PlayerHealth.inventory.Clear();
                        foreach (PlayerHealth.Item item in oldInv)
                        { PlayerHealth.inventory.Add(item); } //set it back to what it was before tutorial
                    }
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerCtl p = other.gameObject.GetComponent<PlayerCtl>();
        if (p != null && playerCtl == null)
        {
            playerCtl = p;
            p.state = PlayerCtl.States.WalkTowardGoal;
            walkTimer = 0;
            playerContactPos = p.transform.position;
        }
    }
}
