using UnityEngine;

public class MouseSensorForMenu : MonoBehaviour
{
    public mainMenuCtl controller;
    public mainMenuCtl.actions MenuAction;
    private void OnMouseEnter()
    {
        controller.CursorOver(gameObject);
        Debug.Log("mouse");
    }
    private void OnMouseExit()
    {
        //controller.CursorLeave(gameObject);
    }
}
