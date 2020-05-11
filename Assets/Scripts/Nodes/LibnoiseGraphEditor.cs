using System;
using System.Collections.Generic;
//using System.ge
using UnityEngine;
using XNode;
using XNodeEditor;

namespace GraphEditor
{
    [CustomNodeGraphEditor(typeof(NoiseGraph.LibnoiseGraph))]
    public class LibnoiseGraphEditor : XNodeEditor.NodeGraphEditor
    {
        List<Type> HiddenTypes = new List<Type>()
        {
            typeof(Graph.Blackboard),
            typeof(Graph.RootInt)
        };

        public override Texture2D GetGridTexture()
        {
            NodeEditorWindow.current.titleContent = new GUIContent(((NoiseGraph.LibnoiseGraph)target).name);

            return base.GetGridTexture();
        }

        public override string GetNodeMenuName(Type type)
        {
            if (!HiddenTypes.Contains(type) && !type.ToString().Contains("Root"))
            {
                return base.GetNodeMenuName(type);
            }

            else return null;
        }

        public override void RemoveNode(Node node)
        {
            if (node != ((NoiseGraph.LibnoiseGraph)target).blackboard)
            {
                base.RemoveNode(node);
            }
        }

        public override void OnGUI()
        {
            NodeEditorWindow.current.Repaint();
        }
    }
}
