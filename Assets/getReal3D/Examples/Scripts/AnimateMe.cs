using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class AnimateMe
	: MonoBehaviour
{
	public Animation anim = null;
	private bool isCrouched = false;
	private bool isAiming = false;
	private float aimTime = 0.0f;
	
	void Awake()
	{
		if (anim)
		{
			anim["Idle"].wrapMode = WrapMode.Loop;
			anim["Idle"].blendMode = AnimationBlendMode.Blend;
			anim["Idle"].layer = 0;

			anim["Standing"].wrapMode = WrapMode.ClampForever;
			anim["Standing"].blendMode = AnimationBlendMode.Blend;
			anim["Standing"].layer = 1;
			
			anim["Crouch"].wrapMode = WrapMode.ClampForever;
			anim["Crouch"].blendMode = AnimationBlendMode.Blend;
			anim["Crouch"].layer = 1;

			anim["CrouchAim"].wrapMode = WrapMode.ClampForever;
			anim["CrouchAim"].blendMode = AnimationBlendMode.Blend;
			anim["CrouchAim"].layer = 1;

			anim["StandingAim"].wrapMode = WrapMode.ClampForever;
			anim["StandingAim"].blendMode = AnimationBlendMode.Blend;
			anim["StandingAim"].layer = 1;
			
			anim.Stop();
		}
	}
	
	void Update()
	{
		if (anim)
		{
			if (!isCrouched && !isAiming && (!anim["Idle"].enabled || anim["Standing"].time >= anim["Standing"].length))
			{
				anim.CrossFade("Idle", 2.5f, PlayMode.StopAll);
			}
			else
			if (!isAiming && Time.time-aimTime > 8.0f)
			{
				if (Random.value > .99f)
				{
					if (isCrouched)
					{
						anim.CrossFade("CrouchAim", 0.5f);
						isAiming = true;
						aimTime = Time.time;
					}
					else
					{
						anim.CrossFade("StandingAim", 0.5f);
						isAiming = true;
						aimTime = Time.time;
					}
				}
			}
			else if (isAiming && Time.time-aimTime > 2.5f)
			{
				if (Random.value > .50f)
				{
					if (isCrouched)
					{
						anim.CrossFade("Crouch", 1.0f);
						isAiming = false;
					}
					else
					{
						anim.CrossFade("Standing", 1.0f);
						isAiming = false;
					}
				}
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (anim && other.tag == "Player")
		{
			if (isCrouched)
			{
				anim.CrossFade("Standing", 1.5f);
				isCrouched = false;
				isAiming = false;
			}
			else
			{
				anim.CrossFade("Crouch", 1.5f);
				isCrouched = true;
				isAiming = false;
			}
		}
	}
}
