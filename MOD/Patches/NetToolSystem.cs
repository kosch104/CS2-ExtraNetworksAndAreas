using System;
using Game.Prefabs;
using Game.Tools;
using HarmonyLib;

namespace ExtraNetworksAndAreas;

class NetToolSystemPatch {
	[HarmonyPatch(typeof(NetToolSystem), nameof(NetToolSystem.GetAvailableSnapMask),
		new Type[] { typeof(NetGeometryData), typeof(PlaceableNetData), typeof(NetToolSystem.Mode), typeof(bool), typeof(bool), typeof(bool), typeof(Snap), typeof(Snap) },
		new ArgumentType[] {ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out })]
	class NetToolSystem_GetAvailableSnapMask
	{
		private static bool Prefix( NetGeometryData prefabGeometryData, PlaceableNetData placeableNetData, NetToolSystem.Mode mode, bool editorMode, bool laneContainer, bool underground, out Snap onMask, out Snap offMask) {

			if (mode == NetToolSystem.Mode.Replace)
			{
				onMask = Snap.ExistingGeometry;
				offMask = onMask;
				if ((placeableNetData.m_PlacementFlags & Game.Net.PlacementFlags.UpgradeOnly) == Game.Net.PlacementFlags.None)
				{
					onMask |= Snap.ContourLines;
					offMask |= Snap.ContourLines;
				}
				if (laneContainer)
				{
					onMask &= ~Snap.ExistingGeometry;
					offMask &= ~Snap.ExistingGeometry;
					onMask |= Snap.NearbyGeometry;
					return false;
				}
				if ((prefabGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) != (Game.Net.GeometryFlags)0)
				{
					offMask &= ~Snap.ExistingGeometry;
				}
				if ((prefabGeometryData.m_Flags & Game.Net.GeometryFlags.SnapCellSize) != (Game.Net.GeometryFlags)0)
				{
					onMask |= Snap.CellLength;
					offMask |= Snap.CellLength;
					return false;
				}

				return false;
			}

			onMask = (Snap.ExistingGeometry | Snap.CellLength | Snap.StraightDirection | Snap.ObjectSide | Snap.GuideLines | Snap.ZoneGrid | Snap.ContourLines);
			offMask = onMask;
			if (underground)
			{
				onMask &= ~(Snap.ObjectSide | Snap.ZoneGrid);
			}
			if (laneContainer)
			{
				onMask &= ~(Snap.CellLength | Snap.ObjectSide);
				offMask &= ~(Snap.CellLength | Snap.ObjectSide);
			}
			else if ((prefabGeometryData.m_Flags & Game.Net.GeometryFlags.Marker) != (Game.Net.GeometryFlags)0)
			{
				onMask &= ~Snap.ObjectSide;
				offMask &= ~Snap.ObjectSide;
			}
			if (laneContainer)
			{
				onMask &= ~Snap.ExistingGeometry;
				offMask &= ~Snap.ExistingGeometry;
				onMask |= Snap.NearbyGeometry;
				offMask |= Snap.NearbyGeometry;
			}
			else if ((prefabGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) != (Game.Net.GeometryFlags)0)
			{
				offMask &= ~Snap.ExistingGeometry;
				onMask |= Snap.NearbyGeometry;
				offMask |= Snap.NearbyGeometry;
			}

			onMask |= Snap.ObjectSurface | Snap.LotGrid;
			offMask |= Snap.ObjectSurface | Snap.LotGrid;

			if (editorMode)
			{
				onMask |= Snap.AutoParent;
				offMask |= Snap.AutoParent;
			}
			
			return false;
		}
	}
}