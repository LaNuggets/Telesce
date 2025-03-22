using UnityEngine;
using System.Collections;
using System.Drawing;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform previousRoom;
    [SerializeField] private Transform nextRoom;
    [SerializeField] private CameraController cam;
    [SerializeField] private Camera cam2;
    [SerializeField] private Transform spawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.transform.position.x < transform.position.x)
            {
                cam.MoveToNewRoom(nextRoom);
                cam2.orthographicSize = 22f;
                collision.transform.position = spawnPoint.position;
            }else{
                cam.MoveToNewRoom(previousRoom);
            }
        }
    }
}