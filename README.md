# For-psychophysical-experiment-Unity-Ultraleap

This is the code that delays hand movements and adds gain to finger movements when using Leapmotion in Unity.

If you have any problems with the program or rights, please contact me.

I have no intention of infringing on any rights.


# GetPastHandData.cs
is Script that stores acquired hand data in a List for each frame.

The hand data obtained by Leapmotion is stored for each frame and is used to realize hand delay.

Place the object from which you attached this script into your project.
![1](https://github.com/motegi-na/For-psychophysical-experiment-Unity-Ultraleap/assets/151810708/3f2d0789-d35d-4bcc-8af3-bac89db91ef5)


# DelayHand.cs
is a script that delays hand movements based on the hand data obtained with GetPastHandData.

Attach to the Leapmotion hand model.

Decide how many frames to delay with "DelayFrame".

![2](https://github.com/motegi-na/For-psychophysical-experiment-Unity-Ultraleap/assets/151810708/f8696cca-fad2-47b8-a327-6ba3c0eedffe)



