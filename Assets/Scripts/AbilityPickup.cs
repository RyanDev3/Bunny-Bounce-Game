using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public int AbilityAmount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            AbilityPickupManager.Instance.IncreaseAbilityLevel(AbilityAmount);
        }
    }
}
