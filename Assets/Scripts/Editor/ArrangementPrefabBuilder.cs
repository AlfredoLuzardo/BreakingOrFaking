using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Editor-only: builds five starter page arrangements in Assets/Prefabs/Arrangements.
// Must live in a folder named "Editor". Running it again overwrites the five prefabs.
public static class ArrangementPrefabBuilder
{
    private const string OutputFolder = "Assets/Prefabs/Arrangements";
    private const int UILayer = 5;
    private const float RowSpacing = 16f;
    private const float RegionSpacing = 8f;

    [MenuItem("Breaking or Faking/Create Page Arrangements")]
    public static void CreateAll()
    {
        EnsureFolder(OutputFolder);

        BuildTitleImageTop();
        BuildImageBanner();
        BuildMainWithSide();
        BuildSidebar();
        BuildPlain();

        AssetDatabase.SaveAssets();
        Object folder = AssetDatabase.LoadAssetAtPath<Object>(OutputFolder);
        Selection.activeObject = folder;
        EditorGUIUtility.PingObject(folder);
        Debug.Log($"Created 5 page arrangements in {OutputFolder}.");
    }

    // ┌───────────┬───────┐
    // │ HEADER    │ MEDIA │
    // ├───────────┴───────┤
    // │ MAIN              │
    // └───────────────────┘
    private static void BuildTitleImageTop()
    {
        RectTransform root = CreateRoot("Arrangement_TitleImageTop");
        RectTransform row = CreateRow("TopRow", root);
        RectTransform header = CreateRegion("Header", row, flexible: true);
        RectTransform media = CreateRegion("Media", row, fixedWidth: 260f);
        RectTransform main = CreateRegion("Main", root);
        Save(root, header, media, main, null, imageHeight: 180f);
    }

    // ┌───────────────────┐
    // │ MEDIA             │
    // ├───────────────────┤
    // │ HEADER            │
    // │ MAIN              │
    // └───────────────────┘
    private static void BuildImageBanner()
    {
        RectTransform root = CreateRoot("Arrangement_ImageBanner");
        RectTransform media = CreateRegion("Media", root);
        RectTransform header = CreateRegion("Header", root);
        RectTransform main = CreateRegion("Main", root);
        Save(root, header, media, main, null, imageHeight: 280f);
    }

    // ┌───────────────────┐
    // │ HEADER            │
    // ├─────────┬─────────┤
    // │ MAIN    │ SIDE    │
    // │         │ + MEDIA │
    // └─────────┴─────────┘
    private static void BuildMainWithSide()
    {
        RectTransform root = CreateRoot("Arrangement_MainWithSide");
        RectTransform header = CreateRegion("Header", root);
        RectTransform row = CreateRow("Columns", root);
        RectTransform main = CreateRegion("Main", row, flexible: true);
        RectTransform side = CreateRegion("Side", row, flexible: true);
        Save(root, header, side, main, side, imageHeight: 200f);
    }

    // ┌───────────┬───────┐
    // │ HEADER    │ SIDE  │
    // │ MAIN      │+MEDIA │
    // └───────────┴───────┘
    private static void BuildSidebar()
    {
        RectTransform root = CreateRoot("Arrangement_Sidebar");
        RectTransform row = CreateRow("Columns", root);
        RectTransform left = CreateRegion("LeftColumn", row, flexible: true);
        RectTransform header = CreateRegion("Header", left);
        RectTransform main = CreateRegion("Main", left);
        RectTransform side = CreateRegion("Side", row, fixedWidth: 240f);
        Save(root, header, side, main, side, imageHeight: 160f);
    }

    // ┌───────────────────┐
    // │ HEADER            │
    // │ MAIN              │
    // │ MEDIA             │
    // └───────────────────┘  (the original one-column look)
    private static void BuildPlain()
    {
        RectTransform root = CreateRoot("Arrangement_Plain");
        RectTransform header = CreateRegion("Header", root);
        RectTransform main = CreateRegion("Main", root);
        RectTransform media = CreateRegion("Media", root);
        Save(root, header, media, main, null, imageHeight: 220f);
    }

    // ---------- Helpers ----------

    private static RectTransform CreateRoot(string name)
    {
        RectTransform root = CreateUI(name, null);
        AddVertical(root.gameObject, RowSpacing);
        root.gameObject.AddComponent<PageArrangement>();
        return root;
    }

    // A horizontal row. Children get their widths from LayoutElements (see CreateRegion).
    private static RectTransform CreateRow(string name, Transform parent)
    {
        RectTransform row = CreateUI(name, parent);
        HorizontalLayoutGroup group = row.gameObject.AddComponent<HorizontalLayoutGroup>();
        group.spacing = RowSpacing;
        group.childAlignment = TextAnchor.UpperLeft;
        group.childControlWidth = true;
        group.childControlHeight = true;
        group.childForceExpandWidth = false;
        group.childForceExpandHeight = false;
        return row;
    }

    // A vertical region. In a row, give it either a fixed width or a share of the leftover width.
    // Preferred width is set to 0 on flexible regions so long text can't squeeze its neighbours.
    private static RectTransform CreateRegion(string name, Transform parent,
        bool flexible = false, float fixedWidth = -1f)
    {
        RectTransform region = CreateUI(name, parent);
        AddVertical(region.gameObject, RegionSpacing);

        if (flexible || fixedWidth > 0f)
        {
            LayoutElement element = region.gameObject.AddComponent<LayoutElement>();
            if (fixedWidth > 0f)
            {
                element.minWidth = fixedWidth;
                element.preferredWidth = fixedWidth;
                element.flexibleWidth = 0f;
            }
            else
            {
                element.minWidth = 0f;
                element.preferredWidth = 0f;
                element.flexibleWidth = 1f;
            }
        }

        return region;
    }

    private static void Save(RectTransform root, RectTransform header, RectTransform media,
        RectTransform main, RectTransform side, float imageHeight)
    {
        SerializedObject so = new SerializedObject(root.GetComponent<PageArrangement>());
        so.FindProperty("header").objectReferenceValue = header;
        so.FindProperty("media").objectReferenceValue = media;
        so.FindProperty("main").objectReferenceValue = main;
        so.FindProperty("side").objectReferenceValue = side;
        so.FindProperty("imageHeight").floatValue = imageHeight;
        so.ApplyModifiedPropertiesWithoutUndo();

        string path = $"{OutputFolder}/{root.name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(root.gameObject, path);
        Object.DestroyImmediate(root.gameObject);
    }

    private static RectTransform CreateUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = UILayer;
        RectTransform rect = (RectTransform)go.transform;
        if (parent != null)
        {
            rect.SetParent(parent, false);
        }

        return rect;
    }

    private static void AddVertical(GameObject go, float spacing)
    {
        VerticalLayoutGroup group = go.AddComponent<VerticalLayoutGroup>();
        group.spacing = spacing;
        group.childControlWidth = true;
        group.childControlHeight = true;
        group.childForceExpandWidth = true;
        group.childForceExpandHeight = false;
    }

    private static void EnsureFolder(string folder)
    {
        if (AssetDatabase.IsValidFolder(folder))
        {
            return;
        }

        Directory.CreateDirectory(folder);
        AssetDatabase.Refresh();
    }
}
