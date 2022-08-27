﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public static GameLoop Instace { get; private set; }
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

    public T GetGameMoudle<T>() where T : class, IGameMoudle
    {
        Type type = typeof(T);
        T result = null;
        foreach (IGameMoudle moudle in MoudleList)
        {
            if (moudle.GetType() == type)
            {
                result = moudle as T;
                break;
            }
        }
        return result;
    }
}