using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    public class NodeAI_Agent : MonoBehaviour
    {
        public NodeAI_Behaviour behaviour;
        public NodeTree nodeTree;
        // Start is called before the first frame update
        void Start()
        {
            nodeTree = Instantiate(behaviour).nodeTree;
            nodeTree.rootLeaf.nodeData.runtimeLogic.Init(nodeTree.rootLeaf);
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log(nodeTree.rootNode.Eval(this, nodeTree.rootLeaf));
        }
    }
}
