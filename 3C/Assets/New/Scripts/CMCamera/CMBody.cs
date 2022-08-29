using System;
using Cinemachine.Utility;
using UnityEngine;

public class CMBody : MonoBehaviour,IWorldBody
{
    public const float MIN_RANGE = 0.01f;

    public float stepOffset = 0.02f;
    public float angleLimit = 45;
    public float VelocityDamping = 0.5f;

    public Transform neck;

    public Body.RotationSettings rotationSettings;
    public Body.MovementSettings movementSettings;

    protected new BoxCollider m_Collider;
    protected new Transform m_Transform;

    protected Vector3 m_PhysicVelocity;

    protected Vector3 m_Normal;

    protected Vector3 Normal
    {
        get => m_Normal;
        set
        {
            //Debug.LogError($"设置   {value}");
            m_Normal = value;
        }
    }

    protected Vector3 m_Drop;
    protected float m_GroundY;
    protected Vector3 m_Direction;

    private Vector3 m_Velocity;

    private WorldRule m_WorldRule;

    public bool IsGrounded { get; protected set; }

    public float GroundY
    {
        get { return this.IsGrounded ? this.m_Transform.position.y : this.m_GroundY; }
    }

    public float High
    {
        get { return this.m_Transform.position.y - this.GroundY; }
    }

    public Vector3 GroundPosition
    {
        get
        {
            Vector3 pos = this.m_Transform.position;
            pos.y = this.GroundY;

            return pos;
        }
    }

    public Vector3 OriginPosition { get; protected set; }

    public Vector3 LegalPosition { get; protected set; }

    public BoxCollider BoxCollider => m_Collider;

    private FSM<CMBody> m_BodyFSM;
    
    protected virtual void Awake()
    {
        this.m_Collider = this.GetComponent<BoxCollider>();
        this.m_Transform = this.gameObject.transform;
        this.Normal = Vector3.up;
        if (m_WorldRule == null)
        {
            m_WorldRule = FindObjectOfType<WorldRule>();
        }

        if (m_WorldRule != null)
        {
            m_WorldRule.SetActiveBody(this);
        }
    }

    protected virtual void Start()
    {
        FSMManager fsmManager = GameLoop.Instace.GetFixedGameMoudle<FSMManager>();
        FSM<CMBody> fsm = fsmManager.FetchFSM<FSM<CMBody>>();
        GroundState state = new GroundState(0, "Ground");
        SkyState skyState = new SkyState(1, "Sky");
        fsm.AddState(state);
        fsm.AddState(skyState);
        fsm.Init();
        m_BodyFSM = fsm;
        
        this.LegalPosition = this.m_Transform.position;
        this.AdjustPosition();
        this.OriginPosition = this.m_Transform.position;
    }

    protected virtual void FixedUpdate()
    {
        GravtyUpdate();
        PhysicVelocity();
        PhysicPostion();
    }

    protected void PhysicVelocity()
    {
        if (this.m_Drop != Vector3.zero)
        {
            this.m_PhysicVelocity = this.IsGrounded ? this.m_Drop : this.m_Drop * 0.5f;
        }

        if (this.IsGrounded)
        {
            float angle = Vector3.Angle(Vector3.up, this.Normal);

            if (angle > this.angleLimit)
            {
                this.m_PhysicVelocity -= Vector3.ProjectOnPlane(Vector3.up, -this.Normal);
            }
        }
    }
    
