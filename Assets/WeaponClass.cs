using UnityEngine;

public class WeaponClass : MonoBehaviour
{
    public PlayerCtl.WeaponTypes weaponType;
    public string weaponName;
    public float attackPower;
    public PlayerCtl playerCtl;
    public float weaponCoolDownDuration;
    public GameObject bullet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCtl != null)
        {
            transform.position = playerCtl.weaponHand.position;
            transform.rotation = playerCtl.weaponHand.rotation;
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCtl>() != null) 
        {
            playerCtl = other.gameObject.GetComponent<PlayerCtl>(); playerCtl.weapon = this;
        }
    }
}
