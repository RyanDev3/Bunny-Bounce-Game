using UnityEngine;

public class AbilityPickupManager : MonoBehaviour
{
    public static AbilityPickupManager Instance;
    public int CurrentAbilityLevel = 1;

    private void Awake()
    {
        Instance = this;
    }

    public void IncreaseAbilityLevel(int v)
    {
        CurrentAbilityLevel += v;
    }
}
