using System.Threading;
using UnityEngine;
using UnityEngine.Scripting;

public class CameraController : MonoBehaviour
    {
        [SerializeField] private float speed;
        private float currentPosX = -25.9f;
        private Vector3 velocity = Vector3.zero;

        private void Update()
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, transform.position.y, transform.position.z), ref velocity, speed);
        }

        public void MoveToNewRoom(Transform _newRoom)
        {
            float roomWidth = 62f;
            currentPosX = _newRoom.position.x + (roomWidth / 2f);
        }
    }
