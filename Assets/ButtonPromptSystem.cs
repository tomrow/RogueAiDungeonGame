using UnityEngine;

public class ButtonPromptSystem : MonoBehaviour
{
    GameObject talk, pc, grab;
    public static ButtonPromptSystem promptObj;
    int delay = 0;
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
        if (delay <= 0) { grab.SetActive(false); pc.SetActive(false); talk.SetActive(false); delay = 0; }
        else { delay--; }

    }
    public void TalkPrompt(){ talk.SetActive(false); delay = 2; }
    public void GrabPrompt() { grab.SetActive(false); delay = 2; }
    public void PcPrompt() { pc.SetActive(false); delay = 2; }
}
