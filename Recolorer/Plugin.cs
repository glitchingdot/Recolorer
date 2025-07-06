using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
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

        playerOneHexColor = Config.Bind<string>("Colors", "Player1HexColor", "None", "Player 1's Body Color (Hex or 'None')");
        playerTwoHexColor = Config.Bind<string>("Colors", "Player2HexColor", "None", "Player 2's Body Color (Hex [FFFFFF] or 'None')");
        playerThreeHexColor = Config.Bind<string>("Colors", "Player3HexColor", "None", "Player 3's Body Color (Hex [FFFFFF] or 'None')");
        playerFourHexColor = Config.Bind<string>("Colors", "Player4HexColor", "None", "Player 4's Body Color (Hex [FFFFFF] or 'None')");

        Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
    }

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
            for (int i = 0; i < PlayerManager.activePlayerCount; i++)
            {
                Log.LogInfo("Updating Player #" + i);

                string text;

                Log.LogDebug("Reading Color!");


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
                        text = "FFFFFF";
                        break;
                }

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
                    Log.LogDebug("Attempting to update material and color!");
                    Renderer bodyRenderer = activePlayer.bodyRenderer;

                    // Player 1 has a white texture on the suit
                    // Using that as a replacement, because other players have textures with colors
                    // Want to change this to use the texture instead of the material

                    bodyRenderer.material = PlayerManager.GetPlayerByIndex(1).bodyRenderer.material;
                    bodyRenderer.material.color = color;
                    Log.LogInfo("Updated Player #" + i + "to color" + text + "!");
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
	