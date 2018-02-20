using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakePlayerJSON
{
    [MenuItem("Assets/Create/MakePlayerListJSON")]
    public static PlayerJSON Create()
    {
        PlayerJSON asset = ScriptableObject.CreateInstance<PlayerJSON>();

        AssetDatabase.CreateAsset(asset, "Assets/PlayerJSONFile.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}