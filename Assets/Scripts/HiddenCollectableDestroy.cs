using UnityEngine;

public class HiddenCollectableDestroy : MonoBehaviour
{

    public int Value;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        { 
            Destroy(gameObject);
            HiddenCollecatbleManager.Instance.IncreaseCollectables(Value);
        }
    }

}
