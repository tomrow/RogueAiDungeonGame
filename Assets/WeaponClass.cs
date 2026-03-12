using UnityEngine;

public class WeaponClass : MonoBehaviour
{
    public PlayerCtl.WeaponTypes weaponType;
    public string weaponName;
    public float attackPower;
    public PlayerCtl playerCtl;
    public float weaponCoolDownDuration;
    public GameObject bullet;
    public GameObject superBullet;
    public GameObject firesfx, superFireSfx;
    public bool hitScan, superHitScan;
    public string resPath;
    public Vector3 rotationForHand;
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
            transform.Rotate(rotationForHand);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCtl>() != null) //if the touching trigger is a player and weapon is not equipped
        {
            Collect();
        }
    }
    public void Collect()
    {
        if (playerCtl == null)
        {
            PlayerHealth.Weapon newWeaponInv = new PlayerHealth.Weapon();
            newWeaponInv.attackPower = attackPower;                           //build inventory obj
            newWeaponInv.name = weaponName;
            newWeaponInv.resPath = resPath;
            newWeaponInv.weaponCoolDownDuration = weaponCoolDownDuration;
            PlayerHealth.inventory.Add(newWeaponInv);                         //add it to list
            Destroy(gameObject);         //remove this gameObject from the world
        }
    }

    public static void Collect(WeaponClass weapon)
    {
        if (weapon.playerCtl == null)
        {
            PlayerHealth.Weapon newWeaponInv = new PlayerHealth.Weapon();
            newWeaponInv.attackPower = weapon.attackPower;                           //build inventory obj
            newWeaponInv.name = weapon.weaponName;
            newWeaponInv.resPath = weapon.resPath;
            newWeaponInv.weaponCoolDownDuration = weapon.weaponCoolDownDuration;
            PlayerHealth.inventory.Add(newWeaponInv);                         //add it to list
            Destroy(weapon.gameObject);         //remove this gameObject from the world
        }
    }
    private void Use(GameObject player)
    {
        if (player.GetComponent<PlayerCtl>() != null) 
        {
            playerCtl = player.GetComponent<PlayerCtl>(); playerCtl.weapon = this;
        }
    }
}
