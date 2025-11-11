using UnityEngine;
using DG.Tweening;

public class PlayerThrowBellState : PlayerBaseState
{
    private readonly int ChainAnim = Animator.StringToHash("Swing");

    private Transform whipBase;
    private LineRenderer lr;
    private GameObject bell;

    private Vector3 grabPoint;
    private Vector3 currentWhipPosition;
    private bool pulling;
    private bool isRetracting;

    // Whip parameters
    public int quality = 500;
    private Spring spring;
    public float damper = 14;
    public float strength = 800;
    public float velocity = 50;
    //public float waveCount = 3;
    //public float waveHeight = 1;
    public float whipSpeed = 50f; //making this faster
    public float snapBackSpeed = 50f;

    /*private AnimationCurve affectCurve;
    
    private float whipAnimationTimer = 0f;
    public float whipDuration = 0.4f;
    public float snapBackDuration = 0.3f;
    */
    
    public PlayerThrowBellState(PlayerStateMachine stateMachine, Vector3 hitInfo) : base(stateMachine)
    {
        lr = stateMachine.WhipLine;
        whipBase = stateMachine.WhipBase;
        //affectCurve = stateMachine.whipCurve;
        spring = new Spring();
        spring.SetTarget(1f); // needed for visible wave
        bell = stateMachine.BellGameObject;
        grabPoint = hitInfo;
    }

    public override void Enter()
    {
        whipBase.gameObject.SetActive(true);
        stateMachine.Animator.Play(ChainAnim);
        pulling = true;
        isRetracting = false;
        //whipAnimationTimer = 0f;

        lr.enabled = true;
        lr.positionCount = quality + 1;
        for (int i = 0; i <= quality; i++)
            lr.SetPosition(i, whipBase.position);

        spring.SetVelocity(velocity);
        currentWhipPosition = whipBase.position;

        if (bell != null)
        {
            bell.SetActive(true);
            bell.transform.position = whipBase.position;
        }
    }

    public override void Tick(float deltaTime)
    {
        if (!pulling) return;

        //whipAnimationTimer += deltaTime;
        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(deltaTime);

        if (!isRetracting)
        {
            currentWhipPosition = Vector3.MoveTowards(currentWhipPosition, grabPoint, deltaTime * whipSpeed);

            if (Vector3.Distance(currentWhipPosition, grabPoint) < 0.05f)
            {
                InstantiateSphere();
                isRetracting = true;
                //whipAnimationTimer = 0f;
            }
        }
        else
        {
            currentWhipPosition = Vector3.MoveTowards(currentWhipPosition, whipBase.position, deltaTime * snapBackSpeed);

            if (Vector3.Distance(currentWhipPosition, whipBase.position) < 0.05f)
            {
                StopPull();
                return;
            }
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
            //float waveIntensity = spring.Value * affectCurve.Evaluate(delta);
            //Removing all animation stuff
            //Vector3 waveOffset = up * (waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * waveIntensity);
            //Vector3 position = Vector3.Lerp(whipBase.position, currentWhipPosition, delta) + waveOffset;
            Vector3 position = Vector3.Lerp(whipBase.position, currentWhipPosition, delta);
            lr.SetPosition(i, position);
        }

        if (bell != null)
            bell.transform.position = currentWhipPosition;
    }

    private void InstantiateSphere()
    {
        Object.Instantiate(stateMachine.visualSpherePrefab, grabPoint, Quaternion.identity);
    }

    private void StopPull()
    {
        pulling = false;
        lr.positionCount = 0;
        lr.enabled = false;

        if (bell != null)
            bell.SetActive(false);

        stateMachine.SwitchState(new PlayerMoveState(stateMachine));
    }

    public override void Exit()
    {
        pulling = false;
        lr.enabled = false;

        if (bell != null)
            bell.SetActive(false);
    }
}
