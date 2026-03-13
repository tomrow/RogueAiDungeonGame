using UnityEngine;
using UnityEngine.UI;
public class spUiText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Text hudText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hudText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        hudText.text = "SP " + PlayerHealth.stamina;
    }
}
