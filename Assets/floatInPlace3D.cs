using UnityEngine;

public class floatInPlace3D : MonoBehaviour
{
    Vector3 initialPosition;
    Vector3 initialRotation;
    public Vector3 movementRange, rotationRange, oscTranslateSpeed, oscRotateSpeed;
    Vector3 oscTimerMove, oscTimerRot;
    Vector3 ofsPos, ofsRot;
    Vector3 sinVector3(Vector3 angleCollection)
    {
        return new Vector3(Mathf.Sin(angleCollection.x), Mathf.Sin(angleCollection.y), Mathf.Sin(angleCollection.z));
    }
    Vector3 capVector3(Vector3 vin)
    {
        Vector3 values = vin;
        while (values.x > (Mathf.PI * 2))
        { values.x -= Mathf.PI * 2; }
        while (values.y > (Mathf.PI * 2))
        { values.y -= Mathf.PI * 2; }
        while (values.z > (Mathf.PI * 2))
        { values.z -= Mathf.PI * 2; }
        return values;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localEulerAngles;
        oscTimerMove = new Vector3(Random.Range(0,Mathf.PI * 2), Random.Range(0, Mathf.PI * 2), Random.Range(0, Mathf.PI * 2));
        oscTimerRot  = new Vector3(Random.Range(0,Mathf.PI * 2), Random.Range(0, Mathf.PI * 2), Random.Range(0, Mathf.PI * 2));
    }

    // Update is called once per frame
    void Update()
    {
        oscTimerMove += (oscTranslateSpeed * Time.deltaTime);
        oscTimerMove = capVector3(oscTimerMove);
        ofsPos = Vector3.Scale(sinVector3(oscTimerMove), movementRange);
        oscTimerRot += (oscRotateSpeed * Time.deltaTime);
        oscTimerRot = capVector3(oscTimerRot);
        ofsRot = Vector3.Scale(sinVector3(oscTimerRot), rotationRange);
        transform.localPosition = initialPosition + ofsPos;
        transform.localEulerAngles = initialRotation + ofsRot;
    }
}
