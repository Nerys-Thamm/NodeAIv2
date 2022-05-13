using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    [System.Serializable]
    public class RuntimeBase : ScriptableObject
    {
        protected NodeData.State state;

        List<NodeData.Property> properties;

        public void AddProperty<T>(string name, T initialValue)
        {
            if(properties == null)
            {
                properties = new List<NodeData.Property>();
            }
            foreach (NodeData.Property property in properties)
            {
                if (property.name == name.ToUpper())
                {
                    Debug.LogError("Property with name " + name + " already exists");
                    return;
                }
            }
            properties.Add(new NodeData.Property<T>()
            {
                name = name.ToUpper(),
                type = typeof(T),
                value = initialValue
            });
        }

        public void SetProperty<T>(string name, T value)
        {
            foreach (NodeData.Property property in properties)
            {
                if (property.name == name.ToUpper() && property.type == typeof(T))
                {
                    ((NodeData.Property<T>)property).value = value;
                    return;
                }
            }
            Debug.LogError("Property with name " + name + " does not exist");
        }

        public T GetProperty<T>(string name)
        {
            if(properties == null)
            {
                return default(T);
            }
            foreach (NodeData.Property property in properties)
            {
                if (property.name == name.ToUpper() && property.type == typeof(T))
                {
                    return ((NodeData.Property<T>)property).value;
                }
            }
            return default(T);
        }

        public NodeData.Property[] GetProperties()
        {
            if(properties == null) properties = new List<NodeData.Property>();
            return properties.ToArray();
        }


        public virtual NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current) => NodeData.State.Failure;
        public void Init(NodeTree.Leaf current) 
        {
            state = NodeData.State.Running;
            foreach (NodeTree.Leaf child in current.children)
            {
                child.nodeData.runtimeLogic.Init(child);
            }
        }
    }

    public class ActionBase : RuntimeBase
    {
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            return NodeData.State.Success;
        }
    }

    public class ConditionBase : RuntimeBase
    {
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            return NodeData.State.Success;
        }
    }

    public class TestCondition : ConditionBase
    {
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            return NodeData.State.Success;
        }
    }

    public class DecoratorBase : RuntimeBase
    {
        public virtual NodeData.State ApplyDecorator(NodeAI_Agent agent, NodeTree.Leaf child) => child.nodeData.Eval(agent, child);
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            return ApplyDecorator(agent, current.children[0]);
        }

        
    }

    public class Inverter : DecoratorBase
    {
        public override NodeData.State ApplyDecorator(NodeAI_Agent agent, NodeTree.Leaf child)
        {
            NodeData.State childState = child.nodeData.Eval(agent, child);
            if (childState == NodeData.State.Success)
            {
                return NodeData.State.Failure;
            }
            else if (childState == NodeData.State.Failure)
            {
                return NodeData.State.Success;
            }
            else
            {
                return childState;
            }
        }
    }

    public class Succeeder : DecoratorBase
    {
        public override NodeData.State ApplyDecorator(NodeAI_Agent agent, NodeTree.Leaf child)
        {
            return NodeData.State.Success;
        }
    }

    public class Sequence : RuntimeBase
    {
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            bool anyChildRunning = false;

            foreach (NodeTree.Leaf child in current.children)
            {
                switch (child.nodeData.Eval(agent, child))
                {
                    case NodeData.State.Failure:
                        state = NodeData.State.Failure;
                        return state;
                    case NodeData.State.Success:
                        continue;
                    case NodeData.State.Running:
                        anyChildRunning = true;
                        continue;
                    default:
                        state = NodeData.State.Success;
                        return state;
                }
            }

            state = anyChildRunning ? NodeData.State.Running : NodeData.State.Success;
            return state;
        }
    }

    public class Selector : RuntimeBase
    {
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            foreach (NodeTree.Leaf child in current.children)
            {
                switch (child.nodeData.Eval(agent, child))
                {
                    case NodeData.State.Failure:
                        continue;
                    case NodeData.State.Success:
                        state = NodeData.State.Success;
                        return state;
                    case NodeData.State.Running:
                        state = NodeData.State.Running;
                        return state;
                    default:
                        state = NodeData.State.Success;
                        return state;
                }
            }
            state = NodeData.State.Failure;
            return state;
        }
    }

    



    


}

