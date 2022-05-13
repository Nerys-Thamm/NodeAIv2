using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    [System.Serializable]
    public class NodeTree 
    {
        public NodeData rootNode;
        public Leaf rootLeaf;
        public List<NodeData> nodes;

        
        [System.Serializable]
        public class Leaf
        {
            public NodeData nodeData;
            public List<Leaf> children;

            
            
            public Leaf()
            {
                children = new List<Leaf>();
            }
        }

        public static NodeTree CreateFromNodeData(NodeData rootNodeData, List<NodeData> data)
        {
            NodeTree nodeTree = new NodeTree();
            nodeTree.rootNode = rootNodeData;
            nodeTree.nodes = data;
            nodeTree.rootLeaf = new Leaf();
            nodeTree.rootLeaf.nodeData = rootNodeData;
            nodeTree.BuildTree(nodeTree.rootLeaf);
            return nodeTree;
        }

        private void BuildTree(Leaf leaf)
        {
            foreach (var child in leaf.nodeData.childGUIDs)
            {
                var childLeaf = new Leaf();
                childLeaf.nodeData = nodes.Find(x => x.GUID == child);
                leaf.children.Add(childLeaf);
                leaf.children.Sort((x, y) => x.nodeData.position.y.CompareTo(y.nodeData.position.y));
                BuildTree(childLeaf);
            }
            
        }
    }
}
