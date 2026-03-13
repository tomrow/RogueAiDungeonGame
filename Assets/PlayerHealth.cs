using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

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
            inventory.Remove(this);
            return Instantiate(Resources.Load(this.resPath).GameObject(), thisPlayer.transform.position, Quaternion.identity);
        }
        public virtual GameObject Deposit()
        {
            return Instantiate(Resources.Load(this.resPath).GameObject(), thisPlayer.transform.position, Quaternion.identity);
        }
        public override string ToString()
        { return this.name; }
    }
    public class Weapon : Item 
    {
        public float attackPower;
        public float weaponCoolDownDuration;
        public override GameObject Drop()
        {
            GameObject weapon = Instantiate(Resources.Load(this.resPath).GameObject(), thisPlayer.transform.position, Quaternion.identity);
            WeaponClass param = weapon.GetComponent<WeaponClass>();
            param.weaponName = this.name;
            param.attackPower = this.attackPower;
            param.weaponCoolDownDuration = this.weaponCoolDownDuration;
            param.resPath = this.resPath;
            return weapon;
        }
        public override GameObject Deposit()
        {
            if (thisPlayer.weapon != null)
            { WeaponClass.Collect(thisPlayer.weapon); thisPlayer.weapon = null; }
            WeaponClass newWeapon = Instantiate(Resources.Load(this.resPath).GameObject(), thisPlayer.transform.position, Quaternion.identity).GetComponent<WeaponClass>();
            newWeapon.weaponName = this.name;
            newWeapon.weaponCoolDownDuration = this.weaponCoolDownDuration;
            newWeapon.attackPower = this.attackPower;
            return newWeapon.gameObject;
        }
        public override string ToString()
        {
            return this.name + " ATK " + attackPower.ToString();
        }
    }
    public class Consumable : Item
    {
        public override GameObject Deposit()
        {
            ConsumableItem newFood = Instantiate(Resources.Load(this.resPath).GameObject(), thisPlayer.transform.position, Quaternion.identity).GetComponent<ConsumableItem>();
            newFood.Consume();
            return newFood.gameObject;
        }
    }
    public static List<Item> inventory;

    public enum MenuModes
    {
        disabled, main, inventory, item, quit
    }
    public static string MenuText;
    public static string MenuTitle;
    public static MenuModes MenuMode;
    InputAction cursor, select, back, start;
    bool selectBtn, startBtn, backBtn, oldSelect, oldBack, oldStart;
    int dpadDir, oldDpad;
    public static int selection;
    public static int inventorySelectionIndex;
    public static int inventorySelectionViewPos;
    public static bool scrollableUp, scrollableDown;
    public static int cursorPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MenuMode = MenuModes.disabled;
        cursor = InputSystem.actions.FindAction("Navigate");
        select = InputSystem.actions.FindAction("Submit");
        back = InputSystem.actions.FindAction("Cancel");
        start = InputSystem.actions.FindAction("OpenMenu");
    }
    // Update is called once per frame
    void Update()
    {
        //send old input states to old vars, and get new input state
        oldSelect = selectBtn; oldStart = startBtn; oldBack = backBtn;
        selectBtn = select.IsPressed(); startBtn = start.IsPressed(); backBtn = back.IsPressed();
        //get up/down
        dpadDir = 0 - Mathf.RoundToInt(Mathf.Clamp(cursor.ReadValue<Vector2>().y, -1, 1));


        //open menu to main if its closed, otherwise close it
        if (JustPressedStart()){if (MenuMode != MenuModes.disabled){MenuMode = MenuModes.main;selection = 0; }else {MenuMode = MenuModes.disabled;}}

        switch (MenuMode)
        {
            case MenuModes.inventory:
                selection += JustPressedDirection();
                Math.Clamp(selection, 0, inventory.Count - 1);
                if(inventorySelectionViewPos > selection) { inventorySelectionViewPos = selection; } // keep cursored over item on-screen
                if (inventorySelectionViewPos < selection+3) { inventorySelectionViewPos = selection; } // keep cursored over item on-screen
                if (JustPressedSelect())
                {
                    inventorySelectionIndex = selection;
                    MenuMode = MenuModes.item;
                }
                break;
            case MenuModes.item:
                selection += JustPressedDirection();
                Math.Clamp(selection, 0, 2);
                cursorPos = selection;
                MenuTitle = inventory[inventorySelectionIndex].ToString();
                MenuText = "Use\nDrop\nBack";
                if (JustPressedBack()) { MenuMode = MenuModes.inventory; selection = inventorySelectionIndex; } // when b pressed, return to inventory menu, setting the selection cursor back to where it was
                if (JustPressedSelect())
                {
                    switch(selection)
                    {   
                        case 0: //use
                            inventory[inventorySelectionIndex].Deposit();
                            inventory.RemoveAt(inventorySelectionIndex);
                            break;
                        case 1: //drop
                            inventory[inventorySelectionIndex].Drop();
                            inventory.RemoveAt(inventorySelectionIndex);
                            break;
                        default:
                            MenuMode = MenuModes.inventory; selection = inventorySelectionIndex; break;
                    }
                }
                break;
            case MenuModes.main: // This mode consists of a submenu selection menu.
                selection += JustPressedDirection();
                Math.Clamp(selection, 0, 1);
                cursorPos = selection;
                MenuText = "Inventory\nUnequip weapon\nQuit";       // very lazy menu text.
                if (JustPressedBack()) { MenuMode = MenuModes.disabled; }
                else
                {
                    switch(selection)
                    {
                        case 0:
                            MenuMode = MenuModes.inventory; break;
                        case 1:
                            WeaponClass.Collect(thisPlayer.weapon); thisPlayer.weapon = null; break;
                        default:
                            MenuMode = MenuModes.quit; break;
                    }
                }
                if (MenuMode != MenuModes.main) { selection = 0; }
                break;

            default:
                MenuMode = MenuModes.main; break; //go back to main if the submenu is unimplemented


        }


    }

    private int JustPressedDirection()
    {
        if (dpadDir != oldDpad) { return dpadDir; }
        else { return 0; }
    }
    private bool JustPressedBack()
    {
        if (backBtn != oldBack) { return backBtn; }
        else { return false; }
    }
    private bool JustPressedSelect()
    {
        if (selectBtn != oldSelect) { return selectBtn; }
        else { return false ; }
    }
    private bool JustPressedStart()
    {
        if (startBtn != oldStart) { return startBtn; }
        else { return false; }
    }
}
