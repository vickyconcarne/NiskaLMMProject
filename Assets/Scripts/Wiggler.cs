// Programmed by Tim Soret @ Odd Tales
// Move & Rotate a GameObject randomly & smoothly
// Accomplished by sampling perlin noise with different seeds for each axis
// Additive timers to enable frequency changes without jarring moves
// Randomizers to enable prefabs to be more unpredictables (flies can move with frequency x0.5 to x3 for instance)

using UnityEngine;
using System.Collections;
using Prime31.ZestKit;

public class Wiggler : MonoBehaviour {

	public float framerate = 60f;

	[Header("Wiggle Position")] 
	public float positionFrequency = 1.0f;
	public Vector3 positionAxisFrequency = new Vector3(1f, 1f, 1f);
	public float positionAmplitude = 1.0f;
	public Vector3 positionAxisAmplitude = new Vector3(1f, 1f, 1f);

	[Header("Wiggle Rotation")]
	public float rotationFrequency = 1.0f;
	public Vector3 rotationAxisFrequency = new Vector3(1f, 1f, 1f);
	public float rotationAmplitude = 1.0f;
	public Vector3 rotationAxisAmplitude = new Vector3(1f, 1f, 1f);

	[Header("Randomizer")] 
	public Vector2 positionAmplitudeMinMaxMultiplier = new Vector2(1f, 1f);
	public Vector2 positionFrequencyMinMaxMultiplier = new Vector2(1f, 1f);
	public Vector2 rotationAmplitudeMinMaxMultiplier = new Vector2(1f, 1f);
	public Vector2 rotationFrequencyMinMaxMultiplier = new Vector2(1f, 1f);
	public float randomizeOverTime = 1f;

	private float seed = 1f;
	private Vector3 startPosition;
	private Quaternion startRotation;
	private Vector3 positionNoise;
	private Vector3 rotationNoise;

	// saved manual values
	private float defaultPositionFrequency;
	private float defaultPositionAmplitude;
	private float defaultRotationFrequency;
	private float defaultRotationAmplitude;

	// random multipliers
	private float positionAmplitudeMultiplier;
	private float positionFrequencyMultiplier;
	private float rotationAmplitudeMultiplier;
	private float rotationFrequencyMultiplier;

	// new target values when impacts
	private float pfX;
	private float paX;
	private float rfX;
	private float raX;

	private float timerPosition = 0f;
	private float timerRotation = 0f;
	private float timerRandomizer = 0f;

	private float startTime = 0f;
	private float tweenDuration = 1f;
	private bool isTweening = false;
	private float t = 0f;

	private float framerateTimer;

	void Start () {

		seed = Random.Range (0f, 10000f);
		startPosition = transform.localPosition;
		startRotation = transform.localRotation;

		randomizeMultipliers ();

		/* Apply multipliers to input values
positionFrequency *= positionFrequencyMultiplier;
positionAmplitude *= positionAmplitudeMultiplier;
rotationFrequency *= rotationFrequencyMultiplier;
rotationAmplitude *= rotationAmplitudeMultiplier;*/

		// Save input values (to save default state, useful when exploding, then coming back to normal for instance)
		defaultPositionFrequency = positionFrequency;
		defaultPositionAmplitude = positionAmplitude;
		defaultRotationFrequency = rotationFrequency;
		defaultRotationAmplitude = rotationAmplitude;

		StartCoroutine(fixedFramerate());

	}

	private void randomizeMultipliers() {
		timerRandomizer += Time.deltaTime * randomizeOverTime;
		positionFrequencyMultiplier = positionFrequencyMinMaxMultiplier.x + (Mathf.PerlinNoise (timerRandomizer, seed * 516f)) * (positionFrequencyMinMaxMultiplier.y - positionFrequencyMinMaxMultiplier.x);
		positionAmplitudeMultiplier = positionAmplitudeMinMaxMultiplier.x + (Mathf.PerlinNoise (timerRandomizer, seed * 617f)) * (positionAmplitudeMinMaxMultiplier.y - positionAmplitudeMinMaxMultiplier.x);
		rotationFrequencyMultiplier = rotationFrequencyMinMaxMultiplier.x + (Mathf.PerlinNoise (timerRandomizer, seed * 718f)) * (rotationFrequencyMinMaxMultiplier.y - rotationFrequencyMinMaxMultiplier.x);
		rotationAmplitudeMultiplier = rotationAmplitudeMinMaxMultiplier.x + (Mathf.PerlinNoise (timerRandomizer, seed * 819f)) * (rotationAmplitudeMinMaxMultiplier.y - rotationAmplitudeMinMaxMultiplier.x);
	}

