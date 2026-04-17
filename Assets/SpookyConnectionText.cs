using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SpookyConnectionText : MonoBehaviour
{
    TextBoxModal t;
    bool spookyMode;
    public bool forceSpookyMode;
    FadeOverlay fade;
    public ConnMachineAnim model;
    bool done;
    string[] spookyMessages = 
    { 
        "Missing file detected:\nBOO.ZIP\nPush A to try connecting again.",
        "What did the beaver say to\nthe tree?\n\nPush A to try connecting again.",
        "Server is under maintenance.\n\nWe'll be right back.\n\nPush A to try connecting again.",
        "Piracy is a serious crime\naccording to copyright law.\nYour client has been disconnected.\nPress A to try connecting again.",
        "Connection failed.\n\nIt's your fault.\n\nPush A to try connecting again.",
        "Connection failed. Connection failed. Connection failed. Connection failed. Connection failed. Connection failed. Connection failed. Connection failed.  Conne\n\nPress A to try connecting again.",
        "Connection refused.\nHow long has it been?\n\nPress A to try connecting again.",
        "Connection failed.\nServer failed authenticity check.\n\nPress A to try connecting again.",
        "Connection failed.\nWhat have you done?\n\nPress A to try connecting again."
    };
    string[] text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        done = false;
        fade = Instantiate(Resources.Load("FadeOverlay").GameObject(), GameObject.Find("Canvas").transform).GetComponent<FadeOverlay>();
        fade.mode = FadeOverlay.Transitions.Wait;
        fade.timer = 0.2f; //hack to make sure the transparency color is only set once preventing even worse memory usage than what we currently use
        spookyMode = (Random.Range(0, 15) < 2) || forceSpookyMode;
        text = new string[1];
        text[0]=("Connecting to server...\n\n2130706433 (localhost)");
        
        if(spookyMode)
        {
            text = new string[3];
            text[0] = ("Connecting to server...\n\n2130706433 (localhost)");
            text[1]=(spookyMessages[Random.Range(1,spookyMessages.Length)-1]);
            text[2] = ("Connecting to server...\n\n2130706433 (localhost)");
        }

        //Spawn text box;
        t = Instantiate(Resources.Load("NpcDialogueBox").GameObject(), GameObject.Find("Canvas").transform).GetComponent<TextBoxModal>();
        t.stopAutoAdvanceOnThisPage = 1;
        t.autoAdvanceText = true;
        t.advanceInterval = 0.6f;
        t.text = text;
    }
    private void Update()
    {
        if (t.mode > 1)
        {
            //closing box, fade out now!
            if (done == false)
            {
                fade.mode = FadeOverlay.Transitions.FadeOut;
                fade.sceneForTransfer = "CharacterCreator";
                done = true;
            }
            
        }
        if (t.dialogueLine > 0) { model.slump = true; Debug.Log("slump=true;"); }
        if (t.dialogueLine > 1) 
        {
            t.stopAutoAdvanceOnThisPage = 4;
            t.advanceInterval = 0.5f;
            t.autoAdvanceText = true;
            
        }
    }

    // Update is called once per frame

}
