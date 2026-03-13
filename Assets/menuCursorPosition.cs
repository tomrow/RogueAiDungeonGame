using UnityEngine;

public class menuCursorPosition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(3, 24,0);
        transform.Translate(Vector3.down * (PlayerHealth.cursorPos * 36));
    }
}
