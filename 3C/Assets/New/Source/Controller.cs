using UnityEngine;

public class Controller : MonoBehaviour {
    public float speed;
    public float controlRotationSensitivity;
    public LookAtCamera camera;
    
    private Body m_Body;
    private Vector3 laterDirection;
    private EaseMove dashMove;
    private EaseMove jumpMove;

    private Vector3 m_Velocity;
    private Vector3 m_MoveInput;
    public Vector2 ControlRotation { get; private set; }
    public Body Body => m_Body;

    protected void Awake()
    {
        if (camera == null)
        {
            camera = FindObjectOfType<LookAtCamera>();
        }
        
        this.m_Body = this.GetComponent<Body>();

        this.laterDirection = Vector3.left;

        this.dashMove = new EaseMove(this.m_Body);
        this.jumpMove = new EaseMove(this.m_Body);
    }

    public void SetMovementInput(Vector2 moveInput)
    {
        if (moveInput.sqrMagnitude < 0.01f)
        {
            return;
        }

        Quaternion yawRotation = Quaternion.Euler(0.0f, ControlRotation.y, 0.0f);
        Vector3 forward = yawRotation * Vector3.forward;
        Vector3 right = yawRotation * Vector3.right;
        Vector3 movementInput = (forward * moveInput.y + right * moveInput.x);

        if (movementInput.sqrMagnitude > 1f)
        {
            movementInput.Normalize();
        }
        Vector3 movedelta = (forward * moveInput.y + right * moveInput.x);

        m_MoveInput = movedelta.normalized;
        //m_Body.SetTargetPostion(movedelta);
        if (movedelta != Vector3.zero)
        {
            this.m_Velocity = m_MoveInput * this.speed;
            this.laterDirection = m_MoveInput;
        }
    }

    public void SetRotationInput(Vector2 rotationInput)
    {
        // Adjust the pitch angle (X Rotation)
        Vector3 rotation = m_Body.transform.rotation.eulerAngles;
        float pitchAngle = rotation.x;
        pitchAngle -= rotationInput.y * controlRotationSensitivity;
        pitchAngle %= 360.0f;
        pitchAngle = Mathf.Clamp(pitchAngle, m_Body.rotationSettings.MinPitchAngle, m_Body.rotationSettings.MaxPitchAngle);
        
        // Adjust the yaw angle (Y Rotation)
        float yawAngle = rotation.y;
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

    protected void FixedUpdate()
    {
        this.dashMove.Update();
        this.jumpMove.Update();

        if (this.m_Velocity != Vector3.zero)
        {
            this.m_Body.Move(this.m_Velocity);
            this.m_Velocity = Vector3.zero;
        }

        if (this.transform.position.y < -100)
        {
            this.m_Body.SetPosition(this.m_Body.LegalPosition, true);
        }
    }

    public void LateUpdate()
    {
        if (camera == null)
            return;
        camera.SetPosition(m_Body.transform.position);
        camera.SetControlRotation(ControlRotation);
    }
}