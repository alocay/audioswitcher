<div id="top"></div>

<br />
<div align="center">
<h3 align="center">Audio Switcher</h3>

  <p align="center">
    Audio Switcher is a simple Windows console application to help a user switch between different playback devices depending on which process has the focus.
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    </li>
    <li><a href="#usage">Usage</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

Audio Switcher is a simple Windows console application to help a user switch between different playback devices depending on which process has the focus. This program does require admin rights to function correct (required for the process start and focus changed events).

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- USAGE EXAMPLES -->
## Usage

Using Audio Switcher is easy, just run the application as administrator, setup a default playback device, configure any mappings between a process/executable and a playback device. Audio Switcher will handle the rest automatically.

### Menu descriptions:

1. List playback devices:    Will list found playback devices
2. View mappings:            Shows an existing mappings between a program and playback device
3. Add/update audio mapping: Adds/Updates a mapping
4. Remove audio mapping:     Removes a mapping
5. View settings:            View the current setting values
6. Change settings:          Update the settings
7. Exit:                     Exit the application

### Adding/Updating an audio mapping

When adding or updating an existing mapping the process is simply to choose a playback device and then the program that is mapped to it:

1. Choose the playback device from the provided list.
2. Then decide how to find the program to map it to. The options are either by locating it's executable file, choosing it from a list of all current process, or simply typing the name of the executable yourself.
   * The preferred method is to locate the executable via the file picker.
3. Once both are chosen, the mapping is create and the application will immediately begin switching your audio based on your `Switch playback device only on process start` setting.

### Settings

The application has a few settings that can be changed:

1. Switch playback device only on process start: Determines if the audio should switch when a program starts instead of when a program takes the foreground focus. The default is false (switch on foreground focus).
2. Switch to fallback playback device when no mapping found: Determines if the audio should automatically be switched to a default playback device if no mapping is found for the current process. For example if you alt-tab out a game, this can change the audio back to a fallback device until you switch to the game.
3. Change the fallback device when no mapping is found: Allows you to update which playback device is the fallback device.

<p align="right">(<a href="#top">back to top</a>)</p>


<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#top">back to top</a>)</p>