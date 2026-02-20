using UnityEngine;
using System.Collections.Generic;

public class mainMenuCtl : MonoBehaviour
{
    public List<GameObject> menuItems;
    Material material; 
    int menuSelection;
    public enum actions
    {startGame, settings}
    public Transform cursorObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        cursorObj.position = menuItems[menuSelection].transform.position;
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
    public void CallMenuAct(actions act)
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
                throw new NotImplementedException();

        }
    }
    
}
