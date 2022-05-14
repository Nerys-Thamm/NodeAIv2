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

        [SerializeField]
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
                switch(property.type.Name)
                    {
                        case "Int32":
                            serializableProperty.ivalue = (int)(object)property.value;
                            break;
                        case "Single":
                            serializableProperty.fvalue = (float)(object)property.value;
                            break;
                        case "Boolean":
                            serializableProperty.bvalue = (bool)(object)property.value;
                            break;
                        case "String":
                            serializableProperty.svalue = (string)(object)property.value;
                            break;
                        case "Vector2":
                            serializableProperty.v2value = (Vector2)(object)property.value;
                            break;
                        case "Vector3":
                            serializableProperty.v3value = (Vector3)(object)property.value;
                            break;
                        case "Vector4":
                            serializableProperty.v4value = (Vector4)(object)property.value;
                            break;
                        case "Color":
                            serializableProperty.cvalue = (Color)(object)property.value;
                            break;
                        default:
                            break;
                    }
                return serializableProperty;
            }

            public static implicit operator Property(SerializableProperty serializableProperty)
            {
                var property = new Property();
                property.name = serializableProperty.name;
                property.GUID = serializableProperty.GUID;
                property.type = System.Type.GetType(serializableProperty.serializedTypename);
                switch(property.type.Name)
                    {
                        case "Int32":
                            property.value = serializableProperty.ivalue;
                            break;
                        case "Single":
                            property.value = serializableProperty.fvalue;
                            break;
                        case "Boolean":
                            property.value = serializableProperty.bvalue;
                            break;
                        case "String":
                            property.value = serializableProperty.svalue;
                            break;
                        case "Vector2":
                            property.value = serializableProperty.v2value;
                            break;
                        case "Vector3":
                            property.value = serializableProperty.v3value;
                            break;
                        case "Vector4":
                            property.value = serializableProperty.v4value;
                            break;
                        case "Color":
                            property.value = serializableProperty.cvalue;
                            break;
                        default:
                            break;
                    }
                return property;
            }
            public string name;
            public string GUID;
            public string serializedTypename;
            public System.Type type => System.Type.GetType(serializedTypename);
            public string svalue;
            public int ivalue;
            public float fvalue;
            public bool bvalue;
            public Color cvalue;
            public Vector2 v2value;
            public Vector3 v3value;
            public Vector4 v4value;

        }
        
    }


}
