using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrappleProjectile : MonoBehaviour
{
    [SerializeField] private Transform ropeStartPoint;

    [SerializeField] private float projectileInitialSpeed;
    [SerializeField] private float projectileSpeedIncreaseRate;
    [SerializeField] private float maxTravelTime = 3f;
    private float elapsedTravelTime = 0f;

    private Transform ropeEndPoint;

    private LineRenderer lRenderer;
    private Rigidbody rb;

    private PlayerControllerStateMachine SM;
    private Vector3 ropeEndOffset = Vector3.zero;

    public void Awake()
    {
        lRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Transform ropeEnd, PlayerControllerStateMachine newSM)
    {
        ropeEndPoint = ropeEnd;
        SM = newSM;
        
    }

    public void Initialize(Transform ropeEnd, Vector3 offset,PlayerControllerStateMachine newSM)
    {
        ropeEndPoint = ropeEnd;
        SM = newSM;
        ropeEndOffset = offset; 

    }

    private void Start()
    {
        rb.velocity = transform.forward * projectileInitialSpeed;    
    }

    private void Update()
    {
        UpdateLineRendererPositions();
        UpdateProjectileMotion();


        UpdateTravelTime();
    }

    private void UpdateTravelTime()
    {
        elapsedTravelTime += Time.deltaTime; 

        if (elapsedTravelTime >= maxTravelTime)
        {
            //reset grapple
            rb.velocity = Vector3.zero;
            if (SM.GetCurrentStateFlag() == PlayerStates.AIM_GRAPPLE)
            {
                PlayerStateAimGrapple state = (PlayerStateAimGrapple)SM.GetCurrentPlayerState();
                state.ResetGrappleAnimation();
            }
            Destroy(this.gameObject);
        }
    }

    private void UpdateProjectileMotion()
    {
        rb.velocity += transform.forward * Time.deltaTime * projectileSpeedIncreaseRate;
    }

    private void UpdateLineRendererPositions()
    {
        lRenderer.SetPosition(0, ropeStartPoint.position);
        lRenderer.SetPosition(1, ropeEndPoint.position + ropeEndOffset);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Grapplable"))
        {
            SM.TransitionState(PlayerStates.GRAPPLE_MOVE);
            PlayerStateGrappleMove state = (PlayerStateGrappleMove)SM.GetCurrentPlayerState();
            state.GiveGrapplePoint(collision.GetContact(0).point);
            state.ResetGrappleAnimation();
            Destroy(this.gameObject);
            return;
        }
        else
        {
            if (SM.GetCurrentStateFlag() == PlayerStates.AIM_GRAPPLE)
            {
                PlayerStateAimGrapple state = (PlayerStateAimGrapple)SM.GetCurrentPlayerState();
                state.ResetGrappleAnimation();
            }
            Destroy(this.gameObject);
        }
    }

    

}
