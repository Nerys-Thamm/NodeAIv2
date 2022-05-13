using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    [System.Serializable]
    public class NodeData 
    {
        public string GUID;
        public string parentGUID;
        public string title;
        public List<string> childGUIDs;
        public Type nodeType;
        public Vector2 position;
        

        public enum Type
            {
                EntryPoint,
                Action,
                Condition,
                Decorator,
                Sequence,
                Selector,
                Parallel,
            }

        public enum State
        {
            Running,
            Success,
            Failure,
            Idle,
        }

        public State Eval(NodeAI_Agent agent, NodeTree.Leaf current) => runtimeLogic.Eval(agent, current);

        public RuntimeBase runtimeLogic;


        public abstract class Property
        {
            public string name;
            public System.Type type;
        }

        [System.Serializable]
        public class Property<T> : Property
        {
            public T value;
        }
        
    }


}
