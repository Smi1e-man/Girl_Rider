using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using Dreamteck.Splines;

public class SirGamesEditor : EditorWindow
{
    static float INDENT_VALUE = 30f;
    static float BUTTON_WIDTH = 90f;
    static float BUTTON_HEIGHT = 30f;
    static float VERTICAL_SPACING = 5f;
    static float FOLDOUT_HEIGHT = 30f;

    // Ground Placer
    bool showGroundPlacer = true;

    LayerMask groundLayerMask;
    float distanceToGround;


    bool showSetRotation = true;


    // Sprite Capturer
    bool showSpriteCapturerFoldout = true;
    bool withAlpha = true;
    string pngOutPath = "Assets/SpriteCapturer/Sprites";
    string pngOutFileName = "Sprite";
    int pngOutTake = 0;
    string capturePath;
    Camera captureCamera;

    // Spline Aligner
    bool showSplineAligner = true;
    Vector3 rotationOffset;
    SplineComputer splineComputer;

    [MenuItem("SirGames/SirGames Tool")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(SirGamesEditor));
    }

    private void OnEnable()
    {
        Reset();
    }

    void Reset()
    {
        FindCaptureCamera();
        groundLayerMask = 1 << LayerMask.NameToLayer("Ground");
    }

    void FindCaptureCamera()
    {
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (var camera in cameras)
        {
            if (camera.activeTexture != null)
            {
                captureCamera = camera;
                break;
            }
        }
    }

    void OnGUI()
    {
        GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldoutHeader);
        foldoutStyle.fixedHeight = FOLDOUT_HEIGHT;


        // Ground Placer
        showGroundPlacer = EditorGUILayout.BeginFoldoutHeaderGroup(showGroundPlacer, "Ground Placer", foldoutStyle);
        if (showGroundPlacer)
        {
            BeginCustomIndent(INDENT_VALUE);

            groundLayerMask = LayerMaskField("Ground Mask", groundLayerMask);
            distanceToGround = EditorGUILayout.Slider("Distance", distanceToGround, -5f, 5f);

            if (NiceCenteredButton("Place"))
                PlaceToGround(groundLayerMask, distanceToGround);


            EndCustomIndent();
            GUILayout.Space(VERTICAL_SPACING);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();


        // Set Rotation
        showSetRotation = EditorGUILayout.BeginFoldoutHeaderGroup(showSetRotation, "Set Rotation", foldoutStyle);
        if (showSetRotation)
        {
            BeginCustomIndent(INDENT_VALUE);

            BeginCenterContent();
            if (NiceButton("Random"))
                SetRandomRotation();

            if (NiceButton("To Center"))
                SetRotationToCenter(false);

            if (NiceButton("From Center"))
                SetRotationToCenter(true);

            EndCenterContent();

            EndCustomIndent();
            GUILayout.Space(VERTICAL_SPACING);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();


        // Sprite Capturer
        showSpriteCapturerFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(showSpriteCapturerFoldout, "Sprite Capturer", foldoutStyle);
        if (showSpriteCapturerFoldout)
        {
            BeginCustomIndent(INDENT_VALUE);

            GUILayout.BeginHorizontal();
            captureCamera = (Camera)EditorGUILayout.ObjectField("Camera", captureCamera, typeof(Camera), true);
            if (GUILayout.Button("Find"))
                FindCaptureCamera();
            GUILayout.EndHorizontal();
            pngOutFileName = EditorGUILayout.TextField("File Name", pngOutFileName);
            pngOutPath = EditorGUILayout.TextField("Save Path", pngOutPath);

            GUILayout.BeginHorizontal();
            pngOutTake = EditorGUILayout.IntField("Take", pngOutTake);
            if (GUILayout.Button("Reset"))
                pngOutTake = 0;
            GUILayout.EndHorizontal();

            withAlpha = EditorGUILayout.Toggle("With Alpha", withAlpha);

           
            BeginCenterContent();
            if (NiceButton("Capture"))
            {
                capturePath = pngOutPath + "/" + pngOutFileName + "_" + pngOutTake + ".png";
                SaveRenderTextureToPNG(captureCamera, capturePath, withAlpha);
                pngOutTake++;

            }

            if (NiceButton("Show"))
            {
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(capturePath);
                EditorGUIUtility.PingObject(obj);
            }
            EndCenterContent();



            EndCustomIndent();
            GUILayout.Space(VERTICAL_SPACING);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();


        // Spline Aligner
        showSplineAligner = EditorGUILayout.BeginFoldoutHeaderGroup(showSplineAligner, "Spline Aligner", foldoutStyle);
        if (showSplineAligner)
        {
            BeginCustomIndent(INDENT_VALUE);

            rotationOffset = EditorGUILayout.Vector3Field("Rotation Offset", rotationOffset);
            splineComputer = (SplineComputer)EditorGUILayout.ObjectField("SplineComputer", splineComputer, typeof(SplineComputer), true);

            if (NiceCenteredButton("Align"))
                AlignToSpline(splineComputer, rotationOffset);


            EndCustomIndent();
            GUILayout.Space(VERTICAL_SPACING);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    void BeginCustomIndent(float indent)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(indent);
        GUILayout.BeginVertical();
    }

    void EndCustomIndent()
    {
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    void BeginCenterContent()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
    }

    void EndCenterContent()
    {
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    bool NiceCenteredButton(string label)
    {
        bool result = false;

        BeginCenterContent();
        result = NiceButton(label);
        EndCenterContent();

        return result;
    }

    bool NiceButton(string label)
    {
        return GUILayout.Button(label, GUILayout.Width(BUTTON_WIDTH), GUILayout.Height(BUTTON_HEIGHT));
    }

    LayerMask LayerMaskField(string label, LayerMask selected)
    {
        List<string> layers = new List<string>();
        string[] layerNames = layerNames = new string[4];

        int emptyLayers = 0;
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);

            if (layerName != "")
            {

                for (; emptyLayers > 0; emptyLayers--) layers.Add("Layer " + (i - emptyLayers));
                layers.Add(layerName);
            }
            else
            {
                emptyLayers++;
            }
        }

        if (layerNames.Length != layers.Count)
        {
            layerNames = new string[layers.Count];
        }
        for (int i = 0; i < layerNames.Length; i++) layerNames[i] = layers[i];

        selected.value = EditorGUILayout.MaskField(label, selected.value, layerNames);

        return selected;
    }

    void PlaceToGround(LayerMask groundMask, float placeToGroundDistance)
    {
        GameObject[] selection = Selection.gameObjects;

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

    void SetRotationToCenter(bool inverse)
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
            item.transform.rotation = Quaternion.LookRotation(dir * (inverse ? -1 : 1));

        }
    }

    void SetRandomRotation()
    {
        GameObject[] selection = Selection.gameObjects;

        foreach (var item in selection)
        {
            item.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
        }


    }

    void SaveRenderTextureToPNG(Camera camera, string path, bool withAlpha)
    {
        RenderTexture rt = camera.activeTexture;
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null;

        byte[] bytes;
        bytes = tex.EncodeToPNG();

        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);



        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);

        importer.textureType = TextureImporterType.Sprite;
        importer.sRGBTexture = false;
        importer.alphaSource = withAlpha ? TextureImporterAlphaSource.FromInput : TextureImporterAlphaSource.None;


        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        Debug.Log("Saved to " + path);
    }

    void AlignToSpline(SplineComputer splineComputer, Vector3 offset)
    {
        GameObject[] selection = Selection.gameObjects;


        SplineSample ss;

        foreach (var item in selection)
        {
            ss = splineComputer.Project(item.transform.position);

            item.transform.rotation = ss.rotation * Quaternion.Euler(offset);

            EditorUtility.SetDirty(item);
        }
    }
}
