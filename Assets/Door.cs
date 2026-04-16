using UnityEngine;

public class Door : MonoBehaviour
{
    public FadeOverlay f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerCtl p = other.gameObject.GetComponent<PlayerCtl>();
        if (p != null ) //if the touching trigger is a player and weapon is not equipped
        {
            f.sceneForTransfer = "CharacterCreator";
            f.mode = FadeOverlay.Transitions.FadeOut;
            PlayerInputAggregator.inputEnabled = false;
        }
    }
}
