using System.Collections.Generic;
using UnityEngine;

public class PickingItemsInfo : MonoBehaviour {

    private float timer=0;
    private List<GameObject> infos;
    
    private void Start()
    {
        infos = StaticVars.game.infos;
    }
    // Update is called once per frame
    void FixedUpdate () {
        if (++timer > 100f)
        {
            infos.Remove(gameObject);
            for (int i = 0, count = infos.Count; i < count; i++) {
                GameObject info = infos[i];
                RectTransform t = info.GetComponent<RectTransform>();
                t.anchoredPosition = new Vector2(t.anchoredPosition.x, t.anchoredPosition.y+26f);
            }
            Destroy(gameObject);
        }
	}
}
