using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Target : MonoBehaviour
{
    public event Action<Target> OnDestroyed;

    private Rigidbody rb;
    private bool isBeingPulled = false;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    private void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }

    public void GetPulled(Vector3 targetPosition, float height, float duration)
    {
        if (!isBeingPulled)
        {
            StartCoroutine(ArcPull(targetPosition, height, duration));
        }
    }
    
    public IEnumerator ArcPull(Vector3 targetPosition, float trajectoryHeight, float duration)
    {
        //rb.linearVelocity = CalculatePullVelocity(transform.position, targetPosition, trajectoryHeight);
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;

        Vector3 start = transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            Vector3 pos = Vector3.Lerp(start, targetPosition, t);
            pos.y = Mathf.Sin(Mathf.PI * t) * trajectoryHeight + Mathf.Lerp(start.y, targetPosition.y, t);
            transform.position = pos;
            yield return null;
        }

        agent.enabled = true;
        isBeingPulled = false;
    }

    private Vector3 CalculatePullVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y; //could maybe integrate with ForceReceiver? So no rigidbody lol, will fix
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) +
                                               Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));
        return velocityXZ + velocityY;
    }
}
