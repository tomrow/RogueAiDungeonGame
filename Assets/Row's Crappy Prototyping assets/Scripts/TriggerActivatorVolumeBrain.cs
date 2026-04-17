using UnityEngine;

public class TriggerActivatorVolumeBrain : MonoBehaviour
{
    [SerializeField, Tooltip("This Variable is what the trigger Volume will interact with.")] GameObject TriggerTargetObject;
    SpawnerBrain SpawnerState;
    public bool triggerIsRoomVolume;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnerState = TriggerTargetObject.GetComponent<SpawnerBrain>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<PlayerCtl>())
        {
            SpawnerState.isActive = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if ((collision.gameObject.GetComponent<PlayerCtl>()!=null) && triggerIsRoomVolume)
        {
            SpawnerState.isActive = false;
        }
    }

}
