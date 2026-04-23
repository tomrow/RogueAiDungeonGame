using UnityEngine;

public class XkullBrain : MonoBehaviour
{
    public Vector3 rotateDegreesPerSecond;
    public Vector3 translatePerSecond;
    public float followPlayerSpeed;
    Vector3 dirToPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(translatePerSecond*Time.deltaTime);
        transform.Rotate(rotateDegreesPerSecond*Time.deltaTime);
        dirToPlayer = (PlayerHealth.thisPlayer.transform.position - transform.position).normalized ;
        transform.position += dirToPlayer * followPlayerSpeed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerCtl p = other.gameObject.GetComponent<PlayerCtl>();
        try
        { p.DamageFrom(transform, 6, 1); }
        catch { }
        transform.Rotate(0, 180, 0);
        transform.Translate(translatePerSecond * Time.deltaTime);
    }
}
