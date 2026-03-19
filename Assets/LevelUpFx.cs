using UnityEngine;

public class LevelUpFx : MonoBehaviour
{
    MeshRenderer mr;
    Material mat;
    float timer;
    Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
        mr = transform.GetChild(0).GetComponent<MeshRenderer>();
        player = GameObject.Find("DungeonPlayer").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position;
        transform.Rotate(0, Time.deltaTime * 6 * 360, 0);
        timer += Mathf.PI * Time.deltaTime;
        mr.material.color = new Color(1,1,1,Mathf.Sin(timer));
        if (timer > Mathf.PI)
        { Destroy(mr.material); Destroy(mr); Destroy(gameObject); }
    }
}
