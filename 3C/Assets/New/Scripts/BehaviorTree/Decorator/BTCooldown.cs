using UnityEngine.Assertions;

public class BTCooldown : BTDecorator
{
    public bool m_StartAfterDecoratee = false;
    private bool m_ResetOnFailiure = false;
	private bool m_FailOnCooldown = false;
    private float m_CooldownTime = 0.0f;
    private float m_RandomVariation = 0.05f;
    private bool m_IsReady = true;

    /// <summary>
    /// The Cooldown decorator ensures that the branch can not be started twice within the given cooldown time.
    /// 
    /// The decorator can start the cooldown timer right away or wait until the child stopps, you can control this behavior with the
    /// `m_StartAfterDecoratee` parameter.
    /// 
    /// The default behavior in case the cooldown timer is active and this node is started again is, that the decorator waits until
    /// the cooldown is reached and then executes the underlying node.
    /// You can change this behavior with the `failOnCooldown` parameter to make the decorator immediately fail instead.
    /// 
    /// </summary>
    /// <param name="cooldownTime">time until next execution</param>
    /// <param name="randomVariation">random variation added to the cooldown time</param>
    /// <param name="m_StartAfterDecoratee">If set to <c>true</c> the cooldown timer is started from the point after the decoratee has been started, else it will be started right away.</param>
    /// <param name="resetOnFailiure">If set to <c>true</c> the timer will be reset in case the underlying node fails.</param>
    /// <param name="failOnCooldown">If currently on cooldown and this parameter is set to <c>true</c>, the decorator will immmediately fail instead of waiting for the cooldown.</param>
    /// <param name="decoratee">Decoratee node.</param>
    public BTCooldown(float cooldownTime, float randomVariation, bool m_StartAfterDecoratee, bool resetOnFailiure, bool failOnCooldown, BTNode decoratee) : base("TimeCooldown", decoratee)
    {
        this.m_StartAfterDecoratee = m_StartAfterDecoratee;
        this.m_CooldownTime = cooldownTime;
        this.m_ResetOnFailiure = resetOnFailiure;
        this.m_RandomVariation = randomVariation;
        this.m_FailOnCooldown = failOnCooldown;
        Assert.IsTrue(cooldownTime > 0f, "cooldownTime has to be set");
    }

    public BTCooldown(float cooldownTime, bool m_StartAfterDecoratee, bool resetOnFailiure, bool failOnCooldown, BTNode decoratee) : base("TimeCooldown", decoratee)
    {
        this.m_StartAfterDecoratee = m_StartAfterDecoratee;
        this.m_CooldownTime = cooldownTime;
        this.m_RandomVariation = cooldownTime * 0.1f;
        this.m_ResetOnFailiure = resetOnFailiure;
        this.m_FailOnCooldown = failOnCooldown;
        Assert.IsTrue(cooldownTime > 0f, "cooldownTime has to be set");
    }

    public BTCooldown(float cooldownTime, float randomVariation, bool m_StartAfterDecoratee, bool resetOnFailiure, BTNode decoratee) : base("TimeCooldown", decoratee)
    {
        this.m_StartAfterDecoratee = m_StartAfterDecoratee;
        this.m_CooldownTime = cooldownTime;
        this.m_ResetOnFailiure = resetOnFailiure;
        this.m_RandomVariation = randomVariation;
        Assert.IsTrue(cooldownTime > 0f, "cooldownTime has to be set");
    }

    public BTCooldown(float cooldownTime, bool m_StartAfterDecoratee, bool resetOnFailiure, BTNode decoratee) : base("TimeCooldown", decoratee)
    {
        this.m_StartAfterDecoratee = m_StartAfterDecoratee;
        this.m_CooldownTime = cooldownTime;
        this.m_RandomVariation = cooldownTime * 0.1f;
        this.m_ResetOnFailiure = resetOnFailiure;
        Assert.IsTrue(cooldownTime > 0f, "cooldownTime has to be set");
    }

    public BTCooldown(float cooldownTime, float randomVariation, BTNode decoratee) : base("TimeCooldown", decoratee)
    {
        this.m_StartAfterDecoratee = false;
        this.m_CooldownTime = cooldownTime;
        this.m_ResetOnFailiure = false;
        this.m_RandomVariation = randomVariation;
        Assert.IsTrue(cooldownTime > 0f, "cooldownTime has to be set");
    }

    public BTCooldown(float cooldownTime, BTNode decoratee) : base("TimeCooldown", decoratee)
    {
        this.m_StartAfterDecoratee = false;
        this.m_CooldownTime = cooldownTime;
        this.m_ResetOnFailiure = false;
        this.m_RandomVariation = cooldownTime * 0.1f;
        Assert.IsTrue(cooldownTime > 0f, "cooldownTime has to be set");
    }

    protected override void OnEnable()
    {
        if (m_IsReady)
        {
            m_IsReady = false;
            if (!m_StartAfterDecoratee)
            {
                BTTimeMenter.AddTimer(m_CooldownTime, m_RandomVariation, 0, TimeoutReached);
            }
            ChildNode.Start();
        }
        else
        {
            if (m_FailOnCooldown)
            {
                Ended(false);
            }
        }
    }

    protected override void OnDisable()
    {
        if (ChildNode.IsActive)
        {
            m_IsReady = true;
            BTTimeMenter.RemoveTimer(TimeoutReached);
            ChildNode.End();
        }
        else
        {
            m_IsReady = true;
            BTTimeMenter.RemoveTimer(TimeoutReached);
            Ended(false);
        }
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        if (m_ResetOnFailiure && !result)
        {
            m_IsReady = true;
            BTTimeMenter.RemoveTimer(TimeoutReached);
        }
        else if (m_StartAfterDecoratee)
        {
            BTTimeMenter.AddTimer(m_CooldownTime, m_RandomVariation, 0, TimeoutReached);
        }
        Ended(result);
    }

    private void TimeoutReached()
    {
        if (IsActive && !ChildNode.IsActive)
        {
            BTTimeMenter.AddTimer(m_CooldownTime, m_RandomVariation, 0, TimeoutReached);
            ChildNode.Start();
        }
        else
        {
            m_IsReady = true;
        }
    }
}