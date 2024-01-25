using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KeyValueAssetStorage), true)]
public sealed class KeyValueAssetStorageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button(nameof(FillEntriesWithEmptyKey)))
        {
            FillEntriesWithEmptyKey();
        }
    }

    private void FillEntriesWithEmptyKey()
    {
        var storage = target as KeyValueAssetStorage;

        for (int i = 0, length = storage.Count(); i < length; i++)
        {
            var key = storage.KeyAt(i);

            if (!string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            var asset = storage.ValueAt(i) as Object;

            if (asset == null)
            {
                continue;
            }

            var path = AssetDatabase.GetAssetPath(asset);

            storage.SetKeyAt(i, path);
        }
    }
}
