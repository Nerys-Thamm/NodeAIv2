using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    public class WaitFor : ActionBase
    {
        public float timer = 0f;
        public WaitFor()
        {
            AddProperty<float>("Time", 1f);
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (timer > GetProperty<float>("Time"))
            {
                state = NodeData.State.Success;
            }
            else
            {
                timer += Time.deltaTime;
                state = NodeData.State.Running;
            }

            return state;
        }

        public override void OnInit()
        {
            timer = 0f;
        }
        
    }
}