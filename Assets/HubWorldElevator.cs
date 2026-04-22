using UnityEngine;

public class HubWorldElevator : MonoBehaviour
{
    bool enableElevator = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enableElevator)
        {
            PlayerHealth.thisPlayer.state = PlayerCtl.States.StandOnGoal;
            PlayerHealth.thisPlayer.transform.position += Vector3.down * Time.deltaTime; transform.position += Vector3.down * Time.deltaTime;
        }
    }
    public void StartElevator()
    { enableElevator = true; }
}
