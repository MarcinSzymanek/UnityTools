using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/*
 * Convenient enumerator helpers to trigger something after a set amount of time, or after a condition is met
 */

namespace Utils
{
	public class Coroutines
	{

		// Do action until condition is satisfied
		public static IEnumerator DoUntilRealtime(System.Action action, System.Func<bool> condition, float tickAmount = 0.05f)
		{
			while (!condition())
			{
				action();
				yield return new WaitForSecondsRealtime(tickAmount);
			}
		}

		// Do action until condition is satisfied. Wait tickInSeconds between each iteration
		public static IEnumerator DoUntil(System.Action action, System.Func<bool> condition, float timeMultiplier = 1f)
		{
			float timeStep = timeMultiplier * Time.fixedDeltaTime;
			while (!condition())
			{
				action();
				yield return new WaitForSeconds(timeStep);
			}
		}

		// Do loopAction until condition is satisfied. Perform onFinish after the condition is satisfied
		public static IEnumerator DoUntilAndThen(System.Action loopAction, System.Func<bool> condition, System.Action onFinish)
		{
			while (!condition())
			{
				loopAction();
				yield return null;
			}
			onFinish();
		}

		// Do loopAction until condition is satisfied. Wait tickInSeconds between each iteration. Perform onFinish after the condition is satisfied
		public static IEnumerator DoUntilAndThenRealtime(System.Action loopAction, System.Func<bool> condition, System.Action onFinish, float tickInSeconds)
		{
			while (!condition())
			{
				loopAction();
				yield return new WaitForSecondsRealtime(tickInSeconds);
			}
			onFinish();
		}

		// Call action after seconds
		public static IEnumerator DoAfter(System.Action action, float seconds)
		{
			yield return new WaitForSecondsRealtime(seconds);
			action();
		}

		public static IEnumerator DestroyAfter(float seconds, GameObject obj)
		{
			yield return new WaitForSeconds(seconds);
			try
			{
				GameObject.Destroy(obj);
			}
			catch (Exception e)
			{
				Debug.LogError("Attempted to destroy object in subroutine, error: " + e.Message);
			}
		}

		public static IEnumerator DisableAfter(float seconds, GameObject obj)
		{
			yield return new WaitForSeconds(seconds);
			try
			{
				GameObject.SetActive(false);
			}
			catch (Exception e)
			{
				Debug.LogError("Attempted to disable object in subroutine, error: " + e.Message);
			}
		}

	}
}