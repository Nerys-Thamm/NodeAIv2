using System.Collections;
using System.Collections.Generic;
using NodeAI;
using UnityEngine;
using UnityEngine.AI;

public class Bonestag_Charge : NodeAI.ActionBase
{
    Animator animator;
    NavMeshAgent navAgent;
    Vector3 target;
    public Bonestag_Charge()
    {
        AddProperty<bool>("InBearTrap", false);
        AddProperty<float>("ChargeSpeed", 0.5f);
    }
    public override void OnInit()
    {
        SetProperty<bool>("InBearTrap", false);
    }

    public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    {
        if (animator == null)
        {
            animator = agent.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Bonestag_Charge: No animator found on agent");
                return NodeData.State.Failure;
            }
        }
        if (navAgent == null)
        {
            navAgent = agent.GetComponent<NavMeshAgent>();
            if (navAgent == null)
            {
                Debug.LogError("Bonestag_Charge: No navAgent found on agent");
                return NodeData.State.Failure;
            }
        }
        if(GetProperty<bool>("InBearTrap"))
        {
            navAgent.SetDestination(agent.transform.position);
            navAgent.isStopped = true;
            animator.SetTrigger("HitBearTrap");
            state = NodeData.State.Failure;
            return state;
        }
        else
        {
            if(Vector3.Distance(agent.transform.position, target) < 0.5f)
            {
                navAgent.SetDestination(agent.transform.position);
                navAgent.isStopped = true;
                animator.SetBool("Charging", false);
                state = NodeData.State.Success;
                return state;
            }
            else
            {
                navAgent.SetDestination(target);
                navAgent.isStopped = false;
                animator.SetBool("Charging", true);
                state = NodeData.State.Running;
                return state;
            }
        }
    }
}
