using Unity.VisualScripting;
using UnityEngine;

public class SoundEffectStorage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameObject openMenuSfx, selectMenuSfx, confirmMenuSfx, backMenuSfx, errorSfx;
    void Start()
    {
        selectMenuSfx = Resources.Load("sfxEmitters/MenuSelect").GameObject();
        confirmMenuSfx = Resources.Load("sfxEmitters/MenuConfirm").GameObject();
        backMenuSfx = Resources.Load("sfxEmitters/MenuBack").GameObject();
        errorSfx = Resources.Load("sfxEmitters/ErrorSfx").GameObject();
        openMenuSfx = Resources.Load("sfxEmitters/OpenMenu").GameObject();
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
