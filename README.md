# App for assessing loudness constancy with source distance
 
This application was developped to carry out a psychoacoustics experiment on the loudness of distant sounds. The associated article can be found [here](https://rdcu.be/dhgHV).

## Introduction

Participants were asked to assess the loudness of either a loudspeaker displaying white noise bursts or a speaker uttering words. The two sources could be either visible or hidden and positionned at various distance from the listening position.

This experiment was setup in virtual reality, with participants wearing a HMD ([HTC Vive](https://www.vive.com/fr/)) and headphones ([Sennheiser HD650](https://www.sennheiser-hearing.com/en-US/p/hd-650/)).

## Softwares

The main application was developped on [MaxMSP](https://cycling74.com/products/max) (see the `Max\Manip.maxpat` file), which managed the audio files, the audio rendering, and communicated with [Unity](https://unity.com/) (thanks to the OSC protocol) for moving the virtual objects.

The virtual environment was rendered in a Unity scene, with 3D models created from scratch in Blender.

## Audio rendering

The room acoustics was simulated in real time. 4th order Ambisonics room impulse responses were recorded in several rooms. Anechoic stimuli (the noise burst or the spoken words, recorded in proximity in a recording booth) were then convolved with these impulse responses. Finally, a dynamic binaural decoding was performed, depending on the participant head orientation (captured thanks to the HMD).

## Demo

A tablet computer was used as the response interface. It was placed in front of the participant. A display of the HMD camera input within the virtual environment allowed the participants to see their hands and the tablet computer.

In a first part, listeners assessed the loudness of noise bursts displayed by a loudspeaker. The visual sound source was a 3D rendered loudspeaker, which had been created using Blender.

![noise bursts assessing](readme_assets/manip_distance_bruit.gif)

In a second part, listeners assessed the loudness of speech stimuli. A speaker was first recorded uttering words in a recording booth, thanks to an omnidirectional microphone placed close to their mouth. Then, the same speaker was filmed uttering the same words in front of a blue screen.

These blue screen recordings were later on incrusted within the virtual environment to serve as the visual virtual sources.


![speech assessing](readme_assets/manip_distance_voix.gif)

Recordings of both experiments (including audio) are available [here (noise)](https://youtu.be/V1ifR558VO4) and [here (speech)](https://youtu.be/F3xAx_j0YZw).