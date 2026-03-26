using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterCreatorLogic : MonoBehaviour
{
    public RectTransform cursorSq;
    InputAction cursor, select, back, start;
    public bool selectBtn, startBtn, backBtn, oldSelect, oldBack, oldStart;
    int dpadDir, oldDpad, horizDir, oldHoriz;
    GameObject openMenuSfx, selectMenuSfx, confirmMenuSfx, backMenuSfx;
    RectTransform selectionPosition;
    public int category;
    public List<Transform> categories; //head, arms, torso, legs
    int justpressedh;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cursor = InputSystem.actions.FindAction("Navigate");
        select = InputSystem.actions.FindAction("Submit");
        back = InputSystem.actions.FindAction("Cancel");
        start = InputSystem.actions.FindAction("OpenMenu");
        openMenuSfx = Resources.Load("sfxEmitters/OpenMenu").GameObject();
        selectMenuSfx = Resources.Load("sfxEmitters/MenuSelect").GameObject();
        confirmMenuSfx = Resources.Load("sfxEmitters/MenuConfirm").GameObject();
        backMenuSfx = Resources.Load("sfxEmitters/MenuBack").GameObject();
    }
    private int JustPressedDirection()
    {
        if ((dpadDir != oldDpad) && dpadDir != 0) { Instantiate(selectMenuSfx); return dpadDir; }
        else { return 0; }
    }
    private int JustPressedDirectionHorizontal()
    {
        Debug.Log(horizDir);
        if ((horizDir != oldHoriz) && horizDir != 0) 
        { 
            Instantiate(selectMenuSfx);
            return horizDir; 
        }
        else { return 0; }
    }
    private bool JustPressedBack()
    {
        if (backBtn && !oldBack) { Instantiate(backMenuSfx); return backBtn; }
        else { return false; }
    }
    private bool JustPressedSelect()
    {
        if (selectBtn && !oldSelect) { GC.Collect(GC.MaxGeneration); Resources.UnloadUnusedAssets(); Instantiate(confirmMenuSfx); return selectBtn; }
        else { return false; }
    }
    private bool JustPressedStart()
    {
        if (startBtn && !oldStart) { Debug.Log("START"); Instantiate(openMenuSfx); return startBtn; }
        else { return false; }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        oldSelect = selectBtn; oldStart = startBtn; oldBack = backBtn;
        oldDpad = dpadDir; oldHoriz = horizDir;
        selectBtn = select.IsPressed(); startBtn = start.IsPressed(); backBtn = back.IsPressed();
        //get up/down
        dpadDir = 0 - Mathf.RoundToInt(Mathf.Clamp(cursor.ReadValue<Vector2>().y, -1, 1));
        horizDir = Mathf.RoundToInt(Mathf.Clamp(cursor.ReadValue<Vector2>().x, -1, 1));
        category += JustPressedDirection();justpressedh = JustPressedDirectionHorizontal()+0;
        category = Math.Clamp(category, 0, 3);
        PlayerHealth.body[category] = Math.Clamp(PlayerHealth.body[category] + justpressedh, 1, 9);
        if (justpressedh != 0) { PlayerHealth.thisPlayer.LoadCharacter(); }
        selectionPosition = categories[category].Find(PlayerHealth.body[category].ToString()).GetComponent<RectTransform>();
        cursorSq.position = selectionPosition.position;
    }
}
