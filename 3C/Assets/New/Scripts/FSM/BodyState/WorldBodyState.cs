using UnityEngine;

public class WorldBodyState : FSMState<CMBody>
{
    public enum IsGround
    {
        IsInGround = 0,
        IsInSky = 1,
    }
    
    public class WorldBodySetting
    {
        public float StepOffset;
        public float MinRange;
    }
    
    protected RaycastHit m_Hit;
    
    public static WorldBodySetting WorldBodySets { get; protected set; }

    public RaycastHit HitInfo => m_Hit;
    
    public WorldBodyState(int id, string name = "SkyState") : base(id, name)
    {
        
    }

    public void SetOffset(float minRange,float stepOffset)
    {
        WorldBodySets = new WorldBodySetting()
        {
            StepOffset = stepOffset,
            MinRange = minRange,
        };
    }
    
    public override void Tick()
    {
        return;
        bool isHit = Physics.BoxCast(t.transform.position + (t.BoxCollider.center + Vector3.up * WorldBodySets.StepOffset), t.BoxCollider.size * 0.5f, Vector3.down, out m_Hit, Quaternion.identity, 100);
        if (isHit)
        {
            bool isGrounded = m_Hit.distance <= WorldBodySets.StepOffset + WorldBodySets.MinRange;
            if (isGrounded)
            {
                FSM.SwitchTo((int)WorldBodyState.IsGround.IsInGround);
            }
            else
            {
                FSM.SwitchTo((int)WorldBodyState.IsGround.IsInSky);
            }
        }
        else
        {
            FSM.SwitchTo((int)WorldBodyState.IsGround.IsInSky);
        }
    }
}