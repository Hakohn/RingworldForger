# Project Title

A versatile ringworld procedural terrain generation tool for the Unity Game
Engine designed for universal usage in game development

# Project Topic

‘**Procedural Content Generation** (PCG) is use of a formal algorithm to
generate game content that would typically be produced by a human. […] Commonly
cited motivations are to use PCG to improve game replayability, reduce a
perceived authoring burden, or introduce adaptive and personalized content.’
(Smith, 2015, p. 1). Procedural terrain generation has been used in games so far
mainly for flat terrain, sphere-shaped planets, and deep cave networks; this
project intends to take this into a new direction and allow the creation of
ringworlds through these algorithms.

![](media/21132d145016b4249b781adbc831b01a.png)**Ringworlds** are not a new
concept; in fact, ‘the notion of a spinning circular space station has been
around for over a century, and the concept of a habitable ringworld was
popularized by the 1970 novel Ringworld by Larry Niven.’ (Andrews, 2021). While
there are a few video games franchises, such as Halo (Bungie, 2000), that used
these types of objects as one of their primary attraction points, the ring
concept was just an illusion; the game’s levels were just flat terrain with
stunning skyboxes that gave you the feeling of being on the surface of a
ring-shaped world. Through this project I intend to throw away the smokes and
mirrors used for simulating these and have them not only *look* real but *be*
real.

The **Unity game engine**, as stated by Brodkin (2013), is the perfect platform
to use for the creation of such projects; it is an easily accessible editor
which allows users to bring their dream video games and / or applications to
life for most available platforms (Windows, Mac, Android, iOS, Xbox,
PlayStation, etc) using the built-in and user-created tools. As it is a very
popular engine nowadays that, unlike its competitors such as Unreal Engine,
focuses on ease of access and versatility, my tool will allow game developers,
both beginners and experts, to create revolutionary gameplay scenarios on
ringworlds.

![](media/93280bbe99c6f92abd916fb8caff53ed.png)The **expected outcome** of my
project is a Unity user-created tool that allows easy and robust procedural
terrain generation in the shape of a ringworld. The whole terrain will be placed
on the inside of the ring, and the outer area may be left rather empty, as that
one is meant to be the only artificial part; that which keeps the ring intact
and functional, but not visible by the average habitant.

# Client, Audience and Motivation

According to a US demographic study (igda, 2005, p. 9), over 44% of the game
industry workers are designers or programmers. The tool is aimed to satisfy
both, and so there will be no need of previous knowledge of the technical bits
of the Unity codebase. This gives immense powers to the users, as the ringworld
environment they’re dreaming about will be sitting just a few clicks away, and
thus would save a lot of time and resources. The level designers can immediately
start working on the base they’ve just laid down and add their own assets,
whereas the programmers will be able to use this as their testing field.

I kept mentioning that the tool will be easy to use; how so? I am a big fan of
custom editors in Unity and to create these ‘all we need is a C\# script and
that’s it.’ (Polidario, 2020). The custom editor will be displayed right after
you create an empty game object with the generator script attached to it by
right clicking on the hierarchy and Create New/3D Object/Ringworld. In it, you
will be given plenty of options to customise the ring, such as:

•	the ring’s radius
•	the ring’s width
•	the thickness and material of the outer ring
•	the sea level	•	the water material
•	the ring’s spinning speed
•	how gravity is calculated
•	the potential biomes

To provide even more manoeuvrability, some features, such as a biome, will be
created and stored as a Scriptable Object, which ‘is a data container that you
can use to save large amounts of data, independent of class instances.’ (Unity,
2020). This will allow reusability of created biomes in multiple scenarios. Once
you are happy with the settings you’ve set in the editor, you will press the
“Generate” button and your desired ring will appear right where that empty
object was placed.

# Primary Research Plan

