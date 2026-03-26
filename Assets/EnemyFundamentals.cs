using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyFundamentals : MonoBehaviour
{
    public float hp;
    public bool dead;
    public int expAward;
    public float KnockBackTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        Destroy(gameObject); //todo: proper death animation and drop items
    }
    public void Damage(float amount)
    {
        GameObject h = Instantiate(Resources.Load("AttackHudAnim").GameObject(), PlayerHealth.canvasObj.transform, false);
        h.transform.position = Camera.main.WorldToViewportPoint(transform.position);
        hp-=amount;
        KnockBackTimer += amount / 6;
        if(hp <= 0) { Die(); }
    }
}
