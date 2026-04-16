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
    public static GameObject canvasObj;
    public static int health=40, stamina=10, money=0, level=1;
    public static int baseAtk=10, exp=0, maxHealth=40;
    //GameObject openMenuSfx,selectMenuSfx,confirmMenuSfx, backMenuSfx;
    GameObject levelUpEffect;
    
    public static int[] body = new int[4];
    public string marco()
    { return "polo"; }
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
        disabled, main, inventory, item, quit, status, emptyInventoryMessage, noWeaponMessage
    }
    public static string MenuText, MenuTitle;
    public static MenuModes MenuMode;
    InputAction cursor, select, back, start;
    public bool selectBtn, startBtn, backBtn, oldSelect, oldBack, oldStart;
    int dpadDir, oldDpad;
    public static int selection, inventorySelectionIndex, inventorySelectionViewPos, cursorPos;
    public static bool scrollableUp, scrollableDown;
    List<string> statuses = new List<string>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MenuMode = MenuModes.disabled;
        cursor = InputSystem.actions.FindAction("Navigate");
        select = InputSystem.actions.FindAction("Submit");
        back = InputSystem.actions.FindAction("Cancel");
        start = InputSystem.actions.FindAction("OpenMenu");
        DontDestroyOnLoad(gameObject);
        /*openMenuSfx = Resources.Load("sfxEmitters/OpenMenu").GameObject();
        selectMenuSfx = Resources.Load("sfxEmitters/MenuSelect").GameObject();
        confirmMenuSfx = Resources.Load("sfxEmitters/MenuConfirm").GameObject();
        backMenuSfx = Resources.Load("sfxEmitters/MenuBack").GameObject();*/
        levelUpEffect = Resources.Load("LevelUpEffect").GameObject();
        statuses.Add("Money:");
        statuses.Add("BaseAtk:");
        statuses.Add("Weapon:");
        statuses.Add("exp");
        canvasObj = GameObject.Find("Canvas");
    }
    // Update is called once per frame
    void Update()
    {
        //clamp body part values in bounds
        PlayerHealth.body[0] = Math.Clamp(PlayerHealth.body[0], 1, 9); PlayerHealth.body[1] = Math.Clamp(PlayerHealth.body[1], 1, 9); PlayerHealth.body[2] = Math.Clamp(PlayerHealth.body[2], 1, 9); PlayerHealth.body[3] = Math.Clamp(PlayerHealth.body[3], 1, 9);
        //send old input states to old vars, and get new input state
        oldSelect = selectBtn; oldStart = startBtn; oldBack = backBtn;
        oldDpad = dpadDir;
        selectBtn = select.IsPressed(); startBtn = start.IsPressed(); backBtn = back.IsPressed();
        //get up/down
        dpadDir = 0 - Mathf.RoundToInt(Mathf.Clamp(cursor.ReadValue<Vector2>().y, -1, 1));
        //level up when exp is maxed out for that level
        UpdateLevelExp();

        //open menu to main if its closed, otherwise close it
        if (JustPressedStart() && PlayerInputAggregator.inputEnabled) { if (MenuMode == MenuModes.disabled) { MenuMode = MenuModes.main; selection = 0; } else { MenuMode = MenuModes.disabled; } }
        //Debug.Log(MenuMode);
        switch (MenuMode)
        {
            case MenuModes.noWeaponMessage:
                MenuTitle = "Inventory";
                MenuText = "You cannot dequip your arm cannon.";
                if (JustPressedBack()) { MenuMode = MenuModes.main; selection = 0; } // when b pressed, return to inventory menu, setting the selection cursor back to where it was
                break;
            case MenuModes.emptyInventoryMessage:
                MenuTitle = "Inventory";
                if (PlayerHealth.inventory.Count > 0)
                { MenuMode = MenuModes.main; selection = 0; break; }
                MenuText = "Your inventory is empty.";
                if (JustPressedBack()) { MenuMode = MenuModes.main; selection = 0; } // when b pressed, return to inventory menu, setting the selection cursor back to where it was
                break;
            case MenuModes.status:
                MenuTitle = "Status";
                statuses[0] = "Money: "+ PlayerHealth.money.ToString();
                statuses[1] = "Base ATK: " + PlayerHealth.baseAtk.ToString();
                statuses[2] = "WPN: " + (PlayerHealth.thisPlayer.weapon != null ? (PlayerHealth.thisPlayer.weapon.weaponName + ";" + PlayerHealth.thisPlayer.weapon.attackPower.ToString()) : "Arm Cannon");
                statuses[3] = "LV." + PlayerHealth.level.ToString() + " EXP " + PlayerHealth.exp.ToString() ;
                MenuText = statuses[0] + "\n" + statuses[1] + "\n" + statuses[2] + "\n" + statuses[3];
                if (JustPressedBack()) { MenuMode = MenuModes.main; selection = 0; } // when b pressed, return to inventory menu, setting the selection cursor back to where it was
                break;
            case MenuModes.inventory:
                selection += JustPressedDirection();  //get user input for selection
                if (selection > PlayerHealth.inventory.Count - 1)
                { selection = PlayerHealth.inventory.Count-1; }
                if (selection < 0) { selection = 0; }
                if (selection < inventorySelectionViewPos) { inventorySelectionViewPos--; }
                if (selection > inventorySelectionViewPos+3) { inventorySelectionViewPos++; }
                cursorPos = selection + inventorySelectionViewPos; //set cursor graphic location
                //Debug.Log(newDir); Debug.Log("cursorPos "+ cursorPos.ToString() + " selection " + selection.ToString());
                if (PlayerHealth.inventory.Count > 0)
                { //populate menu text with list items
                    MenuText = "";
                    for (int i = 0; i < (PlayerHealth.inventory.Count < 4 ? PlayerHealth.inventory.Count : 4); i++)
                    { MenuText += PlayerHealth.inventory[i].name + "\n"; }

                }
                else { MenuMode = MenuModes.emptyInventoryMessage; } //show error when inventory is empty
                if (JustPressedBack()) { MenuMode = MenuModes.main;} //press b to go back
                
                if (JustPressedSelect())
                {
                    inventorySelectionIndex = selection;//press a to perform action on selection
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
                selection = Math.Clamp(selection, 0, 3);
                cursorPos = selection;
                MenuText = "Status\nInventory\nUnequip weapon\nQuit";       // very lazy menu text.
                if (JustPressedBack()) { MenuMode = MenuModes.disabled; }
                if(JustPressedSelect())
                {
                    switch (selection)
                    {
                        case 0:
                            MenuMode = MenuModes.status; break;
                        case 1:
                            MenuMode = MenuModes.inventory; break;
                        case 2:
                            if (PlayerHealth.thisPlayer.weapon != null)
                            { WeaponClass.Collect(PlayerHealth.thisPlayer.weapon); thisPlayer.weapon = null; thisPlayer.animCtl.GetComponent<Animator>().SetTrigger("noBattleStance"); }
                            else
                            { MenuMode = MenuModes.noWeaponMessage; }
                            break;
                        default:
                            MenuMode = MenuModes.disabled; break; //quit will not exit game for this demo
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

    private void UpdateLevelExp()
    {
        int maxExpForLevel = level * level * 4;
        //PlayerHealth.baseAtk = 10 + (PlayerHealth.level * (PlayerHealth.level / 25)); PlayerHealth.maxHealth = 40 + (PlayerHealth.level * (PlayerHealth.level / 45));
        PlayerHealth.baseAtk = 10 + (PlayerHealth.level - 1); PlayerHealth.maxHealth = 40 + ((PlayerHealth.level - 1) * 2);
        if (exp>maxExpForLevel)
        { 
            PlayerHealth.level++;
            PlayerHealth.baseAtk = 10 + (PlayerHealth.level-1); PlayerHealth.maxHealth = 40 + ((PlayerHealth.level-1) * 2);
            Debug.Log("Level Up to "+PlayerHealth.level.ToString());
            exp = exp - maxExpForLevel;
            Instantiate(levelUpEffect, thisPlayer.transform.position, Quaternion.identity);
        }
    }
    public static void AwardXP(int amt)
    {
        exp += amt;
    }
    private int JustPressedDirection()
    {
        if ((dpadDir != oldDpad)&& dpadDir!=0) { Instantiate(SoundEffectStorage.selectMenuSfx); return dpadDir; }
        else { return 0; }
    }
    private bool JustPressedBack()
    {
        if (backBtn && !oldBack) { Instantiate(SoundEffectStorage.backMenuSfx); return backBtn; }
        else { return false; }
    }
    private bool JustPressedSelect()
    {
        if (selectBtn && !oldSelect) { GC.Collect(GC.MaxGeneration); Resources.UnloadUnusedAssets(); Instantiate(SoundEffectStorage.confirmMenuSfx); return selectBtn; }
        else { return false; }
    }
    private bool JustPressedStart()
    {
        if (startBtn && !oldStart) { Debug.Log("START"); Instantiate(SoundEffectStorage.openMenuSfx); return startBtn; }
        else { return false; }
    }
    public static void EmergencyHeal()
    { for(int i=0;i<PlayerHealth.inventory.Count;i++)
        {
            if (PlayerHealth.inventory[i] is PlayerHealth.Consumable)
            {
                PlayerHealth.inventory[i].Deposit();
                PlayerHealth.inventory.RemoveAt(i);
                break;
            }
        }
    }
}
