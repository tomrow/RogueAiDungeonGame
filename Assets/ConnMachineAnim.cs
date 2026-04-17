using System.Threading;
using UnityEngine;

public class ConnMachineAnim : MonoBehaviour
{
    public bool slump;
    Transform pole;
    Transform axle;
    float rotSpeed = 400;
    Vector3 slumpPos = new Vector3(0, 180, -120);
    Vector3 normalPos;
    float timer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pole = transform.Find("joint1");
        axle = pole.Find("joint2");
        normalPos = pole.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        timer = Mathf.Clamp01(timer +  (Time.deltaTime*2));
        if (slump) 
        {
            if (rotSpeed > 0) { rotSpeed -= 400 * Time.deltaTime; }
            if (rotSpeed<0) { rotSpeed = 0; }
            //if (timer < 1) { pole.Rotate(Vector3.forward * Mathf.Cos(timer * Mathf.PI) * Time.deltaTime); }
        }
        axle.Rotate(0, rotSpeed * Time.deltaTime, 0, Space.World);
    }
}
