using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StoreFront : MonoBehaviour
{
    GameObject textOverlay;
    InputAction cursor, select, back, start;
    public bool selectBtn, startBtn, backBtn, oldSelect, oldBack, oldStart;
    int dpadDir, oldDpad, mode;
    float timer;
    Text dialogueTextGrp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public class ShopItem
    {
        public PlayerHealth.Item merch;
        public string description;
        public string description2;
        public int price;
        public string itemType;
        public string effect;
    }
    string[] window = new string[8];
    public List<ShopItem> shelf = new List<ShopItem>();
    public void AddShopItem(PlayerHealth.Item merch, string description, string description2, int price, string itemType, string effect)
    {
        ShopItem newitem = new ShopItem();
        newitem.merch = merch;
        newitem.description = description;
        newitem.description2 = description2;
        newitem.price = price;
        newitem.itemType = itemType;
        newitem.effect = effect;
        shelf.Add(newitem);
    }
    int selection;
    void Start()
    {
        dialogueTextGrp = transform.GetChild(0).gameObject.GetComponent<Text>();
        PlayerInputAggregator.inputEnabled = false;
        cursor = InputSystem.actions.FindAction("Navigate");
        select = InputSystem.actions.FindAction("Submit");
        back = InputSystem.actions.FindAction("Cancel");
        start = InputSystem.actions.FindAction("OpenMenu");
        textOverlay = Resources.Load("AttackHudAnim").GameObject();
        PopulateShop();
        dialogueTextGrp.enabled = false;
        transform.localPosition = Vector3.zero;
        transform.localScale = (Vector3.up + Vector3.forward) * 2f;
    }

    private void PopulateShop()
    {
        AddWeaponToShop(15, "Blaster", "weapons/laserGun", 0.4f, "Compact laser blaster that", "deals moderate damage.", "Type: Pistol", 100);
        AddWeaponToShop(10, "Sword", "weapons/sword", 0.2f, "A long and durable metal", "blade.", "Type: Sword", 150);
        AddConsumableToShop("Heals 20HP.", "HealthPack", "HealthPack", "A big green capsule with a", "repair kit inside.", "Consumable Item", 20);
        AddWeaponToShop(10, "Launcher", "weapons/bazooka", 0.6f, "A rocket launcher with", "dampened recoil.", "Type: Heavy Gun", 150);
        //old debug weapons
        AddWeaponToShop(5, "Cork gun", "weapons/corkgun", 0.1f, "Simple gun that launches cork", "cylinders.", "Type: Pistol", 100);
        AddWeaponToShop(10, "Baguette", "weapons/baguette", 0.1f, "A long loaf of bread that you", "can use to strike things.", "Type: Sword", 150);
        
    }

    private void AddWeaponToShop(float atk, string title, string resPath, float cooldown, string description, string description2, string wtype, int price)
    {
        PlayerHealth.Weapon newWeaponInv = new PlayerHealth.Weapon();
        newWeaponInv.attackPower = atk;                           //build inventory obj
        newWeaponInv.name = title;
        newWeaponInv.resPath = resPath;
        newWeaponInv.weaponCoolDownDuration = cooldown;
        AddShopItem(newWeaponInv, description, description2, price, wtype, "Weapon power: " + atk.ToString());
    }
    private void AddConsumableToShop(string effect, string title, string resPath, string description, string description2, string wtype, int price)
    {
        PlayerHealth.Consumable newcons = new PlayerHealth.Consumable();
        newcons.resPath = resPath;
        newcons.name = title;
        AddShopItem(newcons, description, description2, price, wtype, effect);
    }

    // Update is called once per frame
    void Update()
    {
        oldSelect = selectBtn; oldBack = backBtn; oldDpad = dpadDir;
        selectBtn = select.IsPressed(); backBtn = back.IsPressed();
        //get up/down
        dpadDir = 0 - Mathf.RoundToInt(Mathf.Clamp(cursor.ReadValue<Vector2>().y, -1, 1));
        switch (mode)
        {
            case 0:
                PlayerInputAggregator.inputEnabled = false;
                timer += Time.deltaTime * 3;
                transform.localScale = Vector3.Lerp(Vector3.up, Vector3.one, timer) * 2;
                if (timer > 1) { mode++; }
                break;
            case 1:
                ShopLogic();
                PlayerInputAggregator.inputEnabled = false;
                break;
            default:
                dialogueTextGrp.enabled = false;
                timer += Time.deltaTime * 3;
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.up, timer) * 2;
                if (timer > 1) { PlayerInputAggregator.inputEnabled = true; Destroy(gameObject); }
                break;
        }
    }
    void ShopLogic()
    {
        dialogueTextGrp.enabled = true;
        window[0] = "STORE -- Your money: " + PlayerHealth.money.ToString();                                               //Title
        window[1] = (selection > 0 ? "[^] " : "  ") + shelf[selection].merch.name + " (Price: " + shelf[selection].price + ")"; //hide if already at top                                                  //Scroll indicators
        window[7] = (selection < shelf.Count - 1 ? "[v]" : " ") + " - (A) Buy (B) Leave"; //hide if already at bottom
        window[2] = "    ";
        window[3] = "    " + shelf[selection].description;
        window[4] = "    " + shelf[selection].description2;
        window[5] = "    Type: " + shelf[selection].itemType;
        window[6] = "    " + shelf[selection].effect;
        selection += JustPressedDirection();
        selection = (int)Mathf.Clamp(selection, 0, shelf.Count-1);
        if(JustPressedSelect())
        {
            if (PlayerHealth.money < shelf[selection].price)
            {
                Instantiate(SoundEffectStorage.errorSfx);
                GameObject h = Instantiate(Resources.Load("AttackHudAnim").GameObject(), PlayerHealth.canvasObj.transform, false);
                h.GetComponent<AttackHudAnim>().subject = PlayerHealth.thisPlayer.transform; h.transform.position = Camera.main.WorldToScreenPoint(PlayerHealth.thisPlayer.transform.position);
                h.GetComponent<Text>().text = "Insufficient funds";
            }
            else 
            {
                PlayerHealth.money -= shelf[selection].price;
                PlayerHealth.inventory.Add(shelf[selection].merch);
                Instantiate(SoundEffectStorage.confirmMenuSfx);
                GameObject h = Instantiate(Resources.Load("AttackHudAnim").GameObject(), PlayerHealth.canvasObj.transform, false);
                h.GetComponent<AttackHudAnim>().subject = PlayerHealth.thisPlayer.transform; h.transform.position = Camera.main.WorldToScreenPoint(PlayerHealth.thisPlayer.transform.position);
                h.GetComponent<Text>().text = "Purchase successful";
            }
        }
        dialogueTextGrp.text = "";
        foreach (string i in window)
        { dialogueTextGrp.text += i; dialogueTextGrp.text += "\n"; }
        if(JustPressedBack())
        { mode++; }
    }
    private int JustPressedDirection()
    {
        if ((dpadDir != oldDpad) && dpadDir != 0) { Instantiate(SoundEffectStorage.selectMenuSfx); return dpadDir; }
        else { return 0; }
    }
    private bool JustPressedBack()
    {
        if (backBtn && !oldBack) { Instantiate(SoundEffectStorage.backMenuSfx); return backBtn; }
        else { return false; }
    }
    private bool JustPressedSelect()
    {
        if (selectBtn && !oldSelect) { GC.Collect(GC.MaxGeneration); Resources.UnloadUnusedAssets();  return selectBtn; }
        else { return false; }
    }
}