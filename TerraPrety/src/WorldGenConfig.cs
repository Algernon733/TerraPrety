namespace TerraPrety
{
    public class WorldGenConfig
    {
        public float noiseScale = 300.0f;
        public float heightMapNoiseScale = 32.0f;
        public string fallbackParentLandformCode = "ultraflats"; //This is just in case it somehow rolls a height value with no valid Landforms that would fit it, it will use this one instead.

        public bool Delicate_configs_below__alter_at_your_own_peril = false;

        public float oceanWobbleScale = 2.0f;
        public float oceanWobbleIntensity = 1.0f;
        public double[] remappingKeys = { 0.115, 0.285 };
        public double[] remappingValues = {0.0, 1.0 };

        public double[] coastRemappingKeys = { 0.00, 0.285 }; // Starts at 0 opacity, ends at max ocean opacity. Keep the 2nd element the same as remappingKeys's 2nd element so it matches the ocean.
        public double[] coastRemappingValues = { 0.0, 1.0 }; // Don't touch. Basically the coastmap opacities at no coast and full coast.
        public double coastTargetLandformHeight = 0.3; // In coastal areas, move the landform height down towards this value.
        public double coastMinOpacity = 0.15; // Areas below this coastal opacity dont have the coast lowering the landform height.
        public double coastFullOpacity = 0.35; // Areas above this coastal opacity dont get their landform height above coastTargetLandformHeight.
        public double saltwaterAboveCoastOpacity = 0.2; // Areas above this coastal opacity are set as saltwater areas, below is freshwater.

        public int heightMapOctaves = 1;
        public float heightMapPersistance = 0.1f;

        public double[] midHeightKeys = { 0.0, 0.05, 0.33333, 1.0 };
        public double[] midHeightValues = { 1.0, 0.9, 0.0, 0.0 };
        public float chanceForMidZone = 1.0f;
        public float targetMidLevel = 0.2f;
        public float lowThreshForMidZone = 0.2f;

        public float radiusMultOutwardsForSmoothing = 6.0f;

        public float[] heightThresholdsForOceanicityComp = { 0.0f, 1.0f };
        public float[] heightMultsAtThresholdsForOceanicityComp = { 0.0f, 0.0f };
        public float[] heightFlatsAtThresholdsForOceanicityComp = { 0.0f, 0.0f };
        //public float heightAboveWhichToWatchOceanicity = 0.8f;
        //public float highHeightLowOceanicityMin = 6.15f;
        //public float highHeightLowOceanicityMax = 24.6f;
        //public float heightMidAboveWhichToWatchOceanicity = 0.5f;
        //public float midHeightMidOceanicityMin = 4.1f;
        //public float midHeightMidOceanicityMax = 16.4f; //These values are the oceanicity at the spot multiplied by the OceanicityFactor, this is what it recieves so it makes it easier to calculate them

        public double terrainNoiseFrequencyMult = 1.0;
        public double terrainNoisePersistance = 0.9;
        public bool enableEdgeLandformSmoothing = false;
        public int landformSmoothingRadius = 3;
        public int landformMapPadding = 4;
    }
}