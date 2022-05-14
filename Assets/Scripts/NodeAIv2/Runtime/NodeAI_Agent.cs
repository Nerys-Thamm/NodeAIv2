using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    public class NodeAI_Agent : MonoBehaviour
    {
        public NodeAI_Behaviour AI_Behaviour;
        NodeAI_Behaviour behaviour;
        public NodeTree nodeTree;

        const float tickRate = 0.1f;
        float tickTimer = 0f;

        // Start is called before the first frame update
        void Start()
        {
            behaviour = Instantiate(AI_Behaviour);
            nodeTree = behaviour.nodeTree;
            nodeTree.rootLeaf.nodeData.runtimeLogic.Init(nodeTree.rootLeaf);
        }

        // Update is called once per frame
        void Update()
        {
            tickTimer += Time.deltaTime;
            if (tickTimer > tickRate)
            {
                tickTimer = 0f;
                nodeTree.rootNode.Eval(this, nodeTree.rootLeaf);
            }
            
        }

        public void SetParameter<T>(string name, T value)
        {
            foreach (var property in behaviour.exposedProperties)
            {
                if (property.name == name && property.type == typeof(T))
                {
                    switch(property.type.Name)
                    {
                        case "Int32":
                            property.ivalue = (int)(object)value;
                            break;
                        case "Single":
                            property.fvalue = (float)(object)value;
                            break;
                        case "Boolean":
                            property.bvalue = (bool)(object)value;
                            break;
                        case "String":
                            property.svalue = (string)(object)value;
                            break;
                        case "Vector2":
                            property.v2value = (Vector2)(object)value;
                            break;
                        case "Vector3":
                            property.v3value = (Vector3)(object)value;
                            break;
                        case "Vector4":
                            property.v4value = (Vector4)(object)value;
                            break;
                        case "Color":
                            property.cvalue = (Color)(object)value;
                            break;
                        default:
                            break;
                    }
                    nodeTree.PropogateExposedProperties(behaviour.exposedProperties);
                    return;
                }
            }
            
        }

        public T GetParameter<T>(string name)
        {
            foreach (var property in behaviour.exposedProperties)
            {
                if (property.name == name && property.type == typeof(T))
                {
                    switch(property.type.Name)
                    {
                        case "Int32":
                            return (T)(object)property.ivalue;
                        case "Single":
                            return (T)(object)property.fvalue;
                        case "Boolean":
                            return (T)(object)property.bvalue;
                        case "String":
                            return (T)(object)property.svalue;
                        case "Vector2":
                            return (T)(object)property.v2value;
                        case "Vector3":
                            return (T)(object)property.v3value;
                        case "Vector4":
                            return (T)(object)property.v4value;
                        case "Color":
                            return (T)(object)property.cvalue;
                        default:
                            break;
                    }
                }
            }
            return default(T);
        }

        void OnDrawGizmos()
        {
            if (nodeTree != null)
            {
                nodeTree.DrawGizmos(this);
            }
        }
    }
}
