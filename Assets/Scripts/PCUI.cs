using UnityEngine;

public class PCUI : MonoBehaviour
{
    private void Start()
    {
        #if UNITY_ANDROID
            gameObject.SetActive(false);
        #endif
    }
}