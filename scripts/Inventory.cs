using UnityEngine;
using UnityEngine.UI;


public class Inventory : MonoBehaviour
{
    public int bonesCount;
    public Text bonesCountText;
    public static Inventory instance;

    private void Awake()
    {
        if(instance != null){
            Debug.LogWarning("There are more than one instance of 'Inventory' in the scene");
            return;
        }
        instance = this;
    }

    public void AddBones(int count)
    {
        bonesCount += count;
        bonesCountText.text = bonesCount.ToString();
    }
}
