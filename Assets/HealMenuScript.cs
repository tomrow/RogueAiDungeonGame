using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class HealMenuScript : MonoBehaviour
{
    GameObject textOverlay;
    InputAction cursor, select, back, start;
    public bool selectBtn, startBtn, backBtn, oldSelect, oldBack, oldStart;
    int dpadDir, oldDpad, mode;
    float timer;
    Text dialogueTextGrp;
    bool selection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueTextGrp = transform.GetChild(0).gameObject.GetComponent<Text>();
        PlayerInputAggregator.inputEnabled = false;
        cursor = InputSystem.actions.FindAction("Navigate");
        select = InputSystem.actions.FindAction("Submit");
        back = InputSystem.actions.FindAction("Cancel");
        start = InputSystem.actions.FindAction("OpenMenu");
        textOverlay = Resources.Load("AttackHudAnim").GameObject();
        dialogueTextGrp.enabled = false;
        transform.localPosition = Vector3.zero;
        transform.localScale = (Vector3.up + Vector3.forward) * 2f;
        PlayerHealth.canvasObj = transform.Find("/Canvas").gameObject;
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
                if (PlayerHealth.health >= PlayerHealth.maxHealth)
                {
                    Instantiate(SoundEffectStorage.errorSfx);
                    GameObject h = Instantiate(Resources.Load("AttackHudAnim").GameObject(), PlayerHealth.canvasObj.transform);
                    h.GetComponent<AttackHudAnim>().subject = PlayerHealth.thisPlayer.transform; h.transform.position = Camera.main.WorldToScreenPoint(PlayerHealth.thisPlayer.transform.position);
                    h.GetComponent<Text>().text = "Your robot does not need repair"; mode = 2;break;
                }
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

    private void ShopLogic()
    {
        dialogueTextGrp.enabled = true;
        dialogueTextGrp.text = "Do you want to repair your robot?\nThis costs 3 coins.\n\n";
        dialogueTextGrp.text += selection ? ">Yes<\nNo" : "Yes\n>No<";
        dialogueTextGrp.text += "\n(A): Confirm      (B): Decline";
        if(JustPressedDirection()!=0) { selection = !selection; } //there's only two options so taking into account the specific direction is not necessary
        if(JustPressedBack()) { mode = 2; }
        if (PlayerHealth.health >= PlayerHealth.maxHealth)
        {
            Instantiate(SoundEffectStorage.errorSfx);
            GameObject h = Instantiate(Resources.Load("AttackHudAnim").GameObject(), PlayerHealth.canvasObj.transform, false);
            h.GetComponent<AttackHudAnim>().subject = PlayerHealth.thisPlayer.transform; h.transform.position = Camera.main.WorldToScreenPoint(PlayerHealth.thisPlayer.transform.position);
            h.GetComponent<Text>().text = "Your robot does not need repair"; mode = 2;
        }
        else if (JustPressedSelect() && selection )
        {
            if(PlayerHealth.money >= 3) { PlayerHealth.money -= 3; PlayerHealth.health = Mathf.Clamp(PlayerHealth.health, PlayerHealth.maxHealth, 65535); mode = 2; }
            else
            {
                Instantiate(SoundEffectStorage.errorSfx);
                GameObject h = Instantiate(Resources.Load("AttackHudAnim").GameObject(), PlayerHealth.canvasObj.transform, false);
                h.GetComponent<AttackHudAnim>().subject = PlayerHealth.thisPlayer.transform; h.transform.position = Camera.main.WorldToScreenPoint(PlayerHealth.thisPlayer.transform.position);
                h.GetComponent<Text>().text = "Insufficient funds"; mode = 2;
            }
        }
        
        if (JustPressedSelect() && !selection) { mode = 2; }
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
