using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerThrowBellState : PlayerBaseState
{
    private readonly int ChainAnim = Animator.StringToHash("Swing");
    
    Transform cameraPos, whipBase;
    private float maxDistance;

    private GameObject bell;
    
    private Vector3 grabPoint;
    private LineRenderer lr;

    private bool pulling;

    public int quality = 500;
    
    private Spring spring;
    public float damper = 14;
    public float strength = 800; 
    public float velocity = 50;
    public float waveCount = 3;
    public float waveHeight = 1;
    
    public float baseWidth = 0.3f; //trying to make it tapered like a whip so it doesn't look so much like a line  
    public float tipWidth = 0.05f;  
    private AnimationCurve taperCurve;
    
    private Vector3 currentWhipPosition;
    private float whipAnimationTimer = 0f; 
    public float whipDuration = 0.5f;
    public float snapBackDuration = 0.3f;
    
    private AnimationCurve affectCurve;
    
    // New variables for downward strike
    private Vector3 whipStartPosition;
    private AnimationCurve strikeArcCurve;
    private AnimationCurve snapBackCurve;
    private float arcHeight = 1f; //arc the same for everything even close stuff because ugh
    private bool isSnappingBack = false;
    
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
        bell = stateMachine.BellGameObject;
        
        //Doing all the animation stuff in code because linerenderer
        //Maybe we should go back to the animation bones methods oopsies :)
        strikeArcCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 2f),
            new Keyframe(0.3f, 1f, 0f, 0f),
            new Keyframe(1f, 0f, -3f, 0f)
        );
        
        snapBackCurve = new AnimationCurve(
            new Keyframe(0f, 1f, -3f, -3f),
            new Keyframe(0.7f, 0.2f, -1f, -1f),
            new Keyframe(1f, 0f, 0f, 0f)
        );
    }
    
    public override void Enter()
    {
        stateMachine.Animator.Play(ChainAnim);
        pulling = true;
        whipAnimationTimer = 0f;
        isSnappingBack = false;
        
        lr.enabled = true;
        lr.positionCount = quality + 1;
        spring.SetVelocity(velocity);
        
        whipStartPosition = whipBase.position + Vector3.up * 2f;
        currentWhipPosition = whipStartPosition;
        
        //Making bell not visible, assume this will be all figured out with animations idkkk
        if (bell != null)
        {
            bell.SetActive(true);
            bell.transform.position = whipStartPosition;
        }
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

        Vector3 targetPosition;
        float waveIntensityMultiplier = 1f;
        
        if (!isSnappingBack)
        {
            float strikeProgress = Mathf.Clamp01(whipAnimationTimer / whipDuration);
            float currentArcHeight = strikeArcCurve.Evaluate(strikeProgress) * arcHeight;
            targetPosition = Vector3.Lerp(whipStartPosition, grabPoint, strikeProgress);
            targetPosition += Vector3.up * currentArcHeight;
            waveIntensityMultiplier = 1f + strikeProgress * 2f;
            currentWhipPosition = Vector3.Lerp(currentWhipPosition, targetPosition, Time.deltaTime * 15f);
        }
        else
        {
            float snapProgress = Mathf.Clamp01((whipAnimationTimer - whipDuration) / snapBackDuration);
            float snapCurveValue = snapBackCurve.Evaluate(snapProgress);
            targetPosition = Vector3.Lerp(whipBase.position, grabPoint, snapCurveValue);
            waveIntensityMultiplier = 2f + (1f - snapProgress) * 3f; // More intense at start of snap
            currentWhipPosition = Vector3.Lerp(currentWhipPosition, targetPosition, Time.deltaTime * 25f); //faster on the snapback
        }
        
        Vector3 whipDirection = (currentWhipPosition - whipBase.position).normalized;
        
        var up = Quaternion.LookRotation(whipDirection) * Vector3.up;
        var right = Quaternion.LookRotation(whipDirection) * Vector3.right;
        
        Vector3 lastPosition = whipBase.position;
        for (var i = 0; i < quality + 1; i++) 
        {
            var delta = i / (float) quality;
            float waveIntensity = spring.Value * affectCurve.Evaluate(delta) * waveIntensityMultiplier;
            var waveOffset = up * (waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * waveIntensity);
            var swingOffset = right * (waveHeight * 0.5f * Mathf.Sin(delta * waveCount * Mathf.PI) * waveIntensity);
            
            Vector3 position = Vector3.Lerp(whipBase.position, currentWhipPosition, delta) + waveOffset + swingOffset;
            lr.SetPosition(i, position);
            lastPosition = position;
        }
        
        bell.transform.position = lastPosition;
    }

    public override void Exit()
    {
        pulling = false;
        lr.positionCount = 0;
        lr.enabled = false;
        bell.SetActive(false);
    }

    public override void Tick(float deltaTime)
    {
        if (pulling)
        {
            whipAnimationTimer += deltaTime;
            
            if (!isSnappingBack && whipAnimationTimer >= whipDuration)
            {
                isSnappingBack = true;
                spring.SetVelocity(velocity * 1.5f);
            }
            
            DrawWhip();
        }
        
        bool animationComplete = stateMachine.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
        bool snapBackComplete = isSnappingBack && whipAnimationTimer >= (whipDuration + snapBackDuration);
        
        if (animationComplete && snapBackComplete)
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