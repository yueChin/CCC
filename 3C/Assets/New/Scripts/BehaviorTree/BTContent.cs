using System.Collections.Generic;

public class BTContent : IGameMoudle,ILifeCycle
{
    private Dictionary<string, BlackBoard> m_Blackboards ;

    private BTTimeMenter m_TimeMenter ;

    public BTTimeMenter TimeMenter => m_TimeMenter;
    
    public void Awake()
    {
        m_Blackboards = new Dictionary<string, BlackBoard>();
        m_TimeMenter = new BTTimeMenter();
        
        m_TimeMenter.Awake();
    }

    public void Destroy()
    {
        foreach (KeyValuePair<string, BlackBoard> keyValuePair in m_Blackboards)
        {
            keyValuePair.Value.Destroy();
        }
        m_TimeMenter.Destroy();

        m_Blackboards = null;
        m_TimeMenter = null;
    }

    public void Tick()
    {
        m_TimeMenter.Tick();
    }
    
    public BlackBoard GetBlackboard(string key)
    {
        if (!m_Blackboards.ContainsKey(key))
        {
            m_Blackboards.Add(key, new BlackBoard(TimeMenter));
        }
        return m_Blackboards[key];
    }
    
    public static BlackBoard GetSharedBlackboard(string key)
    {
        BTContent btc = GameLoop.Instace.GetGameMoudle<BTContent>();
        return btc.GetBlackboard(key);
    }
}