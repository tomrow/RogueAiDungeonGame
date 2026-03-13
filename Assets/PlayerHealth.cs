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
    GameObject openMenuSfx;
    GameObject selectMenuSfx;
    GameObject confirmMenuSfx;
    GameObject backMenuSfx;
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
            WeaponClass.Equip(newWeapon);
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
    public static List<Item> inventory = new List<Item>();

    public enum MenuModes
    {
        disabled, main, inventory, item, quit
    }
    public static string MenuText;
    public static string MenuTitle;
    public static MenuModes MenuMode;
    InputAction cursor, select, back, start;
    public bool selectBtn, startBtn, backBtn, oldSelect, oldBack, oldStart;
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
        DontDestroyOnLoad(gameObject);
        openMenuSfx = Resources.Load("sfxEmitters/OpenMenu").GameObject();
        selectMenuSfx = Resources.Load("sfxEmitters/MenuSelect").GameObject();
        confirmMenuSfx = Resources.Load("sfxEmitters/MenuConfirm").GameObject();
        backMenuSfx = Resources.Load("sfxEmitters/MenuBack").GameObject();
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("MenuUpdate");
        //send old input states to old vars, and get new input state
        oldSelect = selectBtn; oldStart = startBtn; oldBack = backBtn;
        oldDpad = dpadDir;
        selectBtn = select.IsPressed(); startBtn = start.IsPressed(); backBtn = back.IsPressed();
        //get up/down
        dpadDir = 0 - Mathf.RoundToInt(Mathf.Clamp(cursor.ReadValue<Vector2>().y, -1, 1));


        //open menu to main if its closed, otherwise close it
        if (JustPressedStart()) { if (MenuMode == MenuModes.disabled) { MenuMode = MenuModes.main; selection = 0; } else { MenuMode = MenuModes.disabled; } }
        Debug.Log(MenuMode);
        switch (MenuMode)
        {
            case MenuModes.inventory:
                int newDir = JustPressedDirection();
                selection += newDir;
                if (selection > PlayerHealth.inventory.Count - 1)
                { selection = PlayerHealth.inventory.Count-1; }
                if (selection < 0) { selection = 0; }
                if (inventorySelectionViewPos > selection) { inventorySelectionViewPos = selection; } // keep cursored over item on-screen
                if (inventorySelectionViewPos < selection + 3) { inventorySelectionViewPos = selection; } // keep cursored over item on-screen
                if (PlayerHealth.inventory.Count > 0)
                {
                    MenuText = "";
                    for(int i = 0;i < (PlayerHealth.inventory.Count<4? PlayerHealth.inventory.Count : 4) ;i++)
                    { MenuText += PlayerHealth.inventory[i].name + "\n"; }
                    
                }
                if (JustPressedSelect())
                {
                    inventorySelectionIndex = selection;
                    MenuMode = MenuModes.item;
                }
                break;
            case MenuModes.item:
                selection += JustPressedDirection();
                selection = Math.Clamp(selection, 0, 2);
                cursorPos = selection;
                MenuTitle = inventory[inventorySelectionIndex].ToString();
                MenuText = "Use\nDrop\nBack";
                if (JustPressedBack()) { MenuMode = MenuModes.inventory; selection = inventorySelectionIndex; } // when b pressed, return to inventory menu, setting the selection cursor back to where it was
                if (JustPressedSelect())
                {
                    switch (selection)
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
                            break;
                    }
                    MenuMode = MenuModes.inventory; selection = inventorySelectionIndex;
                }
                break;
            case MenuModes.main: // This mode consists of a submenu selection menu.
                selection += JustPressedDirection();
                selection = Math.Clamp(selection, 0, 2);
                cursorPos = selection;
                MenuText = "Inventory\nUnequip weapon\nQuit";       // very lazy menu text.
                if (JustPressedBack()) { MenuMode = MenuModes.disabled; }
                if(JustPressedSelect())
                {
                    switch (selection)
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
            case MenuModes.disabled:
                break;
            default:
                break; //go back to main if the submenu is unimplemented


        }


    }

    private int JustPressedDirection()
    {
        if (dpadDir != oldDpad) { Instantiate(selectMenuSfx); return dpadDir; }
        else { return 0; }
    }
    private bool JustPressedBack()
    {
        if (backBtn && !oldBack) { Instantiate(backMenuSfx); return backBtn; }
        else { return false; }
    }
    private bool JustPressedSelect()
    {
        if (selectBtn && !oldSelect) { Instantiate(confirmMenuSfx); return selectBtn; }
        else { return false; }
    }
    private bool JustPressedStart()
    {
        if (startBtn && !oldStart) { Debug.Log("START"); Instantiate(openMenuSfx); return startBtn; }
        else { return false; }
    }
}
