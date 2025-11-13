using UnityEngine;
using DG.Tweening;

public class PlayerThrowBellState : PlayerBaseState
{
    private Transform whipBase;
    private LineRenderer lr;
    private GameObject bell;

    private Vector3 targetDirection;
    private Vector3 currentWhipPosition;
    private float currentWhipLength;
    private bool pulling;
    private bool isRetracting;
    private bool hitSomething;

    public int quality = 500;
    private Spring spring;
    public float damper = 14;
    public float strength = 800;
    public float velocity = 50;
    public float waveCount = 3;
    public float waveHeight = 0.05f;
    public float whipSpeed = 50f;
    public float snapBackSpeed = 50f;
    public float maxWhipLength = 10f;

    private AnimationCurve affectCurve;
    
    private float whipAnimationTimer = 0f;
    public float whipDuration = 0.5f;
    public float snapBackDuration = 0.4f;
    
    private RectTransform crosshair;
    private Camera mainCamera;
    
    public PlayerThrowBellState(PlayerStateMachine stateMachine, RectTransform crosshairUI) : base(stateMachine)
    {
        lr = stateMachine.WhipLine;
        whipBase = stateMachine.WhipBase;
        affectCurve = stateMachine.whipCurve;
        spring = new Spring();
        spring.SetTarget(1f);
        bell = stateMachine.BellGameObject;
        crosshair = crosshairUI;
        mainCamera = Camera.main;
    }

    public override void Enter()
    {

        whipBase.gameObject.SetActive(true);
        stateMachine.Animator.SetTrigger("ThrewBell");
        pulling = true;
        isRetracting = false;
        hitSomething = false;
        whipAnimationTimer = 0f;
        currentWhipLength = 0f;

        Vector3 crosshairWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(
            crosshair.position.x, 
            crosshair.position.y, 
            mainCamera.nearClipPlane + 10f
        ));
        
        targetDirection = (crosshairWorldPos - whipBase.position).normalized;

        lr.enabled = true;
        lr.positionCount = quality + 1;
        for (int i = 0; i <= quality; i++)
            lr.SetPosition(i, whipBase.position);

        spring.SetVelocity(velocity);
        currentWhipPosition = whipBase.position;

        if (bell != null)
        {
            bell.transform.position = whipBase.position;
        }
    }

    public override void Tick(float deltaTime)
    {
        if (!pulling) return;

        whipAnimationTimer += deltaTime;
        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(deltaTime);

        if (!isRetracting)
        {
            currentWhipLength += deltaTime * whipSpeed;
            currentWhipPosition = whipBase.position + targetDirection * currentWhipLength;

            RaycastHit hit;
            if (Physics.Raycast(whipBase.position, targetDirection, out hit, currentWhipLength))
            {
                currentWhipPosition = hit.point;
                hitSomething = true;
                InstantiateSphere(hit.point);
                isRetracting = true;
                whipAnimationTimer = 0f;
                currentWhipLength = Vector3.Distance(whipBase.position, currentWhipPosition);
            }
            //Changed this to max length as we're using a crosshair; so it can extend outwards until reaching max length
            else if (currentWhipLength >= maxWhipLength)
            {
                currentWhipPosition = whipBase.position + targetDirection * maxWhipLength;
                currentWhipLength = maxWhipLength;
                isRetracting = true;
                whipAnimationTimer = 0f;
            }
        }
        else
        {
            currentWhipLength -= deltaTime * snapBackSpeed;
            
            if (currentWhipLength <= 0f)
            {
                StopPull();
                return;
            }
            
            currentWhipPosition = whipBase.position + targetDirection * currentWhipLength;
        }

        DrawWhip();
    }

    private void DrawWhip()
    {
        Vector3 whipDirection = (currentWhipPosition - whipBase.position).normalized;
        var up = Quaternion.LookRotation(whipDirection) * Vector3.up;
        
        for (int i = 0; i <= quality; i++)
        {
            float delta = i / (float)quality;
            float waveIntensity = spring.Value * affectCurve.Evaluate(delta);
            Vector3 waveOffset = up * (waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * waveIntensity);
            Vector3 position = Vector3.Lerp(whipBase.position, currentWhipPosition, delta) + waveOffset;
            lr.SetPosition(i, position);
        }

        if (bell != null)
            bell.transform.position = lr.GetPosition(lr.positionCount - 1);
    }

    private void InstantiateSphere(Vector3 position)
    {
        Object.Instantiate(stateMachine.visualSpherePrefab, position, Quaternion.identity);
    }

    private void StopPull()
    {
        pulling = false;
        lr.positionCount = 0;
        lr.enabled = false;

        stateMachine.SwitchState(new PlayerMoveState(stateMachine));
    }

    public override void Exit()
    {
        stateMachine.Animator.SetBool("ThrewBell", false);
        pulling = false;
        lr.enabled = false;
    }
}