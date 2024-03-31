using System;
using System.Collections.Generic;
using Extra.Lib.Helper;
using Extra.Lib;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Colossal.Entities;
using Game.Net;
using Game.Objects;
using UnityEngine;

namespace ExtraNetworksAndAreas
{
	internal static class EditEntities
	{
		internal static void SetupEditEntities()
		{

			EntityQueryDesc spacesEntityQueryDesc = new()
			{
				All = [
					ComponentType.ReadOnly<SpaceData>(),
				],
				None = [
					ComponentType.ReadOnly<PlaceholderObjectElement>(),
				]
			};

			EntityQueryDesc pathwaysEntityQueryDesc = new()
			{
				All = [
					ComponentType.ReadOnly<PathwayData>(),
				],
				None = [
					ComponentType.ReadOnly<PlaceholderObjectElement>(),
				]
			};

			EntityQueryDesc tracksEntityQueryDesc = new()
			{
				All = [
					ComponentType.ReadOnly<TrackData>(),
				],
				None = [
					ComponentType.ReadOnly<PlaceholderObjectElement>(),
				]
			};

			EntityQueryDesc markerObjectsEntityQueryDesc = new EntityQueryDesc
			{
				All =
				[
					ComponentType.ReadOnly<UIObjectData>()
				],
				Any =
				[
					ComponentType.ReadOnly<TrafficSpawnerData>(),
					ComponentType.ReadOnly<CreatureSpawnData>(),
					ComponentType.ReadOnly<OutsideConnectionData>(),
					//ComponentType.ReadOnly<NetObjectData>(),
					//ComponentType.ReadOnly<NetData>(),
					ComponentType.ReadOnly<TransportStopData>(),
				]
			};

			ExtraLib.AddOnEditEnities(new(OnEditSpacesEntities, spacesEntityQueryDesc));
			ExtraLib.AddOnEditEnities(new(OnEditPathwayEntities, pathwaysEntityQueryDesc));
			ExtraLib.AddOnEditEnities(new(OnEditTrackEntities, tracksEntityQueryDesc));
			ExtraLib.AddOnEditEnities(new(OnEditMarkerObjectEntities, markerObjectsEntityQueryDesc));
			// TransportStopData
		}

		private static void Log(string message)
		{
			ENA.Logger.Info(message);
		}

		private static string GetIcon(PrefabBase prefab)
		{
			Dictionary<string, string> overrideIcons = new()
			{
				{ "Double Train Track - Twoway", "Media/Game/Icons/DoubleTrainTrack.svg" },
				{ "Oneway Tram Track - Inside", "Media/Game/Icons/OnewayTramTrack.svg" },
				{ "Double Subway Track - Twoway", "Media/Game/Icons/DoubleTrainTrack.svg" },
				{ "Twoway Subway Track", "Media/Game/Icons/TwoWayTrainTrack.svg" },
			};
			if (overrideIcons.TryGetValue(prefab.name, out string icon))
			{
				return icon;
			}
			return Icons.GetIcon(prefab);
		}

		private static void OnEditMarkerObjectEntities(NativeArray<Entity> entities)
		{
			ENA.Logger.Info("Marker Object entities:");
			foreach (Entity entity in entities)
			{
				Log("Test");
				if (ExtraLib.m_PrefabSystem.TryGetPrefab(entity, out MarkerObjectPrefab prefab))
				{
					if (prefab != null)
					{
						Log("Marker entities prefab found: " + prefab.name);
					}
					else
					{
						try
						{
							Log("Prefab is null");
							if (ExtraLib.m_PrefabSystem.TryGetPrefab(entity, out ObjectPrefab prefab2))
							{
								Log("Prefab Type: " + prefab2.GetType());
							}
							else
							{
								Log("Prefab Type not found");
							}

							Log("Prefab done");
						}
						catch (Exception x)
						{
							Log("Exception: " + x);
						}
						continue;
					}
					var prefabUI = prefab.GetComponent<UIObject>();
					if (prefabUI == null)
					{
						Log("UI Object not found");
						prefabUI = prefab.AddComponent<UIObject>();
						prefabUI.active = true;
						prefabUI.m_IsDebugObject = false;
						prefabUI.m_Icon = GetIcon(prefab);
						prefabUI.m_Priority = 1;
					}
					else
					{
						Log("UI Object found");
						prefabUI.m_Icon = GetIcon(prefab);
					}

					prefabUI.m_Group?.RemoveElement(entity);
					prefabUI.m_Group = PrefabsHelper.GetOrCreateUIAssetCategoryPrefab("Landscaping", "Marker Object Prefabs", Icons.GetIcon, "Spaces");
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
				else
				{
					if (ExtraLib.m_PrefabSystem.TryGetPrefab(entity, out ObjectPrefab prefab2))
					{
						ENA.Logger.Info("Non-Marker prefab found: " + prefab2.name);
					}
				}
			}
			ENA.Logger.Info("Spawner entities done");
		}

		private static void OnEditTrackEntities(NativeArray<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				if (ExtraLib.m_PrefabSystem.TryGetPrefab(entity, out TrackPrefab prefab))
				{
					var prefabUI = prefab.GetComponent<UIObject>();
					if (prefabUI == null)
					{
						prefabUI = prefab.AddComponent<UIObject>();
						prefabUI.active = true;
						prefabUI.m_IsDebugObject = false;
						prefabUI.m_Icon = GetIcon(prefab);
						prefabUI.m_Priority = 1;
					}

					prefabUI.m_Group?.RemoveElement(entity);
					if (prefab.m_TrackType == TrackTypes.Train)
						prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationTrain");
					if (prefab.m_TrackType == TrackTypes.Subway)
						prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationSubway");
					if (prefab.m_TrackType == TrackTypes.Tram)
						prefabUI.m_Group = PrefabsHelper.GetUIAssetCategoryPrefab("TransportationTram");
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
			}
		}

		private static void OnEditPathwayEntities(NativeArray<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				if (ExtraLib.m_PrefabSystem.TryGetPrefab(entity, out PathwayPrefab prefab))
				{
					var prefabUI = prefab.GetComponent<UIObject>();
					if (prefabUI == null)
					{
						prefabUI = prefab.AddComponent<UIObject>();
						prefabUI.active = true;
						prefabUI.m_IsDebugObject = false;
						prefabUI.m_Icon = GetIcon(prefab);
						prefabUI.m_Priority = 1;

					}
					prefabUI.m_Group?.RemoveElement(entity);
					prefabUI.m_Group = PrefabsHelper.GetOrCreateUIAssetCategoryPrefab("Landscaping", "Pathways", Icons.GetIcon);
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
			}
		}

		private static void OnEditSpacesEntities(NativeArray<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				if (ExtraLib.m_PrefabSystem.TryGetPrefab(entity, out SpacePrefab prefab))
				{
					var prefabUI = prefab.GetComponent<UIObject>();
					if (prefabUI == null)
					{
						prefabUI = prefab.AddComponent<UIObject>();
						prefabUI.active = true;
						prefabUI.m_IsDebugObject = false;
						prefabUI.m_Icon = GetIcon(prefab);
						prefabUI.m_Priority = 1;
					}

					prefabUI.m_Group?.RemoveElement(entity);
					prefabUI.m_Group = PrefabsHelper.GetOrCreateUIAssetCategoryPrefab("Landscaping", "Spaces", Icons.GetIcon, "Pathways");
					prefabUI.m_Group.AddElement(entity);

					ExtraLib.m_EntityManager.AddOrSetComponentData(entity, prefabUI.ToComponentData());
				}
			}
		}

	}
}
