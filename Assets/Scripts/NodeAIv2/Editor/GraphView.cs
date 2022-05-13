using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

namespace NodeAI
{
    public class GraphView : UnityEditor.Experimental.GraphView.GraphView
    {
        private SearchWindow searchWindow;

        public GraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("GraphStyle"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
            searchWindow = ScriptableObject.CreateInstance<SearchWindow>();
            searchWindow.Init(this);
            
        }

        private void AddSearchWindow(Vector2 position)
        {
            
            UnityEditor.Experimental.GraphView.SearchWindow.Open(new SearchWindowContext(position), searchWindow);

        }

        public void AddEntryNode()
        {
            AddElement(GenerateEntryPointNode());
        }

        private Port GeneratePort(Node node, Direction dir, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, dir, capacity, typeof(float));
        }

        private Node GenerateEntryPointNode()
        {
            Node entryPointNode = new Node();
            entryPointNode.title = "Entry Point";
            entryPointNode.GUID = System.Guid.NewGuid().ToString();
            entryPointNode.nodeType = NodeData.Type.EntryPoint;
            entryPointNode.SetPosition(new Rect(new Vector2(0, 0), new Vector2(200, 200)));

            entryPointNode.capabilities = UnityEditor.Experimental.GraphView.Capabilities.Selectable;

            Port newPort = GeneratePort(entryPointNode, Direction.Output);
            newPort.portName = "";
            entryPointNode.outputContainer.Add(newPort);
            entryPointNode.outputPort = newPort;

            Button btn_newChild = new Button(() =>
            {
                if(entryPointNode.outputPort.Query("connection").ToList().Count > 1) return;
                AddSearchWindow(entryPointNode.outputPort.GetPosition().position);
                // GenericMenu menu = new GenericMenu();
                
                // menu.AddItem(new GUIContent("Create/Action"), false, () => {
                //     ContextCreateNode(entryPointNode, NodeData.Type.Action);
                // });
                // menu.AddItem(new GUIContent("Create/Condition"), false, () => {
                //     ContextCreateNode(entryPointNode, NodeData.Type.Condition);
                // });
                // menu.AddItem(new GUIContent("Create/Decorator"), false, () => {
                //     ContextCreateNode(entryPointNode, NodeData.Type.Decorator);
                // });
                // menu.AddItem(new GUIContent("Create/Sequence"), false, () => {
                //     ContextCreateNode(entryPointNode, NodeData.Type.Sequence);
                // });
                // menu.AddItem(new GUIContent("Create/Fallback"), false, () => {
                //     ContextCreateNode(entryPointNode, NodeData.Type.Fallback);
                // });
                // menu.AddItem(new GUIContent("Create/Parallel"), false, () => {
                //     ContextCreateNode(entryPointNode, NodeData.Type.Parallel);
                // });

                
                // menu.ShowAsContext();
            });
            entryPointNode.titleContainer.Add(btn_newChild);

            

            entryPointNode.RefreshExpandedState();
            entryPointNode.RefreshPorts();


            return entryPointNode;
        }

        public Node ContextCreateNode(Node parent, NodeData.Type type, string name, RuntimeBase logic)
        {
            Node newNode = GenerateNode(type, name, logic);
            AddElement(newNode);

            newNode.SetPosition(new Rect(new Vector2(parent.GetPosition().x + parent.GetPosition().width, parent.GetPosition().y), new Vector2(200, 200)));

            AddElement(parent.outputPort.ConnectTo(newNode.inputPort));

            parent.RefreshExpandedState();
            parent.RefreshPorts();
            newNode.RefreshExpandedState();
            newNode.RefreshPorts();

            return newNode;
            
        }

