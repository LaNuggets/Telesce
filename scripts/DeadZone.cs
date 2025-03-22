using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public Transform spawnPoint;

	void OnTriggerEnter2D(Collider2D player) 
	{
		if (player.CompareTag("Player"))
		{
            player.transform.position = spawnPoint.position;
        }
	} 
}
