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
            public System.Type type;

            public object value;
        }

        [System.Serializable]
        public class Property<T> : Property
        {
            public Property()
            {
                type = typeof(T);
                GUID = System.Guid.NewGuid().ToString();
                value = default(T);
            }
            public T Value => (T)this.value;
            
        }
        [System.Serializable]
        public class SerializableProperty
        {
            public static implicit operator SerializableProperty(Property property)
            {
                var serializableProperty = new SerializableProperty();
                serializableProperty.name = property.name;
                serializableProperty.GUID = property.GUID;
                serializableProperty.serializedTypename = property.type.AssemblyQualifiedName;
                serializableProperty.value = property.value;
                return serializableProperty;
            }

            public static implicit operator Property(SerializableProperty serializableProperty)
            {
                var property = new Property();
                property.name = serializableProperty.name;
                property.GUID = serializableProperty.GUID;
                property.type = System.Type.GetType(serializableProperty.serializedTypename);
                property.value = serializableProperty.value;
                return property;
            }
            public string name;
            public string GUID;
            public string serializedTypename;

            public object value;

        }
        
    }


}
