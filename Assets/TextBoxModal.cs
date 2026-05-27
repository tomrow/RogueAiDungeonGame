using JetBrains.Annotations;
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TextBoxModal : MonoBehaviour
{
    Text dialogueTextGrp;
    public int mode = 0;
    float timer = 0;
    [TextArea(5,5)]public string[] text;
    InputAction ok;
    bool selectBtn, oldSelect;
    GameObject confirmMenuSfx;
    public int dialogueLine=0;
    public bool autoAdvanceText;
    public float advanceInterval;
    float advTimer = 0;
    public int stopAutoAdvanceOnThisPage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueTextGrp = transform.GetChild(0).gameObject.GetComponent<Text>();
        dialogueTextGrp.enabled = false;
        transform.localScale = (Vector3.up + Vector3.forward) * 2f;
        PlayerInputAggregator.inputEnabled = false;
        try { dialogueTextGrp.text = text[0]; } catch { Destroy(gameObject); }
        ok = InputSystem.actions.FindAction("Submit");
        confirmMenuSfx = Resources.Load("sfxEmitters/TextBoxAdvance").GameObject();
        transform.position = Vector3.zero;
        transform.localPosition = Vector3.zero;
        //GetComponent<RectTransform>().position = Vector3.zero;
    }
    private bool JustPressedSelect()
    {
        if (selectBtn && !oldSelect) { GC.Collect(GC.MaxGeneration); Resources.UnloadUnusedAssets(); Instantiate(confirmMenuSfx); return selectBtn; }
        else { return false; }
    }
    // Update is called once per frame
    void Update()
    {
        oldSelect = selectBtn; selectBtn = ok.IsPressed();
        switch (mode)
        {
            case 0:
                timer += Time.deltaTime * 3;
                transform.localScale = Vector3.Lerp(Vector3.up, Vector3.one, timer)*2;
                if (timer > 1) { mode++; }
                break;
            case 1:
                timer = 0;dialogueTextGrp.enabled = true;
                if (autoAdvanceText) { advTimer += Time.deltaTime; }
                if(dialogueLine>=stopAutoAdvanceOnThisPage)
                { autoAdvanceText = false; }
                //if (dialogueLine == 0) { dialogueTextGrp.text = text[dialogueLine]; }
                if ((!autoAdvanceText && JustPressedSelect()) || (autoAdvanceText && (advTimer>advanceInterval))) 
                {
                    advTimer = 0;
                    dialogueLine++; 
                    if (dialogueLine >= text.Length)
                    {
                        dialogueLine = text.Length; //probably wont be needed but ill put it here for safety
                        mode++;
                        dialogueTextGrp.enabled = false;
                        break;
                    }
                    dialogueTextGrp.text = text[dialogueLine];
                }
                
                
                break;
            default:
                dialogueTextGrp.enabled = false;
                timer += Time.deltaTime * 3;
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.up, timer) * 2;
                if (timer > 1) { PlayerInputAggregator.inputEnabled = true; Destroy(gameObject); }
                break;
        }
    }
}
