using DG.Tweening;
using UnityEngine;

public class ShuffleManager : MonoBehaviour
{
    public Deck mainDeck;
    public Deck[] playerDecks;

    public GameObject CardPrefab;

    public int InitialCardCount = 144;

    int deckIndex = 0;

    void Start()
    {
        for(int i = 0; i < InitialCardCount; i++)
		{
            Card newCard = Instantiate(CardPrefab).GetComponent<Card>();
            mainDeck.AddCard(newCard);
		}

        Shuffle();
    }

    void Shuffle()
	{
		DOVirtual.DelayedCall(1f, () =>
		{
            Card topCard = mainDeck.GetTopCard();
            if(topCard != null)
			{
				topCard.Fly(playerDecks[deckIndex]);
				deckIndex++;
				if (deckIndex == playerDecks.Length)
					deckIndex = 0;

				Shuffle();
			}
        });
	}

    void Update()
    {
        
    }
}
