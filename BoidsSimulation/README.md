# NVM 202 Proj 2

[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)

_REPLACE OR REMOVE EVERYTING BETWEEN "\_"_

### Student Info

-   Name: Nathan McAndrew
-   Section: 4

## Simulation Design

My simulation will feature a variety of shapes using the autonomous agent behaviors discussed in class. 
One large arrow will be a seeker and chase the nearest large arrow, if they come in contact the second large arrow will also be a seeker (similar to a game of infection). Smaller arrows will follow a nearby large arrow. Once all large arrows are in the seeking state, a seeker is chosen to restart the game over, and all other arrows, both small and large, will flee from the seeker.
Squares will flee from the nearest large arrow.
Hexagons will be obstacles avoided by all agents except for the small arrows.

### Controls

-Click: Places an octagon at the mouse's position and removes the last one in the data structure to keep a finite number of obstacles on the screen.

##Large Arrow

Seeks the nearest large arrow not in the "infected" (seeker) state
Avoids Hexagons and large arrows (if not seeker state)

###Seeker

**Seek Large Arrows**: Seek the nearest large arrow to add to the flock size

#### Steering Behaviors

- Seek, separate, stay in bounds
- Obstacles - avoids screen bounds and obstacles (hexagons)
- Seperation - small arrows
   
#### State Transistions

- Be selected as the first seeker
- Be in range of a seeker's radius
   
### Fleer

**Objective:** Flee from the nearest large arrow

#### Steering Behaviors

- Flee, Stay In Bounds, Separate
- Obstacles - Avoids Large Arrows and Hexagons
- Seperation - Separates from other large arrows
   
#### State Transistions

- All large arrows have become seekers

##Small arrows

Seeks the nearest large arrow and becomes apart of its flock when in a certain range

### Seek

**Seek:** Find the nearest large arrow and follow it

#### Steering Behaviors

- Seek, Separate, Stay in Bounds
- Obstacles - Avoids Screen Bounds
- Seperation - Separates from all agents
   
#### State Transistions

- The desinated amount of time since entering this state has passed
   
###Flee

**Flee from large arrows:**Separate from large arrows

#### Steering Behaviors

- Stay In Bounds, Flee, Separate
- Obstacles -Screen Bounds and large arrows
- Seperation - All agents
   
#### State Transistions

- All large arrows are in the seeking state

## Sources

-   _List all project sources here –models, textures, sound clips, assets, etc._
-   _If an asset is from the Unity store, include a link to the page and the author’s name_

## Make it Your Own

The color of each agent will change to indicate behavior. For instance, a large arrow's color will resemble if it is seeking, fleeing, or counting and a small's arrow will resemble if it is fleeing or a flock member.
Additionally, all assets will be created by me.

## Known Issues

N/A

### Requirements not completed

N/A

