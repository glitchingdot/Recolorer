using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using HB.Collections;
using HB.Runtime.Player;
using UnityEngine;

namespace Recolorer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Heavenly Bodies.exe")]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;

    public override void Load()
    {
        // Plugin startup logic
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        exampleHex = Config.Bind<string>("Examples", "ExampleHex", "FF0000", "Entering a color code of pure red");
        exampleNone = Config.Bind<string>("Examples", ExampleNone", "None", "Player will not be recolored, will use default skin");

        playerOneHexColor = Config.Bind<string>("Colors", "Player1HexColor", "FFFFFF", "Player 1's Body Color (Hex [e.x. FFFFFF] or 'None')");
        playerTwoHexColor = Config.Bind<string>("Colors", "Player2HexColor", "None", "Player 2's Body Color (Hex [e.x. FFFFFF] or 'None')");
        playerThreeHexColor = Config.Bind<string>("Colors", "Player3HexColor", "None", "Player 3's Body Color (Hex [e.x. FFFFFF] or 'None')");
        playerFourHexColor = Config.Bind<string>("Colors", "Player4HexColor", "None", "Player 4's Body Color (Hex [e.x. FFFFFF] or 'None')");

        Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
    }

    internal static ConfigEntry<string> exampleHex;
    internal static ConfigEntry<string> exampleNone;

    internal static ConfigEntry<string> playerOneHexColor;
    internal static ConfigEntry<string> playerTwoHexColor;
    internal static ConfigEntry<string> playerThreeHexColor;
    internal static ConfigEntry<string> playerFourHexColor;

    [HarmonyPatch(typeof(Player), "OnSpawn")]
    private class PlayerSpawnPatch
    {
        [HarmonyPostfix]
        private static void RecolorPlayers()
        {
            // Load the material collection
            int baseSkinsCollectionID = 12590;
            Object loadedObj = Object.ForceLoadFromInstanceID(baseSkinsCollectionID);

            // Player 1 has a white texture on the suit
            // Using that as a replacement, because other players have textures with colors
            MaterialCollection materialCollection = loadedObj.TryCast<MaterialCollection>();

            if (materialCollection == null)
            {
                Log.LogError("Error casting Object to MaterialCollection!");
                return;
            }
            else
            {
                Log.LogDebug("Casted Object to MaterialCollection!");
            }

            int whiteMaterialIndex = 1;
            Material whiteMaterial = materialCollection.ToArray()[whiteMaterialIndex];

            for (int i = 0; i < PlayerManager.activePlayerCount; i++)
            {
                Log.LogInfo("Updating Player #" + i);

                string text;

                Log.LogDebug("Reading color from player index!");

                // not great but best i've got rn
                switch (i)
                {
                    case 0:
                        text = playerOneHexColor.Value;
                        break;
                    case 1:
                        text = playerTwoHexColor.Value;
                        break;
                    case 2:
                        text = playerThreeHexColor.Value;
                        break;
                    case 3:
                        text = playerFourHexColor.Value;
                        break;
                    default:
                        Log.LogInfo("Player is outside of regular bounds!");
                        text = "FFFFFF";
                        break;
                }

                // To-Do: flip this to a reg-ex for hex colors
                if (text == "None")
                {
                    Log.LogInfo("Skipping Player #" + i);
                    continue;
                }

                Player activePlayer = PlayerManager.GetPlayerByIndex(i);
                Log.LogDebug("Got Player from PlayerManager!");

                // Add the # to the color code
                text = text.PadLeft(7, '#');

                bool flag = ColorUtility.TryParseHtmlString(text, out Color color);

                if (flag)
                {
                    Log.LogDebug("Parsed color as " + color.ToString());
                    Log.LogDebug("Attempting to update material and color!");
                    Renderer bodyRenderer = activePlayer.bodyRenderer;

                    if (whiteMaterial == null)
                    {
                        Log.LogError("Error with replacement material!");
                    }
                    else
                    {
                        bodyRenderer.material = whiteMaterial;
                        Log.LogDebug("Updated material!");
                    }
                    bodyRenderer.material.color = color;

                    Log.LogInfo("Updated Player #" + i + " to color " + color.ToString() + "!");
                }
                else
                {
                    Log.LogError("Error reading color " + text);
                }

                Log.LogDebug("Exiting Player" + i + "!");
            }
        }
    }
}
	