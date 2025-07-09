# Recolorer
Because cosmonauts need fashion too.

## Description
Recolorer is a mod for 2pt Interactive's [Heavenly Bodies](https://heavenlybodiesgame.com/) that adds support for custom colors for all four player characters.

## Installation
1. Download and Install BepInEx following the instructions on their [website](https://docs.bepinex.dev/master/articles/user_guide/installation/unity_il2cpp.html)  
(Make sure to download the **Unity.IL2CPP-win-x86** version of BepInEx)
2. Download the most recent `Recolorer.zip` from the [Releases](https://github.com/glitchingdot/Recolorer/releases) page
3. Extract `Recolorer.zip` into a folder
4. Copy the `plugins/com-jackdotpng-Recolorer` folder into the `BepInEx/plugins/` folder  
(Final install should follow this path: `BepInEx/plugins/com-jackdotpng-Recolorer/com.jackdotpng.Recolorer.dll`)
5. Run Heavenly Bodies once to generate config files and data for Recolorer and BepInEx

Congrats, you've installed Recolorer!

# Usage
* Open `\BepInEx\config\Recolorer.cfg` in your preferred text editor
* Edit the provided colors to change the color of each cosmonaut
* Colors can either be entered as:
    * Hex (ex: `0F0F0F`)  
    OR
    * None (will use default color)
* Save the config file
* Launch Heavenly Bodies!  
  Heavenly Bodies can be launched from both Steam or the game .exe `Heavenly Bodies.exe`. Both work!)

# Compiling from source
* Ensure you have **.NET SDK (version 6 or newer)** installed
* Download the source files from the repo
* Create a /lib/ directory in the root of Recolorer (`Recolorer/lib/`)
* Run Heavenly Bodies at least once after installing BepInEx
* Inside the Heavenly Bodies root directory, navigate to `BepInEx/interop/`, and copy:
	* Assembly-CSharp.dll
	* Il2Cppmscorlib.dll
	* UnityEngine.CoreModule
   	* ScriptableObject-Architecture
into the lib folder
* Compile using `dotnet build`

## Other
If you run into issues, DM me on Discord: `@jackdotpng`
