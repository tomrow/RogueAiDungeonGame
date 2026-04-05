using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value;
    public GameObject fx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void Update()
    {
        transform.Rotate(0, Time.deltaTime * 3, 0);
    }
    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCtl>() != null)
        {
            PlayerHealth.money += value; Instantiate(fx, transform.position, Quaternion.identity); Destroy(gameObject);
        }
    }
}
