using System.Collections.Generic;
using UnityEngine;

// This is ONLY the damage
// I don't think this approach needs to be optimised. No gain in disabling one claw
public class DealDamage : MonoBehaviour
{
    [SerializeField] private List<GameObject> catchableHands;

    //TODO: For both hands, currently both are enabled at once
    
    //For enemies, will change after we have enemy models.
    public void EnableDamageDealing()
    {
        foreach (var x in catchableHands)
        {
            x.SetActive(true);    
        }
        
    }
    
    public void DisableDamageDealing()
    {
        foreach (var x in catchableHands)
        {
            x.SetActive(false);    
        }
    }
    
}
