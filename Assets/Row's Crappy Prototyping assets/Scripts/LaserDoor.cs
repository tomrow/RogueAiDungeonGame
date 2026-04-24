using UnityEngine;

public class LaserDoor : MonoBehaviour
{
    [SerializeField, Tooltip("This is the object that needs to remain for this one to keep doing it's usual behaviour.")] GameObject PowerGenerator;
    [SerializeField, Tooltip("This is the timer for timed gates. This defines the amount of time the gate remains open.")] float TimeOpenedMax;
    [SerializeField, Tooltip("This Gate is timed.")] public bool GateOpeningIsTimed;
    public bool GateHasBeenOpened = false;
    #region ARGH
    GameObject SHITEASS1;
    GameObject SHITEASS2;
    GameObject SHITEASS3;
    #endregion
    float timeRemainingOpen;
    GameObject ClosedNoise;
    bool soundPlayed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ClosedNoise = Resources.Load<GameObject>("sfxEmitters/doorOpen");
        if (!PowerGenerator)
        {
            Instantiate(ClosedNoise);
            Debug.Log("Object is Unpowered" + this.gameObject + "Will now be deleted.");            
            Destroy(this.gameObject);
        }
        timeRemainingOpen = TimeOpenedMax;
    }
    // Just a real simple sanity check. If there's no generator assigned, the gate auto-opens.

    // Update is called once per frame
    void Update()
    {
        SHITEASS1 =this.gameObject.transform.Find("Shitass1").gameObject;
        SHITEASS2 = this.gameObject.transform.Find("shitass2").gameObject;
        SHITEASS3 = this.gameObject.transform.Find("shitassCollision").gameObject;// uhh? Take child objects from this script's container?
        if (PowerGenerator) //if there IS a generator...
        {
            if (GateHasBeenOpened == false)
            {
                soundPlayed = false;
                SHITEASS1.SetActive(true); // The gate closes.
                SHITEASS2.SetActive(true); // The gate closes.
                SHITEASS3.SetActive(true); // The gate closes.
            }
            else if (GateHasBeenOpened && GateOpeningIsTimed) // else, if the gate is open and the gate has a timer...
            {
                SHITEASS1.SetActive(false); // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
                SHITEASS2.SetActive(false); // WHO'S IDEA WAS "this.gameObject.transform.Find("shitassCollision").gameObject;", ESPECIALLY IN A LANGUAGE THAT EMPHASISES PUTTING THINGS, ATTHEFRONT!
                SHITEASS3.SetActive(false); //set selected objects to Inactive?
                Soundplay();

                timeRemainingOpen -= Time.deltaTime; // start decrementing the timer.

                if (timeRemainingOpen <= 0)// if timer runs out...
                {
                    timeRemainingOpen = TimeOpenedMax; //reset timer.
                    GateHasBeenOpened = false; // close gate.

                }
            }
            else if (GateHasBeenOpened && !GateOpeningIsTimed) // else, if the gate is opened and not timed then...
            {

                SHITEASS1.SetActive(false); // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
                SHITEASS2.SetActive(false); // WHO'S IDEA WAS "this.gameObject.transform.Find("shitassCollision").gameObject;", ESPECIALLY IN A LANGUAGE THAT EMPHASISES PUTTING THINGS, ATTHEFRONT!
                SHITEASS3.SetActive(false); //set selected objects to Inactive?
                Soundplay();
            }
        }
    }
    void Soundplay()
    {
        if (!soundPlayed) { Instantiate(ClosedNoise); soundPlayed = true; }
    }
}
#region ARGH2
// THERE  BETTER BE NO MORE FUCKING PROBLEMS, OR ELSE I'M GONNA FUCKING LOSE IT!
// UNDER NO CIRCUMSTANCES WOULD ANY NORMAL PERSON EVER BE EXPECTED TO KNOW THAT
//     "this.gameObject.transform.Find("shitassCollision").gameObject;"
// WORKS LIKE >THAT<!
// NOBODY IS! SERIOUSLY, WHO DECIDED THAT!? LEMME AT 'EM!
#endregion
