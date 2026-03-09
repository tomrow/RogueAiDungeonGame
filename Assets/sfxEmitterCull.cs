using UnityEngine;

public class sfxEmitterCull : MonoBehaviour
{
    AudioSource s;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        s= GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!s.isPlaying)
        { Destroy(gameObject); }
    }
}
