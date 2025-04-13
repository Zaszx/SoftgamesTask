using DG.Tweening;
using UnityEngine;

public class ShuffleManager : MonoBehaviour
{
    public Deck mainDeck;
    public Deck[] playerDecks;

    public GameObject CardPrefab;

    public int InitialCardCount = 144;

	public float DealingSpeed = 1.0f;
	public float DealingCardSpeed = 2.0f;

	public float GatheringSpeed = 0.3f;
	public float GatheringCardSpeed = 0.5f;

	int deckIndex = 0;

    void Start()
    {
        for(int i = 0; i < InitialCardCount; i++)
		{
            Card newCard = Instantiate(CardPrefab).GetComponent<Card>();
            mainDeck.AddCard(newCard);
		}

        Deal();
    }

    void Deal()
	{
		DOVirtual.DelayedCall(DealingSpeed, () =>
		{
            Card topCard = mainDeck.GetTopCard();
            if(topCard != null)
			{
				topCard.Fly(playerDecks[deckIndex], DealingCardSpeed);
				deckIndex++;
				if (deckIndex == playerDecks.Length)
					deckIndex = 0;

				Deal();
			}
			else
			{
				deckIndex = 0;
				DOVirtual.DelayedCall(DealingCardSpeed, () => Gather());
			}
        });
	}

    void Gather()
	{
		DOVirtual.DelayedCall(GatheringSpeed, () =>
		{
			bool done = true;
			foreach (Deck deck in playerDecks)
			{
				Card topCard = playerDecks[deckIndex].GetTopCard();
				if (topCard != null)
				{
					done = false;
					topCard.Fly(mainDeck, GatheringCardSpeed);
					deckIndex++;
					if (deckIndex == playerDecks.Length)
						deckIndex = 0;

				}
			}
			if (done)
			{
				deckIndex = 0;
				DOVirtual.DelayedCall(GatheringCardSpeed, () => Deal());
			}
			else
			{
				Gather();
			}
		});
	}
}
