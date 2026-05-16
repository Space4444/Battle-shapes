using UnityEngine;

public class CameraFollowing : MonoBehaviour {

    public Transform player;

    private void Update()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -10);
    }

}
