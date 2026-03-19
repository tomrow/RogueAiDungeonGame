using UnityEngine;

public class LockOnChecker : MonoBehaviour
{
    GameObject closest;
    public PlayerCtl playerCtl;
    GameObject origTarget;
    float timer;
    public bool autoLockOn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        origTarget = playerCtl.lockedOnEnemy;
        timer = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer > 0)
        {
            if ((origTarget == closest) && !autoLockOn)
            {
                try { Debug.Log("Locked on enemy " + closest.name + " is the same as the last one! " + origTarget.name); } catch { }
                playerCtl.lockedOnEnemy = null;
            }
            else { playerCtl.lockedOnEnemy = closest; }
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
            }
        }
        else
        {
            if( (collision.gameObject.GetComponent<LockOnTarget>() != null) //if there is no prior objects found, dont check the distance
                && (collision.gameObject.GetComponent<LockOnTarget>().on) ) 
            {
                closest = collision.gameObject;
            }
        }
    }
}
