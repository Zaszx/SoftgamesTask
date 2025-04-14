1. “Ace of Shadows”

Decision making: 

The task mentions 144 cards stacked, but doesn't specify other stacks which the cards will be moved to.
I decided to add 5 of them, and make a card dealing effect from the base stack to the other stacks, as if we were dealing cards to 5 different players.
When all cards are dealt, they are gathered back into the main stack, and then dealt again.

About the top card covering the bottom card but not completely, I thought about adding some noise so that the stack would be slightly chaotic, just like how it would be in a real life card game.
However, I checked Balatro and decided to add a similar stacking effect, which also does meet the requirement in the instructions, and has already been done in a successful card game.

Tricky parts:

The stacking effect on the deck makes this tricky. Moving cards from one stack to another can be done with a simple DOTween method, but this risks moving them too fast in the y axis.
As a result, the moving card may go through other cards in the stack and may cause a buggy looking visual effect.
My solution to this was to not touch the y position of the moving card for the first and last 30% of the animation, and only move it on the y axis during the middle 40%.

Code:

Simple logic with Card and Deck prefabs, and a manager (ShuffleManager) to handle the logic. Used DOTween to handle the animations.


2. “Magic Words”

Decision making:

I didn't decide on a lot of things here, since the instruction was clear that I should fetch the dialogue from an online source.
I normally would prefer to use addressables in this case, which would have made atlas and TMP_SpriteAsset generation simpler.

Also, the initializing text on the dialogue screen while the dialogue was fetched and processed looks bad. 
This would normally be moved to a loading screen, but I thought that would be outside the scope of this task.

Tricky parts:

The problem here is to make TextMeshPro work with emoji sprites. 
This normally is straightforward with a pre-packed sprite atlas and TMP_SpriteAsset, but doing this in runtime makes it much more trickier.
The instruction was to fetch the dialogue data from an online source, which forced me to come up with a runtime solution to pack an atlas and make a dynamically generated SpriteAsset work.
TMP_SpriteAsset was probably not implemented with runtime generation in mind, but through custom runtime atlas generation and some reflection hacking to set its private version parameter, it worked.

Code:

Data, Manager and Display elements are separated. DialogueData holds both the raw and processed data, DialogueManager handles the processing, DialogueDisplay handles showing.
Except for PrepareEmojiSpriteAsset method in DialogueManager, overall logic is very straightforward. 

3. “Phoenix Flame”

Decision making:

I decided to go for a big bonfire effect here. This decision totally has nothing to do with me first attempting to do a phoenix like glow - fire mixed effect and failing miserably.

Tricky parts:

This one is probably my weakest. I'm not too familiar with particle systems, so I found a sprite sheet online, and did some trial and error on particle system parameters.
In the end I was able to come up with something that looks somewhat decent.
Since I'm not fully confident with my particle effect, I added a ground texture and some rocks to make it look like a firepit, hoping to make it look better.
The animator controller part was straightforward.