using JetBrains.Annotations;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFundamentals : MonoBehaviour
{
    public float hp;
    public bool dead;
    public int expAward;
    public float KnockBackTimer;
    public GameObject smoke;
    GameObject healthHudPrefab;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthHudPrefab = Resources.Load<GameObject>("AttackHudAnim");
        if (smoke == null) { smoke = Resources.Load("EnemyExplosion").GameObject(); }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        KnockBackTimer -= Time.fixedDeltaTime;
        if( KnockBackTimer <= 0) { KnockBackTimer = 0; }
    }
    public void Die()
    {
        PlayerHealth.AwardXP(expAward);
        Instantiate(smoke, transform.position, Quaternion.identity);
        try { Instantiate(LootTables.currentLootDrops[UnityEngine.Random.Range(0, LootTables.currentLootDrops.Count - 1)], transform.position, Quaternion.identity); } catch { Debug.Log("Loot not working or not initialized"); }
        Destroy(gameObject); //todo: proper death animation and drop items
    }
    public void Damage(float amount)
    {
        GameObject healthHudElement;
        healthHudElement = Instantiate(healthHudPrefab, transform.Find("/Canvas"));
        healthHudElement.GetComponent<AttackHudAnim>().subject = this.transform;
        healthHudElement.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        healthHudElement.GetComponent<Text>().text = ((int)amount).ToString();
        

        hp -=(int)amount;
        KnockBackTimer += amount / 6;
        if(hp <= 0) { Die(); }
    }
}
