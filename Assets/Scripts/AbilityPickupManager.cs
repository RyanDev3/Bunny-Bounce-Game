using System;
using UnityEngine;

public class AbilityPickupManager : MonoBehaviour
{
    public static AbilityPickupManager Instance;
    public int CurrentAbilityLevel = 1;

    public static Action<int> OnAbilityLevelIncrease;

    private void Awake()
    {
        Instance = this;
    }

    public void IncreaseAbilityLevel(int v)
    {
        CurrentAbilityLevel += v;
        OnAbilityLevelIncrease?.Invoke(CurrentAbilityLevel);
    }
}
