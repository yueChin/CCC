using UnityEngine;

public partial class Body
{
    [System.Serializable]
    public class MovementSettings
    {
        public float Acceleration = 25.0f; // In meters/second
        public float Decceleration = 25.0f; // In meters/second
        public float MaxHorizontalSpeed = 8.0f; // In meters/second
        public float JumpSpeed = 10.0f; // In meters/second
        public float JumpAbortSpeed = 10.0f; // In meters/second
    }
}