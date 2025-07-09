using UnityEngine;
using System.Collections;

/*
 * Smoothly crossfade from one audio clip to another. Configurate the curves in the inspector beforehand
 * Creates a copy of an AudioSource attached to the GO during Start()
 * Performant, but a little memory intensive. Memory usage can be improved by consolidating AudioSource volume (fx. to 0-1)
 * and using a shared object to get lookup values from
 * Alternatively, the lookup could be made only for the volume coefficients, and the volume values could be calculated at runtime
 *
 * Copying the audiosource could be avoided by making a second AS beforehand
 */
 [RequireComponent(typeof(AudioSource))]
public class AudioCrossfade : MonoBehaviour
{
	private AudioSource originalSource;
	private AudioSource secondSource;
	private AudioSource current;

	public ParticleSystem.MinMaxCurve crossFadeCurrent;
	public ParticleSystem.MinMaxCurve crossFadeNext;
	public float duration = 0.2f;
	private int ticks;

	private float volume;

	private float[] currentFadeLookup;
	private float[] nextFadeLookup;


	void Awake()
	{
		originalSource = GetComponent<AudioSource>();
		crossFadeCurrent.curveMultiplier = 1f;
		crossFadeNext.curveMultiplier = 1f;
		volume = originalSource.volume;
		current = originalSource;

		// Setup lookup tables to calculate values before runtime
		ticks = (int)(duration / Time.fixedDeltaTime);
		currentFadeLookup = new float[ticks];
		nextFadeLookup = new float[ticks];
		for (int i = 0; i < ticks; i++)
		{
			float timeRatio = (i * Time.fixedDeltaTime) / duration;
			currentFadeLookup[i] = crossFadeCurrent.Evaluate(timeRatio) * volume;
		}

		for (int i = 0; i < ticks; i++)
		{
			float timeRatio = (i * Time.fixedDeltaTime) / duration;
			nextFadeLookup[i] = crossFadeNext.Evaluate(timeRatio) * volume;
			Debug.Log(nextFadeLookup[i]);
		}
	}

	void Start()
	{
		secondSource = gameObject.AddComponent<AudioSource>();
		secondSource.pitch = originalSource.pitch;
		secondSource.outputAudioMixerGroup = originalSource.outputAudioMixerGroup;
		secondSource.spatialBlend = originalSource.spatialBlend;
		secondSource.spread = originalSource.spread;
		secondSource.rolloffMode = originalSource.rolloffMode;
		secondSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, originalSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff));
		secondSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, originalSource.GetCustomCurve(AudioSourceCurveType.SpatialBlend));
		secondSource.SetCustomCurve(AudioSourceCurveType.Spread, originalSource.GetCustomCurve(AudioSourceCurveType.Spread));
		secondSource.maxDistance = originalSource.maxDistance;
	}

	public void CrossfadeClip(AudioClip clip, bool loopNext = false)
	{
		AudioSource to = (current == originalSource) ? secondSource : originalSource;
		to.loop = loopNext;
		to.PlayOneShot(clip);
		StartCoroutine(doCrossfade(current, to));
	}

	private IEnumerator DoCrossfade(AudioSource from, AudioSource to)
	{
		int elapsedFrames = 0;

		while (elapsedFrames < ticks)
		{
			from.volume = currentFadeLookup[elapsedFrames];
			to.volume = nextFadeLookup[elapsedFrames];
			elapsedFrames++;
			yield return new WaitForFixedUpdate();
		}
		from.volume = 0f;
		from.Stop();
		to.volume = volume;
		current = to;
	}

}
