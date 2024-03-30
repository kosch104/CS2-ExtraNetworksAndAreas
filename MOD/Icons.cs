using Game.Prefabs;
using System.IO;

namespace ExtraSpacesAndNetworks
{
    internal class Icons
    {
        internal const string IconsResourceKey = "extraspacesandnetworks";
        internal static readonly string COUIBaseLocation = $"coui://{IconsResourceKey}";

        public static readonly string Placeholder = $"{COUIBaseLocation}/Icons/Misc/placeholder.svg";
        public static readonly string GameCrashWarning = $"{COUIBaseLocation}/Icons/Misc/GameCrashWarning.svg";

        internal static void LoadIcons(string path)
        {
            Extra.Lib.UI.Icons.LoadIconsFolder(IconsResourceKey, path);
        }

        public static string GetIcon(PrefabBase prefab)
        {

            if (prefab is null)
                return $"{COUIBaseLocation}/Icons/Misc/placeholder.svg";

            if (File.Exists($"{ESN.ResourcesIcons}/{prefab.GetType().Name}/{prefab.name}.svg"))
            {
                ESN.Logger.Info($"Found icon in mod folder: {ESN.ResourcesIcons}/{prefab.GetType().Name}/{prefab.name}.svg");
                return $"{COUIBaseLocation}/Icons/{prefab.GetType().Name}/{prefab.name}.svg";
            }

            ESN.Logger.Info($"Did not find icon in mod folder: {ESN.ResourcesIcons}/{prefab.GetType().Name}/{prefab.name}.svg");

            if (prefab is SurfacePrefab)
            {
                return "Media/Game/Icons/LotTool.svg";
            }
            else if (prefab is UIAssetCategoryPrefab)
            {

                return $"{COUIBaseLocation}/Icons/Misc/placeholder.svg";
            }
            else if (prefab is UIAssetMenuPrefab)
            {

                return $"{COUIBaseLocation}/Icons/Misc/placeholder.svg";
            }

            return Placeholder;
        }

    }
}
