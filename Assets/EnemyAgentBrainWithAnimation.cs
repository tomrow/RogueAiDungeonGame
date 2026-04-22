using UnityEngine;
using UnityEngine.AI;

public class EnemyAgentBrainWithAnimation : MonoBehaviour
{

    /*Dear lord this is really primitive. At current, this enemy can recognise the target location and move toward it. THAT'S IT.*/
    #region variables setup
    #region Basic Targeting Variables
    [SerializeField, Tooltip("This is the generic enemy target. It moves toward this object's position.")] GameObject Mytarget;
    [SerializeField] NavMeshAgent agent;
    [SerializeField, Tooltip("This is the brain's sight range. enemies farther than the max distance cannot be targets.")] float SightRange;
    #endregion

    [SerializeField, Tooltip("This is the max attack distance. this trigger object tells the brain it can attack.")] float MyAttackMaxDist;
    [SerializeField, Tooltip("This is the ''Too Close'' range. Enemies with this enabled will Cease Moving if within this range. Set to 0 to disable this behaviour. (INADVISED)")] float MyComfortableDist;
    EnemyFundamentals EnemyAttributes;
    [SerializeField, Tooltip("This object is the bluprint template for the Enemy attack. It's only used for ranged attacks.")] GameObject EnemyAttackBullet;
    [SerializeField, Tooltip("This object is the bluprint template for the Enemy attack. It's only used for melee attacks.")] GameObject EnemyAttackMelee;
    bool IsInRange;
    [SerializeField, Tooltip("This handles what attack the enemy should do. True makes the enemy ranged, false makes them Melee.")] bool MyAttackIsRanged;
    float AttackInterval;
    [SerializeField, Tooltip("Configureable wait period before attack in seconds.")] float TimeToAttack;
    RaycastHit rayout;
    #region MiscVariables
    [SerializeField, Tooltip("The entity forgoes normal behaviour, in favour of spooking the player.")] bool isSpooky;
    NavMeshAgent MyBodyAndProperties;
    [SerializeField, Tooltip("This float is the speed an abnormal enemy travels at.")] float SpookSpeed;
    [SerializeField, Tooltip("This Gameobject is used in conjunction with the isSpooky Parameter. It will not be used otherwise.")] GameObject SpookObj;

    
    #region Animation Controls
    /*
     * I'm adding this in order to delay the attack action. I'm adding this changed script where instead of immediately calling attack(), 
     * the below boolean gets set to true, which changes the behaviour to trigger an animation and wait until it is close to
     * finished before calling attack(). The old Update() subroutine has been renamed "ScanForPlayer", and has been changed to set 
     * currentlyAttacking to true. A new Update function has been made to simply call ScanForPlayer each cycle when this is false, 
     * and trigger the attack animation when true. --Tom
     */
    bool currentlyAttacking = false;
    Animator animator;
    AnimatorStateInfo currentAnimation;
    #endregion
    #endregion
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        animator = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        if (animator == null) { animator = GetComponent<Animator>(); }
        MyBodyAndProperties = this.gameObject.GetComponent<NavMeshAgent>();
        LockOnTarget MyTargetableState = this.GetComponent<LockOnTarget>();
        MyTargetableState.on = true;
        EnemyAttributes = GetComponent<EnemyFundamentals>();
        /*
         * This script sets the enemyattributes and LockOnTargetability.
         * Doing this here allows for the template to have it's targetability off by default.
         * Enemies will automatically turn theirs on.
         */
        if (isSpooky)
        {
            // Here will handle changing the base speed for the enemy, provided it is behaving abnormally.
            MyBodyAndProperties.speed = SpookSpeed;
            Debug.Log("The entity" + this.gameObject + "is no longer just an enemy.");
        }
        else
        {
            Debug.Log(this.gameObject + "is a regular enemy.");
        }
    }

    // Update is called once per frame
    void ScanForPlayer()
    {
        #region Dev Commentary
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
        #endregion



        Vector3 directionToPlayer = ((this.transform.position - PlayerHealth.thisPlayer.transform.position) * -1);
        //determines angle to shoot raycast at?
        Debug.DrawRay(this.transform.position, directionToPlayer, Color.teal);
        if (Vector3.Distance(this.transform.position, PlayerHealth.thisPlayer.transform.position) <= SightRange)
        { //if the player is within sight range
            if (Physics.Raycast(this.transform.position, directionToPlayer.normalized, out rayout, SightRange))
            { //I fthe raycast hits something
                if (rayout.transform.gameObject.name == "DungeonPlayer")
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
        
        if (Mytarget != null)
        {
            if (Vector3.Distance(this.transform.position, PlayerHealth.thisPlayer.transform.position) <= MyComfortableDist)
            {
                this.agent.ResetPath();
                //this.agent.SetDestination(this.transform.position);
                //Alternative if above code fails.  
            }
            else
                this.agent.SetDestination(Mytarget.transform.position);
        }

        if (Vector3.Distance(this.transform.position, PlayerHealth.thisPlayer.transform.position) <= MyAttackMaxDist)
        {
            AttackInterval = AttackInterval + Time.deltaTime;
            if (AttackInterval >= TimeToAttack)
            {
                if (isSpooky) //if the isSpooky paremeter is true, then forgo normal behaviour, in favour of the following.
                {
                    Instantiate<GameObject>(SpookObj); // spook the player.
                    Destroy(this.gameObject); // die.
                    return;
                }
                else if (!isSpooky)
                {
                    currentlyAttacking=true; Debug.Log("Entity has attacked!");
                    AttackInterval = 0;
                }

            }
        }






        //agent.SetDestination(new Vector3(Mytarget.transform.position.x, Mytarget.transform.position.y, Mytarget.transform.position.z));
        //alternative if the above fails to work as intended.
    }
    void attack()
    {
        /*
         Hey, Just a note, I really don't know what you want to do regarding enemy attacks.
        Maybe a switchcase? It'd be suitably modular, and can be customised further still.
        In any case, attack() gets called when the enemy needs to attack. Place whatever
        the enemy needs to do here!
        -Rowan.
         */

        /*
         In the projectile script:
        To damage the player do the following;

         */
        if (MyAttackIsRanged)
        {
            Instantiate(EnemyAttackBullet, this.transform.position, this.transform.rotation);
        }
        else if ((Vector3.Distance(this.transform.position, PlayerHealth.thisPlayer.transform.position) <= MyComfortableDist) && !MyAttackIsRanged)
        {
            Instantiate(EnemyAttackMelee, this.transform.position, this.transform.rotation);
        }
        return;
    }
    private void Update()
    {
        if (currentlyAttacking)
        {
            animator.SetInteger("mode", 2);
            currentAnimation = animator.GetNextAnimatorStateInfo(0);
            Debug.Log(currentAnimation.IsName("attack").ToString() + currentAnimation.normalizedTime.ToString());
            if (currentAnimation.IsName("attack") && currentAnimation.normalizedTime > 0.2f) //if playing the attack animation and it's over 90% complete
            { attack(); currentlyAttacking = false; Debug.Log("attack finished"); }  //then start the attack call
        }
        else 
        {
            Debug.Log("scanning");
            ScanForPlayer(); //chase player
            animator.SetInteger("mode", (agent.velocity.magnitude > Time.deltaTime * 0.1f) ? 1 : 0); // do walk or idle depending on speed, less than 0.1 m/s will do idle.
        } 
    }
}
