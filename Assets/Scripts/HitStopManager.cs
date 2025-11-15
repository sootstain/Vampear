using UnityEngine;
using System.Collections;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance { get; private set; }

    [SerializeField] private float minTimeScale = 0.05f;

    private float _restoreDelay; 
    private Coroutine _routine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void TriggerHitStop(float duration)
    {
        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(HitstopRoutine(duration));
    }

    private IEnumerator HitstopRoutine(float duration)
    {
        float original = Time.timeScale;

        // freeze-ish
        Time.timeScale = minTimeScale; 
        _restoreDelay = duration;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = original;
        _routine = null;
    }
}