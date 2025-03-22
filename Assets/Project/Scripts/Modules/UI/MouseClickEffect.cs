using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickEffect : MonoBehaviour
{
    public ParticleSystem particleEffect;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0)) {
			EmitParticleEffect();
		}
	}
	private void EmitParticleEffect()
	{
		Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		spawnPosition.z = 0f;
		particleEffect.Emit(10);
		particleEffect.transform.position = spawnPosition;
	}
}
