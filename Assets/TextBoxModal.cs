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
    int mode = 0;
    float timer = 0;
    public string[] text;
    InputAction ok;
    bool selectBtn, oldSelect;
    GameObject confirmMenuSfx;
    int dialogueLine=0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueTextGrp = transform.GetChild(0).gameObject.GetComponent<Text>();
        dialogueTextGrp.enabled = false;
        transform.localScale = Vector3.up + Vector3.forward;
        PlayerInputAggregator.inputEnabled = false;
        dialogueTextGrp.text = text[0];
        ok = InputSystem.actions.FindAction("Submit");
        confirmMenuSfx = Resources.Load("sfxEmitters/TextBoxAdvance").GameObject();
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
                timer += Time.deltaTime;
                transform.localScale = Vector3.Lerp(Vector3.up, Vector3.one, timer)*2;
                if (timer > 1) { mode++; }
                break;
            case 1:
                timer = 0;
                if (JustPressedSelect()) { dialogueLine++; }
                dialogueTextGrp.enabled = false;
                if (dialogueLine >= text.Length)
                {
                    dialogueLine = text.Length; //probably wont be needed but ill put it here for safety
                    mode++;
                    dialogueTextGrp.enabled = false;
                    break;
                }
                dialogueTextGrp.text += text[dialogueLine];
                break;
            default:
                timer += Time.deltaTime;
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.up, timer) * 2;
                if (timer > 1) { PlayerInputAggregator.inputEnabled = true; Destroy(gameObject); }
                break;
        }
    }
}