    protected void PhysicPostion()
    {
        if (this.m_PhysicVelocity == Vector3.zero)
        {
            return;
        }

        //Debug.LogError("```````````````````````````````````````````````````````````");
        //Debug.LogError(this.velocity.y + "    this    normal   " + this.normal);
        bool isApp = Mathf.Approximately(this.m_PhysicVelocity.y, 0);
        Vector3 velocity;

        if (isApp)
        {
            velocity = Vector3.ProjectOnPlane(this.m_PhysicVelocity, this.Normal);
        }
        else
        {
            velocity = this.m_PhysicVelocity;
        }

        //Debug.LogError($"velocity        {velocity}");
        Vector3 position = this.m_Transform.position;
        Vector3 offset = this.m_Collider.center + Vector3.up * this.stepOffset;
        Vector3 size = this.m_Collider.size * 0.5f;
        RaycastHit hit;
        //Debug.LogError($"position0 :   {position}");
        position.x += this.MoveDirection(position + offset, size, velocity, 0);
        position.y += this.MoveDirection(position + offset, size, velocity, 1);
        position.z += this.MoveDirection(position + offset, size, velocity, 2);

        if (velocity.x != 0 || velocity.z != 0)
        {
            this.m_Direction = velocity.normalized;
        }

        //Debug.LogError($"position1 :   {position}");
        bool isHit = Physics.BoxCast(position + offset, size, Vector3.down, out hit, Quaternion.identity, 100);
        if (isHit)
        {
            this.IsGrounded = hit.distance <= this.stepOffset + MIN_RANGE;
            this.Normal = hit.normal;
            this.m_GroundY = hit.point.y + MIN_RANGE;

            if (this.IsGrounded)
            {
                position.y = this.m_GroundY;
                if (hit.collider.material.bounciness > 0 && this.m_Drop == Vector3.zero)
                {
                    this.m_Drop = new Vector3(-this.m_Direction.x, 0, -this.m_Direction.z);
                }
                else if (hit.collider.material.bounciness == 0)
                {
                    this.m_Drop = Vector3.zero;
                    this.CheckLegalPosition(hit, position);
                }
                m_BodyFSM.SwitchTo((int)WorldBodyState.IsGround.IsInGround);
            }
            else
            {
                m_BodyFSM.SwitchTo((int)WorldBodyState.IsGround.IsInSky);
            }
        }
        else
        {
            m_BodyFSM.SwitchTo((int)WorldBodyState.IsGround.IsInSky);
            this.IsGrounded = false;
            this.Normal = Vector3.up;
            this.m_GroundY = position.y;
        }
        //Debug.LogError($"position2 :   {position}");
        this.SetTargetPostion(position);
    }
    
    protected virtual void Update()
    {
        LerpTargetPosition();
    }

    protected bool m_HasMoveInput;
    protected Vector3 m_MoveInput;
    protected Vector3 m_LastMoveInput;

    protected float m_LastPositionFixedTime;
    protected Vector3 m_TargetPhysicPostion;
    protected Vector3 m_LatePosition;

    public virtual void SetTargetPostion(Vector3 position)
    {
        this.m_LastPositionFixedTime = Time.time;
        this.m_TargetPhysicPostion = position;
        this.m_LatePosition = this.m_Transform.position;
    }

    public void LerpTargetPosition()
    {
        if (MathF.Abs((this.m_Transform.position - m_TargetPhysicPostion).magnitude) < 0.01)
        {
            return;
        }

        float lerp = Easing.Linear((Time.time - m_LastPositionFixedTime) / (Time.fixedDeltaTime));
        Vector3 pos = Vector3.Lerp(this.m_LatePosition, m_TargetPhysicPostion, lerp);
        this.m_Transform.position = pos;
        this.m_PhysicVelocity = Vector3.zero;
    }

    public virtual void SetLegalPosition(Vector3 position, bool adjust = false)
    {
        //Debug.LogError($"SetPosition    {position}");
        this.m_LatePosition = this.m_Transform.position;
        this.m_Transform.position = position;
        this.m_PhysicVelocity = Vector3.zero;

        if (adjust)
        {
            this.AdjustPosition();
        }
    }

    protected virtual float MoveDirection(Vector3 center, Vector3 size, Vector3 velocity, int index)
    {
        if (velocity[index] == 0)
        {
            return 0;
        }

        //Debug.LogError(velocity[index] + "               " + index);
        float distance = Mathf.Abs(velocity[index]);
        Vector3 direction = new Vector3();
        direction[index] = velocity[index] > 0 ? 1 : -1;
        RaycastHit hit;

        bool isHit = Physics.BoxCast(center, size, direction, out hit, Quaternion.identity, distance);

        return isHit ? (hit.distance - MIN_RANGE) * direction[index] : velocity[index];
    }

