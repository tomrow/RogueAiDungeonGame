using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerCtl thisPlayer;
    public static float health, stamina;

    public class Item
    {
        public string resPath;
        public string name;
        
        public virtual GameObject Drop()
        {
            return Instantiate(Resources.Load(this.resPath).GameObject(), thisPlayer.transform.position, Quaternion.identity);
        }
        public virtual GameObject Deposit()
        {
            return Instantiate(Resources.Load(this.resPath).GameObject(), thisPlayer.transform.position, Quaternion.identity);
        }
    }
    public class Weapon : Item 
    {
        public float attackPower;
        public float weaponCoolDownDuration;
        public override GameObject Deposit()
        {
            WeaponClass newWeapon = Instantiate(Resources.Load(this.resPath).GameObject(), thisPlayer.transform.position, Quaternion.identity).GetComponent<WeaponClass>();
            newWeapon.weaponName = this.name;
            newWeapon.weaponCoolDownDuration = this.weaponCoolDownDuration;
            newWeapon.attackPower = this.attackPower;
            return newWeapon.gameObject;
        }
    }
    public class Consumable : Item
    {
        public float attackPower;
        public float weaponCoolDownDuration;
        public override GameObject Deposit()
        {
            ConsumableItem newFood = Instantiate(Resources.Load(this.resPath).GameObject(), thisPlayer.transform.position, Quaternion.identity).GetComponent<ConsumableItem>();
            newFood.Consume();
            return newFood.gameObject;
        }
    }
    public static List<Item> inventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
