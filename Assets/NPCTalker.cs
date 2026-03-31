using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class NPCTalker : MonoBehaviour
{
    public string[] text;
    bool lt, oldLt;
    GameObject confirmMenuSfx, textBoxPrefab, canvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        confirmMenuSfx = Resources.Load("sfxEmitters/MenuConfirm").GameObject();
        textBoxPrefab = Resources.Load("NpcDialogueBox").GameObject();
        canvas = GameObject.Find("/Canvas");
    }
    private bool JustPressedSelect()
    {
        if (lt && !oldLt) { GC.Collect(GC.MaxGeneration); Resources.UnloadUnusedAssets(); return lt; }
        else { return false; }
    }
    // Update is called once per frame
    void Update()
    {
        oldLt = lt; lt = PlayerHealth.thisPlayer.Atk3;
        if (JustPressedSelect() && Vector3.Distance(transform.position, PlayerHealth.thisPlayer.transform.position)<2)
        {
            Instantiate(confirmMenuSfx);
            TextBoxModal box = Instantiate(textBoxPrefab, Vector3.zero, Quaternion.identity, canvas.transform).GetComponent<TextBoxModal>();
            box.text = text;
        }
    }
}
