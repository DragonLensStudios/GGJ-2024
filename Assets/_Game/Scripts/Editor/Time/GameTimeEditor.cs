using DLS.Time;
using UnityEditor;
using UnityEngine;

namespace Editor.Time
{
    [CustomEditor(typeof(GameTimeObject))]
    public class GameTimeEditor : UnityEditor.Editor
    {
        private GameTimeObject _gameTime;
        private void Awake()
        {
            _gameTime = (GameTimeObject)target;
        }

        public override void OnInspectorGUI()
        {
            if(_gameTime == null) return;
            if (GUILayout.Button("Get Months and Days"))
            {
                _gameTime.GetMonthsAndDays();
            }

            base.OnInspectorGUI();

            if (GUILayout.Button("Validate Time"))
            {
                _gameTime.ValidateTime();
            }

            if (GUILayout.Button("Reset Full Time"))
            {
                _gameTime.ResetFullDate();
            }
        }
        
    }
}