| Gantt Chart – Project Schedule |   |   |   |   |   |   |   |   |   |    |    |    |    |
|--------------------------------|---|---|---|---|---|---|---|---|---|----|----|----|----|
| Week                           | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13 |
| Project Proposal               | # | # | # | # |   |   |   |   |   |    |    |    |    |
| Ethics Form                    |   |   |   | # |   |   |   |   |   |    |    |    |    |
| Ring generation                |   |   |   |   | # | # |   |   |   |    |    |    |    |
| Gravity                        |   |   |   |   |   | # |   |   |   |    |    |    |    |
| Terrain                        |   |   |   |   |   |   | # | # |   |    |    |    |    |
| Water                          |   |   |   |   |   |   |   | # |   |    |    |    |    |
| Vegetation                     |   |   |   |   |   |   |   |   | # | #  |    |    |    |
| Biomes                         |   |   |   |   |   |   |   |   | # | #  | #  |    |    |
| User Testing                   |   |   |   |   |   |   |   |   |   |    |    | #  | #  |

## The base of the spinning ring (Weeks 5-6)

To lay the foundation of the project, I will need to create the ring itself. The
size of the ring is a key component for what type of world you envision to
create. This tool will be designed to craft large open environments, as ‘Gamers
today love to play games that allow them to explore […] The future of gaming is
open-world games.’ (Delatte, 2019); however, that will not impede you from also
creating goofy small linear levels. Thinking about a Battle Royale
desktop/console game where all the players are dropped from space onto the ring,
and the whole map (along with battles, loot drops, teammate’s position, etc)
will be visible by just taking a quick glance into the sky? You can do that.
What about a mobile game where you need to survive by dodging incoming asteroids
while looping around the 10m radius ring? You’ll get it up in no-time. And last,
but not least, what if you want to make an open-world role-playing game where,
after hours of grinding and questing, you get to the opposite side of the 1000km
wide ring to defeat the final boss? It shall be done.

## Gravity (Week 6)

In the sixth week the three forms of gravity available to the user will be
added. In real life it is nearly impossible to create the artificial gravity we
see in sci-fi. The closest we could get to that would be achieved by ‘using
centripetal force, to produce a pulling sensation toward the “floor” that would
mimic the effects of gravity.’ (Feltman, 2013).

However, we must keep in mind that my tool is intended to be used for making
video games; sci-fi physics are allowed to happen! There will be three gravity
options available on these ringworlds: realistic (using the centripetal force;
the gravity will thus be defined by how fast the ring spins), artificial
(constant gravity defined by the user; it will be calculated by pushing the
objects away from the ring centre), and none.

## Terrain (Week 7-8)

The terrain is what you’ll see most of the time while on the ring. I plan now on
generating it using the Diamond Square algorithm, as it can create realistic
looking zones and it’s easily malleable by just changing some variable values
here and there. However, I know it can sometimes create some spike-looking
shapes when the height difference gets too large, which is why I am planning on
adding Perlin Noise on top to
![](media/28ef155c3d622c9d6d8ed7d2ffd584ca.png)smooth the spiky results.
Multiple studies have suggested that the most *beautiful* results of procedural
generation are achieved by stacking multiple algorithms on top of each other.

## Water (Week 8)

You’ll be able to choose the sea level for the ring. If the generated terrain
has any areas that are below the sea level, patches of water may be generated as
well to cover these. This option will be a toggle in the editor for each
individual biome, as there may be some users that do not want any present
liquids, even if under the sea level. These so-called water patches will be made
from one or more planes aligned in parallel with the outer ring, with the water
material applied onto them.

## Vegetation (Week 9-10)

Vegetation is one of the most important aspects when it comes to how “alive” the
world feels. Each biome will have its own individual types of vegetation, such
as trees, logs, grass, bushes, flowers etc. These will be randomly placed within
these biomes, with a user-defined density and rarity (for example, how high is
the chance for a tree to spawn instead of a bush).

## Biomes (Week 9-11)

I’ve been talking for quite a while now about biomes. But what *exactly* are
these? In real life, they are ‘areas with similar climates, landscapes, animals,
and plants.’ (BBC, 2022). That is exactly my target as well. Many open world
games have done this in the past, one very well known for this being Minecraft.
They are present for multiple reasons, the main being to ‘spice up your
exploration with a dash of diversity’ (Hunt, 2020).

