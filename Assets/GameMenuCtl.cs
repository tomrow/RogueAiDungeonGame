using System.Threading;
using UnityEngine;

public class GameMenuCtl : MonoBehaviour
{
    Vector3 closedPos = Vector3.left * 120;
    bool oldEnabled;
    bool enabled;
    float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        enabled = PlayerHealth.MenuMode != PlayerHealth.MenuModes.disabled;
        if (enabled != oldEnabled)
        {
            transform.position = Vector3.Lerp(enabled ? Vector3.zero : closedPos, enabled ? closedPos : Vector3.zero, timer );
            timer += Time.deltaTime * 3;
            if (timer >= 1) { oldEnabled = enabled; }

        }
        else { timer = 0; }
    }
}
