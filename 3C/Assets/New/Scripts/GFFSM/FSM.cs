//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    /// <summary>
    /// 有限状态机。
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型。</typeparam>
    internal class FSM<T> : FSMBase, IFSM<T> where T : class
    {
        private T m_Owner;
        private readonly Dictionary<Type, FSMState<T>> m_States;
        private Dictionary<string, Variable> m_Datas;
        private FSMState<T> m_CurrentState;
        private float m_CurrentStateTime;
        private bool m_IsDestroyed;

        /// <summary>
        /// 初始化有限状态机的新实例。
        /// </summary>
        public FSM()
        {
            m_Owner = null;
            m_States = new Dictionary<Type, FSMState<T>>();
            m_Datas = null;
            m_CurrentState = null;
            m_CurrentStateTime = 0f;
            m_IsDestroyed = true;
        }

        /// <summary>
        /// 获取有限状态机持有者。
        /// </summary>
        public T Owner
        {
            get
            {
                return m_Owner;
            }
        }

        /// <summary>
        /// 获取有限状态机持有者类型。
        /// </summary>
        public override Type OwnerType
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// 获取有限状态机中状态的数量。
        /// </summary>
        public override int FsmStateCount
        {
            get
            {
                return m_States.Count;
            }
        }

        /// <summary>
        /// 获取有限状态机是否正在运行。
        /// </summary>
        public override bool IsRunning
        {
            get
            {
                return m_CurrentState != null;
            }
        }

        /// <summary>
        /// 获取有限状态机是否被销毁。
        /// </summary>
        public override bool IsDestroyed
        {
            get
            {
                return m_IsDestroyed;
            }
        }

        /// <summary>
        /// 获取当前有限状态机状态。
        /// </summary>
        public FSMState<T> CurrentState
        {
            get
            {
                return m_CurrentState;
            }
        }

        /// <summary>
        /// 获取当前有限状态机状态名称。
        /// </summary>
        public override string CurrentStateName
        {
            get
            {
                return m_CurrentState != null ? m_CurrentState.GetType().FullName : null;
            }
        }

        /// <summary>
        /// 获取当前有限状态机状态持续时间。
        /// </summary>
        public override float CurrentStateTime
        {
            get
            {
                return m_CurrentStateTime;
            }
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>创建的有限状态机。</returns>
        public static FSM<T> Create(string name, T owner, params FSMState<T>[] states)
        {
            if (owner == null)
            {
                Debug.LogError("FSM owner is invalid.");
            }

            if (states == null || states.Length < 1)
            {
                Debug.LogError("FSM states is invalid.");
            }

            FSM<T> fsm = new FSM<T>();
            fsm.Name = name;
            fsm.m_Owner = owner;
            fsm.m_IsDestroyed = false;
            foreach (FSMState<T> state in states)
            {
                if (state == null)
                {
                    Debug.LogError("FSM states is invalid.");
                }

                Type stateType = state.GetType();
                if (fsm.m_States.ContainsKey(stateType))
                {
                    Debug.LogError(String.Format("FSM '{0}' state '{1}' is already exist.", new TypeNamePair(typeof(T), name), stateType.FullName));
                }

                fsm.m_States.Add(stateType, state);
                state.OnInit(fsm);
            }

            return fsm;
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>创建的有限状态机。</returns>
        public static FSM<T> Create(string name, T owner, List<FSMState<T>> states)
        {
            if (owner == null)
            {
                Debug.LogError("FSM owner is invalid.");
            }

            if (states == null || states.Count < 1)
            {
                Debug.LogError("FSM states is invalid.");
            }

            FSM<T> fsm = new FSM<T>();
            fsm.Name = name;
            fsm.m_Owner = owner;
            fsm.m_IsDestroyed = false;
            foreach (FSMState<T> state in states)
            {
                if (state == null)
                {
                    Debug.LogError("FSM states is invalid.");
                }

                Type stateType = state.GetType();
                if (fsm.m_States.ContainsKey(stateType))
                {
                    Debug.LogError(String.Format("FSM '{0}' state '{1}' is already exist.", new TypeNamePair(typeof(T), name), stateType.FullName));
                }

                fsm.m_States.Add(stateType, state);
                state.OnInit(fsm);
            }

            return fsm;
        }

        /// <summary>
        /// 清理有限状态机。
        /// </summary>
        public void Clear()
        {
            if (m_CurrentState != null)
            {
                m_CurrentState.OnLeave(this, true);
            }

            foreach (KeyValuePair<Type, FSMState<T>> state in m_States)
            {
                state.Value.OnDestroy(this);
            }

            Name = null;
            m_Owner = null;
            m_States.Clear();

            if (m_Datas != null)
            {
                foreach (KeyValuePair<string, Variable> data in m_Datas)
                {
                    if (data.Value == null)
                    {
                        continue;
                    }
                    data.Value.Clear();
                }

                m_Datas.Clear();
            }

            m_CurrentState = null;
            m_CurrentStateTime = 0f;
            m_IsDestroyed = true;
        }

        /// <summary>
        /// 开始有限状态机。
        /// </summary>
        /// <typeparam name="TState">要开始的有限状态机状态类型。</typeparam>
        public void Start<TState>() where TState : FSMState<T>
        {
            if (IsRunning)
            {
                Debug.LogError("FSM is running, can not start again.");
            }

            FSMState<T> state = GetState<TState>();
            if (state == null)
            {
                Debug.LogError(String.Format("FSM '{0}' can not start state '{1}' which is not exist.", new TypeNamePair(typeof(T), Name), typeof(TState).FullName));
            }

            m_CurrentStateTime = 0f;
            m_CurrentState = state;
            m_CurrentState.OnEnter(this);
        }

        /// <summary>
        /// 开始有限状态机。
        /// </summary>
        /// <param name="stateType">要开始的有限状态机状态类型。</param>
        public void Start(Type stateType)
        {
            if (IsRunning)
            {
                Debug.LogError("FSM is running, can not start again.");
            }

            if (stateType == null)
            {
                Debug.LogError("State type is invalid.");
            }

            if (!typeof(FSMState<T>).IsAssignableFrom(stateType))
            {
                Debug.LogError(String.Format("State type '{0}' is invalid.", stateType.FullName));
            }

            FSMState<T> state = GetState(stateType);
            if (state == null)
            {
                Debug.LogError(String.Format("FSM '{0}' can not start state '{1}' which is not exist.", new TypeNamePair(typeof(T), Name), stateType.FullName));
            }

            m_CurrentStateTime = 0f;
            m_CurrentState = state;
            m_CurrentState.OnEnter(this);
        }

        /// <summary>
        /// 是否存在有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要检查的有限状态机状态类型。</typeparam>
        /// <returns>是否存在有限状态机状态。</returns>
        public bool HasState<TState>() where TState : FSMState<T>
        {
            return m_States.ContainsKey(typeof(TState));
        }

        /// <summary>
        /// 是否存在有限状态机状态。
        /// </summary>
        /// <param name="stateType">要检查的有限状态机状态类型。</param>
        /// <returns>是否存在有限状态机状态。</returns>
        public bool HasState(Type stateType)
        {
            if (stateType == null)
            {
                Debug.LogError("State type is invalid.");
            }

            if (!typeof(FSMState<T>).IsAssignableFrom(stateType))
            {
                Debug.LogError(String.Format("State type '{0}' is invalid.", stateType.FullName));
            }

            return m_States.ContainsKey(stateType);
        }

        /// <summary>
        /// 获取有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要获取的有限状态机状态类型。</typeparam>
        /// <returns>要获取的有限状态机状态。</returns>
        public TState GetState<TState>() where TState : FSMState<T>
        {
            FSMState<T> state = null;
            if (m_States.TryGetValue(typeof(TState), out state))
            {
                return (TState)state;
            }

            return null;
        }

        /// <summary>
        /// 获取有限状态机状态。
        /// </summary>
        /// <param name="stateType">要获取的有限状态机状态类型。</param>
        /// <returns>要获取的有限状态机状态。</returns>
        public FSMState<T> GetState(Type stateType)
        {
            if (stateType == null)
            {
                Debug.LogError("State type is invalid.");
            }

            if (!typeof(FSMState<T>).IsAssignableFrom(stateType))
            {
                Debug.LogError(String.Format("State type '{0}' is invalid.", stateType.FullName));
            }

            FSMState<T> state = null;
            if (m_States.TryGetValue(stateType, out state))
            {
                return state;
            }

            return null;
        }

        /// <summary>
        /// 获取有限状态机的所有状态。
        /// </summary>
        /// <returns>有限状态机的所有状态。</returns>
        public FSMState<T>[] GetAllStates()
        {
            int index = 0;
            FSMState<T>[] results = new FSMState<T>[m_States.Count];
            foreach (KeyValuePair<Type, FSMState<T>> state in m_States)
            {
                results[index++] = state.Value;
            }

            return results;
        }

        /// <summary>
        /// 获取有限状态机的所有状态。
        /// </summary>
        /// <param name="results">有限状态机的所有状态。</param>
        public void GetAllStates(List<FSMState<T>> results)
        {
            if (results == null)
            {
                Debug.LogError("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<Type, FSMState<T>> state in m_States)
            {
                results.Add(state.Value);
            }
        }

        /// <summary>
        /// 是否存在有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>有限状态机数据是否存在。</returns>
        public bool HasData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Data name is invalid.");
            }

            if (m_Datas == null)
            {
                return false;
            }

            return m_Datas.ContainsKey(name);
        }

        /// <summary>
        /// 获取有限状态机数据。
        /// </summary>
        /// <typeparam name="TData">要获取的有限状态机数据的类型。</typeparam>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>要获取的有限状态机数据。</returns>
        public TData GetData<TData>(string name) where TData : Variable
        {
            return (TData)GetData(name);
        }

        /// <summary>
        /// 获取有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>要获取的有限状态机数据。</returns>
        public Variable GetData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Data name is invalid.");
            }

            if (m_Datas == null)
            {
                return null;
            }

            Variable data = null;
            if (m_Datas.TryGetValue(name, out data))
            {
                return data;
            }

            return null;
        }

        /// <summary>
        /// 设置有限状态机数据。
        /// </summary>
        /// <typeparam name="TData">要设置的有限状态机数据的类型。</typeparam>
        /// <param name="name">有限状态机数据名称。</param>
        /// <param name="data">要设置的有限状态机数据。</param>
        public void SetData<TData>(string name, TData data) where TData : Variable
        {
            SetData(name, (Variable)data);
        }

        /// <summary>
        /// 设置有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <param name="data">要设置的有限状态机数据。</param>
        public void SetData(string name, Variable data)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Data name is invalid.");
            }

            if (m_Datas == null)
            {
                m_Datas = new Dictionary<string, Variable>(StringComparer.Ordinal);
            }

            Variable oldData = GetData(name);
            if (oldData != null)
            {
                oldData.Clear();
            }

            m_Datas[name] = data;
        }

        /// <summary>
        /// 移除有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>是否移除有限状态机数据成功。</returns>
        public bool RemoveData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Data name is invalid.");
            }

            if (m_Datas == null)
            {
                return false;
            }

            Variable oldData = GetData(name);
            if (oldData != null)
            {
                oldData.Clear();
            }

            return m_Datas.Remove(name);
        }

        /// <summary>
        /// 有限状态机轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_CurrentState == null)
            {
                return;
            }

            m_CurrentStateTime += elapseSeconds;
            m_CurrentState.OnUpdate(this, elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 关闭并清理有限状态机。
        /// </summary>
        internal override void Shutdown()
        {
            Clear();
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要切换到的有限状态机状态类型。</typeparam>
        internal void ChangeState<TState>() where TState : FSMState<T>
        {
            ChangeState(typeof(TState));
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <param name="stateType">要切换到的有限状态机状态类型。</param>
        internal void ChangeState(Type stateType)
        {
            if (m_CurrentState == null)
            {
                Debug.LogError("Current state is invalid.");
            }

            FSMState<T> state = GetState(stateType);
            if (state == null)
            {
                Debug.LogError(String.Format("FSM '{0}' can not change state to '{1}' which is not exist.", new TypeNamePair(typeof(T), Name), stateType.FullName));
            }

            m_CurrentState.OnLeave(this, false);
            m_CurrentStateTime = 0f;
            m_CurrentState = state;
            m_CurrentState.OnEnter(this);
        }
    }
}