        public Node GenerateNode(NodeData data)
        {
            Node newNode = new Node();
            newNode.title = data.title;
            newNode.GUID = data.GUID;
            newNode.nodeType = data.nodeType;
            newNode.SetPosition(new Rect(data.position, new Vector2(200, 200)));

            switch (data.nodeType)
            {
                case NodeData.Type.Action:
                    newNode.inputPort = GeneratePort(newNode, Direction.Input);
                    newNode.inputPort.portName = "";
                    
                    break;
                case NodeData.Type.Condition:
                    newNode.inputPort = GeneratePort(newNode, Direction.Input);
                    newNode.inputPort.portName = "";
                    
                    break;
                case NodeData.Type.Decorator:
                    newNode.inputPort = GeneratePort(newNode, Direction.Input);
                    newNode.inputPort.portName = "";
                    newNode.outputPort = GeneratePort(newNode, Direction.Output);
                    newNode.outputPort.portName = "";
                    break;
                case NodeData.Type.Sequence:
                    newNode.inputPort = GeneratePort(newNode, Direction.Input);
                    newNode.inputPort.portName = "";
                    newNode.outputPort = GeneratePort(newNode, Direction.Output, Port.Capacity.Multi);
                    newNode.outputPort.portName = "";
                    break;
                case NodeData.Type.Selector:
                    newNode.inputPort = GeneratePort(newNode, Direction.Input);
                    newNode.inputPort.portName = "";
                    newNode.outputPort = GeneratePort(newNode, Direction.Output, Port.Capacity.Multi);
                    newNode.outputPort.portName = "";
                    break;
                case NodeData.Type.Parallel:
                    newNode.inputPort = GeneratePort(newNode, Direction.Input);
                    newNode.inputPort.portName = "";
                    newNode.outputPort = GeneratePort(newNode, Direction.Output, Port.Capacity.Multi);
                    newNode.outputPort.portName = "";
                    break;
                case NodeData.Type.EntryPoint:
                    newNode.outputPort = GeneratePort(newNode, Direction.Output );
                    newNode.outputPort.portName = "";
                    break;
            }

            if(newNode.inputPort != null)
                newNode.inputContainer.Add(newNode.inputPort);
            if(newNode.outputPort != null)
                newNode.outputContainer.Add(newNode.outputPort);

            newNode.runtimeLogic = data.runtimeLogic;
            if(newNode.runtimeLogic != null)
            {
                NodeData.Property[] properties = newNode.runtimeLogic.GetProperties();
                foreach(NodeData.Property p in properties)
                {
                    AddPropertyField(newNode, p, newNode.runtimeLogic);
                }
            }
            if(!(newNode.nodeType == NodeData.Type.Action || newNode.nodeType == NodeData.Type.Condition))
            {
                Button btn_newChild = new Button(() =>
                {
                    if(newNode.outputPort.connected && newNode.outputPort.capacity == Port.Capacity.Single) return;
                    AddSearchWindow(GUIUtility.GUIToScreenPoint(newNode.GetGlobalCenter()));
                    searchWindow.SetSelectedNode(newNode);
                });
                newNode.titleContainer.Add(btn_newChild);
            }
            newNode.RefreshExpandedState();
            newNode.RefreshPorts();

            return newNode;
        }

        public Node GenerateNode(NodeData.Type nodeType, string name, RuntimeBase logic)
        {
            Node node = new Node();
            node.title = name;
            node.GUID = System.Guid.NewGuid().ToString();
            node.nodeType = nodeType;
            node.SetPosition(new Rect(new Vector2(0, 0), new Vector2(200, 200)));
            node.runtimeLogic = logic;
            switch (nodeType)
            {
                case NodeData.Type.Action:
                    {
                        Port newPort = GeneratePort(node, Direction.Input);
                        newPort.portName = "";
                        node.inputContainer.Add(newPort);
                        node.inputPort = newPort;
                    }
                    break;
                case NodeData.Type.Condition:
                    {
                        Port newPort = GeneratePort(node, Direction.Input);
                        newPort.portName = "";
                        node.inputContainer.Add(newPort);
                        node.inputPort = newPort;
                    }
                    break;
                case NodeData.Type.Decorator:
                    {
                        Port newPort = GeneratePort(node, Direction.Input);
                        newPort.portName = "";
                        node.inputContainer.Add(newPort);
                        node.inputPort = newPort;

                        newPort = GeneratePort(node, Direction.Output);
                        newPort.portName = "";
                        node.outputContainer.Add(newPort);
                        node.outputPort = newPort;
                    }
                    break;
                case NodeData.Type.Sequence:
                    {
                        Port newPort = GeneratePort(node, Direction.Input);
                        newPort.portName = "";
                        node.inputContainer.Add(newPort);
                        node.inputPort = newPort;

                        newPort = GeneratePort(node, Direction.Output, Port.Capacity.Multi);
                        newPort.portName = "";
                        node.outputContainer.Add(newPort);
                        node.outputPort = newPort;
                    }
                    break;
                case NodeData.Type.Selector:
                    {
                        Port newPort = GeneratePort(node, Direction.Input);
                        newPort.portName = "";
                        node.inputContainer.Add(newPort);
                        node.inputPort = newPort;

                        newPort = GeneratePort(node, Direction.Output, Port.Capacity.Multi);
                        newPort.portName = "";
                        node.outputContainer.Add(newPort);
                        node.outputPort = newPort;
                    }
                    break;
                case NodeData.Type.Parallel:
                    {
                        Port newPort = GeneratePort(node, Direction.Input);
                        newPort.portName = "";
                        node.inputContainer.Add(newPort);
                        node.inputPort = newPort;

                        newPort = GeneratePort(node, Direction.Output, Port.Capacity.Multi);
                        newPort.portName = "";
                        node.outputContainer.Add(newPort);
                        node.outputPort = newPort;
                    }
                    break;
            }
            if(!(node.nodeType == NodeData.Type.Action || node.nodeType == NodeData.Type.Condition))
            {
                Button btn_newChild = new Button(() =>
                {
                    if(node.outputPort.connected && node.outputPort.capacity == Port.Capacity.Single) return;
                    AddSearchWindow(GUIUtility.GUIToScreenPoint(node.GetGlobalCenter()));
                    searchWindow.SetSelectedNode(node);
                });
                node.titleContainer.Add(btn_newChild);
            }
            NodeData.Property[] properties = logic.GetProperties();
            foreach(NodeData.Property p in properties)
            {
                AddPropertyField(node, p, logic);
            }
            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(new Vector2(0, 0), new Vector2(200, 200)));

