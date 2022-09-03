using System;
using Cinemachine.Utility;
using UnityEngine;

public class CMController : MonoBehaviour
{
    public float speed;
    public float controlRotationSensitivity;
    public PlayerInput playerInput;
    public enum ForwardMode { Camera, Player, World };
    public ForwardMode InputForward;
    
    private CMBody m_Body;
    private Vector3 m_LaterDirection;
    private CMEaseMove m_DashMove;
    private CMEaseMove m_JumpMove;

    private Vector3 m_InputVelocity;
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

        this.m_LaterDirection = Vector3.left;

        this.m_DashMove = new CMEaseMove();
        this.m_JumpMove = new CMEaseMove();
    }

    private void Start()
    {
        FSMManager fsmManager = GameLoop.Instace.GetGameMoudle<FSMManager>();
        FSM<CMController> fsm = fsmManager.FetchFSM<FSM<CMController>>();
        fsm.SetT(this);
        NormalInptuState state = new NormalInptuState(0, "NormalInput");
        fsm.AddState(state);
        fsm.Init();
        
        BuffSystemManager buffSystemManager = GameLoop.Instace.GetFixedGameMoudle<BuffSystemManager>();
        MoveBuffSystem buffSystem = buffSystemManager.FetchSystem<MoveBuffSystem>();
        buffSystem.SetPriority(1);
        buffSystemManager.AddBuffSystem(buffSystem);
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
        this.m_InputVelocity = m_MoveInput * this.speed;
        this.m_LaterDirection = m_InputVelocity;
        
        m_Body.InputRotate(fwd,InputForward == ForwardMode.Player);
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
            //this.m_JumpMove.Enter(0.7f, 0.05f, Vector3.up);
            Debug.LogError("跳跃输入");
            if (!Body.HasBuff(2))
            {
                MoveBuffSystem buffSystem = GameLoop.Instace.GetFixedGameMoudle<BuffSystemManager>().FetchSystem<MoveBuffSystem>();
                MoveBuff moveBuff = new MoveBuff(2,buffSystem,Body);
                moveBuff.SetT(Body);
                moveBuff.SetEaseEnter(0.7f, 0.05f, Vector3.up);
                buffSystem.AddBuff(moveBuff);
            }
        }
    }


    public void SetDashInput(bool dash)
    {
        if (dash)
        {
            this.m_DashMove.Enter(0.8f, 0.02f, this.m_LaterDirection);
        }
    }

    // protected void Update()
    // {
    //     SetMovementInput(playerInput.WASDInput);
    //     SetJumpInput(playerInput.JumpInput);
    //     SetDashInput(playerInput.DashInput);
    // }

    protected void FixedUpdate()
    {
        this.m_DashMove.FixedUpdate();
        this.m_JumpMove.FixedUpdate();
        
        // this.m_InputVelocity += this.m_DashMove.EaseVelocity * this.speed; 
        // this.m_InputVelocity += this.m_JumpMove.EaseVelocity * this.speed;;
        this.Body.Move(this.m_DashMove.EaseVelocity );
        this.Body.Move(this.m_JumpMove.EaseVelocity );
        
        this.m_Body.InputMove(this.m_InputVelocity);
        this.m_InputVelocity = Vector3.zero;

        if (this.transform.position.y < -100)
        {
            this.m_Body.SetLegalPosition(this.m_Body.LegalPosition, true);
        }
    }
}