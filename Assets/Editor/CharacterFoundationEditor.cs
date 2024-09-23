using Unity.VisualScripting;
using UnityEditor;
/*
This is a script for the inspector of the CharacterFoundation script.
This script allows for the characters to have easily accessible and well organized stats that can be modified
This should not be changed unless requested by the design team
*/
[CustomEditor(typeof(CharacterFoundation), true)]
public class CharacterFoundationEditor : Editor
{
    #region SerializedProperties
            SerializedProperty weaponType;
        #region MovementProperties
            bool playerMovementGroup = true;
            SerializedProperty moveSpeed;
            SerializedProperty dashDistance;
            SerializedProperty jumpSpeed;
            SerializedProperty fallSpeed;
            SerializedProperty dashEndLag;
            SerializedProperty gravityScaling;
            SerializedProperty maxFallSpeed;
            SerializedProperty advancedMovement;
            SerializedProperty friction;
            SerializedProperty inertia;
        #endregion
        #region Cooldowns
            bool cooldownGroup = true;
            SerializedProperty dashCooldown;
            SerializedProperty activeCooldown;
            SerializedProperty attackCooldown;
        #endregion
        #region WeaponProperties
            bool weaponProperites = true;
            SerializedProperty usePrefabDefaults;
            SerializedProperty weaponObject;
            SerializedProperty weaponStartLag;
            SerializedProperty weaponEndLag;
        #region Melee Variables
            SerializedProperty weaponLifespan;
            SerializedProperty weaponHitbox;
        #endregion
        #region Ranged Variables
            SerializedProperty bulletSpeed;
            SerializedProperty bulletLifespan;
            SerializedProperty bulletPrefab;
            SerializedProperty bulletSpawnPoint;
            SerializedProperty pierceValue;
            SerializedProperty ignoreWalls;
        #endregion
        #endregion
    #endregion
    private void OnEnable() 
    {
            weaponType = serializedObject.FindProperty("weaponType");
        #region MovementProperties
            moveSpeed = serializedObject.FindProperty("moveSpeed");
            dashDistance = serializedObject.FindProperty("dashDistance");
            jumpSpeed = serializedObject.FindProperty("jumpSpeed");
            fallSpeed = serializedObject.FindProperty("fallSpeed");
            dashEndLag = serializedObject.FindProperty("dashEndLag");
            gravityScaling = serializedObject.FindProperty("gravityScaling");
            maxFallSpeed = serializedObject.FindProperty("maxFallSpeed");
            advancedMovement = serializedObject.FindProperty("advancedMovement");
            friction = serializedObject.FindProperty("friction");
            inertia = serializedObject.FindProperty("inertia");
        #endregion
        #region Cooldowns
            dashCooldown = serializedObject.FindProperty("dashCooldown");
            activeCooldown = serializedObject.FindProperty("activeCooldown");
            attackCooldown = serializedObject.FindProperty("attackCooldown");
        #endregion
        #region WeaponProperties
            weaponObject = serializedObject.FindProperty("weaponObject");
            weaponStartLag = serializedObject.FindProperty("weaponStartLag");
            weaponLifespan = serializedObject.FindProperty("weaponLifespan");
            weaponEndLag = serializedObject.FindProperty("weaponEndLag");
            weaponHitbox = serializedObject.FindProperty("weaponHitbox");
            bulletPrefab = serializedObject.FindProperty("bulletPrefab");
            pierceValue = serializedObject.FindProperty("pierceValue");
            ignoreWalls = serializedObject.FindProperty("ignoreWalls");
            bulletLifespan = serializedObject.FindProperty("bulletLifespan");
            bulletSpeed = serializedObject.FindProperty("bulletSpeed");
            usePrefabDefaults = serializedObject.FindProperty("usePrefabDefaults");
        #endregion
    }
    public override void OnInspectorGUI()
    {
        CharacterFoundation characterFoundation = (CharacterFoundation)target;
        serializedObject.Update();
        EditorGUILayout.PropertyField(weaponType);
        #region MovementProperties
            playerMovementGroup = EditorGUILayout.BeginFoldoutHeaderGroup(playerMovementGroup, "Player Movement");
            if (playerMovementGroup)
            {
                EditorGUILayout.PropertyField(moveSpeed);
                EditorGUILayout.PropertyField(dashDistance);
                EditorGUILayout.PropertyField(jumpSpeed);
                EditorGUILayout.PropertyField(fallSpeed);
                EditorGUILayout.PropertyField(dashEndLag);
                EditorGUILayout.PropertyField(gravityScaling);
                EditorGUILayout.PropertyField(maxFallSpeed);
                EditorGUILayout.PropertyField(friction);
                EditorGUILayout.PropertyField(inertia);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion
        EditorGUILayout.Space(10);
        #region Cooldowns
            cooldownGroup = EditorGUILayout.BeginFoldoutHeaderGroup(cooldownGroup, "Cooldowns");
            if (cooldownGroup)
            {
                EditorGUILayout.PropertyField(dashCooldown);
                EditorGUILayout.PropertyField(activeCooldown);
                EditorGUILayout.PropertyField(attackCooldown);
            }
        #endregion
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(10);
        #region AttackProperties
            weaponProperites = EditorGUILayout.BeginFoldoutHeaderGroup(weaponProperites, "Weapon Properties");
            if (weaponProperites)
            {
                EditorGUILayout.PropertyField(weaponObject);
                EditorGUILayout.PropertyField(usePrefabDefaults);
                if (!characterFoundation.usePrefabDefaults)
                {
                    EditorGUILayout.PropertyField(weaponStartLag);
                    EditorGUILayout.PropertyField(weaponEndLag);
                    if (characterFoundation.weaponType == CharacterFoundation.WeaponTypes.Melee)
                    {
                        EditorGUILayout.PropertyField(weaponHitbox);
                        EditorGUILayout.PropertyField(weaponLifespan);
                    }
                    else if(characterFoundation.weaponType == CharacterFoundation.WeaponTypes.Ranged)
                    {
                        EditorGUILayout.PropertyField(bulletLifespan);
                        EditorGUILayout.PropertyField(bulletSpeed);
                        EditorGUILayout.PropertyField(ignoreWalls);
                        EditorGUILayout.PropertyField(pierceValue);
                    }
                }
            }
        #endregion
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}
