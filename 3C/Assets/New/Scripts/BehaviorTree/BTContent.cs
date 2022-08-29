using System.Collections.Generic;
using Unity.VisualScripting;

public class BTContent : IGameMoudle,ILifeCycle
{
    private Dictionary<string, BlackBoard> m_Blackboards ;

    private  BTTimeMenter m_BtTimeMenter;
    public BTTimeMenter BtTimeMenter => m_BtTimeMenter;
    public BTContent()
    {
        m_BtTimeMenter = new BTTimeMenter();
    }
    
    public void Awake()
    {
        m_Blackboards = new Dictionary<string, BlackBoard>();
    }

    public void Destroy()
    {
        foreach (KeyValuePair<string, BlackBoard> keyValuePair in m_Blackboards)
        {
            keyValuePair.Value.Destroy();
        }
        m_Blackboards = null;
        
        m_BtTimeMenter.Destroy();
        m_BtTimeMenter = null;
    }

    public void Tick(float timedelta)
    {
        m_BtTimeMenter.Tick(timedelta);
    }
    
    public BlackBoard GetBlackboard(string key)
    {
        if (!m_Blackboards.ContainsKey(key))
        {
            m_Blackboards.Add(key, new BlackBoard(m_BtTimeMenter));
        }
        return m_Blackboards[key];
    }

    public static BlackBoard GetSharedBlackboard(string key)
    {
        BTContent btc = GameLoop.Instace.GetGameMoudle<BTContent>();
        return btc.GetBlackboard(key);
    }
}