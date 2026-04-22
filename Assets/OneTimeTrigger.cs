using UnityEngine;

public class OneTimeTrigger : MonoBehaviour
{
    public GameObject prefab;
    public Transform parent;
    public Vector3 offsetPos;
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
        if (other.gameObject.GetComponent<PlayerCtl>() != null)
        {
            if (parent != null)
            { Instantiate(prefab, offsetPos, Quaternion.identity, parent); }
            else { Instantiate(prefab, offsetPos, Quaternion.identity); }
            Destroy(this.gameObject);
        }
    }
}
