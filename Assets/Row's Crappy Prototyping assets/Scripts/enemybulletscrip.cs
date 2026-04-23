using UnityEngine;

public class enemybulletscrip : MonoBehaviour
{
    #region Variables
    [SerializeField, Tooltip("Variable damage amount")] int damagenumber;
    [SerializeField, Tooltip("Variable Stun timer. Temporary?")] int stuntimertemp;
    [SerializeField, Tooltip("Variable Projectile speed?")] float ProjectileSpeed;
    [SerializeField, Tooltip("IS This A Melee Attack?")] bool IsMelee;
    [SerializeField] float MeleeAttackScaleVar;
    [SerializeField, Tooltip("Lifespan of projectile in Seconds.")] float ProjectileLifespan;
    [SerializeField, Tooltip("Scale Of Melee Attack. Currently Unused.")] float ProjectileScale;
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (IsMelee)
        {
            ProjectileLifespan = 0.5f;
        }
        /*Do we want some kind of algorithmic stun time?
         Something to think about.*/
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsMelee)
        {
            this.transform.position += (this.transform.forward * ProjectileSpeed)* Time.deltaTime;
        }
        else
        {
            this.transform.localScale = Vector3.one * ProjectileScale;
        }
        if (ProjectileLifespan <=0)
        {
            Destroy(this.gameObject);
        }
        ProjectileLifespan -= Time.deltaTime;
    }
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("I've hit something!");
        try
        {
            PlayerCtl p = collision.gameObject.GetComponent<PlayerCtl>();
            if (p != null) { p.DamageFrom(this.transform, damagenumber, stuntimertemp); } //using the player's Playerctrl script, sets position, Power, and stun timer value of an attack.
                                                                       //
            #region Collapsed Commentary on damage-stun script
            //p.DamageFrom(this.transform, damagenumber, damagenumber*1.5f);
            //Alternative, In the case we want to algorithmically stun players for getting hit.
            //Ideally, this would also be modular.
            //hooray, more [serializefield]'s!
            #endregion
        }
        finally
        {
            if (collision.gameObject.GetComponent<PlayerCtl>())
            {
                Destroy(this.gameObject);
            }
            else if (collision.gameObject.GetComponent<Terrain>())
            {
                Destroy(this.gameObject);
            }
        }
    }
}
