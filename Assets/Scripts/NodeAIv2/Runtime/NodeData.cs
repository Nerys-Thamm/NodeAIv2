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
                Parameter
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

        [System.Serializable]
        public class Property
        {
            [SerializeField]
            public string name;
            [SerializeField]
            public string GUID;
            [SerializeField]
            public string typeName;
        }

        [System.Serializable]
        public class Property<T> : Property
        {
            public Property()
            {
                typeName = typeof(T).Name;
                GUID = System.Guid.NewGuid().ToString();
                value = default(T);
            }
            [SerializeField]
            public T value;
        }
        
    }


}
