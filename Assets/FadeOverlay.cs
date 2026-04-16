using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeOverlay : MonoBehaviour
{
    public enum Transitions { FadeIn = 0, Wait = 1, FadeOut = 2, Exit=3 }
    public Transitions mode = Transitions.FadeIn;
    float timer;
    RawImage fadeOverlay;
    float alpha;
    public string sceneForTransfer = "";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
        fadeOverlay = GetComponent<RawImage>();
    }
    public void FadeOut(){ timer = 0; mode = Transitions.FadeOut; }
    public void FadeIn(){ timer = 0; mode = Transitions.FadeIn; }
    public void FadeOutToNewScene(string sceneName)
    { FadeOut(); sceneForTransfer = sceneName; }

    // Update is called once per frame
    void Update()
    {
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
                break;
            case Transitions.Exit:
                try { SceneManager.LoadScene(sceneForTransfer); } 
                catch { Debug.Log("Invalid scene"); } 
                finally { mode = Transitions.Wait; } break;
            default:
                timer = 0;
                break;
        }
    }
}
