using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(DropDownAttribute))]
public class DropDownDrawer : PropertyDrawer
{
    private bool isExpanded = false;
    private List<SerializedProperty> dropdownProperties = new List<SerializedProperty>();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        DropDownAttribute dropdown = (DropDownAttribute)attribute;

        if (isExpanded)
        {
            float height = EditorGUIUtility.singleLineHeight; // Start with header height

            // Calculate the height required for all properties under the dropdown
            foreach (var prop in dropdownProperties)
            {
                height += EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }

        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DropDownAttribute dropdown = (DropDownAttribute)attribute;
        Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // Draw foldout header
        isExpanded = EditorGUI.Foldout(headerRect, isExpanded, dropdown.DropdownName, true, EditorStyles.foldout);

        if (isExpanded)
        {
            EditorGUI.indentLevel++;
            Rect propertyRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);

            // Ensure we have the properties for this dropdown
            if (dropdownProperties.Count == 0)
            {
                PopulateDropdownProperties(property.serializedObject);
            }

            // Draw each property in the dropdown list
            foreach (var prop in dropdownProperties)
            {
                EditorGUI.PropertyField(propertyRect, prop, true);
                propertyRect.y += EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            EditorGUI.indentLevel--;
        }
    }

    private void PopulateDropdownProperties(SerializedObject serializedObject)
    {
        dropdownProperties.Clear();
        SerializedProperty iterator = serializedObject.GetIterator();

        while (iterator.NextVisible(true))
        {
            if (IsPropertyInDropdown(iterator))
            {
                dropdownProperties.Add(iterator.Copy());
            }
        }
    }

    private bool IsPropertyInDropdown(SerializedProperty property)
    {
        // Check if the property has the same dropdown attribute as the current property
        DropDownAttribute dropdown = (DropDownAttribute)attribute;
        return property.isArray || property.propertyType == SerializedPropertyType.Generic || property.propertyPath.Contains(property.name);
    }
}
