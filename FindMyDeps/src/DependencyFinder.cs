using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace FMD
{
	public class DependencyFinder : MonoBehaviour
	{
		#if UNITY_EDITOR
		[Tooltip("Log which dependencies are set to console")]
		public bool log = false;
		[Tooltip("Only assign script fields which are set to null")]
		public bool onlySetNullFields = false;
		
		const BindingFlags p_flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty;
		const BindingFlags i_flag = BindingFlags.Instance | BindingFlags.SetField | BindingFlags.NonPublic;
		const BindingFlags self_dep_flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
		
		[ContextMenu("Assign Dependencies")]
		[ExecuteInEditMode]
		public void AssignDependencies()
		{
			if(log) Debug.Log("Assigning dependencies of: " + transform.root.gameObject);

			MonoBehaviour[] behaviours = Helper.GetMBInTree(transform.root);
			
			// Run [AssignCompToSelf] methods
			foreach(var b in behaviours)
			{
				Type t = b.GetType();
				foreach(var method in t.GetMethods(self_dep_flag))
				{
					if(Attribute.IsDefined(method, typeof(InitMethodAttribute)))
					{
						if(log) Debug.Log("Running method " + method.Name + " of " + t.Name);
						method.Invoke(b, null);
						EditorUtility.SetDirty(b);
					}
				}
			}
			
			
			Dictionary<Type, object> providerMap = new Dictionary<Type, object>(); 
			
			// Get a list of provided objects via [IntProvide] marked property
			var providers = behaviours.OfType<IInternalProvider>();
			foreach(var provider in providers)
			{
				Type t = provider.GetType();
				foreach(var p in t.GetProperties(p_flag))
				{
					if(Attribute.IsDefined(p, typeof(IntProvideAttribute))) 
					{
						if(log) Debug.Log("Adding " + t + " to list of providers");
						providerMap[t] = p.GetValue(provider);
					}
				}
			}
			
			// Inject provided objects into [IntInject] marked fields
			foreach(var b in behaviours)
			{
				Type t = b.GetType();
			
				foreach(var f in t.GetFields(i_flag))
				{
					var fieldType = f.FieldType;
					if(Attribute.IsDefined(f, typeof(IntInjectAttribute)))
					{
						if(onlySetNullFields && f.GetValue(b) != null) continue;
						if(log) Debug.Log("Setting: " + fieldType + " of: " + t);
						if(providerMap.ContainsKey(fieldType)) f.SetValue(b, providerMap[fieldType]);
					}
				}
				
				foreach(var p in t.GetProperties(i_flag))
				{
					var propType = p.PropertyType;
					if(Attribute.IsDefined(p, typeof(IntInjectAttribute)))
					{
						if(log) Debug.Log("Setting: " + propType + " of: " + b.GetType());
						p.SetValue(b, providerMap[propType]);
					}
				}
				EditorUtility.SetDirty(b);
			}
		}
		#endif
	}
}
