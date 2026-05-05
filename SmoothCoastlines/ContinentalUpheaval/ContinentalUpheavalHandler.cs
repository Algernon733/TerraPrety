

// SmoothCoastlines.ContinentalUpheaval.ContinentalUpheavalHandler
using SmoothCoastlines;
using SmoothCoastlines.LandformHeights;
using Vintagestory.API.Common;
using Vintagestory.ServerMods;

public class ContinentalUpheavalHandler
{
	public static void PostGenMapsOnMapRegionGen(IMapRegion mapRegion, int regionX, int regionZ)
	{
		MapLayerLandformsSmooth landformsGen = (MapLayerLandformsSmooth) SmoothCoastlinesModSystem.Sapi.ModLoader.GetModSystem<GenMaps>(true).landformsGen;
        landformsGen?.AddHeightmapToRegion(mapRegion);
	}
}
