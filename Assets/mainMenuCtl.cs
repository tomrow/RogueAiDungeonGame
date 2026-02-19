using System.Security.Cryptography;
using UnityEngine;

public class mainMenuCtl : MonoBehaviour
{
    Material material; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CursorOver()
    {
        material.color = Color.red;
    }
    public void CursorLeave()
    { material.color = Color.blue; }

    private void OnMouseOver()
    {
        CursorOver();
        Debug.Log("mouse");
    }
    private void OnMouseExit()
    {
        CursorLeave();
    }
}
