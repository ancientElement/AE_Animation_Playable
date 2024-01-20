using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace AE_FSM
{
    public class DefualtIExcuteState : IExcuteState
    {
        private Dictionary<string, IFSMState> states = new Dictionary<string, IFSMState>();

        public void Enter(FSMStateNode node)
        {
            IFSMState state = GetState(node.stateNodeData.scriptName, node.stateNodeData.script);
            if (state != null)
            {
                state.Enter(node.controller);
            }
        }
        public void Update(FSMStateNode node)
        {
            IFSMState state = GetState(node.stateNodeData.scriptName, node.stateNodeData.script);
            if (state != null)
            {
                state.OnUpdate(node.controller);
            }
        }
        public void LaterUpdate(FSMStateNode node)
        {
            IFSMState state = GetState(node.stateNodeData.scriptName, node.stateNodeData.script);
            if (state != null)
            {
                state.LaterUpdate(node.controller);
            }
        }
        public void FixUpdate(FSMStateNode node)
        {
            IFSMState state = GetState(node.stateNodeData.scriptName, node.stateNodeData.script);
            if (state != null)
            {
                state.FixUpdate(node.controller);
            }
        }
        public void Exit(FSMStateNode node)
        {
            IFSMState state = GetState(node.stateNodeData.scriptName, node.stateNodeData.script);
            if (state != null)
            {
                state.Exit(node.controller);
            }
        }

        private IFSMState GetState(string scripteName, MonoScript script)
        {
            IFSMState state;

            if (!states.TryGetValue(scripteName, out state))
            {
                if (script != null)
                {
                    Type scriptType = script.GetClass();
                    state = Activator.CreateInstance(scriptType) as IFSMState;
                }
                else
                {
                    Type stateType = GetType(scripteName);
                    if (stateType != null)
                    {
                        state = Activator.CreateInstance(stateType) as IFSMState;
                    }
                }
                if (state != null)
                {
                    states.Add(scripteName, state);
                }
            }
            
            return state;
        }

        private Type GetType(string sctriptName)
        {
            if (sctriptName == null) return null;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var item in assemblies)
            {
                if (item.FullName.StartsWith("UnityEngin") || item.FullName.StartsWith("UnityEditor") || item.FullName.StartsWith("System") || item.FullName.StartsWith("Microsoft"))
                { continue; }

                Type t = item.GetTypes().Where(x => x.FullName == sctriptName).FirstOrDefault();
                if (t != null && t.GetInterfaces().Where(x => x == typeof(IFSMState)).FirstOrDefault() != null)
                {
                    return t;
                }
            }
            return null;
        }
    }
}