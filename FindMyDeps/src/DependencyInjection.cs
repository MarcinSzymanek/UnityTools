using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FMD
{
	// Mark property as one which provides instance of itself to the DI internally (within one object hierarchy tree)
	// For example, characters who can take damage will need TakeDamage instance, which has to be on the object itself
	// This field marks a public 'getter' for that instance
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class IntProvideAttribute : Attribute {
		public IntProvideAttribute() {}
	}

	// Marks a public setter for the dependency
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class IntInjectAttribute : Attribute {
		public IntInjectAttribute() {}
	}
	
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class InitMethodAttribute : Attribute {
		public InitMethodAttribute() {}
	}
	
	interface IInternalProvider{}
	
	public class Helper{
		public static MonoBehaviour[] GetMBInTree(Transform root)
		{
			List<MonoBehaviour> results = new List<MonoBehaviour>(20);
			TreeDFS(root, results);
			return results.ToArray();
		}
		
		// Recursively get components of type
		private static void TreeDFS<T>(Transform node, List<T> results) where T : Component
		{
			
			foreach(Transform child in node)
			{
				TreeDFS(child, results);
			}
			
			foreach(var component in node.GetComponents<T>())
			{
				if(component is T) results.Add(component);
			}
		}
	}
}

