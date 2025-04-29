using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
    Dictionary<int, bool> abilities = new Dictionary<int, bool>()
    {
        {0, true },
        {1, false },
        {2, false }
    };

    int currentAbility = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SwitchAbility();
        ActivateAbility();
    }

    void SwitchAbility()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            IncrementAbility();
        }
    }

    void IncrementAbility()
    {
        if (currentAbility + 1 == abilities.Count)
        {
            currentAbility = 0;
            
        }
        else
        {
            currentAbility++;
        }

        if (!CheckAbility())
        {
            print("Ability at " + currentAbility + " is locked. Moving to next ability");
            IncrementAbility();
        }
        else
        {
            print("Current ability: " + currentAbility);
        }
    }

    bool CheckAbility()
    {
        return abilities[currentAbility];
    }

    void ActivateAbility()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            abilities[1] = true;
        }

        if (Input.GetKeyDown(KeyCode.RightAlt))
        {
            abilities[2] = true;
        }
    }
}