![](media/032378a39274f109d1878bde028d1156.png)A few predefined biomes will be
available from the get-go, such as plains, pine forests, deserts, snowy hills,
oceans, and mountains. The users will be able (and encouraged) to also create
their own biomes with the given tools: minimum and maximum biome heights, how
abruptly these height differences can happen, what vegetation is present, what
terrain textures are used, and so on. All these will be randomly selected when
the procedural generation happens, but each will have its own weight – you may
want to have plains biomes far more often than mountain biomes; or you may want
grass textures to be applied less often than ice textures in an arctic biome.

In the editor, the user will select what biomes can there be, their weights, and
how many biomes will cover the whole ring, as the ring is not infinite and will
thus have a limited amount of space.

## User Testing (Week 12-13)

Once all the features I’ve listed are done, I will hand my tool to a few Unity
users and see what crazy creations they can come up with. I will gather data by
asking these users to complete a survey on what their user experience was, what
may need to be improved, what features should be added for more variety, and so
on.

I will try to select users of various backgrounds and experiences with Unity. As
this is a universal usage tool, both designers and programmers need to be
tested.

# Initial Literature Review

## Procedural content generation (PCG)

Gellel & Sweetser (2020) discussed in **A Hybrid Approach to Procedural
Generation of Roguelike Video Game Levels** thoroughly about the use and
importance of PCG in video games. Many high-profile games, such as Minecraft and
Fortnite, made clever use of PCG in their own ways: the former used it for world
generation, and the latter used it for weapon distribution. This drastically
reduced the time needed to be spent by the designers during the development
process, and allows the games to feel fresh on every log-in. As games nowadays
often tend to be evaluated by their replayability, amount of content and hours
of play, PCG may be one of the main reasons for which these were able to make
their way to the top flawlessly. Gellel & Sweetser (2020) listed various PCG
techniques along with their pros and cons and stated that for the best-looking
results it’d be best to not use only *one* single generation technique, but
layer *multiple* on top of each other, also known as a hybrid approach to the
situation.

![](media/5d9f6167bdac4b0c2a6c40205ccfc801.jpeg)Ivanov et al. (2020) pointed out
in **An Explorative Design Process for Game Map Generation Based on Satellite
Images and Playability Factors** that even though procedural generation has the
potential to generate large space of unique solutions for game content, it still
has the drawback of the loss of control over the design process. There is also
the chance for poorly generated terrain to block pathways and thus impede the
player from traversing chunks from the world unintentionally. Ivanov et al.
(2020) also pointed out that some generation methods that rely too much on
randomness can lead to unnatural appearances for the terrain, while others that
use some mathematical equations to ease things out, such as Perlin noise, can
often lead to producing relatively repetitive patters and therefore a sense of
deja-vu when exploring the map. In the given paper the fix for this was the
usage and implementation of real-world height maps.

The two authors do seem to have slightly different opinions on how powerful and
timesaving PCG can be: the first one mentions only the good sides of PCG and how
it can be used for almost anything, and the latter focuses more on the negative
side when it comes to *purely* algorithmic content generation. I so believe that
for high quality content designers are still needed, even for PCG, and that is
exactly what I am aiming for with my project. The tool I am creating is *not*
intended to leave the level designers unemployed or overburden them, but the
contrary – save the designers’ time when it comes to terrain generation and have
them focus more on placing assets on the map for creating a more immersive
experience for the player.

## A tool for universal usage

Tanner et al. (2019) talked in **Poirot: A Web Inspector for Designers** about
the difficulties faced by the designers when having to deal with developers’
world of code instead of using their world of visual design. While there are
many tools present to visually design websites, such as Chrome DevTools and
Firefox Firebug, it is crystal clear even from the very names of these tools
that they were not created with designers in mind. The designers are needed so
to delve into the role of developer as end-user programmers, as these tools,
even if able to solve complex issues like animation and interactivity, often
lack simple features such as changing colours or font sizes, which is neither
instinctive nor necessarily welcome. Tanner et al. (2019) designed and
implemented a web inspector, known as Poirot, tailored for designers that avoids
the mistakes made by their competitors, providing a familiar and easy to use
graphical interface. The tool assists the user in making consistent style
choices by constraining the available options to those existing in the website’s
design system. The tool was given and tested by multiple users with no previous
training, and it was demonstrated that the designers using Poirot were able to
successfully complete more tasks than they would using Chrome DevTools.

