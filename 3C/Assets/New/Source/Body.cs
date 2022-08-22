using System;
using UnityEngine;

public partial class Body : MonoBehaviour {
    public const float MIN_RANGE = 0.01f;

    public float stepOffset = 0.02f;
    public float angleLimit = 45;

    public Transform neck;

    public RotationSettings rotationSettings;
    public MovementSettings movementSettings;
    
    private new BoxCollider collider;
    private new Transform transform;

    private Vector3 velocity;

    private Vector3 m_Normal;
    private Vector3 normal
    {
        get=>m_Normal;
        set
        {
            //Debug.LogError($"设置   {value}");
            m_Normal = value;
        }
    }
    private Vector3 drop;
    private EaseMove gravityMove;
    private float groundY;
    private Vector3 direction;

    //private Controller m_Controller;
    public float Gravity {
        get {
            return this.gravityMove.Power;
        }
    }
    
    public bool IsGrounded {
        get;
        private set;
    }

    public float GroundY {
        get {
            return this.IsGrounded ? this.transform.position.y : this.groundY;
        }
    }

    public float High {
        get {
            return this.transform.position.y - this.GroundY;
        }
    }

    public Vector3 GroundPosition {
        get {
            Vector3 pos = this.transform.position;
            pos.y = this.GroundY;
            
            return pos;
        }
    }

    public Vector3 OriginPosition {
        get;
        protected set;
    }

    public Vector3 LegalPosition {
        get;
        protected set;
    }

    protected void Awake() {
        this.collider = this.GetComponent<BoxCollider>();
        this.transform = this.gameObject.transform;
        this.gravityMove = new EaseMove(this);
        this.normal = Vector3.up;
        //this.m_Controller.Init(this);
    }

    protected void Start() {
        this.LegalPosition = this.transform.position;
        this.AdjustPosition();
        this.OriginPosition = this.transform.position;
    }

    protected void FixedUpdate() {
        if (!this.IsGrounded && !this.gravityMove.IsRunning) {
            this.gravityMove.Enter(0, -0.035f, Vector3.down);
        }
        else if (this.IsGrounded && this.gravityMove.IsRunning) {
            this.gravityMove.Exit();
            this.gravityMove.Power = 0;
        }

        if (this.drop != Vector3.zero) {
            this.velocity = this.IsGrounded ? this.drop : this.drop * 0.5f;
        }

        this.gravityMove.Update();

        if (this.IsGrounded) {
            float angle = Vector3.Angle(Vector3.up, this.normal);

            if (angle > this.angleLimit) {
                this.velocity -= Vector3.ProjectOnPlane(Vector3.up, -this.normal);
            }
        }

        if (this.velocity == Vector3.zero) {
            return;
        }

        //Debug.LogError("```````````````````````````````````````````````````````````");
        //Debug.LogError(this.velocity.y + "    this    normal   " + this.normal);
        bool isApp = Mathf.Approximately(this.velocity.y, 0);
        Vector3 velocity  ;

        if (isApp)
        {
            velocity = Vector3.ProjectOnPlane(this.velocity, this.normal);
        }
        else
        {
            velocity = this.velocity;
        }
        //Debug.LogError($"velocity        {velocity}");
        Vector3 position = this.transform.position;
        Vector3 offset = this.collider.center + Vector3.up * this.stepOffset;
        Vector3 size = this.collider.size * 0.5f;
        RaycastHit hit;
        //Debug.LogError($"position0 :   {position}");
        position.x += this.MoveDirection(position + offset, size, velocity, 0);
        position.y += this.MoveDirection(position + offset, size, velocity, 1);
        position.z += this.MoveDirection(position + offset, size, velocity, 2);

        if (velocity.x != 0 || velocity.z != 0) {
            this.direction = velocity.normalized;
        }
        //Debug.LogError($"position1 :   {position}");
        bool ok = Physics.BoxCast(position + offset, size, Vector3.down, out hit, Quaternion.identity, 100);
        if (ok) {
            this.IsGrounded = hit.distance <= this.stepOffset + MIN_RANGE;
            this.normal = hit.normal;
            this.groundY = hit.point.y + MIN_RANGE;

            if (this.IsGrounded) {
                position.y = this.groundY;
                if (hit.collider.material.bounciness > 0 && this.drop == Vector3.zero) {
                    this.drop = new Vector3(-this.direction.x, 0, -this.direction.z);
                }
                else if (hit.collider.material.bounciness == 0) {
                    this.drop = Vector3.zero;
                    this.CheckLegalPosition(hit, position);
                }
            }
        }
        else {
            this.IsGrounded = false;
            this.normal = Vector3.up;
            this.groundY = position.y;
        }
        //Debug.LogError($"position2 :   {position}");
        this.SetTargetPostion(position);
    }

