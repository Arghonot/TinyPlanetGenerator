using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace NoiseGraph
{
    [CustomNodeEditor(typeof(Renderer))]
    public class RendererEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            //base.OnBodyGUI();
            OriginalUI();

            Renderer rend = target as Renderer;

            GUILayout.Space(5);

            rend.size = EditorGUILayout.IntField("Size ", rend.size);
            GUILayout.Label("Render time (ms) : " + rend.RenderTime.ToString());


            GUILayout.Space(rend.Space);

            if (rend.tex != null)
            {
                GUI.DrawTexture(rend.TexturePosition, rend.tex);
            }

            if (GUILayout.Button("Render"))
            {
                rend.Render();
            }
            if (GUILayout.Button("Save"))
            {
                rend.Save();
            }

            //GUILayout.Label("Render time (ms) : " + rend.RenderTime.ToString());
        }

        void OriginalUI()
        {
            // Unity specifically requires this to save/update any serial object.
            // serializedObject.Update(); must go at the start of an inspector gui, and
            // serializedObject.ApplyModifiedProperties(); goes at the end.
            serializedObject.Update();
            string[] excludes = {
                "m_Script",
                "graph",
                "position",
                "ports",
                "TexturePosition",
                "Space",
                "tex",
                "size",
                "RenderTime" };

            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                NodeEditorGUILayout.PropertyField(iterator, true);
            }

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            foreach (XNode.NodePort dynamicPort in target.DynamicPorts)
            {
                if (NodeEditorGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
                NodeEditorGUILayout.PortField(dynamicPort);
            }

            serializedObject.ApplyModifiedProperties();

        }
    }
}