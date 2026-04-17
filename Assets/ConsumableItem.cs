using System;
using UnityEngine;

public class ConsumableItem : MonoBehaviour
{
    public int HP, SP;
    public GameObject useParticles;
    public string resPath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Consume() 
    {
        if (HP > 0 && PlayerHealth.health >= PlayerHealth.maxHealth)
        { CollectThis(); return; }
        if (SP > 0 && PlayerHealth.stamina >= PlayerHealth.maxStamina)
        { CollectThis(); return; }
        PlayerHealth.health += HP;
        PlayerHealth.stamina += SP;
        if (useParticles != null) { Instantiate(useParticles, transform.position, Quaternion.identity); }
        Destroy(gameObject);
    }

    private void CollectThis()
    {
        PlayerHealth.Consumable newcons = new PlayerHealth.Consumable();
        newcons.resPath = resPath;
        newcons.name = gameObject.name;
        PlayerHealth.inventory.Add(newcons);
        Destroy(this.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCtl>() != null)
        {
            if(other.gameObject.GetComponent<PlayerCtl>().Atk3)
            {
                CollectThis();
            }
        }
    }
}
