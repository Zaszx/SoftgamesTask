using UnityEngine;

public class FireToggle : MonoBehaviour
{
	public Animator fireAnimator;

	public void ToggleFire()
	{
		bool isBurning = fireAnimator.GetBool("isBurning");
		fireAnimator.SetBool("isBurning", !isBurning);
	}
}