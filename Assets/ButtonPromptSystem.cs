using JetBrains.Annotations;
using UnityEngine;

public class ButtonPromptSystem : MonoBehaviour
{
    GameObject talk, pc, grab;
    public static ButtonPromptSystem promptObj;
    int delay = 0;
    int icon = 0, oldIcon=0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        talk = transform.Find("talk").gameObject;
        pc = transform.Find("pc").gameObject;
        grab = transform.Find("grab").gameObject;
        ButtonPromptSystem.promptObj = this;
    }

    // Update is called once per frame
    void Update()
    {
        
        Debug.Log("ButtonPromptDelay"+delay.ToString());
        if ((oldIcon!=icon) && (icon!=0)) { Instantiate(SoundEffectStorage.selectMenuSfx, PlayerHealth.thisPlayer.transform.position, Quaternion.identity); }
        oldIcon = icon;
        if (delay <= 0) { grab.SetActive(false); pc.SetActive(false); talk.SetActive(false); delay = 0; icon = 0; }
        else { delay--; }
        
    }
    public void TalkPrompt(){ icon = 1; grab.SetActive(false); pc.SetActive(false); talk.SetActive(true); if (delay >= 1) { delay = 1; } else { delay = 2; } }
    public void GrabPrompt() { icon = 2; grab.SetActive(true); pc.SetActive(false); talk.SetActive(false); if (delay >= 1) { delay = 1; } else { delay = 2; } }
    public void PcPrompt() { icon = 3; grab.SetActive(false); pc.SetActive(true); talk.SetActive(false); if (delay >= 1) { delay = 1; } else { delay = 2; } }
}
