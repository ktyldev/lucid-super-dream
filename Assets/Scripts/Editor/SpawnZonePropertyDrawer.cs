using UnityEditor;
using UnityEngine;
using Weapons.Spawning;

namespace Weapons.Editor
{
    [CustomPropertyDrawer(typeof(SpawnZone))]
    public class SpawnZonePropertyDrawer : PropertyDrawer
    {
        private const float spacing = 0.3f;

        private float _compositeHeight = 0;
        
        private int numFields = 5;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var currentPos = position;
            currentPos.height = 18;

            var space = currentPos.height * spacing;
            
            numFields = 0;
            ++numFields;
            EditorGUI.LabelField(currentPos, label, EditorStyles.boldLabel);
            
            EditorGUI.indentLevel++;
            var currentType = (SpawnType) property.FindPropertyRelative("spawnType").enumValueIndex;
            
            
            currentPos = EditorGUI.IndentedRect(currentPos);
            
            AddRelativeProperty( property, "numToSpawn", space, ref currentPos);
            
            AddRelativeProperty( property, "spawnDir", space, ref currentPos);
            AddRelativeProperty( property, "spawnPlane", space, ref currentPos);
            AddRelativeProperty( property, "surfaceOnly", space, ref currentPos);
            
            AddRelativeProperty(property, "offset", space, ref currentPos);
            
            AddRelativeProperty( property, "spawnType", space, ref currentPos);

            if (currentType == SpawnType.Circle)
            {
                AddRelativeProperty( property, "radius", space, ref currentPos);
                
                AddRelativeProperty( property, "arc", space, ref currentPos);
                
                AddRelativeProperty( property, "evenDistribution", space, ref currentPos);
            } else if (currentType == SpawnType.Square)
            {
                AddRelativeProperty( property, "width", space, ref currentPos);
                
                AddRelativeProperty( property, "height", space, ref currentPos);
            } else if (currentType == SpawnType.Composite)
            {
                _compositeHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("composite"));
                AddRelativeProperty(property, "composite", space, ref currentPos);
            } else if (currentType == SpawnType.Polygon)
            {
                AddRelativeProperty(property, "numSides", space, ref currentPos);
                AddRelativeProperty(property, "numPerSide", space, ref currentPos);
                AddRelativeProperty( property, "radius", space, ref currentPos);
                AddRelativeProperty(property, "flipVertical", space, ref currentPos);
            }
            
            EditorGUI.indentLevel--;
        }

        private void AddRelativeProperty(SerializedProperty property, string name, float space, ref Rect currentPos)
        {
            currentPos.y += currentPos.height + space;
            ++numFields;
            EditorGUI.PropertyField(currentPos, property.FindPropertyRelative(name), true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var baseHeight = base.GetPropertyHeight(property, label);
            return  (baseHeight * (1 + spacing) * numFields) + _compositeHeight;
        }
    }
}
