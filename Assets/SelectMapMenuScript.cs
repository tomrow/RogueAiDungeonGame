using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class SelectMapMenuScript : MonoBehaviour
{
    GameObject textOverlay;
    InputAction cursor, select, back, start;
    public bool selectBtn, startBtn, backBtn, oldSelect, oldBack, oldStart;
    int dpadDir, oldDpad, mode;
    float timer;
    Text dialogueTextGrp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public class DestinationItem
    {
        public string mapPath;
        public string description;
    }
    string[] window = new string[3];
    public List<DestinationItem> shelf = new List<DestinationItem>();
    public void AddShopItem(string mapPath, string description)
    {
        DestinationItem newitem = new DestinationItem();
        newitem.mapPath = mapPath;
        newitem.description = description;
        
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
        AddShopItem("tutorialWorld", "Tutorial World\nLearn how to play here.");
        AddShopItem("MeshVerTemplate", "Caves 1\nThe first floor of a mine for rare power crystals. The unknown energy has made the rocks mobile and has attracted hostile creatures. ");
        AddShopItem("titlescreen", "Return to Title Screen ");
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
        window[0] = "SELECT LOCATION: ";                                               //Title
        window[1] = (selection > 0 ? "[^] " : "  ")+ shelf[selection].description; //hide if already at top                                                  //Scroll indicators
        window[2] = (selection < shelf.Count - 1 ? "[v]" : " ") + " - (A) Go (B) Decline"; //hide if already at bottom
        
        selection += JustPressedDirection();
        selection = (int)Mathf.Clamp(selection, 0, shelf.Count - 1);
        if (JustPressedSelect())
        {
            FadeOverlay f = transform.Find("/Canvas/FadeOverlay").gameObject.GetComponent<FadeOverlay>();
            f.sceneForTransfer = shelf[selection].mapPath;
            f.mode = FadeOverlay.Transitions.FadeOut; mode++;
            HubWorldElevator e = transform.Find("/hubworld2/elevator").gameObject.GetComponent<HubWorldElevator>();
            e.StartElevator();
        }
        dialogueTextGrp.text = "";
        foreach (string i in window)
        { dialogueTextGrp.text += i; dialogueTextGrp.text += "\n"; }
        if (JustPressedBack())
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
        if (selectBtn && !oldSelect) { GC.Collect(GC.MaxGeneration); Resources.UnloadUnusedAssets(); return selectBtn; }
        else { return false; }
    }
}
