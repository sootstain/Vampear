using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForceReceiver : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private NavMeshAgent agent;
    
    [SerializeField] private float drag = 0.3f;
    [SerializeField] private float baseGravity = -9.81f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2.0f;
    [SerializeField] private float apexHangGravityScale = 0.5f;
    [SerializeField] private float apexThreshold = 0.5f;
    [SerializeField] private float maxFallSpeed = -25f;
    

    private Vector3 dampingVelocity;
    private Vector3 impact;
    private float verticalVelocity;
    public bool GravityEnabled { get; set; } = true;
    
    public InputReader InputReader { get; set; }

    public Vector3 Movement => impact + Vector3.up * verticalVelocity;

    private void Update()
    {
        if (!GravityEnabled)
            return;
        
        ApplyVariableGravity(Time.deltaTime);

        impact = Vector3.SmoothDamp(impact, Vector3.zero, ref dampingVelocity, drag);

        if (agent != null)
        {
            if (impact.sqrMagnitude < 0.2f * 0.2f)
            {
                impact = Vector3.zero;
                agent.enabled = true;
            }
        }
    }
    
    private void ApplyVariableGravity(float deltaTime)
    {
        bool isGrounded = controller.isGrounded;
        float gravityForce = baseGravity;

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = baseGravity * deltaTime;
            return;
        }

        if (verticalVelocity > 0f)
        {
            if (InputReader != null && !InputReader.isJumpHeld)
            {
                gravityForce *= lowJumpMultiplier;
            }
            else if (Mathf.Abs(verticalVelocity) < apexThreshold)
            {
                gravityForce *= apexHangGravityScale;
            }
        }
        else if (verticalVelocity < 0f)
        {
            gravityForce *= fallMultiplier;
        }

        verticalVelocity += gravityForce * deltaTime;
        verticalVelocity = Mathf.Max(verticalVelocity, maxFallSpeed);
    }

    public void Reset()
    {
        impact = Vector3.zero;
        verticalVelocity = 0f;
    }

    public void AddForce(Vector3 force)
    {
        impact += force;
        if (agent != null)
        {
            agent.enabled = false;
        }
    }
    
    public void Jump(float jumpForce)
    {
        verticalVelocity += jumpForce;
    }

    public void SetJump(float force)
    {
        verticalVelocity = force;
    }

    public float GetVelocity()
    {
        return verticalVelocity;
    }
}
