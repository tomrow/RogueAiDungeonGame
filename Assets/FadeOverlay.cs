using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeOverlay : MonoBehaviour
{
    public enum Transitions { FadeIn = 0, Wait = 1, FadeOut = 2, Exit=3 }
    public Transitions mode = Transitions.FadeIn;
    public float timer;
    RawImage fadeOverlay;
    float alpha;
    Color transparent = new Color(0, 0, 0, 0);
    public string sceneForTransfer = "";
    [Tooltip("Amount of time to wait until scene is automatically changed. Set to a negative number to disable this function")] public float automaticallyLeaveTimer = 900;
    bool enableAutoLeave;
    public string autoLeaveLocation = "DemoEndScreen";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
        fadeOverlay = GetComponent<RawImage>();
        enableAutoLeave = automaticallyLeaveTimer >= 0;
    }
    public void FadeOut(){ timer = 0; mode = Transitions.FadeOut; }
    public void FadeIn(){ timer = 0; mode = Transitions.FadeIn; }
    public void FadeOutToNewScene(string sceneName)
    { FadeOut(); sceneForTransfer = sceneName; }

    // Update is called once per frame
    void Update()
    {

        if (enableAutoLeave) 
        { 
            automaticallyLeaveTimer -= Time.deltaTime;
            if (automaticallyLeaveTimer <= 0) { FadeOutToNewScene(autoLeaveLocation == "" ? sceneForTransfer : autoLeaveLocation); enableAutoLeave = false; }
        }
        switch (mode)
        {
            case Transitions.FadeIn: //one second fade in
                timer += Time.deltaTime;
                if (timer >= 1) { mode = Transitions.Wait; }
                alpha = Mathf.Clamp01(1 - timer);
                fadeOverlay.color = new Color(0, 0, 0, alpha);
                break;
            case Transitions.FadeOut:
                //fade out
                timer += Time.deltaTime;
                if (timer >= 1) { mode = Transitions.Exit; }
                alpha = Mathf.Clamp01(timer);
                fadeOverlay.color = new Color(0, 0, 0, alpha);
                PlayerHealth.MenuMode = PlayerHealth.MenuModes.disabled;
                break;
            case Transitions.Exit:
                try { SceneManager.LoadScene(sceneForTransfer); } 
                catch { Debug.Log("Invalid scene"); } 
                finally { mode = Transitions.Wait; } break;
            default:
                if (timer > 0) { fadeOverlay.color = transparent; }
                timer = 0;
                
                break;
        }
    }
}
