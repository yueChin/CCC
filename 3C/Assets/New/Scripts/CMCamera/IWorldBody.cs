using UnityEngine;

public interface IWorldBody
{
    public void AffectByWorldRule(Vector3 delta);
}