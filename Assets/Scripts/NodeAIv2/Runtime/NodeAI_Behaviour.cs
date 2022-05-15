using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    [System.Serializable]
    public class NodeAI_Behaviour : ScriptableObject
    {
        [SerializeField]
        public List<NodeData> nodeData;
        [SerializeField]
        public NodeTree nodeTree;
        [SerializeField]
        public List<NodeData.SerializableProperty> exposedProperties = new List<NodeData.SerializableProperty>();

        
    }
}