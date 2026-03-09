using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float attackPower;
    public bool hitscan;
    public float speed;
    public float maxRange;
    RaycastHit rayout;
    float travel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(hitscan == false)
        { 
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            travel += Time.deltaTime * speed;
            if (Physics.Raycast(transform.position, transform.forward, out rayout, Time.deltaTime * speed * 2f))
            {
                try { rayout.collider.gameObject.GetComponent<EnemyFundamentals>().Damage(Random.Range(attackPower * 0.8f, attackPower * 1.2f)); }
                finally 
                {
                    Debug.Log(rayout.point);
                    Debug.Log(rayout.transform.gameObject.name);
                    //this.enabled = false;
                    Destroy(gameObject); 
                }
            }
        }
        else 
        {
            if (Physics.Raycast(transform.position, transform.forward, out rayout, maxRange))
            {
                Debug.DrawRay(transform.position, transform.forward * maxRange, Color.red);
                try { rayout.collider.gameObject.GetComponent<EnemyFundamentals>().Damage(Random.Range(attackPower * 0.8f, attackPower * 1.2f)); }
                finally
                {
                    Debug.Log(rayout.point);
                    Debug.Log(rayout.transform.gameObject.name);
                    //this.enabled = false;
                    Destroy(gameObject); 
                }
            }
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        try { collision.gameObject.GetComponent<EnemyFundamentals>().Damage(Random.Range(attackPower * 0.8f, attackPower * 1.2f)); }
        finally
        {
            Debug.Log(rayout.point);
            Debug.Log(rayout.transform.gameObject.name);
            //this.enabled = false;
            Destroy(gameObject);
        }
    }
}
