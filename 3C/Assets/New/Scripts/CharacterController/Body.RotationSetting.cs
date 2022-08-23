using UnityEngine;

public partial class Body
{
    public enum ERotationBehavior
    {
        OrientRotationToMovement,
        UseControlRotation
    }

    [System.Serializable]
    public class RotationSettings
    {
        [Header("Control Rotation")]
        public float MinPitchAngle = -45.0f;
        public float MaxPitchAngle = 75.0f;

        [Header("Character Orientation")]
        public ERotationBehavior RotationBehavior = ERotationBehavior.OrientRotationToMovement;
        public float MinRotationSpeed = 600.0f; // The turn speed when the player is at max speed (in degrees/second)
        public float MaxRotationSpeed = 1200.0f; // The turn speed when the player is stationary (in degrees/second)
    }
}