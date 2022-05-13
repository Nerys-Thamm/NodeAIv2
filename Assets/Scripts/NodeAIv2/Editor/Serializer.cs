using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

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

        public NodeAI_Behaviour Serialize()
        {
            var nodeAI_Behaviour = ScriptableObject.CreateInstance<NodeAI_Behaviour>();
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
                    runtimeLogic = node.runtimeLogic
                };
                nodeAI_Behaviour.nodeData.Add(nodeData);
            }

            foreach (var edge in edges)
            {
                var inputNodeData = nodeAI_Behaviour.nodeData.Find(x => x.GUID == ((Node)edge.input.node).GUID);
                var outputNodeData = nodeAI_Behaviour.nodeData.Find(x => x.GUID == ((Node)edge.output.node).GUID);

                inputNodeData.parentGUID = outputNodeData.GUID;
                outputNodeData.childGUIDs.Add(inputNodeData.GUID);
            }
            
            nodeAI_Behaviour.nodeTree = NodeTree.CreateFromNodeData(nodeAI_Behaviour.nodeData.Find(x => x.nodeType == NodeData.Type.EntryPoint), nodeAI_Behaviour.nodeData);

            return nodeAI_Behaviour;
        }


        public void Deserialize(NodeAI_Behaviour nodeAI_Behaviour)
        {
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
                if (nodeData.nodeType != NodeData.Type.EntryPoint)
                {
                    var parent = nodes.Find(x => x.GUID == nodeData.parentGUID);
                    var edge = parent.outputPort.ConnectTo(node.inputPort);
                    edges.Add(edge);
                    target.AddElement(edge);
                }
                
            }

            
        }

        

        
    }
}
