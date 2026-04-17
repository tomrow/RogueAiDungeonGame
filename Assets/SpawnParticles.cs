using UnityEngine;

public class SpawnParticles : MonoBehaviour
{
    public GameObject particles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try { Instantiate(particles, transform.position, transform.rotation); } catch { Debug.Log("no particles to spawn"); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
