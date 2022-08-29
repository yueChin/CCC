using UnityEngine;


public class GroundState : WorldBodyState
{
    public GroundState(int id, string name = "Ground") : base(id, name)
    {
        
    }
    
    // public override void Tick()
    // {
    //     bool isHit = Physics.BoxCast(t.transform.position + (t.BoxCollider.center + Vector3.up * this.m_StepOffset), t.BoxCollider.size * 0.5f, Vector3.down, out m_Hit, Quaternion.identity, 100);
    //     if (isHit)
    //     {
    //         bool isGrounded = m_Hit.distance <= this.m_StepOffset + m_MinRange;
    //         if (!isGrounded)
    //         {
    //             FSM.SwitchTo(1);
    //         }
    //     }
    //     else
    //     {
    //         FSM.SwitchTo(1);
    //     }
    // }
}