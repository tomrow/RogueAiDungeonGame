using System.Threading.Tasks;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    public Transform targetPosition;
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
        PlayerCtl p = other.GetComponent<PlayerCtl>();
        try
        {
            p.leapOrigin = p.transform.position;
            p.leapTarget = targetPosition.position;
            p.state = PlayerCtl.States.Leap;
        }
        catch { }
    }
}
