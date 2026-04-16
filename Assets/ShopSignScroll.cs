using UnityEngine;

public class ShopSignScroll : MonoBehaviour
{
    MeshRenderer mr;
    float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
        mr = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime / 3;
        timer = Mathf.Repeat(timer, 1);
        mr.material.mainTextureOffset = Vector2.right * timer;
    }
}
