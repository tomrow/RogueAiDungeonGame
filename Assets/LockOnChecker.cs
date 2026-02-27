using UnityEngine;

public class LockOnChecker : MonoBehaviour
{
    GameObject closest;
    public PlayerCtl playerCtl;
    GameObject origTarget;
    float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        origTarget = playerCtl.lockedOnEnemy;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.03)
        {
            if (origTarget == closest) { playerCtl.lockedOnEnemy = null; }
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name + "found!");
        if (closest != null)
        {
            if (Vector3.Distance(transform.position, collision.transform.position) < Vector3.Distance(transform.position, closest.transform.position) //if the obj is closer than an already found target
                && (collision.gameObject.GetComponent<LockOnTarget>() != null) //if it even is a lockon target
                && (collision.gameObject.GetComponent<LockOnTarget>().on)) //and targeting is enabled for this thing
            {
                closest = collision.gameObject;
                playerCtl.lockedOnEnemy = closest; //then add it
                Debug.Log("replacing...");
            }
        }
        else
        {
            if( (collision.gameObject.GetComponent<LockOnTarget>() != null) //if there is no prior objects found, dont check the distance
                && (collision.gameObject.GetComponent<LockOnTarget>().on) ) 
            {
                closest = collision.gameObject;
                playerCtl.lockedOnEnemy = closest;
                Debug.Log("Setting...");
            }
        }
    }
}
