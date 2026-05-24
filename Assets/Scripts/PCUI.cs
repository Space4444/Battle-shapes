using UnityEngine;

public class PCUI : MonoBehaviour
{
    private void Start()
    {
        #if UNITY_WEBGL

            if (Application.isMobilePlatform)
                gameObject.SetActive(false);

        #elif UNITY_ANDROID

            gameObject.SetActive(false);

        #endif
    }
}