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
        public List<Property> properties;

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


        [System.Serializable]
        public abstract class Property
        {
            public string name;
            public string typename;

        }

        [System.Serializable]
        public class StringProperty : Property
        {
            public string value;

            StringProperty(string name, string value)
            {
                this.name = name;
                this.value = value;
                this.typename = "string";
            }
        }

        [System.Serializable]
        public class IntProperty : Property
        {
            public int value;

            IntProperty(string name, int value)
            {
                this.name = name;
                this.value = value;
                this.typename = "int";
            }
        }

        [System.Serializable]
        public class FloatProperty : Property
        {
            public float value;

            FloatProperty(string name, float value)
            {
                this.name = name;
                this.value = value;
                this.typename = "float";
            }
        }

        [System.Serializable]
        public class BoolProperty : Property
        {
            public bool value;

            BoolProperty(string name, bool value)
            {
                this.name = name;
                this.value = value;
                this.typename = "bool";
            }
        }

        [System.Serializable]
        public class Vector2Property : Property
        {
            public Vector2 value;

            Vector2Property(string name, Vector2 value)
            {
                this.name = name;
                this.value = value;
                this.typename = "Vector2";
            }
        }

        [System.Serializable]
        public class Vector3Property : Property
        {
            public Vector3 value;

            Vector3Property(string name, Vector3 value)
            {
                this.name = name;
                this.value = value;
                this.typename = "Vector3";
            }
        }

        


            

        
    }


}
