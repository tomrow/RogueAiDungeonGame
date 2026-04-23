using Unity.VisualScripting;
using UnityEngine;

public class SoundEffectStorage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameObject openMenuSfx, selectMenuSfx, confirmMenuSfx, backMenuSfx, errorSfx;
    void Start()
    {
        SoundEffectStorage.selectMenuSfx = Resources.Load("sfxEmitters/MenuSelect").GameObject();
        SoundEffectStorage.confirmMenuSfx = Resources.Load("sfxEmitters/MenuConfirm").GameObject();
        SoundEffectStorage.backMenuSfx = Resources.Load("sfxEmitters/MenuBack").GameObject();
        SoundEffectStorage.errorSfx = Resources.Load("sfxEmitters/ErrorSfx").GameObject();
        SoundEffectStorage.openMenuSfx = Resources.Load("sfxEmitters/OpenMenu").GameObject();
        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
