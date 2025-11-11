using System.Collections.Generic;
using UnityEngine;

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
