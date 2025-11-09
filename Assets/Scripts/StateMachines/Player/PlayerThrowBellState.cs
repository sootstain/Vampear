using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerThrowBellState : PlayerBaseState
{
    //private readonly int PullAnim = Animator.StringToHash("Pull");
    private readonly int ChainAnim = Animator.StringToHash("Swing");
    
    Transform cameraPos, whipBase;
    private float maxDistance;
    
    private Vector3 grabPoint;
    private LineRenderer lr;

    private bool pulling;

    public int quality = 500; //how many parts in the chain, assigning here instead of player FOR NOW
    
    private Spring spring;
    public float damper = 14;
    public float strength = 800; 
    public float velocity = 50;
    public float waveCount = 3;
    public float waveHeight = 1;
    
    private Vector3 currentWhipPosition;
    private float whipAnimationTimer = 0f; 
    public float whipDuration = 0.5f;
    
    private AnimationCurve affectCurve;
    
    public PlayerThrowBellState(PlayerStateMachine stateMachine, Vector3 hitInfo) : base(stateMachine)
    {
        cameraPos = stateMachine.MainCameraPosition;
        maxDistance = stateMachine.WhipLength;
        lr = stateMachine.WhipLine;
        spring = new Spring();
        spring.SetTarget(0);
        whipBase = stateMachine.WhipBase;
        affectCurve = stateMachine.whipCurve;
        currentWhipPosition = whipBase.position;
        grabPoint = hitInfo;
    }
    
    public override void Enter()
    {
        
        stateMachine.Animator.Play(ChainAnim);
        pulling = true;
        whipAnimationTimer = 0f;
        
        lr.enabled = true;
        lr.positionCount = quality + 1;
        spring.SetVelocity(velocity);
        
    }

    private void DrawWhip()
    {
        if (lr.positionCount != quality + 1) 
        {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
        }
        
        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        Vector3 whipDirection = (grabPoint - whipBase.position).normalized;
        
        var up = Quaternion.LookRotation(whipDirection) * Vector3.up;
        var right = Quaternion.LookRotation(whipDirection) * Vector3.right;
        
        currentWhipPosition = Vector3.Lerp(currentWhipPosition, grabPoint, Time.deltaTime *12f);
        
        for (var i = 0; i < quality + 1; i++) {
            var delta = i / (float) quality;
            var waveOffset = up * (waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta));
            var swingOffset = right * (waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta));
            lr.SetPosition(i, Vector3.Lerp(whipBase.position, currentWhipPosition, delta) + waveOffset + swingOffset);
        }
    }

    public override void Exit()
    {
        pulling = false;
        lr.positionCount = 0;
        lr.enabled = false;
    }

    public override void Tick(float deltaTime)
    {
        if (pulling)
        {
            whipAnimationTimer += deltaTime;
            DrawWhip();
        }
        
        bool animationComplete = stateMachine.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
        bool whipExtended = whipAnimationTimer >= whipDuration;
        if (animationComplete && whipExtended)
        {
            InstantiateSphere();
            StopPull();                
        }
    }

    private void InstantiateSphere()
    {
        var sphere = Object.Instantiate(stateMachine.visualSpherePrefab, grabPoint, Quaternion.identity);
        stateMachine.SwitchState(new PlayerMoveState(stateMachine));    
    }

    private void StopPull()
    {
        pulling = false;
        currentWhipPosition = whipBase.position;
        stateMachine.SwitchState(new PlayerMoveState(stateMachine));
    }
}
