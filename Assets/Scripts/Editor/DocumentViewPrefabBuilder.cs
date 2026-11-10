using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class DocumentViewPrefabBuilder
{
    private const string OutputFolder = "Assets/Prefabs/Documents";
    private const int UILayer = 5;
    private const float Width = 360f;
    private const float PhotoHeight = 240f;

    [MenuItem("Breaking or Faking/Create Photograph View")]
    public static void CreatePhotographView()
    {
        SectionView sectionPrefab = FindSectionPrefab();
        if (sectionPrefab == null)
        {
            Debug.LogError("Couldn't find a prefab with a SectionView component.");
            return;
        }

        RectTransform root = CreateUI("PhotographView", null);
        AddImage(root, Color.white).raycastTarget = true;
        AddVertical(root, padding: 16, spacing: 10f);
        root.gameObject.AddComponent<LayoutElement>().preferredWidth = Width;
        FitToContent(root);
        root.gameObject.AddComponent<DraggableDocument>();

        RectTransform photo = CreateUI("Photo", root);
        Image photoImage = AddImage(photo, Color.white);
        photoImage.preserveAspect = true;
        photo.gameObject.AddComponent<LayoutElement>().preferredHeight = PhotoHeight;

        RectTransform textArea = CreateUI("TextArea", root);
        AddVertical(textArea, padding: 0, spacing: 4f);

        PhotographView view = root.gameObject.AddComponent<PhotographView>();
        SetReferences(view, sectionPrefab, photoImage, textArea);
        Save(root);
    }

    private static void SetReferences(PhotographView view, SectionView sectionPrefab, Image photo, RectTransform textArea)
    {
        SerializedObject serialized = new SerializedObject(view);
        serialized.FindProperty("sectionPrefab").objectReferenceValue = sectionPrefab;
        serialized.FindProperty("photo").objectReferenceValue = photo;
        serialized.FindProperty("textArea").objectReferenceValue = textArea;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void Save(RectTransform root)
    {
        if (!AssetDatabase.IsValidFolder(OutputFolder))
        {
            Directory.CreateDirectory(OutputFolder);
            AssetDatabase.Refresh();
        }

        string path = $"{OutputFolder}/{root.name}.prefab";
        GameObject asset = PrefabUtility.SaveAsPrefabAsset(root.gameObject, path);
        Object.DestroyImmediate(root.gameObject);
        EditorGUIUtility.PingObject(asset);
        Debug.Log($"Created {path}.", asset);
    }

    private static SectionView FindSectionPrefab()
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab SectionView"))
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            SectionView view = prefab != null ? prefab.GetComponent<SectionView>() : null;
            if (view != null)
            {
                return view;
            }
        }

        return null;
    }

    private static RectTransform CreateUI(string name, Transform parent)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform));
        gameObject.layer = UILayer;
        RectTransform rect = (RectTransform)gameObject.transform;
        if (parent != null)
        {
            rect.SetParent(parent, false);
        }

        return rect;
    }

    private static Image AddImage(RectTransform rect, Color color)
    {
        Image image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static void FitToContent(RectTransform rect)
    {
        ContentSizeFitter fitter = rect.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private static void AddVertical(RectTransform rect, int padding, float spacing)
    {
        VerticalLayoutGroup group = rect.gameObject.AddComponent<VerticalLayoutGroup>();
        group.padding = new RectOffset(padding, padding, padding, padding);
        group.spacing = spacing;
        group.childControlWidth = true;
        group.childControlHeight = true;
        group.childForceExpandWidth = true;
        group.childForceExpandHeight = false;
    }
}
