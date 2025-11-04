using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;        
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
        {
            Die();
        }
        Debug.Log(currentHealth);
        currentHealth = Mathf.Max(0, currentHealth - damage);

        StartCoroutine(Flash());
        
    }

    private void Die()
    {
        //FOR NOW, JUST TESTING
        //Add death anim
        Destroy(gameObject);
    }

    private IEnumerator Flash()
    {
        //TODO: Change this, it's just testing how this looks and it's dumb

        GetComponent<MeshRenderer>().material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        GetComponent<MeshRenderer>().material.color = Color.red;
        
    }
}
