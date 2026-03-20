using UnityEngine;
using UnityEngine.AI;

public class EnemyAgentBrain : MonoBehaviour
{

    /*Dear lord this is really primitive. At current, this enemy can recognise the target location and move toward it. THAT'S IT.*/
    #region variables setup
    [SerializeField, Tooltip("This is the generic enemy target. It moves toward this object's position.")] GameObject Mytarget;
    [SerializeField] NavMeshAgent agent;
    [SerializeField, Tooltip("This is the brain's sight range. enemies farther than the max distance cannot be targets.")] float SightRange;
    [SerializeField, Tooltip("This is the max attack distance. this trigger object tells the brain it can attack.")] float MyAttackMaxDist;
    [SerializeField, Tooltip("This is the ''Too Close'' range. Enemies with this enabled will back away if both IsInRange and UhOhTooClose are TRUE. Set to 0 to disable this behaviour.")] float MyComfortableDist;
    EnemyFundamentals EnemyAttributes;
    bool IsInRange;
    bool UhOhTooClose;
    RaycastHit rayout;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnemyAttributes = GetComponent<EnemyFundamentals>();
        if (MyComfortableDist == 0)
        {
            UhOhTooClose = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
         So... Here's what we need to do.

        1) Define a maximum sight range.
        2) Get a loop to run without breaking game logic.
        3) Within each loop iteration we need:
            a) A raycast that occurs at regular angle offsets.
            b) A check that the raycast did not collide with anything that is not the player.
            c) If the player moves too far away, remove the player as the target.
        4) Set the destination to the target, and move. [Handled Below.]
        5) Create a trigger volume that tells the brain [This script] when the enemy can attack.
            a) while the player is in said trigger, the enemy needs to stop moving.
            b) A second trigger can be used to get the enemy to walk backwards, if it's a ranged attacker.
                b) Addendum: These trigger inputs will need sanitization.
        6) Create an attack state that the brain can switch to when needed.
            a) addendum: We may need to manually rotate the enemy while in this state.
         */



        Vector3 directionToPlayer = (this.transform.position - PlayerHealth.thisPlayer.transform.position);
        //determines angle to shoot raycast at?
        
        if (Vector3.Distance(this.transform.position, PlayerHealth.thisPlayer.transform.position) <= SightRange)
        { //if the player is within sight range
            if (Physics.Raycast(this.transform.position, directionToPlayer.normalized, out rayout, SightRange))
            { //I fthe raycast hits something
                if(rayout.transform.gameObject.name == "DungeonPlayer")
                    //Check if the player.
                { Mytarget = rayout.transform.gameObject; }
                // if so, set as target
                else { Mytarget = null; }
            }//else it's not the player, set to null.
            else { Mytarget = null; }
        //else it's obstructed, set to null
        }
        else { Mytarget = null; }
        //else its OOB, Set to null.
        /*for (float i=0; i<360; i+=22.5f)
        {
            Vector3 direction = new Vector3(Mathf.Sin(i * Mathf.Deg2Rad), 0, Mathf.Cos(i * Mathf.Deg2Rad));
            Debug.DrawRay(this.transform.position,direction*SightRange,Color.red);
            if(Physics.Raycast(this.transform.position,direction, out rayout, SightRange))
            { GameObject objHit = rayout.collider.gameObject;
                if (objHit.GetComponent<PlayerCtl>() != null)
                { 
                    Mytarget = objHit;
                    
                    break;
                }
            }
        }*/
        if (Mytarget != null)
        {
            if (Vector3.Distance(this.transform.position, PlayerHealth.thisPlayer.transform.position) <= MyAttackMaxDist)
            {
                this.agent.ResetPath();
                //this.agent.SetDestination(this.transform.position);
                //Alternative if above code fails.
            }
            else
            this.agent.SetDestination(Mytarget.transform.position);
        }
        
        //agent.SetDestination(new Vector3(Mytarget.transform.position.x, Mytarget.transform.position.y, Mytarget.transform.position.z));
        //alternative if the above fails to work as intended.
    }
}
