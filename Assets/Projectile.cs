using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float attackPower;
    public bool hitscan;
    public float speed;
    public float maxRange;
    RaycastHit rayout;
    float travel;
    public GameObject muzzleFlash, explosion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try { Instantiate(muzzleFlash, transform.position, Quaternion.identity); } catch { }
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
                Debug.Log(rayout.collider.gameObject.name);
                try { rayout.collider.gameObject.GetComponent<EnemyFundamentals>().Damage(UnityEngine.Random.Range(attackPower * 0.8f, attackPower * 1.2f)); } catch(Exception e) { Debug.Log("Shot Failed!" + e.Message); }
                finally 
                {
                    DestroyOnHitWall();
                }
            }
        }
        else 
        {
            if (Physics.Raycast(transform.position, transform.forward, out rayout, maxRange))
            {
                Debug.DrawRay(transform.position, transform.forward * maxRange, Color.red);
                Debug.Log(rayout.collider.gameObject.name);
                try { rayout.collider.gameObject.GetComponent<EnemyFundamentals>().Damage(UnityEngine.Random.Range(attackPower * 0.8f, attackPower * 1.2f)); }catch (Exception e) { Debug.Log("Shot Failed!" + e.Message); }
                finally
                {
                    DestroyOnHitWall();
                }
            }
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        try { collision.gameObject.GetComponent<EnemyFundamentals>().Damage(UnityEngine.Random.Range(attackPower * 0.8f, attackPower * 1.2f)); }
        finally
        {
            DestroyOnHitWall();
        }
    }

    private void DestroyOnHitWall()
    {
        /*Debug.Log(rayout.point);
        Debug.Log(rayout.transform.gameObject.name);*/
        //this.enabled = false;
        try 
        { 
            Projectile explObj = Instantiate(explosion, transform.position, transform.rotation).GetComponent<Projectile>();
            explObj.attackPower = attackPower*0.7f;
        } catch { }
        Destroy(gameObject);
    }
}
