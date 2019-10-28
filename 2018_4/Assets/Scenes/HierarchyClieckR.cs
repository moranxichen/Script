
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.Rendering;

public class MyHierarchyMenu
{
    [MenuItem("Window/HierarchyMenu/使用前先保存/同步Transform")]
    static void TransformUnite()
    {
        Transform transformDeposit;
        GameObject[] positionFirst =  Selection.gameObjects;
        for (int i = 0; i < positionFirst.Length; i++)
        {
            if (positionFirst[i].tag == "thisway")
            {
                transformDeposit = positionFirst[i].transform;
                for (int x = 0; x < positionFirst.Length; x++)
                {
                    positionFirst[x].transform.position = transformDeposit.position;
                    positionFirst[x].transform.rotation = transformDeposit.rotation;
                    positionFirst[x].transform.localScale = transformDeposit.localScale;
                }
            }
        }
    }
    [MenuItem("Window/HierarchyMenu/使用前先保存/同步MeshRender")]
    static void MeshRenderUnite()
    {
        MeshRenderer MeshRnederDeposit;
        GameObject[] positionFirst = Selection.gameObjects;
        for (int i = 0; i < positionFirst.Length; i++)
        {
            if (positionFirst[i].tag == "thisway")
            {
                MeshRnederDeposit = positionFirst[i].GetComponent<MeshRenderer>();
                for (int x = 0; x < positionFirst.Length; x++)
                {
                    positionFirst[x].GetComponent<MeshRenderer>().lightProbeUsage = MeshRnederDeposit.GetComponent<MeshRenderer>().lightProbeUsage;
                    positionFirst[x].GetComponent<MeshRenderer>().reflectionProbeUsage = MeshRnederDeposit.GetComponent<MeshRenderer>().reflectionProbeUsage;
                    positionFirst[x].GetComponent<MeshRenderer>().shadowCastingMode = MeshRnederDeposit.GetComponent<MeshRenderer>().shadowCastingMode;
                    positionFirst[x].GetComponent<MeshRenderer>().receiveShadows = MeshRnederDeposit.GetComponent<MeshRenderer>().receiveShadows;
                }
                break;
            }
        }
    }

    [InitializeOnLoadMethod]
    static void StartInitializeOnLoadMethod()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        if (Event.current != null && selectionRect.Contains(Event.current.mousePosition)
            && Event.current.button == 1 && Event.current.type <= EventType.MouseUp)
        {
            GameObject selectedGameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            //这里可以判断selectedGameObject的条件
            if (selectedGameObject)
            {
                Vector2 mousePosition = Event.current.mousePosition;

                EditorUtility.DisplayPopupMenu(new Rect(mousePosition.x, mousePosition.y, 0, 0), "Window/HierarchyMenu", null);
                Event.current.Use();
            }
        }
    }

}