            return node;
        }

        void AddPropertyField(Node node, NodeData.Property property, RuntimeBase logic)
        {
            if(property.type == typeof(bool))
            {
                var boolField = new Toggle
                {
                    name = property.name,
                    value = ((NodeData.Property<bool>)property).value
                };
                boolField.RegisterValueChangedCallback(evt =>
                {
                    ((NodeData.Property<bool>)property).value = boolField.value;
                    logic.SetProperty(property.name, boolField.value);
                });
                node.inputContainer.Add(boolField);
            }
            else if(property.type == typeof(int))
            {
                var intField = new IntegerField
                {
                    name = property.name,
                    value = ((NodeData.Property<int>)property).value
                };
                intField.RegisterValueChangedCallback(evt =>
                {
                    ((NodeData.Property<int>)property).value = intField.value;
                    logic.SetProperty(property.name, intField.value);
                });
                node.inputContainer.Add(intField);
            }
            else if(property.type == typeof(float))
            {
                var floatField = new FloatField
                {
                    name = property.name,
                    value = ((NodeData.Property<float>)property).value
                };
                floatField.RegisterValueChangedCallback(evt =>
                {
                    ((NodeData.Property<float>)property).value = floatField.value;
                    logic.SetProperty(property.name, floatField.value);
                });
                node.inputContainer.Add(floatField);
            }
            else if(property.type == typeof(string))
            {
                var textField = new TextField
                {
                    name = property.name,
                    value = ((NodeData.Property<string>)property).value
                };
                textField.RegisterValueChangedCallback(evt =>
                {
                    ((NodeData.Property<string>)property).value = textField.value;
                    logic.SetProperty<string>(property.name, textField.value);
                });
                node.inputContainer.Add(textField);
            }
            else if(property.type == typeof(Vector2))
            {
                var vectorField = new Vector2Field
                {
                    name = property.name,
                    value = ((NodeData.Property<Vector2>)property).value
                };
                vectorField.RegisterValueChangedCallback(evt =>
                {
                    ((NodeData.Property<Vector2>)property).value = vectorField.value;
                    logic.SetProperty<Vector2>(property.name, vectorField.value);
                });
                node.inputContainer.Add(vectorField);
            }
            else if(property.type == typeof(Vector3))
            {
                var vectorField = new Vector3Field
                {
                    name = property.name,
                    value = ((NodeData.Property<Vector3>)property).value
                };
                vectorField.RegisterValueChangedCallback(evt =>
                {
                    ((NodeData.Property<Vector3>)property).value = vectorField.value;
                    logic.SetProperty<Vector3>(property.name, vectorField.value);
                });
                node.inputContainer.Add(vectorField);
            }
            else if(property.type == typeof(Vector4))
            {
                var vectorField = new Vector4Field
                {
                    name = property.name,
                    value = ((NodeData.Property<Vector4>)property).value
                };
                vectorField.RegisterValueChangedCallback(evt =>
                {
                    ((NodeData.Property<Vector4>)property).value = vectorField.value;
                    logic.SetProperty<Vector4>(property.name, vectorField.value);
                });
                node.inputContainer.Add(vectorField);
            }
            else if(property.type == typeof(Color))
            {
                var colorField = new ColorField
                {
                    name = property.name,
                    value = ((NodeData.Property<Color>)property).value
                };
                colorField.RegisterValueChangedCallback(evt =>
                {
                    ((NodeData.Property<Color>)property).value = colorField.value;
                    logic.SetProperty<Color>(property.name, colorField.value);
                });
                node.inputContainer.Add(colorField);
            }
            else if(property.type == typeof(Rect))
            {
                var rectField = new RectField
                {
                    name = property.name,
                    value = ((NodeData.Property<Rect>)property).value
                };
                rectField.RegisterValueChangedCallback(evt =>
                {
                    ((NodeData.Property<Rect>)property).value = rectField.value;
                    logic.SetProperty<Rect>(property.name, rectField.value);
                });
                node.inputContainer.Add(rectField);
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort!=port && startPort.node!=port.node)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        public void AddPropertyToBlackboard()
        {
            
        }

        

        

        

        
    }
}
