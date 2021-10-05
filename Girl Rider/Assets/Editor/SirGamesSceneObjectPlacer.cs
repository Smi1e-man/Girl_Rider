using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SirGamesSceneObjectPlacer
{

#if UNITY_EDITOR
    
    [MenuItem("SirGames/Place To Ground")]
    public static void PlaceToGround()
    {
        GameObject[] selection = Selection.gameObjects;

        LayerMask groundMask = 1 << LayerMask.NameToLayer("Ground");
        float placeToGroundDistance = 0;

        foreach (var item in selection)
        {
            Vector3 pos = item.transform.position;
            if (Physics.Raycast(new Ray(pos + Vector3.up * 100f, Vector3.down), out RaycastHit hit, 300f, groundMask))
            {
                pos.y = hit.point.y + placeToGroundDistance;
                item.transform.position = pos;
            }
        }

       
    }

    [MenuItem("SirGames/Align To Ground Normal")]
    public static void AlignToGroundNormal()
    {
        GameObject[] selection = Selection.gameObjects;

        LayerMask groundMask = 1 << LayerMask.NameToLayer("Ground");

        foreach (var item in selection)
        {
            Vector3 pos = item.transform.position;
            if (Physics.Raycast(new Ray(pos + Vector3.up * 100f, Vector3.down), out RaycastHit hit, 300f, groundMask))
            {
                Vector3 rot = item.transform.rotation.eulerAngles;
                Vector3 targetRot = (Quaternion.LookRotation(hit.normal) * Quaternion.Euler(-90, 0, 180)).eulerAngles;

                item.transform.rotation = Quaternion.Euler(targetRot.x, rot.y, targetRot.z);
            }
        }


    }

    [MenuItem("SirGames/Set Random Rotation")]
    public static void SetRandomRotation()
    {
        GameObject[] selection = Selection.gameObjects;

        foreach (var item in selection)
        {
            item.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
        }


    }

    [MenuItem("SirGames/Rotate To Center")]
    public static void RotateToCenter()
    {
        GameObject[] selection = Selection.gameObjects;

        if (selection.Length <= 1) return;

        Vector3 center = Vector3.zero;
        foreach (var item in selection)
        {
            center += item.transform.position;
        }
        center /= selection.Length;

        foreach (var item in selection)
        {
            Vector3 dir = center - item.transform.position;
            dir.y = 0;
            item.transform.rotation = Quaternion.LookRotation(dir);

        }
    }

    [MenuItem("SirGames/Rotate From Center")]
    public static void RotateToFromCenter()
    {
        GameObject[] selection = Selection.gameObjects;

        if (selection.Length <= 1) return;

        Vector3 center = Vector3.zero;
        foreach (var item in selection)
        {
            center += item.transform.position;
        }
        center /= selection.Length;

        foreach (var item in selection)
        {
            Vector3 dir = center - item.transform.position;
            dir.y = 0;
            item.transform.rotation = Quaternion.LookRotation(-dir);

        }
    }

  

#endif
}
