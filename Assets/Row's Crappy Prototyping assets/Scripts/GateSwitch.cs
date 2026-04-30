using UnityEngine;

public class GateSwitch : MonoBehaviour
{
    [SerializeField, Tooltip("This is the switch's accompanying gate.")] GameObject MyDoor;
    LaserDoor laserDoorReference;
    float timer = 0;
    Transform orb;
    bool touched = false;
    Vector3 orbInitScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!MyDoor)
        {
            Debug.Log("There's no gate attached to" + this.gameObject + "Please return to editor and attach a gameobject.");
        }
        //At this point, this might be closer to an INsanity check.
        // well, anyway... This just adds another log to the alrady bloated console.
        // sorry tom...
        // = row.
        orb = transform.Find("orb");
        orbInitScale = orb.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        //laserDoorReference = MyDoor.GetComponentInChildren<LaserDoor>();
        //Debug.Log(laserDoorReference);

        try
        {
            if (touched)
            {
                orb.localScale = Vector3.Lerp(orbInitScale, Vector3.zero, timer); //when touched, the floating orb should start shrinking until it is nothing
                timer = Mathf.Clamp01(timer+Time.deltaTime); //duration of shrink animation should not exceed 1 as it will invert and become visible again
            }
        }
        catch { }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerCtl>()) //So. If the player triggers the object...
        {
            laserDoorReference = MyDoor.GetComponentInChildren<LaserDoor>(); // pull from the connected object's children to find the LaserDoor script.
            laserDoorReference.GateHasBeenOpened = true; //open the door.
            touched = true;
        }
    }
}
