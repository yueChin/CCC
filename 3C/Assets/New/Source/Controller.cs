using UnityEngine;

public class Controller : MonoBehaviour {
    public float speed;

    public float controlRotationSensitivity;
    
    private Body m_Body;
    private Vector3 laterDirection;
    private EaseMove dashMove;
    private EaseMove jumpMove;

    private Vector3 m_Velocity;
    private Vector3 m_MoveInput;
    protected void Awake() {
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
        
        // Calculate the move direction relative to the character's yaw rotation
        Transform transf = m_Body.transform;
        Quaternion yawRotation = Quaternion.Euler(0.0f, transf.rotation.y, 0.0f);
        Vector3 forward = yawRotation * transf.forward;
        Vector3 right = yawRotation * transf.right;
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
        
        Vector2 controlRotation = m_Body.GetControlRotation();
        
        // Adjust the pitch angle (X Rotation)
        float pitchAngle = controlRotation.x;
        pitchAngle -= rotationInput.y * controlRotationSensitivity;
        
        // Adjust the yaw angle (Y Rotation)
        float yawAngle = controlRotation.y;
        yawAngle += rotationInput.x * controlRotationSensitivity;
        
        controlRotation = new Vector2(pitchAngle, yawAngle);
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
    
    protected void FixedUpdate() {
        this.dashMove.Update();
        this.jumpMove.Update();

        if (this.m_Velocity != Vector3.zero) {
            this.m_Body.Move(this.m_Velocity);
            this.m_Velocity = Vector3.zero;
        }

        if (this.transform.position.y < -100) {
            this.m_Body.SetPosition(this.m_Body.LegalPosition, true);
        }
    }
}