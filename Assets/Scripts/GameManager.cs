using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameEvent startingEvent;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Debug.Log("Starting the Level");
        Cursor.visible = false;
        startingEvent.TriggerEvent();
    }
}
