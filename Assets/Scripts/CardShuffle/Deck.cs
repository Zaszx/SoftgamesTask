using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public Card baseCard;

    [SerializeField] private Transform cardsParent;

    private List<Card> _cards = new List<Card>();

    public void AddCard(Card card)
	{
        _cards.Add(card);
        card.transform.position = transform.position + _cards.Count * 0.01f * transform.forward + _cards.Count * 0.01f * transform.right + _cards.Count * 0.0001f * transform.up;
        card.transform.SetParent(cardsParent);
        card.deck = this;
	}

    public void RemoveCard(Card card)
	{
        if(_cards.Count == 0 || card != _cards[_cards.Count - 1])
		{
            Debug.LogWarning("Attempting to remove non top card!");
		}
        _cards.Remove(card);
        card.transform.SetParent(null);
        card.deck = null;
	}

    public Card GetTopCard()
	{
        if (_cards.Count == 0)
            return null;
        return _cards[_cards.Count - 1];
	}
}
