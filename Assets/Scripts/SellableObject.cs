using UnityEngine;

public class SellableObject : MonoBehaviour
{
    public string objectType;
    public float size;
    public float value;

    // private void Awake()
    // {
    //     size = transform.localScale.x * transform.localScale.y * transform.localScale.z;
    //     value = size * 10; // Get log value based on size
    // }
}