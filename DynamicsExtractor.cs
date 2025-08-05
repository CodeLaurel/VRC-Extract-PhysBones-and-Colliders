using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
using VRC.Dynamics;

public class PhysBoneExtractor : EditorWindow
{
    [MenuItem("Tools/Extract PhysBones and Colliders")]
    static void ExtractPhysBonesAndColliders()
    {
        // Find Avatar Root
        var avatar = GameObject.FindObjectOfType<VRCAvatarDescriptor>();
        if (avatar == null)
        {
            Debug.LogError("❌ No GameObject with VRC_AvatarDescriptor found in the scene.");
            return;
        }
        Debug.Log("Avatar: " + avatar.name);

        GameObject avatarRoot = avatar.gameObject;

        // Create root containers under avatar
        GameObject dynamicsRoot = new GameObject("Dynamics");
        dynamicsRoot.transform.SetParent(avatarRoot.transform);
        dynamicsRoot.transform.localPosition = Vector3.zero;
        dynamicsRoot.transform.localRotation = Quaternion.identity;
        dynamicsRoot.transform.localScale = Vector3.one;

        GameObject physbonesContainer = new GameObject("physbones");
        GameObject collidersContainer = new GameObject("colliders");

        physbonesContainer.transform.SetParent(dynamicsRoot.transform);
        collidersContainer.transform.SetParent(dynamicsRoot.transform);

        // Store mapping: original collider → new collider (as base type)
        Dictionary<VRCPhysBoneColliderBase, VRCPhysBoneColliderBase> colliderMap = new();

        // --- Step 1: Copy all VRCPhysBoneColliders ---
        VRCPhysBoneCollider[] originalColliders = GameObject.FindObjectsOfType<VRCPhysBoneCollider>(true);

        foreach (var original in originalColliders)
        {
            // Fix missing rootTransform if needed
            SerializedObject so = new SerializedObject(original);
            var rootProp = so.FindProperty("rootTransform");
            if (rootProp != null && rootProp.objectReferenceValue == null)
            {
                rootProp.objectReferenceValue = original.transform.parent;
                so.ApplyModifiedProperties();
            }

            // Create new GameObject
            GameObject newObj = new GameObject(original.gameObject.name /*+ "_Collider"*/);
            newObj.transform.SetParent(collidersContainer.transform);
            newObj.transform.position = original.transform.position;
            newObj.transform.rotation = original.transform.rotation;
            newObj.transform.localScale = original.transform.lossyScale;

            // Copy component
            var newCollider = newObj.AddComponent<VRCPhysBoneCollider>();
            EditorUtility.CopySerialized(original, newCollider);

            colliderMap[original] = newCollider;

            // Remove original
            DestroyImmediate(original);
        }

        // --- Step 2: Copy all VRCPhysBones and update colliders ---
        VRCPhysBone[] originalPhysBones = GameObject.FindObjectsOfType<VRCPhysBone>(true);

        foreach (var original in originalPhysBones)
        {
            // Fix missing rootTransform if needed
            SerializedObject so = new SerializedObject(original);
            var rootProp = so.FindProperty("rootTransform");
            if (rootProp != null && rootProp.objectReferenceValue == null)
            {
                rootProp.objectReferenceValue = original.transform.parent;
                so.ApplyModifiedProperties();
            }

            // Create new GameObject
            GameObject newObj = new GameObject(original.gameObject.name /*+ "_PhysBone"*/);
            newObj.transform.SetParent(physbonesContainer.transform);
            newObj.transform.position = original.transform.position;
            newObj.transform.rotation = original.transform.rotation;
            newObj.transform.localScale = original.transform.lossyScale;

            // Add and copy component
            var newPhysBone = newObj.AddComponent<VRCPhysBone>();
            EditorUtility.CopySerialized(original, newPhysBone);

            // Remap colliders array
            List<VRCPhysBoneColliderBase> newCollidersList = new();
            foreach (var oldCol in original.colliders)
            {
                if (colliderMap.TryGetValue(oldCol, out var newCol))
                    newCollidersList.Add(newCol);
            }

            newPhysBone.colliders = newCollidersList;

            // Remove original
            DestroyImmediate(original);
        }

        Debug.Log("✅ PhysBones and Colliders extracted under avatar's 'Dynamics' object.");
    }
}
