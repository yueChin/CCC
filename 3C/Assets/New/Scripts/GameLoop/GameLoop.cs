using System;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public GameLoop Instace { get; private set; }
    private List<IGameMoudle> MoudleList ;
    private List<IGameMoudle> FixedMoudleList;

    public void Awake()
    {
        Instace = this;
        MoudleList = new List<IGameMoudle> { new FSMManager() };
        //FixedMoudleList = new List<IGameMoudle>();
    }

    public void Start()
    {
        foreach (IGameMoudle gameMoudle in MoudleList)
        {
            gameMoudle.Awake();
        }
    }

    public void Update()
    {
        foreach (IGameMoudle gameMoudle in MoudleList)
        {
            gameMoudle.Tick();
        }
    }
}