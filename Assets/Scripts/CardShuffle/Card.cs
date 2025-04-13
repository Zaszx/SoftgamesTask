using DG.Tweening;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Deck deck;

	public void Fly(Deck target)
	{
		deck?.RemoveCard(this);

		Vector3 targetPos = target.transform.position;
		targetPos.y = transform.position.y;
		Quaternion targetRotation = target.transform.rotation;

		transform.DOMove(targetPos, 2f).SetEase(Ease.InOutQuad).OnComplete(() =>
		{
			target.AddCard(this);
			deck = target;
		});
		transform.DORotateQuaternion(targetRotation, 2f).SetEase(Ease.InOutQuad);
	}
}
