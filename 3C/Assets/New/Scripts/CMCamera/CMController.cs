using System;
using UnityEngine;

public class CMController : MonoBehaviour
{
    public float speed;
    public float controlRotationSensitivity;
    public PlayerInput playerInput;
    public enum ForwardMode { Camera, Player, World };
    public ForwardMode InputForward;
    
    private CMBody m_Body;
    private Vector3 laterDirection;
    private EaseMove dashMove;
    private EaseMove jumpMove;

    private Vector3 m_Velocity;
    private Vector3 m_MoveInput;
    public Vector2 ControlRotation { get; private set; }
    public CMBody Body => m_Body;

    protected void Awake()
    {
        if (playerInput == null)
        {
            playerInput = FindObjectOfType<PlayerInput>();
        }

        this.m_Body = this.GetComponent<CMBody>();

        this.laterDirection = Vector3.left;

        this.dashMove = new EaseMove(this.m_Body);
        this.jumpMove = new EaseMove(this.m_Body);
    }

    public void SetMovementInput(Vector2 moveInput)
    {
        Vector3 fwd;
        switch (InputForward)
        {
            case ForwardMode.Camera: fwd = Camera.main.transform.forward; break;
            case ForwardMode.Player: fwd = transform.forward; break;
            case ForwardMode.World: default: fwd = Vector3.forward; break;
        }

        fwd.y = 0;
        fwd = fwd.normalized;
        if (fwd.sqrMagnitude < 0.01f)
            return;

        Quaternion inputFrame = Quaternion.LookRotation(fwd, Vector3.up);
        //Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 input = new Vector3(moveInput.x,0,moveInput.y);
        
        if (input.sqrMagnitude < 0.01f)
        {
            return;
        }
        input = inputFrame * input;
        m_MoveInput = input.normalized;
        //m_Body.SetTargetPostion(movedelta);
        this.m_Velocity = m_MoveInput * this.speed;
        this.laterDirection = m_Velocity;
    }

    public void SetRotationInput(Vector2 rotationInput)
    {
        // Adjust the pitch angle (X Rotation)
        float pitchAngle = ControlRotation.x;
        pitchAngle -= rotationInput.y * controlRotationSensitivity;
        pitchAngle %= 360.0f;
        pitchAngle = Mathf.Clamp(pitchAngle, -45, 75);

        // Adjust the yaw angle (Y Rotation)
        float yawAngle = ControlRotation.y;
        yawAngle += rotationInput.x * controlRotationSensitivity;
        yawAngle %= 360.0f;

        ControlRotation = new Vector2(pitchAngle, yawAngle);
        m_Body.SetTargetRotation();
    }


    public void SetJumpInput(bool jump)
    {
        if (jump)
        {
            this.jumpMove.Enter(0.7f, 0.02f, Vector3.up);
        }
    }


    public void SetDashInput(bool dash)
    {
        if (dash)
        {
            this.dashMove.Enter(0.8f, 0.05f, this.laterDirection);
        }
    }

    protected void Update()
    {
        SetRotationInput(playerInput.CameraInput);
        SetMovementInput(playerInput.MoveInput);
        SetJumpInput(playerInput.JumpInput);
        SetDashInput(playerInput.DashInput);
    }

    protected void FixedUpdate()
    {
        this.dashMove.Update();
        this.jumpMove.Update();

        this.m_Body.Move(this.m_Velocity);
        this.m_Velocity = Vector3.zero;

        if (this.transform.position.y < -100)
        {
            this.m_Body.SetLegalPosition(this.m_Body.LegalPosition, true);
        }
    }
}