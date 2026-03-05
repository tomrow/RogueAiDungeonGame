using JetBrains.Annotations;
using UnityEngine;

public class EnemyFundamentals : MonoBehaviour
{
    public float hp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Die()
    {
        Destroy(gameObject); //todo: proper death animation and drop items
    }
    public void Damage(float amount)
    {
        hp-=amount;
        if(hp <= 0) { Die(); }
    }
}