Poirot so tried and successfully achieved providing the right tools for
universal usage when it comes to designing a website, by putting a lot of effort
into the user experience and user interface, while keeping a familiar look to
it.

## Conclusion

Procedural content generation requires some effort to be put well into practice
and, when done right, can lead to endless replayability and therefore success
for video games well after the development was done. As this may sound daunting
for the designers reading this, my project intends to provide an easy-to-use
interface within the Unity Game Engine while keeping it rather familiar and
avoid confusion. This, however, does not mean it will have limited features;
multiple PCG methods will be able to be layered on top of each other from the
Unity Inspector prior to generating the ring if the user desires to do so.

I’d also like to point out that the ringworlds seem to be missing completely
from the market, as I was unable to find any academic study and / or paper
linked with them. This strongly suggests that I am stepping into an uncharted
territory with this and that the project will fill an empty gap.

# List of References

Andrews, R. G. (2021). The Ringworlds of Halo. Supercluster  
<https://www.supercluster.com/editorial/the-ringworlds-of-halo>

BBC. (2022). Biomes, Part of Geography \| The natural world. BBC  
<https://www.bbc.co.uk/bitesize/topics/z849q6f/articles/zvsp92p>

Brodkin, J. (2013). How Unity3D Became a Game-Development Beast. Dice  
<https://insights.dice.com/2013/06/03/how-unity3d-become-a-game-development-beast/>

Bungie. (2000). Bungie dreams of rings and things, part 2. Halo  
<https://web.archive.org/web/20050408113351/http://www.cdmag.com/articles/028/170/halo_preview2.html>

Delatte, T. (2019). The 25 Biggest Open-World Video Games, Ranked by Size.
Screenrant  
<https://screenrant.com/biggest-open-world-video-games-ranked-size/>

Feltman, R. (2013). Why Don’t We Have Artificial Gravity? Popularmechanics  
<https://www.popularmechanics.com/space/rockets/a8965/why-dont-we-have-artificial-gravity-15425569/>

Gellel, A., & Sweetser, P. (2020). A Hybrid Approach to Procedural Generation of
Roguelike Video Game Levels. Australian National University  
https://dl.acm.org/doi/pdf/10.1145/3402942.3402945

Hunt, C. (2020). Minecraft Guide to Biomes: A list of every biome currently in
the game  
<https://www.windowscentral.com/beginners-guide-biomes-minecraft-windows-10-edition>

igda. (2005). Game Developer Demographics: An Exploration of Workforce Diversity

<https://gamesindustryskills.files.wordpress.com/2009/11/igda_developerdemographics_oct05.pdf>

Ivanov, G., Petersen, M. H., Kovalsky, K., Engberg, K., & Palamas, G. (2020). An
Explorative Design Process for Game Map Generation Based on Satellite Images and
Playability Factors. Aalborg University  
<https://dl.acm.org/doi/pdf/10.1145/3402942.3402997>

Polidario, B. (2020). Unity Tutorials: How to Create Custom Editor Window.
Weeklyhow  
<https://weeklyhow.com/unity-custom-editor-window/>

Smith, G. (2015). An Analog History of Procedural Content Generation.
Northeastern University  
<http://www.fdg2015.org/papers/fdg2015_paper_19.pdf>

Tanner, K., Johnson, N., & Landay, J. A. (2019). Poirot: A Web Inspector for
Designers. University of Virginia  
https://dl.acm.org/doi/pdf/10.1145/3290605.3300758

Unity. (2020). ScriptableObject. Docs Unity3D  
<https://docs.unity3d.com/Manual/class-ScriptableObject.html>
