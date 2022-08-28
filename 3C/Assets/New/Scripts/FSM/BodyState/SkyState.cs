using UnityEngine;

public class SkyState : GenericFSMState<CMBody>
{
    private RaycastHit m_Hit;
    private float m_StepOffset;
    private float m_MinRange;
    public SkyState(int id, string name = "SkyState") : base(id, name)
    {
    }

    public void SetOffset(float stepOffset,float minRange)
    {
        m_MinRange = minRange;
    }
    
    public override void Tick()
    {
        bool isHit = Physics.BoxCast(t.transform.position + (t.BoxCollider.center + Vector3.up * this.m_StepOffset), t.BoxCollider.size * 0.5f, Vector3.down, out m_Hit, Quaternion.identity, 100);
        if (isHit)
        {
            bool isGrounded = m_Hit.distance <= this.m_StepOffset + m_MinRange;
            if (isGrounded)
            {
                FSM.SwitchTo(0);
            }
        }
    }
}