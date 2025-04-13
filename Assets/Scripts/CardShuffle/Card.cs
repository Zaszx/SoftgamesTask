using DG.Tweening;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Deck deck;

	public void Fly(Deck target, float speed)
	{
		deck?.RemoveCard(this);

		Vector3 targetPos = target.GetTargetPosition() + transform.up * 0.03f;
		Quaternion targetRotation = target.transform.rotation;

		Vector3 startPos = transform.position;

		transform.DORotateQuaternion(targetRotation, speed).SetEase(Ease.InOutQuad);

		DOTween.To(
			() => 0f,
			t =>
			{
				float x = Mathf.Lerp(startPos.x, targetPos.x, t);
				float z = Mathf.Lerp(startPos.z, targetPos.z, t);

				float y;
				if (t < 0.3f)
					y = startPos.y;
				else if (t > 0.7f)
					y = targetPos.y;
				else
					y = Mathf.Lerp(startPos.y, targetPos.y, (t - 0.3f) / 0.4f);

				transform.position = new Vector3(x, y, z);
			},
			1f, // tween from 0 to 1
			speed
		).SetEase(Ease.InOutQuad)
		 .OnComplete(() =>
		 {
			 target.AddCard(this);
			 deck = target;
		 });
	}
}
