using UnityEngine;

public class Despawn : MonoBehaviour
{
    public float duration = 2;
    float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        timer+= Time.deltaTime;
        if(timer > duration) { Destroy(gameObject); }
    }
}
