using UnityEngine;
using UnityEditor;
using FMD;

#if UNITY_EDITOR
[CustomEditor(typeof(DependencyFinder))]
public class DependencyFinderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DependencyFinder script = (target as DependencyFinder);
		if(GUILayout.Button("Find Dependencies"))
		{
			script.AssignDependencies();
		}
		base.OnInspectorGUI();
	}
}

namespace FMD
{
	public class Helpers
	{
		
		[MenuItem("Assets/FMD/Find Dependencies Of Selected Prefabs")]
		static void Test(MenuCommand menuCommand)
		{
			var context = Selection.objects;
			foreach(var obj in context)
			{
				string path = AssetDatabase.GetAssetPath(obj);
				GameObject prefab = PrefabUtility.LoadPrefabContents(path);
				if(prefab.TryGetComponent<DependencyFinder>(out DependencyFinder script))
				{
					script.AssignDependencies();
					PrefabUtility.SaveAsPrefabAsset(prefab, path);
				}
			}
		}
		
		[MenuItem("FMD/Add FMD")]
		static void AddDependencyFinderToObjectMainMenu(MenuCommand menuCommand)
		{
			var context = Selection.activeTransform;
			if(context == null)
			{
				Debug.LogError("Cannot add FMD: no Transform was selected");
				return;
			}
			GameObject fmd = new GameObject("FindMyDeps");
			fmd.tag = "EditorOnly";
			fmd.transform.parent = context.root;
			fmd.AddComponent<DependencyFinder>();
		}
		
		[MenuItem("GameObject/FMD/Add FMD")]
		static void AddDependencyFinderToObjectContextMenu(MenuCommand menuCommand)
		{
			var context = Selection.activeTransform;
			if(context == null)
			{
				Debug.LogError("Cannot add FMD: no Transform was selected");
				return;
			}
			GameObject fmd = new GameObject("FindMyDeps");
			fmd.tag = "EditorOnly";
			fmd.transform.parent = context.root;
			fmd.AddComponent<DependencyFinder>();
		}
	}
}
#endif