using System;
using Unity.VisualScripting;
using UnityEngine;

public class RepairCenterClerk : MonoBehaviour
{
    bool lt, oldLt;
    GameObject confirmMenuSfx, shopFrontPrefab, canvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        confirmMenuSfx = Resources.Load("sfxEmitters/MenuConfirm").GameObject();
        shopFrontPrefab = Resources.Load("HealMenu").GameObject();
        canvas = GameObject.Find("/Canvas");
    }
    private bool JustPressedSelect()
    {
        if (lt && !oldLt) {  return lt; }
        else { return false; }
    }
    // Update is called once per frame
    void Update()
    {
        oldLt = lt; lt = PlayerHealth.thisPlayer.Atk3;
        if (Vector3.Distance(transform.position, PlayerHealth.thisPlayer.transform.position) < 2)
        {
            try { ButtonPromptSystem.promptObj.TalkPrompt(); } catch { }
            if(JustPressedSelect())
            {
                GC.Collect(GC.MaxGeneration); Resources.UnloadUnusedAssets();
                Instantiate(confirmMenuSfx);
                Instantiate(shopFrontPrefab, Vector3.zero, Quaternion.identity, canvas.transform);
            }
        }
    }
}
