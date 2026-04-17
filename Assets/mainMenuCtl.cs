using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class mainMenuCtl : MonoBehaviour
{
    public List<GameObject> menuItems;
    Material material; 
    int menuSelection;
    public enum actions
    {startGame, settings}
    public Transform cursorObj;
    string[] errorText = { "This option is currently unavailable.\nWe are sorry for the inconvenience." };
    InputAction navigate;
    InputAction confirm;
    InputAction cancel;
    [SerializeField]int dpadDir;
    int oldDpadDir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navigate = InputSystem.actions.FindAction("Navigate");
        confirm = InputSystem.actions.FindAction("Submit");
        cancel = InputSystem.actions.FindAction("Cancel");
        CursorOver(menuItems[menuSelection]);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cursorObj.position = menuItems[menuSelection].transform.position;
        dpadDir = 0-Mathf.RoundToInt(Mathf.Clamp(navigate.ReadValue<Vector2>().y, -1,1));
        if (dpadDir != oldDpadDir) { menuSelection += dpadDir; }
        if (menuSelection < 0) { menuSelection = menuItems.Count-1; }
        if(menuSelection >= menuItems.Count) {menuSelection = 0;}
        if (dpadDir != oldDpadDir)
        {
            CursorOver(menuItems[menuSelection]);
        }
        if (confirm.IsPressed()) 
        {
            CallMenuAct(menuItems[menuSelection].GetComponent<MouseSensorForMenu>().MenuAction);
        }
        oldDpadDir = dpadDir;
    }
    public void CursorOver(GameObject g)
    {
        foreach(GameObject i in menuItems)
        {i.GetComponent<MeshRenderer>().material.color = Color.blue;}
        material = g.GetComponent<MeshRenderer>().material;
        material.color = Color.red;
        menuSelection = menuItems.IndexOf(g);
        Debug.Log(menuSelection);
    }
    public void CursorLeave(GameObject g)
    { material = g.GetComponent<MeshRenderer>().material; material.color = Color.blue; }
    void CallMenuAct(actions act)
    {
        switch(act)
        {
            case actions.startGame:
                StartGame();
                break;
            case actions.settings:
                gotoSettings();
                break;
            default:
                //throw new NotImplementedException();
                Debug.Log("nothing");
                throw new NotImplementedException();
                break;

        }
    }

    private void StartGame()
    {
        Debug.Log("Start");
        SceneManager.LoadScene("Connection Screen Scene");
    }

    private void gotoSettings()
    {
        try { SceneManager.LoadScene("testLevel"); }
        catch {
            TextBoxModal t = Instantiate(Resources.Load("NpcDialogueBox").GameObject(), GameObject.Find("Canvas").transform).GetComponent<TextBoxModal>();
            t.stopAutoAdvanceOnThisPage = 1;
            t.autoAdvanceText = true;
            t.advanceInterval = 0.6f;
            t.text = errorText;
        }
    }
}