    protected void AdjustPosition()
    {
        Vector3 pos = this.m_Transform.position;
        Vector3 size = this.m_Collider.size;
        Vector3 center = pos + this.m_Collider.center + Vector3.up * size.y;
        RaycastHit hit;
        bool ok = Physics.BoxCast(center, size, Vector3.down, out hit, Quaternion.identity, size.y);

        if (ok)
        {
            this.Normal = hit.normal;
            this.m_GroundY = pos.y;
            this.IsGrounded = true;

            pos.y = hit.point.y + MIN_RANGE;
            this.m_Transform.position = pos;
            this.CheckLegalPosition(hit, pos);
        }
        else
        {
            this.Normal = Vector3.up;
            this.m_GroundY = this.m_Transform.position.y;
            this.IsGrounded = false;
        }
    }

    protected void CheckLegalPosition(RaycastHit hit, Vector3 position)
    {
        float angle = Vector3.Angle(Vector3.up, hit.normal);

        if (angle <= this.angleLimit)
        {
            this.LegalPosition = position;
        }
    }

    protected float m_HorizontalSpeed;
    protected float m_TargetHorizontalSpeedp;

    private void GravtyUpdate()
    {
        if (!this.IsGrounded )
        {
            m_WorldRule.ActiveBody(this);
        }
        else if (this.IsGrounded)
        {
            m_WorldRule.NegtiveBody(this);
        }
    }

    public void InputRotate(Vector3 forward,bool isPlayer)
    {
        if (m_MoveInput.sqrMagnitude > 0.01f)
        {
            float fdt = Time.fixedDeltaTime;
            Quaternion qA = transform.rotation;
            Quaternion qB = Quaternion.LookRotation((isPlayer && Vector3.Dot(forward, m_MoveInput) < 0) ? -m_MoveInput : m_MoveInput);
            transform.rotation = Quaternion.Slerp(qA, qB, Damper.Damp(1, VelocityDamping, fdt));
        }
    }
    
    public void InputMove(Vector3 velocityInput)
    {
        float fdt = Time.fixedDeltaTime;
        Vector3 deltaVel = velocityInput - m_Velocity;
        m_Velocity += Damper.Damp(deltaVel, VelocityDamping, fdt);
        this.m_PhysicVelocity += m_Velocity * fdt;
        bool hasMovementInput = velocityInput.sqrMagnitude > 0.0f;

        if (m_HasMoveInput && !hasMovementInput)
        {
            m_LastMoveInput = m_MoveInput;
        }

        m_MoveInput = velocityInput;
        m_HasMoveInput = hasMovementInput;
    }
    
    public void Move(Vector3 velocityDelta)
    {
        this.m_PhysicVelocity += velocityDelta;
    }

    public void SetTargetRotation()
    {
        Vector3 movementInput = m_MoveInput;
        if (movementInput.sqrMagnitude > 1.0f)
        {
            movementInput.Normalize();
        }

        m_TargetHorizontalSpeedp = movementInput.magnitude * movementSettings.MaxHorizontalSpeed;
        float acceleration = m_HasMoveInput ? movementSettings.Acceleration : movementSettings.Decceleration;

        m_HorizontalSpeed = Mathf.MoveTowards(m_HorizontalSpeed, m_TargetHorizontalSpeedp, acceleration * Time.deltaTime);

        Vector3 moveDir = m_HasMoveInput ? m_MoveInput : m_LastMoveInput;
        if (moveDir.sqrMagnitude > 1f)
        {
            moveDir.Normalize();
        }
        //Debug.LogError(moveDir);
        // if (moveDir.Equals(Vector3.zero) || moveDir.sqrMagnitude < 0.01)
        // {
        //     return;
        // }

        Vector3 horizontalMovement = moveDir.SetY(0.0f); //+ this.velocity.y * Vector3.up;
        if (horizontalMovement.sqrMagnitude < 0.01f)
            return;

        float rotationSpeed = Mathf.Lerp(rotationSettings.MaxRotationSpeed, rotationSettings.MinRotationSpeed, m_HorizontalSpeed / m_TargetHorizontalSpeedp);

        Quaternion targetRotation = Quaternion.LookRotation(horizontalMovement, Vector3.up);
        m_Transform.rotation = Quaternion.RotateTowards(m_Transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void AffectByWorldRule(Vector3 delta)
    {
        Move(delta);
    }
}