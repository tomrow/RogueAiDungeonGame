using UnityEngine;

public class SpookScripting : MonoBehaviour
{
    #region ToDoList
    /*
     This Script needs to do the following.
    1) Manipulate a gameobject to do something spooky.
    2) Be able to be triggered by an external source.
    3) Be modular for ease of use and building from.
     */
    #endregion
    #region Variables
    #region Config Variables
    public enum SpookType {WalkThruWalls,WalkThruSpoopmode,inactive,random}; // This Enum Should allow for easy selection in-editor, and ease of development going forward.
    [SerializeField, Tooltip("This deisgnates what this Gameobject will Be when triggered.")] SpookType EntitySpookType;
    [SerializeField, Tooltip("This gameobject is the target for entities with the WalkThru Spooktypes. This is their target destination.")] GameObject TargetDestination;
    bool EventActive;
    float TimeRemainingFromActivation = 5f;
    #endregion
    #region Misc
    Vector3 StartPos;
    [SerializeField, Tooltip("This is the object the Scripted Event will spawn when in process. The event ends after this.")] GameObject SpawnedSpookObject;
    #endregion
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPos = this.gameObject.transform.position;
        if(EntitySpookType == SpookType.random)
        {
            if(Random.Range(1,3) == 1)
            {
                EntitySpookType = SpookType.WalkThruWalls;
                Debug.Log("Walk through walls, Ignoring the Player");
            }else if(Random.Range(1, 3) == 2)
            {
                EntitySpookType = SpookType.WalkThruSpoopmode;
                Debug.Log("Walk Through one wall, then spawn a SPOOPMODE enemy.");
            }else if(Random.Range(1, 3) == 3)
            {
                EntitySpookType = SpookType.inactive;
                Debug.Log("NoSpook");
            }
        }
        // huzzah! There is now a randomised element that we can implement if wanted!
        // So far none of my implementations of the Random.Range class have worked in a noticeable manner.
        // Here's hoping this does.
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 DirectionOfMovement = ((this.gameObject.transform.position - TargetDestination.transform.position) * -1);
        if (EventActive)
        {
            switch (EntitySpookType)
            {
                case SpookType.inactive:
                    Destroy(this.gameObject);
                    break; //No scare activates, and the player goes on their merry way.

                case SpookType.WalkThruSpoopmode:
                    this.transform.position += (DirectionOfMovement * (Time.deltaTime / 2));
                    // This determines the direction of movement, then moves along that direction at a rate of Time.deltatime / 2.
                    // It's fast, but not TOO fast.
                    if (Vector3.Distance(this.gameObject.transform.position, TargetDestination.transform.position) >= Vector3.Distance(StartPos, TargetDestination.transform.position)/2)
                    {
                        Instantiate(SpawnedSpookObject,this.transform.position,this.transform.rotation);
                        Destroy(this.gameObject);
                    }
                    break;
                case SpookType.WalkThruWalls:

                    this.transform.position += (DirectionOfMovement * (Time.deltaTime / 2));
                    // This determines the direction of movement, then moves along that direction at a rate of Time.deltatime / 2.
                    // It's fast, but not TOO fast.
                    TimeRemainingFromActivation -= Time.deltaTime;
                    break;

            }
        }
        
        if(TimeRemainingFromActivation <= 0f)
        {
            Destroy(this.gameObject);
        }

        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == TargetDestination)
        {
            Destroy(this.gameObject);
        }
    }
    //these last few lines of code clean up after the event is over.
}
