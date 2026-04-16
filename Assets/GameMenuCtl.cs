using System.Threading;
using UnityEngine;

public class GameMenuCtl : MonoBehaviour
{
    Vector3 closedPos = Vector3.left * 312;
    Vector3 openPos = Vector3.left * 185;
    RectTransform r;
    bool oldEnabled;
    bool menuenabled;
    float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        r= GetComponent<RectTransform>();
        menuenabled = PlayerHealth.MenuMode != PlayerHealth.MenuModes.disabled;
        oldEnabled = PlayerHealth.MenuMode != PlayerHealth.MenuModes.disabled;
    }

    // Update is called once per frame
    void Update()
    {
        menuenabled = PlayerHealth.MenuMode != PlayerHealth.MenuModes.disabled;
        if (menuenabled != oldEnabled)
        {
            transform.localPosition = Vector3.Lerp(!menuenabled ? openPos : closedPos, !menuenabled ? closedPos : openPos, timer);
            timer += Time.deltaTime * 3;
            if (timer >= 1) { oldEnabled = menuenabled; }

        }
        else 
        { 
            timer = 0;
            transform.localPosition = menuenabled ? openPos : closedPos;
        }
    }







}
