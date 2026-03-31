using UnityEngine;

public class AttackHudAnim : MonoBehaviour
{
    float duration = 1;
    float timer=0;
    Vector3 position;
    public Transform subject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        try { position = Camera.main.WorldToScreenPoint(subject.position); } catch { }
        transform.position = position + Vector3.up * Mathf.Atan(timer * 2) * 16;
        timer += Time.deltaTime;
        if(timer > duration) { Destroy(gameObject); }
    }
}
