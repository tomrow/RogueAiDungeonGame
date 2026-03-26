using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    MeshRenderer mr;
    Material mat;
    float duration = 0.2f;
    float timer;
    Transform player;
    AudioSource sfx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sfx = GetComponent<AudioSource>();
        timer = 0;
        mr = GetComponent<MeshRenderer>();
        player = GameObject.Find("DungeonPlayer").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = player.position;
        transform.LookAt(Camera.main.transform.position);
        transform.Translate(Vector3.forward);
        transform.Rotate(180, 0, 0);
        timer += Time.deltaTime;
        transform.localScale = Vector3.one * (timer / duration);
        mr.material.mainTextureOffset = Vector2.left * (Mathf.Floor(timer * 8) / 4);
        mr.material.color = new Color(1, 1, 1, Mathf.Sin((timer/duration)*Mathf.PI));
        if (timer > duration)
        { sfx.Stop(); Destroy(sfx); Destroy(mr.material); Destroy(mr); Destroy(gameObject); }

    }
}
