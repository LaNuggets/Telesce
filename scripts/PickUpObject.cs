using UnityEngine;
using UnityEngine.UIElements;

public class PickUpObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision){

        if(collision.CompareTag("Player")){
            Inventory.instance.AddBones(1);
            Destroy(gameObject);
        }
    }
}
