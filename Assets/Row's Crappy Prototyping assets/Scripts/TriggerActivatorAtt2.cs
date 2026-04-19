using UnityEngine;

public class TriggerActivatorAtt2 : MonoBehaviour
{
    [SerializeField, Tooltip("This is the object the trigger is interacting with.")] GameObject TriggerTargetObject;

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
        if(other.gameObject.GetComponent<PlayerCtl>())
        {
            TriggerTargetObject.gameObject.SetActive(true);
        }
    }
}
