using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] private GameObject[] catchableHands;

    //TODO: For both hands, currently both are enabled at once
    public void EnableDamageDealing(int hand)
    {
        //left and right hands
        catchableHands[hand].SetActive(true);
    }
    
    public void DisableDamageDealing(int hand)
    {
        catchableHands[hand].SetActive(false);
    }
}
