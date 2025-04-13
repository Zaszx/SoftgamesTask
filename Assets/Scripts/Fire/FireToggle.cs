using UnityEngine;

public class FireToggle : MonoBehaviour
{
	public Animator fireAnimator;
	private bool isBurning = true;

	public void ToggleFire()
	{
		isBurning = !isBurning;
		fireAnimator.SetBool("isBurning", isBurning);
	}
}