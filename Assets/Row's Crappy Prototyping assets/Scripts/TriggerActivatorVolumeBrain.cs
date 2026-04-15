using UnityEngine;

public class TriggerActivatorVolumeBrain : MonoBehaviour
{
    [SerializeField, Tooltip("This Variable is what the trigger Volume will interact with.")] GameObject TriggerTargetObject;
    SpawnerBrain SpawnerState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnerState = TriggerTargetObject.GetComponent<SpawnerBrain>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerCtl>())
        {
            SpawnerState.isActive = true;
        }
    }
}
