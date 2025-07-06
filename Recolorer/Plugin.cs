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

        Plugin.playerOneHexColor = base.Config.Bind<string>("Colors", "Player1HexColor", "FFFFFF", "Player 1's Body Color (Hex or 'None')");
        Plugin.playerTwoHexColor = base.Config.Bind<string>("Colors", "Player2HexColor", "FFFFFF", "Player 2's Body Color (Hex or 'None')");
        Plugin.playerThreeHexColor = base.Config.Bind<string>("Colors", "Player3HexColor", "FFFFFF", "Player 3's Body Color (Hex or 'None')");
        Plugin.playerFourHexColor = base.Config.Bind<string>("Colors", "Player4HexColor", "FFFFFF", "Player 4's Body Color (Hex or 'None')");
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

                Player playerByIndex = PlayerManager.GetPlayerByIndex(i);
                Log.LogDebug("Got player from manager!");

                Renderer bodyRenderer = playerByIndex.bodyRenderer;

                string text;

                Log.LogDebug("Trying to update color!");

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

                text = text.PadLeft(7, '#');


                bool flag = ColorUtility.TryParseHtmlString(text, out Color color);

                if (flag)
                {
                    Log.LogDebug("Attempting to update color!");
                    Log.LogInfo("Old color: #" + bodyRenderer.material.color);
                    bodyRenderer.material.color = color;
                    Log.LogInfo("Updated Player #" + i + "to color" + text + "!");
                }
                else
                {
                    Log.LogError("Error reading color " + text);
                }

                Log.LogInfo("Exiting Player" + i + "!");
            }
        }
    }
}
	