using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    public class NodeAI_Behaviour : ScriptableObject
    {
        public List<NodeData> nodeData;
        
        public NodeTree nodeTree;
        [SerializeField]
        public List<NodeData.Property> exposedProperties = new List<NodeData.Property>();
    }
}