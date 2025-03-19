using UnityEditor;
using UnityEngine;

namespace NAwakening.Dijkstra
{
    [CustomEditor(typeof(Dijkstra))]
    public class Dijkstra_Editor : Editor
    {
        [SerializeField]protected Dijkstra _dijkstra;

        public override void OnInspectorGUI()
        {
            if (_dijkstra == null)
            {
                _dijkstra = (Dijkstra)target;
            }
            DrawDefaultInspector();

            if (GUILayout.Button("Create Layout"))
            {
                _dijkstra.CreateLayout();
            }
            if (GUILayout.Button("Clear All"))
            {
                _dijkstra.ResestAll();
            }
        }
    }
}