using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    public class DebugNode : ActionBase
    {
        public DebugNode()
        {
            AddProperty<string>("Message", "Debug");
            AddProperty<float>("Duration", 1f);
            AddProperty<bool>("Show", true);
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            Debug.Log("Debug: " + GetProperty<string>("Message"));
            return NodeData.State.Success;
        }
    }
}
