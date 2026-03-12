using UnityEngine;

public class ConsumableItem : MonoBehaviour
{
    public float HP, SP;
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
        PlayerHealth.health += HP;
        PlayerHealth.stamina += SP;
        if (useParticles != null) { Instantiate(useParticles, transform.position, Quaternion.identity); }
        Destroy(gameObject);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCtl>() != null)
        {
            if(other.gameObject.GetComponent<PlayerCtl>().Atk3)
            {
                PlayerHealth.Consumable newcons = new PlayerHealth.Consumable();
                newcons.resPath = resPath;
                PlayerHealth.inventory.Add(newcons);
                Destroy(gameObject) ;
            }
        }
    }
}
