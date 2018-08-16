using UnityEngine;


public class DeleteGameObject : MonoBehaviour
{
    private const string TAG_BLOCK = "Block", TAG_STICK = "Stick";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TAG_BLOCK) || other.CompareTag(TAG_STICK))
        {
            Destroy(other.gameObject);
        }
    }
}