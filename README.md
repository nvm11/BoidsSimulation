# Boids Simulation - NVM

### Student Info

-   Name: Nathan McAndrew
-   Second Year

# Simulation Information

### Technologies Used
- Unity
- C#

## Resources
- [https://natureofcode.com/](The Nature of Code - Daniel Shiffman)
- IGME 202 LEctures

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

## Asset Production
All assets were created by me using Adobe Illustrator.
