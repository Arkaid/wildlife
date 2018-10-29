# Creating new characters

## Preparing the data

To create a new character you will need

* A character sheet for the menu
  * An icon (150 x 150 pixels)
  * A name banner (600 x 67 pixels) - I used the [Umberto](http://www.blambot.com/font_umberto.shtml) font here, from Blambot, then pixelated the edges
  * Two portraits (600 x 800 pixels) - "Normal" and "Selected" versions
  * Three or Four preview cutouts (150 x 300 pixels)
* Three or four images for the game
  * The main color image (960 x 540 or 540 x 960 pixels)
  * The shadow, black and white image (must match the main color image size)
  
## Making a character sheet

You can use the character_sheet.xcf file (editable with GIMP) for reference. Take the separate images you prepared in the previous step and lay them down in the following format:

![layout](https://github.com/Arkaid/wildlife/blob/master/SampleCharacter/layout.png)

Just make the background transparent and save as .png

## Building a character 

Open Unity and load the game project. **There is no need to build or run the game**. 

From the editor menu, select Jintori > Character Data and the following window should appear:

![editor](https://github.com/Arkaid/wildlife/blob/master/SampleCharacter/character_editor.png)

Here's what each entry does:
* **Clear**: Resets the form
* **Load**: Loads a previously created .chr character file
* **Save**: Saves the current data into a .chr character file
* **New ID**: Randomly generates a GUID. Each character should have a unique ID. This is very unlikely to collide with an existing ID
* **Character**: Character's name
* **Artist**: Artist's name. Used to give credit at the end of each round
* **Created**: When was the character first created (automatically filled)
* **Updated**: Last update (automatically filled)
* **Tags (IB Style)**: Filter tags for the game, separated by commas (example: male, cub). The following tags are currently supported:
  * Male, Female, Transexual, Adult, Teen, Cub, Baby, Toddler, Furry, Human
  * You can add other tags without problem, they are just not filtered in the UI
* **Character Sheet**: Loads the character sheet. You can check if you layed everything out correctly as explained above.
* **Round 1/4**: Loads a pair of images in the following order: 1) Color image, 2) Mask/Shadow image

## Adding to the game

Simple copy the generated .chr file into your game installation ([GAME_FOLDER]/wildlife_Data/Characters)
