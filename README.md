# feed

**feed** is a [Breakout](https://en.wikipedia.org/wiki/Breakout_(video_game) inspired game that pits the player's pizza ball against a field of hungry pizza boxes. I put this game together on my own as a practice project. I was inspired by the incredible drive and generosity of [TheFreePizzaDude](https://imgur.com/user/TheFreePizzaDude), an Imgur user of grand repute. I participated in his donation drives (Imgur users are regularly encouraged via TheFreePizzaDude's popular posts to donate a pizza delivery to those who identify as in-need), which led me to devise a pizza-themed game that might drive ad revenue to support hungry people.



# Game Mechanics
**Core Loop**
1. Tap anywhere on the field to aim. An aim reticle will appear there. The ball will move in this direction once launched.
2. Tap on the pizza ball, sitting on the shot line at the bottom of the screen, to launch the ball in the direction of the aim reticle.
3. The pizza ball will eventually return to the bottom of the screen and stick there, ready to be aimed and launched again. The field of pizza boxes will move toward the bottom of the screen. This constitutes a "turn".

**Win/Lose Condition**
The object of the game is to destroy all the pizza boxes on the field before they reach the bottom of the screen. The game is **lost** when a pizza box makes it to the bottom of the screen. The game is **won** when all pizza boxes are destroyed.

**Power Level**
The player is encouraged to increase their power level, which is displayed at the top of the screen. The player always starts at level 1. The user is able to increase their Power Level by **tapping the pizza ball at the moment it collides with a pizza box**. Each successful tap in this manner increases the power level by 1. Each time the power level is increased, the pizza ball's speed is slightly increased, making it more difficult to hit as the power level increases.

**Pizza Boxes**
When the pizza ball strikes a pizza box, an amount of resource equal to the current Power Level is deducted from the pizza box (for the demo, there is only one kind of resource, and that is Cheese, though under the hood there is support for additional resource types). Once a pizza box's resource is reduced to zero, it is destroyed.

**Hyper Mode**
Once the user reaches a predetermined Power Level (5 in the demo) the player will enter "Hyper Mode". When this mode is active the pizza ball **will split into two balls** when it destroys a pizza box. This applies to all balls during hyper mode, so balls can divide very quickly, leading to many balls in play and many boxes getting destroyed. Hyper mode continues for a predetermined amount of time. When the time is up, all split balls will be destroyed, leaving the player with the original launch ball. This ball's power level is reset to 1, along with its speed.

## Additional Features
**Audio**
This game makes use of an audio synchronization system which I developed myself and have used in other project (see [World of Color Simulation](https://github.com/JetsterDajet/WorldOfColor). The game start out with slow music, with the background bouncing along with the beat of the music. When the player enters Hyper Mode the music is kicked up in tempo and intensity; the background continues to bounce in sync with the rhythm of the music. Though this feature has no bearing on the outcome of gameplay, it lends a very fun and engaging experience to the player.