    private void Update()
    {
        LerpTargetPosition();
    }

    private bool m_HasMoveInput;
    private Vector3 m_MoveInput;
    private Vector3 m_LastMoveInput;
    public void Move(Vector3 velocity) {
        this.velocity += velocity;
        bool hasMovementInput = velocity.sqrMagnitude > 0.0f;

        if (m_HasMoveInput && !hasMovementInput)
        {
            m_LastMoveInput = m_MoveInput;
        }
        m_MoveInput = velocity;
        m_HasMoveInput = hasMovementInput;
    }

    private float m_LastPositionFixedTime;
    private Vector3 m_TargetPostion;
    private Vector3 m_LatePosition;
    public void SetTargetPostion(Vector3 position)
    {
        this.m_LastPositionFixedTime = Time.time;
        this.m_TargetPostion = position;
        this.m_LatePosition = this.transform.position;
    }

    public void LerpTargetPosition()
    {
        if (MathF.Abs((this.transform.position - m_TargetPostion).magnitude) < 0.01)
        {
            return;
        }
        float lerp = Easing.Linear((Time.time - m_LastPositionFixedTime) / (Time.fixedDeltaTime));
        Vector3 pos = Vector3.Lerp(this.m_LatePosition, m_TargetPostion, lerp);
        this.transform.position = pos;
        this.velocity = Vector3.zero;
    }
    
    public void SetPosition(Vector3 position, bool adjust=false)
    {
        //Debug.LogError($"SetPosition    {position}");
        this.m_LatePosition = this.transform.position;
        this.transform.position = position;
        this.velocity = Vector3.zero;
        
        if (adjust) {
            this.AdjustPosition();
        }
    }

    private float MoveDirection(Vector3 center, Vector3 size, Vector3 velocity, int index) {
        if (velocity[index] == 0) {
            return 0;
        }
        //Debug.LogError(velocity[index] + "               " + index);
        float distance = Mathf.Abs(velocity[index]);
        Vector3 direction = new Vector3();
        direction[index] = velocity[index] > 0 ? 1 : -1;
        RaycastHit hit;

        bool ok = Physics.BoxCast(center, size, direction, out hit, Quaternion.identity, distance);

        return ok ? (hit.distance - MIN_RANGE) * direction[index] : velocity[index];
    }
    
    private void AdjustPosition() {
        Vector3 pos = this.transform.position;
        Vector3 size = this.collider.size;
        Vector3 center = pos + this.collider.center + Vector3.up * size.y;
        RaycastHit hit;
        bool ok = Physics.BoxCast(center, size, Vector3.down, out hit, Quaternion.identity, size.y);
        
        if (ok) {
            this.normal = hit.normal;
            this.groundY = pos.y;
            this.IsGrounded = true;

            pos.y = hit.point.y + MIN_RANGE;
            this.transform.position = pos;
            this.CheckLegalPosition(hit, pos);
        }
        else {
            this.normal = Vector3.up;
            this.groundY = this.transform.position.y;
            this.IsGrounded = false;
        }
    }

    private void CheckLegalPosition(RaycastHit hit, Vector3 position) {
        float angle = Vector3.Angle(Vector3.up, hit.normal);

        if (angle <= this.angleLimit) {
            this.LegalPosition = position;
        }
    }

    private float m_HorizontalSpeed;
    private float m_TargetHorizontalSpeedp;
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
       
        Vector3 horizontalMovement = moveDir.SetY(0.0f);//+ this.velocity.y * Vector3.up;
        if (horizontalMovement.sqrMagnitude < 0.01f)
            return;
        
        float rotationSpeed = Mathf.Lerp(rotationSettings.MaxRotationSpeed, rotationSettings.MinRotationSpeed, m_HorizontalSpeed / m_TargetHorizontalSpeedp);

        Quaternion targetRotation = Quaternion.LookRotation(horizontalMovement, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
    }

}