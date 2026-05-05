using Newtonsoft.Json;
using Vintagestory.API.Common;

namespace SmoothCoastlines.LandformHeights {
    public class LandformsHeightsWorldProperty : WorldProperty<LandformGenHeight> {
        [JsonIgnore]
        public LandformGenHeight[] LandformHeightsByIndex;
    }
}
