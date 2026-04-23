using JetBrains.Annotations;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        Instantiate(LootTables.currentLootDrops[Random.Range(0, LootTables.currentLootDrops.Count-1)], transform.position, Quaternion.identity);
        Destroy(gameObject); //todo: proper death animation and drop items
    }
    public void Damage(float amount)
    {
        GameObject h = Instantiate(Resources.Load("AttackHudAnim").GameObject(), PlayerHealth.canvasObj.transform, false);
        h.GetComponent<AttackHudAnim>().subject = this.transform; h.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        h.GetComponent<Text>().text = ((int)amount).ToString();
        hp-=(int)amount;
        KnockBackTimer += amount / 6;
        if(hp <= 0) { Die(); }
    }
}
