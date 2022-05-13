using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

namespace NodeAI
{
    public class SearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private GraphView graphView;

        private Node selectedNode;

        public void Init(GraphView graphView)
        {
            this.graphView = graphView;
        }

        public void SetSelectedNode(Node node)
        {
            selectedNode = node;
        }



        Type[] GetInheritedClasses(Type MyType) 
        {
            return Assembly.GetAssembly(MyType).GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(MyType)).ToArray();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("New Node"), 0),
                new SearchTreeGroupEntry(new GUIContent("Control Node"), 1),
                new SearchTreeEntry(new GUIContent("Sequence"))
                {
                    userData = typeof(Sequence), level = 2
                },
                
            };
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Action"), 1));
            foreach (var type in GetInheritedClasses(typeof(ActionBase)))
            {
                tree.Add(new SearchTreeEntry(new GUIContent(type.Name))
                {
                    userData = type, level = 2
                });
            }
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Condition"), 1));
            foreach (var type in GetInheritedClasses(typeof(ConditionBase)))
            {
                tree.Add(new SearchTreeEntry(new GUIContent(type.Name))
                {
                    userData = type, level = 2
                });
            }
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            Node newNode = null;
            if(((Type)entry.userData) == typeof(Sequence))
            {
                newNode = graphView.ContextCreateNode(selectedNode, NodeData.Type.Sequence, ((Type)entry.userData).Name);
                newNode.runtimeLogic = ScriptableObject.CreateInstance<Sequence>();
            }
            else if(((Type)entry.userData) == typeof(Selector))
            {
                newNode = graphView.ContextCreateNode(selectedNode, NodeData.Type.Selector, ((Type)entry.userData).Name);
                newNode.runtimeLogic = ScriptableObject.CreateInstance<Selector>();
            }
            else if(((Type)entry.userData).BaseType == typeof(DecoratorBase))
            {
                newNode = graphView.ContextCreateNode(selectedNode, NodeData.Type.Decorator, ((Type)entry.userData).Name);
                newNode.runtimeLogic = (DecoratorBase)ScriptableObject.CreateInstance(((Type)entry.userData));
            }
            else if(((Type)entry.userData).BaseType == typeof(ActionBase))
            {
                newNode = graphView.ContextCreateNode(selectedNode, NodeData.Type.Action, ((Type)entry.userData).Name);
                newNode.runtimeLogic = (ActionBase)ScriptableObject.CreateInstance(((Type)entry.userData));
            }
            else if(((Type)entry.userData).BaseType == typeof(ConditionBase))
            {
                newNode = graphView.ContextCreateNode(selectedNode, NodeData.Type.Condition, ((Type)entry.userData).Name);
                newNode.runtimeLogic = (ConditionBase)ScriptableObject.CreateInstance(((Type)entry.userData));
            }
            return true;
        }
    }
}
