using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;

namespace NodeAI
{
    public class Graph : EditorWindow
    {
        private GraphView graphView;
        private NodeAI_Behaviour behaviour;

        [MenuItem("Window/NodeAI/Graph")]
        public static void OpenGraphWindow()
        {
            Graph window = (Graph)EditorWindow.GetWindow(typeof(Graph));
            window.titleContent = new GUIContent("NodeAI Graph");
            window.Show();
        }

        private void OnEnable()
        {
            graphView = new GraphView();
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
            GenerateBlackboard();
            GenerateToolbar();
            if(behaviour != null)
            {
                Serializer.GetInstance(graphView).Deserialize(behaviour);
            }
        }

        

        private void DrawUI()
        {
            
        }

        private void CreateNewBehaviour()
        {
            GraphView newGraph = new GraphView();
            newGraph.AddEntryNode();
            var serializer = Serializer.GetInstance(newGraph);

            behaviour = serializer.Serialize();
            
            
            ProjectWindowUtil.CreateAsset(behaviour, "NodeAI_Behaviour.asset");
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(behaviour);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Serializer.GetInstance(graphView).Deserialize(behaviour);
        }

        
        private void ProcessEvents(Event e)
        {
            
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            
        }

        private void OnDisable()
        {
            //GraphView graphView = rootVisualElement.GetFirstAncestorOfType<GraphView>();
            graphView.RemoveFromHierarchy();
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();
            var saveButton = new ToolbarButton(() => { 
                if(behaviour != null)
                {
                    NodeAI_Behaviour newData = Serializer.GetInstance(graphView).Serialize();
                    behaviour.nodeData = newData.nodeData;
                    behaviour.nodeTree = newData.nodeTree;
                    behaviour.exposedProperties = newData.exposedProperties;
                    EditorUtility.SetDirty(behaviour);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                else
                {
                    behaviour = Serializer.GetInstance(graphView).Serialize();
                    ProjectWindowUtil.CreateAsset(behaviour, "NodeAI_Behaviour.asset");
                    AssetDatabase.Refresh();
                    EditorUtility.SetDirty(behaviour);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            });
            saveButton.text = "Save";
            toolbar.Add(saveButton);

            var newBehaviourButton = new ToolbarButton(() => { CreateNewBehaviour(); });
            newBehaviourButton.text = "New Behaviour";
            toolbar.Add(newBehaviourButton);

            var objField = new ObjectField();
            objField.objectType = typeof(NodeAI_Behaviour);
            objField.allowSceneObjects = false;
            objField.value = behaviour;
            objField.RegisterValueChangedCallback(evt => {
                behaviour = (NodeAI_Behaviour)evt.newValue;
                if(behaviour != null)
                {
                    Serializer.GetInstance(graphView).Deserialize(behaviour);
                }
            });
            
            toolbar.Add(objField);

            
            rootVisualElement.Add(toolbar);


        }

        private void GenerateBlackboard()
        {
            var blackboard = new Blackboard(graphView);
            blackboard.Add(new BlackboardSection{ title = "Exposed Properties" });
            blackboard.addItemRequested = _blackboard =>
            {
                graphView.AddPropertyToBlackboard(new NodeData.Property<string> { name = "New Property", value = "String" });
            };
            blackboard.editTextRequested = (bb, element, newVal) =>
            {
                var oldName = ((BlackboardField)element).text;
                if(graphView.exposedProperties.Any(x => x.name == newVal))
                {
                    EditorUtility.DisplayDialog("Error", "Property with name " + newVal + " already exists", "OK");
                    return;
                }

                var index = graphView.exposedProperties.FindIndex(x => x.name == oldName);
                graphView.exposedProperties[index].name = newVal;
                ((BlackboardField)element).text = newVal;
            };

            blackboard.SetPosition(new Rect(10, 30, 200, 300));
            graphView.blackboard = blackboard;
            graphView.Add(blackboard);
        }



        
    }
}
