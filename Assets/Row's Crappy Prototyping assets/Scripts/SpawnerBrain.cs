using UnityEngine;

public class SpawnerBrain : MonoBehaviour
{
    #region ToDoList
    /*
     List of things this script needs to do:

    1) Define what enemies spawn from this spawner. [DONE]
    2) Define how enemies spawn, (Either permanantly, or temportarily) through this spawner. [A better way of wording this: Is the spawner mortal?] [In the process of doing]
    3) Define a variable for handling the random range selection for time between spawns.[DONE]
    4) Define an interactable to handle remote destruction, if needed. [NEEDS COMPLETION]
    5) Test enemy spawning[TO DO]
    6) Refine further. [TO DO]
     */
    #endregion
    #region Variables
    #region Spawner Specific Vars
    [SerializeField, Tooltip("This Is the main enemy the spawner will create.")] GameObject CreatedEnemyTypeMain;
    [SerializeField, Tooltip("This is a Variant enemy the spawner can make.")] GameObject CreatedEnemyTypeSecondary;
    [SerializeField, Tooltip("This variable handles the maximum time value between spawns.")] float maxTimeBetweenSpawns;
    [SerializeField, Tooltip("This Variable handles the minimum time value between spawns.")] float minTimeBetweenSpawns;
    float spawnerTimer;
    bool isDestructible; //A more accurate term would be, "has health".
    int chosenEnemyType;
    [SerializeField,Tooltip("This field activates the spawner's main behaviour. Use it in conjunction with a trigger volume.")]public bool isActive;

    #endregion
    #region Config Variables
    [SerializeField, Tooltip("This Bool Toggles unfair limiters. Ideally this should remain off.")] bool isUnfairSpawner;
    [SerializeField, Tooltip("This Int is the Fair Spawn timer value, in seconds. This should never Change from it's intended value, and is only a Serialize out of convenience.")] int minimumFairValue;
    #endregion
    #region Error Handling and Debugging
    bool isMissingEnemyTemplates;
    #endregion
    #region Other Variables
    EnemyFundamentals EnemyAttributes;
    #endregion
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        #region Fairness Check
        //Immediatly Check if isUnfairSpawner is on, if not, Check to make sure the Minimum time is reasonable.
        if (!isUnfairSpawner)
        {
            if (minTimeBetweenSpawns < minimumFairValue)
            {
                minTimeBetweenSpawns = minimumFairValue;
            }
        }
        #endregion
        #region Sanity Check
        if (!CreatedEnemyTypeMain) // Check to see if the SPAWNER has objects to SPAWN.
        {
            if (!CreatedEnemyTypeSecondary) //Check to see if we didn't just fuck up and forget the main object only.
            {
                Debug.Log("PANIC --- SPAWNER IS MISSING ANY ENEMY TEMPLATES. SPAWNER WILL NOT FUNCTION");
                isMissingEnemyTemplates = true;
                //oops.mp4
            }
            else
            {
                CreatedEnemyTypeMain = CreatedEnemyTypeSecondary;
                //This is not ideal, but we can make this work.
            }
        }
        #endregion

        //Ha ha! I'm so glad this (hopefully) works only having to copy paste from the enemy script!
        if (isDestructible)
        {
            EnemyAttributes = GetComponent<EnemyFundamentals>();
        }

        LockOnTarget MyTargetableState = this.GetComponent<LockOnTarget>();
        MyTargetableState.on = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (isMissingEnemyTemplates)
            {
                return; //This should only ever run if there's an empty spawner in the map. Even then, it's gonna do fuck all anyway, so what's the issue?
            }//Failsafe in case no templates are assigned.
        else
        {

            if(spawnerTimer <= 0f)
            {
                //This choses between 0 and 1, hopefully.
                chosenEnemyType = Random.Range(0, 1);
                if (chosenEnemyType == 0)
                {   
                    // This'll make an enemy, at the Spawner's location. Make sure to remove collision with enemies, if possible.
                    // Or, Alternatively, Find a way to spawn them around the spawner, not inside of it.
                    Instantiate<GameObject>(CreatedEnemyTypeMain,this.transform.position,this.transform.rotation);
                }
                else
                {
                    //Same here, but it's the "rare" enemy. Fudge the ranges if you want it to be actually RARE.
                    if (!CreatedEnemyTypeSecondary)
                    {
                        Instantiate<GameObject>(CreatedEnemyTypeMain, this.transform.position, this.transform.rotation);
                    }
                    else
                    {
                        Instantiate<GameObject>(CreatedEnemyTypeSecondary, this.transform.position, this.transform.rotation);
                    }
                    
                }
                //This may not generate nice full second intervals. Probably not a problem.
                spawnerTimer = Random.Range(minTimeBetweenSpawns,maxTimeBetweenSpawns); 
            }
            spawnerTimer -= Time.deltaTime;
        }
        }
        
    }
}
