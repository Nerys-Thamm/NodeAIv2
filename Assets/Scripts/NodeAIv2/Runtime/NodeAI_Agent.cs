using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    public class NodeAI_Agent : MonoBehaviour
    {
        public NodeAI_Behaviour behaviour;
        public NodeTree nodeTree;

        const float tickRate = 0.1f;
        float tickTimer = 0f;

        // Start is called before the first frame update
        void Start()
        {
            behaviour = Instantiate(behaviour);
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
    }
}