	private void calculations () {

		framerateTimer += Time.deltaTime;

		//startPosition = transform.localPosition;
		//startRotation = transform.localRotation;

		// Randomize multipliers
		if (randomizeOverTime > 0)
			randomizeMultipliers ();

		// Additive timer (makes possible to change frequencies, because they add up to the timer instead of changing the entire timer)
		timerPosition += Time.deltaTime * positionFrequency * positionFrequencyMultiplier;
		timerRotation += Time.deltaTime * rotationFrequency * rotationFrequencyMultiplier;

		if (positionAmplitude > 0) {
			positionNoise = new Vector3 (positionAmplitude * positionAmplitudeMultiplier * positionAxisAmplitude.x * (-0.5f + Mathf.PerlinNoise (timerPosition * positionAxisFrequency.x, seed)),
				positionAmplitude * positionAmplitudeMultiplier * positionAxisAmplitude.y * (-0.5f + Mathf.PerlinNoise (timerPosition * positionAxisFrequency.y, seed * 27f)),
				positionAmplitude * positionAmplitudeMultiplier * positionAxisAmplitude.z * (-0.5f + Mathf.PerlinNoise (timerPosition * positionAxisFrequency.z, seed * 43f)));
		}

		if (rotationAmplitude > 0) {
			rotationNoise = new Vector3 (rotationAmplitude * rotationAmplitudeMultiplier * rotationAxisAmplitude.x * (-0.5f + Mathf.PerlinNoise (timerRotation * rotationAxisFrequency.x, seed * 3f)),
				rotationAmplitude * rotationAmplitudeMultiplier * rotationAxisAmplitude.y * (-0.5f + Mathf.PerlinNoise (timerRotation * rotationAxisFrequency.y, seed * 24f)),
				rotationAmplitude * rotationAmplitudeMultiplier * rotationAxisAmplitude.z * (-0.5f + Mathf.PerlinNoise (timerRotation * rotationAxisFrequency.z, seed * 41f)));
		}

		// Apply calculations
		if (framerateTimer > 1f / framerate) {

			if (positionAmplitude > 0)
				transform.localPosition = positionNoise + startPosition;

			if (rotationAmplitude > 0)
				transform.localRotation = Quaternion.Euler (rotationNoise) * startRotation;

			framerateTimer = 0f;
		}

		if (Input.GetKeyDown ("i"))
			bigImpact ();

		if (isTweening) {
			t = (Time.time - startTime) / tweenDuration;
			if (t > 1f) {
				isTweening = false;
			} else {
				if (t < 0.3f) {
					t = t * 10f;
					t = t * t * t * (t * (6f * t - 15f) + 10f);
					positionFrequency = Mathf.Lerp (defaultPositionFrequency, pfX, t);
					positionAmplitude = Mathf.Lerp (defaultPositionAmplitude, paX, t);
					rotationFrequency = Mathf.Lerp (defaultRotationFrequency, rfX, t);
					rotationAmplitude = Mathf.Lerp (defaultRotationAmplitude, raX, t);
				} else {
					t = t * 1.1111f;
					t = t * t * t * (t * (6f * t - 15f) + 10f);
					positionFrequency = Mathf.Lerp (pfX, defaultPositionFrequency, t);
					positionAmplitude = Mathf.Lerp (paX, defaultPositionAmplitude, t);
					rotationFrequency = Mathf.Lerp (rfX, defaultRotationFrequency, t);
					rotationAmplitude = Mathf.Lerp (raX, defaultRotationAmplitude, t);
				}
			}
		}
	}

	IEnumerator fixedFramerate()
	{
		while(true) { 
			calculations ();
			yield return new WaitForSeconds(1f/framerate);
		}
	}

	void OnDrawGizmosSelected () {
		Gizmos.color = new Color(255, 0, 0);
		Gizmos.DrawWireCube (transform.position, positionAxisAmplitude*positionAmplitude);
	}

	public void bigImpact () {
		pfX = 1f;
		paX = 0.8f;
		rfX = 2f;
		raX = 4f;
		tweenDuration = 2f;
		startTime = Time.time;
		isTweening = true;
	}

	public void smallImpact () {
		pfX = 0.5f;
		paX = 0.2f;
		rfX = 2f;
		raX = 1.5f;
		tweenDuration = 1f;
		startTime = Time.time;
		isTweening = true;
	}
}