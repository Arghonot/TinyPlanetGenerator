using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNodeEditor;
using System.Linq;

namespace NoiseGraph
{
    [CustomNodeEditor(typeof(PlanetRenderer))]
    public class PlanetRendererEditor : NodeEditor
    {
        Editor PlanetEditor;
        Editor CubeEditor;

        public override void OnBodyGUI()
        {
            if (target == null) return;

            OriginalUI();

            PlanetRenderer rend = target as PlanetRenderer;

            GUILayout.Space(5);

            GUIStyle bgColor = new GUIStyle();
            bgColor.normal.background = EditorGUIUtility.whiteTexture;

            //if (rend.PlanetTex != null)
            //{
            //    GUI.DrawTexture(rend.TexturePosition, rend.PlanetTex);
            //}

            if (rend.Planet != null)
            {
                if (PlanetEditor == null || rend.asChanged)
                {
                    rend.asChanged = false;
                    PlanetEditor = Editor.CreateEditor(rend.Planet.gameObject);
                }

                PlanetEditor.OnPreviewGUI(
                    GUILayoutUtility.GetRect(
                        200,
                        200), 
                    bgColor);
            }

            if (GUILayout.Button("Render"))
            {
                rend.Render();
            }
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
                "Planet",
                "asChanged",
                "PlanetTex",
                "size",
                "mat"
                };

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