using UnityEngine;

public class CrystalDestroySwitchChecker : MonoBehaviour
{

    public bool Check()
    {
        return (transform.childCount>0);
    }
}
