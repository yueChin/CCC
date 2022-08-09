using UnityEngine;

public class Controller : MonoBehaviour {
    public float speed;

    public float controlRotationSensitivity;
    
    private Body m_Body;
    private Vector3 laterDirection;
    private EaseMove dashMove;
    private EaseMove jumpMove;

    private Vector3 velocity;

    protected void Awake() {
        this.m_Body = this.GetComponent<Body>();
        this.laterDirection = Vector3.left;

        this.dashMove = new EaseMove(this.m_Body);
        this.jumpMove = new EaseMove(this.m_Body);
    }

    // protected void Update() {
    //     Vector3 dir = Vector3.zero;
    //
    //     if (Input.GetKey(KeyCode.A)) {
    //         dir.x = -1;
    //     }
    //     else if (Input.GetKey(KeyCode.D)) {
    //         dir.x = 1;
    //     }
    //
    //     if (Input.GetKey(KeyCode.W)) {
    //         dir.z = 1;
    //     }
    //     else if (Input.GetKey(KeyCode.S)) {
    //         dir.z = -1;
    //     }
    //
    //     if (Input.GetKeyDown(KeyCode.LeftShift)) {
    //         this.dashMove.Enter(0.8f, 0.05f, this.laterDirection);
    //     }
    //     else if (Input.GetKeyDown(KeyCode.Space)) {
    //         this.jumpMove.Enter(0.7f, 0.02f, Vector3.up);
    //     }
    //
    //     if (dir != Vector3.zero)
    //     {
    //         this.velocity = dir.normalized * this.speed;
    //         this.laterDirection = dir.normalized;
    //     }
    // }

    // public void Init(Body body)
    // {
    //     this.m_Body = body;
    // }
    
    public void SetMovementInput(Vector2 moveInput)
    {
        if (moveInput.sqrMagnitude < 0.01f)
        {
            return;
        }
        
        // Calculate the move direction relative to the character's yaw rotation
        Quaternion yawRotation = Quaternion.Euler(0.0f, m_Body.transform.rotation.y, 0.0f);
        Vector3 forward = yawRotation * Vector3.forward;
        Vector3 right = yawRotation * Vector3.right;
        Vector3 movedelta = (forward * moveInput.y + right * moveInput.x);
        
        //m_Body.SetTargetPostion(movedelta);
        if (movedelta != Vector3.zero)
        {
            this.velocity = movedelta.normalized * this.speed;
            this.laterDirection = movedelta.normalized;
        }
    }

    public void SetRotationInput(Vector2 rotationInput)
    {
        if (rotationInput.sqrMagnitude < 0.01f)
        {
            return;
        }
        Debug.LogError(rotationInput);
        Vector2 controlRotation = this.transform.eulerAngles;

        // Adjust the pitch angle (X Rotation)
        float pitchAngle = controlRotation.x;
        pitchAngle -= rotationInput.y * controlRotationSensitivity;

        // Adjust the yaw angle (Y Rotation)
        float yawAngle = controlRotation.y;
        yawAngle += rotationInput.x * controlRotationSensitivity;

        controlRotation = new Vector2(pitchAngle, yawAngle);
        m_Body.SetTargetRotation(Quaternion.Euler(controlRotation));
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

        if (this.velocity != Vector3.zero) {
            this.m_Body.Move(this.velocity);
            this.velocity = Vector3.zero;
        }

        if (this.transform.position.y < -100) {
            this.m_Body.SetPosition(this.m_Body.LegalPosition, true);
        }
    }
}