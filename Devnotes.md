
### With cube prefabs of different pivot positions 
Pivot in the center: 5 radius, 15 beat scale Y
Pivot at the bottom: 11.2 radius, 5 beat scale Y

TODO:
1. editor presentation TODO in VFX_heartpounding
2. The animated character version of visualization (pop up effect on change + change on three beats) (跑跳坐)

2020-07-01
Added responsive variations on the heart's colors and pounding distance, the standard used is *the number of beats* as a standard.
New idea: animated characters.

2020-06-25
With audio band processing, I tried two ways to make them - one that sums up frequencies and lerp the change in these frequencies, and another that uses a threshold value to determine "beats" and manipulate effects on beats. Both are from Youtube videos, I've tuned the first one better, while the second one doesn't work exactly as I wanted, even though I think it's more intuitive and understand it better. 

After visualizing the waves, I realized the higher frequencies tend to have much smaller values compared to lower frequencies, that's why directly distributing these frequencies evenly into representations won't work. A Log function did get rid of the uneven distribution, however each freqeuncy bands function at their own scales, and there is no mutual threshold I can use to determine the beat. Later I decided to use the positive difference between timepoints as the measurement value and threshold that. This actually worked, and it looks pretty good visually! Sadly though, the beats aren's restlessly occuring fro all frequency bands, so sometimes some frequency bands just stay at rest -- which isn't visually desired. I think I'll add an option to turn off the bands when they're at rest.

The origianl code for method 2 is pretty elegant and scalable. I will probably take a look at it again and add the color change effect as well.

p.s. when the RestSmoothSpeed is turned really high (e.g. 50), the quick appearence effect is pretty cool.

Next steps:
1. glow effect on the rectangles

### Writing shaders manually (without Shader Graph)
Bloom effect: https://learnopengl.com/Advanced-Lighting/Bloom
Glow effect: https://www.steelskies.com/HLSL2.htm, https://www.youtube.com/watch?v=nZZ6MDY3JOk

2. make the heart react to music
maybe makes it change color and distance of pounding, when certain percentage of frequency bands are on beat. This should be super achivable with the current method 2.

3. add a dev option to turn off the bands when they're at rest, OR make all resting bands do some designed move (a certain degree of contraction). go with the first one for now.
