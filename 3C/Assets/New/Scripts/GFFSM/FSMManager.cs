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
    /// 有限状态机管理器。
    /// </summary>
    internal sealed class FSMManager :  IFSMManager
    {
        private readonly Dictionary<TypeNamePair, FSMBase> m_Fsms;
        private readonly List<FSMBase> m_TempFsms;

        /// <summary>
        /// 初始化有限状态机管理器的新实例。
        /// </summary>
        public FSMManager()
        {
            m_Fsms = new Dictionary<TypeNamePair, FSMBase>();
            m_TempFsms = new List<FSMBase>();
        }

        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        internal int Priority
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// 获取有限状态机数量。
        /// </summary>
        public int Count
        {
            get
            {
                return m_Fsms.Count;
            }
        }

        /// <summary>
        /// 有限状态机管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal void Update(float elapseSeconds, float realElapseSeconds)
        {
            m_TempFsms.Clear();
            if (m_Fsms.Count <= 0)
            {
                return;
            }

            foreach (KeyValuePair<TypeNamePair, FSMBase> fsm in m_Fsms)
            {
                m_TempFsms.Add(fsm.Value);
            }

            foreach (FSMBase fsm in m_TempFsms)
            {
                if (fsm.IsDestroyed)
                {
                    continue;
                }

                fsm.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 关闭并清理有限状态机管理器。
        /// </summary>
        internal void Shutdown()
        {
            foreach (KeyValuePair<TypeNamePair, FSMBase> fsm in m_Fsms)
            {
                fsm.Value.Shutdown();
            }

            m_Fsms.Clear();
            m_TempFsms.Clear();
        }

        /// <summary>
        /// 检查是否存在有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <returns>是否存在有限状态机。</returns>
        public bool HasFsm<T>() where T : class
        {
            return InternalHasFsm(new TypeNamePair(typeof(T)));
        }

        /// <summary>
        /// 检查是否存在有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <returns>是否存在有限状态机。</returns>
        public bool HasFsm(Type ownerType)
        {
            if (ownerType == null)
            {
                Debug.LogError("Owner type is invalid.");
            }

            return InternalHasFsm(new TypeNamePair(ownerType));
        }

        /// <summary>
        /// 检查是否存在有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="name">有限状态机名称。</param>
        /// <returns>是否存在有限状态机。</returns>
        public bool HasFsm<T>(string name) where T : class
        {
            return InternalHasFsm(new TypeNamePair(typeof(T), name));
        }

        /// <summary>
        /// 检查是否存在有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <param name="name">有限状态机名称。</param>
        /// <returns>是否存在有限状态机。</returns>
        public bool HasFsm(Type ownerType, string name)
        {
            if (ownerType == null)
            {
                Debug.LogError("Owner type is invalid.");
            }

            return InternalHasFsm(new TypeNamePair(ownerType, name));
        }

        /// <summary>
        /// 获取有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <returns>要获取的有限状态机。</returns>
        public IFSM<T> GetFsm<T>() where T : class
        {
            return (IFSM<T>)InternalGetFsm(new TypeNamePair(typeof(T)));
        }

        /// <summary>
        /// 获取有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <returns>要获取的有限状态机。</returns>
        public FSMBase GetFsm(Type ownerType)
        {
            if (ownerType == null)
            {
                Debug.LogError("Owner type is invalid.");
            }

            return InternalGetFsm(new TypeNamePair(ownerType));
        }

        /// <summary>
        /// 获取有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="name">有限状态机名称。</param>
        /// <returns>要获取的有限状态机。</returns>
        public IFSM<T> GetFsm<T>(string name) where T : class
        {
            return (IFSM<T>)InternalGetFsm(new TypeNamePair(typeof(T), name));
        }

        /// <summary>
        /// 获取有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <param name="name">有限状态机名称。</param>
        /// <returns>要获取的有限状态机。</returns>
        public FSMBase GetFsm(Type ownerType, string name)
        {
            if (ownerType == null)
            {
                Debug.LogError("Owner type is invalid.");
            }

            return InternalGetFsm(new TypeNamePair(ownerType, name));
        }

        /// <summary>
        /// 获取所有有限状态机。
        /// </summary>
        /// <returns>所有有限状态机。</returns>
        public FSMBase[] GetAllFsms()
        {
            int index = 0;
            FSMBase[] results = new FSMBase[m_Fsms.Count];
            foreach (KeyValuePair<TypeNamePair, FSMBase> fsm in m_Fsms)
            {
                results[index++] = fsm.Value;
            }

            return results;
        }

        /// <summary>
        /// 获取所有有限状态机。
        /// </summary>
        /// <param name="results">所有有限状态机。</param>
        public void GetAllFsms(List<FSMBase> results)
        {
            if (results == null)
            {
                Debug.LogError("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<TypeNamePair, FSMBase> fsm in m_Fsms)
            {
                results.Add(fsm.Value);
            }
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>要创建的有限状态机。</returns>
        public IFSM<T> CreateFsm<T>(T owner, params FSMState<T>[] states) where T : class
        {
            return CreateFsm(string.Empty, owner, states);
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>要创建的有限状态机。</returns>
        public IFSM<T> CreateFsm<T>(string name, T owner, params FSMState<T>[] states) where T : class
        {
            TypeNamePair typeNamePair = new TypeNamePair(typeof(T), name);
            if (HasFsm<T>(name))
            {
                Debug.LogError(String.Format("Already exist FSM '{0}'.", typeNamePair));
            }

            FSM<T> fsm = FSM<T>.Create(name, owner, states);
            m_Fsms.Add(typeNamePair, fsm);
            return fsm;
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>要创建的有限状态机。</returns>
        public IFSM<T> CreateFsm<T>(T owner, List<FSMState<T>> states) where T : class
        {
            return CreateFsm(string.Empty, owner, states);
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>要创建的有限状态机。</returns>
        public IFSM<T> CreateFsm<T>(string name, T owner, List<FSMState<T>> states) where T : class
        {
            TypeNamePair typeNamePair = new TypeNamePair(typeof(T), name);
            if (HasFsm<T>(name))
            {
                Debug.LogError(String.Format("Already exist FSM '{0}'.", typeNamePair));
            }

            FSM<T> fsm = FSM<T>.Create(name, owner, states);
            m_Fsms.Add(typeNamePair, fsm);
            return fsm;
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <returns>是否销毁有限状态机成功。</returns>
        public bool DestroyFsm<T>() where T : class
        {
            return InternalDestroyFsm(new TypeNamePair(typeof(T)));
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <returns>是否销毁有限状态机成功。</returns>
        public bool DestroyFsm(Type ownerType)
        {
            if (ownerType == null)
            {
                Debug.LogError("Owner type is invalid.");
            }

            return InternalDestroyFsm(new TypeNamePair(ownerType));
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="name">要销毁的有限状态机名称。</param>
        /// <returns>是否销毁有限状态机成功。</returns>
        public bool DestroyFsm<T>(string name) where T : class
        {
            return InternalDestroyFsm(new TypeNamePair(typeof(T), name));
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <param name="name">要销毁的有限状态机名称。</param>
        /// <returns>是否销毁有限状态机成功。</returns>
        public bool DestroyFsm(Type ownerType, string name)
        {
            if (ownerType == null)
            {
                Debug.LogError("Owner type is invalid.");
            }

            return InternalDestroyFsm(new TypeNamePair(ownerType, name));
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="fsm">要销毁的有限状态机。</param>
        /// <returns>是否销毁有限状态机成功。</returns>
        public bool DestroyFsm<T>(IFSM<T> fsm) where T : class
        {
            if (fsm == null)
            {
                Debug.LogError("FSM is invalid.");
            }

            return InternalDestroyFsm(new TypeNamePair(typeof(T), fsm.Name));
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <param name="fsm">要销毁的有限状态机。</param>
        /// <returns>是否销毁有限状态机成功。</returns>
        public bool DestroyFsm(FSMBase fsm)
        {
            if (fsm == null)
            {
                Debug.LogError("FSM is invalid.");
            }

            return InternalDestroyFsm(new TypeNamePair(fsm.OwnerType, fsm.Name));
        }

        private bool InternalHasFsm(TypeNamePair typeNamePair)
        {
            return m_Fsms.ContainsKey(typeNamePair);
        }

        private FSMBase InternalGetFsm(TypeNamePair typeNamePair)
        {
            FSMBase fsm = null;
            if (m_Fsms.TryGetValue(typeNamePair, out fsm))
            {
                return fsm;
            }

            return null;
        }

        private bool InternalDestroyFsm(TypeNamePair typeNamePair)
        {
            FSMBase fsm = null;
            if (m_Fsms.TryGetValue(typeNamePair, out fsm))
            {
                fsm.Shutdown();
                return m_Fsms.Remove(typeNamePair);
            }

            return false;
        }
    }
}
