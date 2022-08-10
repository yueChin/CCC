using System;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [Tooltip("How fast the camera rotates around the pivot. Value <= 0 are interpreted as instant rotation")]
    public float RotationSpeed = 0.0f;
    public float PositionSmoothDamp = 0.0f;
    public Transform Rig; // The root transform of the camera rig
    public Transform Pivot; // The point at which the camera pivots around

    private Vector3 m_CameraVelocity;

    private Vector3 m_LastPosition;
    private Vector2 m_LastControlRotation;
    public void SetPosition(Vector3 position)
    {
        m_LastPosition = position;
        Rig.position = Vector3.SmoothDamp(Rig.position, m_LastPosition, ref m_CameraVelocity, PositionSmoothDamp);

    }

    public void SetControlRotation(Vector2 controlRotation)
    {
        m_LastControlRotation = controlRotation;
        // Y Rotation (Yaw Rotation)
        Quaternion rigTargetLocalRotation = Quaternion.Euler(0.0f, m_LastControlRotation.y, 0.0f);
        // X Rotation (Pitch Rotation)
        Quaternion pivotTargetLocalRotation = Quaternion.Euler(m_LastControlRotation.x, 0.0f, 0.0f);
        if (RotationSpeed > 0.0f)
        {
            Rig.localRotation = Quaternion.Lerp(Rig.localRotation, rigTargetLocalRotation, RotationSpeed * Time.deltaTime);
            Pivot.localRotation = Quaternion.Lerp(Pivot.localRotation, pivotTargetLocalRotation, RotationSpeed * Time.deltaTime);
        }
        else
        {
            Rig.localRotation = rigTargetLocalRotation;
            Pivot.localRotation = pivotTargetLocalRotation;
        }
    }
}
