# FPS-DL-Authoring-Tool-

A design tool for first person shooter levels, that leverages Deep Learning in order to provide the designer with gameplay predictions and map suggestions.

Demo vid: https://www.youtube.com/watch?v=BKhDlI-br24&t=2s

# Project overview

SuSketch is design tool which takes best practices from the literature and exploits Deep Learning in a different way, 
using machine learned models as surrogates of gameplay rather than as a collection of level patterns. 
SuSketch builds on the extensive work of Karavolos et al mapping first person shooter (FPS) levels 
and the competing players’ classes to gameplay outcomes such as the duration and winner of the match. 
Using the extensive corpus collected by this work, SuSketch uses surrogate models to inform a designer on 
the predicted gameplay outcomes of their work in progress. This real-time feedback allows the designer to tweak parts of the 
level or the character classes assigned to each player, in order to ensure a balanced matchup when the level is finally played.

Moreover, SuSketch generates suggestions for the character classes and powerup placement in the level, 
towards countering any (predicted) advantage a player may have over the other. 
SuSketch is the first instance where AI assistance—in the form of both visualizations and generated suggestions—is based on surrogate models of gameplay, 
which consider both the level layout and the game’s rules (in the form of character classes) in a multi-faceted fashion.

![image](https://user-images.githubusercontent.com/15057375/166097438-391bb703-0763-453b-ba90-ae1437d853db.png)

# Publication
https://ieeexplore.ieee.org/abstract/document/9384155?casa_token=hySd_Zy4cYgAAAAA:c3Eqyz4V18JezgX1pch-mAusZPWncdhwh7Pu3ZYdtRTOvTg5O5yiu1NI6a2jM9dum0txlUjaOfw
