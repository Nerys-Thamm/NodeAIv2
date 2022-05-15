using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System.Linq;

namespace NodeAI
{
    public class Serializer
    {
        private GraphView target;

        private List<Edge> edges => target.edges.ToList();
        private List<Node> nodes => target.nodes.ToList().ConvertAll(x => x as Node);

        

        public static Serializer GetInstance(GraphView target)
        {
            return new Serializer
            {
                target = target
            };
            
        }

        public void Serialize(NodeAI_Behaviour nodeAI_Behaviour)
        {
            nodeAI_Behaviour.nodeData = new List<NodeData>();
            nodeAI_Behaviour.nodeTree = new NodeTree();
            foreach (var node in nodes)
            {
                var nodeData = new NodeData
                {
                    GUID = node.GUID,
                    nodeType = node.nodeType,
                    position = node.GetPosition().position,
                    childGUIDs = new List<string>(),
                    title = node.title,
                    runtimeLogic = ScriptableObject.Instantiate(node.runtimeLogic)
                };
                if(node.nodeType == NodeData.Type.Parameter) nodeData.parentGUID = node.paramReference;
                foreach (var input in node.inputPorts)
                {
                    if (input.connections.Count() > 0)
                    {
                        nodeData.runtimeLogic.SetPropertyParamReference(input.portName, ((Node)input.connections.First().output.node).paramReference);
                    }
                    else
                    {
                        nodeData.runtimeLogic.SetPropertyParamReference(input.portName, "null");
                    }
                }
                nodeAI_Behaviour.nodeData.Add(nodeData);
            }

            foreach (var edge in edges)
            {
                if(((Node)edge.output.node).nodeType == NodeData.Type.Parameter)
                {
                    ((Node)edge.input.node).runtimeLogic.SetPropertyGUID(edge.input.portName, ((Node)edge.output.node).GUID);
                }
                else
                {
                    var inputNodeData = nodeAI_Behaviour.nodeData.Find(x => x.GUID == ((Node)edge.input.node).GUID);
                    var outputNodeData = nodeAI_Behaviour.nodeData.Find(x => x.GUID == ((Node)edge.output.node).GUID);

                    inputNodeData.parentGUID = outputNodeData.GUID;
                    outputNodeData.childGUIDs.Add(inputNodeData.GUID);
                }
            }
            
            nodeAI_Behaviour.nodeTree = NodeTree.CreateFromNodeData(nodeAI_Behaviour.nodeData.Find(x => x.nodeType == NodeData.Type.EntryPoint), nodeAI_Behaviour.nodeData);

            
            foreach(var p in target.exposedProperties)
            {
                nodeAI_Behaviour.exposedProperties.Add(p);
            }
            
            
        }


        public void Deserialize(NodeAI_Behaviour nodeAI_Behaviour)
        {
            target.exposedProperties.Clear();
            target.blackboard.Clear();
            foreach (var property in nodeAI_Behaviour.exposedProperties)
            {
                target.AddPropertyToBlackboard(property);
            }
            
            foreach (var node in nodes)
            {
                target.RemoveElement(node);
            }
            foreach (var edge in edges)
            {
                target.RemoveElement(edge);
            }

            foreach (var nodeData in nodeAI_Behaviour.nodeData)
            {
                var node = target.GenerateNode(nodeData);
                target.AddElement(node);
                nodes.Add(node);
            }

            foreach (var nodeData in nodeAI_Behaviour.nodeData)
            {
                var node = nodes.Find(x => x.GUID == nodeData.GUID);
                if (nodeData.nodeType != NodeData.Type.EntryPoint && nodeData.nodeType != NodeData.Type.Parameter)
                {
                    var parent = nodes.Find(x => x.GUID == nodeData.parentGUID);
                    var edge = parent.outputPort.ConnectTo(node.inputPort);
                    edges.Add(edge);
                    target.AddElement(edge);
                }
                else if(nodeData.nodeType == NodeData.Type.Parameter)
                {
                    node.paramReference = nodeData.parentGUID;
                    
                }
                foreach(Port input in node.inputPorts)
                {
                    string connGUID = nodeData.runtimeLogic.GetProperties().Find(x => x.name == input.portName).GUID;
                    var conn = nodes.Find(x => x.GUID == connGUID);
                    if (conn != null)
                    {
                        var edge = conn.outputPort.ConnectTo(input);
                        edges.Add(edge);
                        target.AddElement(edge);
                    }
                }
                
            }
            
            

            
        }

        

        
    }
}
