using UnityEngine;

public class LaserGateArcAnimation : MonoBehaviour
{
    MeshRenderer mr;
    Material mat;
    public float speed;
    float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * speed;
        timer = Mathf.Repeat(timer, 1);
        mr.material.mainTextureOffset = Vector2.right * (Mathf.Floor(timer*3) / 3);
    }
}
