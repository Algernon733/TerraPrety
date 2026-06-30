using System;
using System.Collections.Generic;
using TerraPrety;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.ServerMods;

namespace TerraPrety.Noise {

    // Basic voronoi noise with the ability to force the position of some voronoi points based on a list.
    interface Noise2D {
        double getValueAt(int unscaledXpos, int unscaledZpos);
    }

    public class VoronoiNoise: NoiseBase, Noise2D {

        double scale;
        const double maxDistanceConstant = 1.41421356237309505; //Square Root of 2
        List<XZ> forcedPoints;
        public Dictionary<XZ, VoronoiDataPoint> pointCache => ObjectCacheUtil.GetOrCreate(TerraPretyModSystem.Sapi as ICoreAPI, "continentalVoronoiPoints", () => new Dictionary<XZ, VoronoiDataPoint>());

        private readonly long mapGenSeed;

        public VoronoiNoise(long seed, double scale, List<XZ> forcedPoints) : base(seed) {
            this.scale = scale;
            this.forcedPoints = forcedPoints;

            // A separate seed from NoiseBase so we don't mutate NoiseBase's seed by using it at the same time on different genTerra threads.
            this.mapGenSeed = seed;
            this.mapGenSeed *= (seed * 6364136223846793005L) + 1442695040888963407L;
            this.mapGenSeed += 1L;
            this.mapGenSeed *= (seed * 6364136223846793005L) + 1442695040888963407L;
            this.mapGenSeed += 2L;
            this.mapGenSeed *= (seed * 6364136223846793005L) + 1442695040888963407L;
            this.mapGenSeed += 3L;
        }

        //Will generate the voronoi noise value at the given point normalized to [0,1]
        public double getValueAt(int unscaledXpos, int unscaledZpos) {
            this.GetValueAt(unscaledXpos, unscaledZpos, out double min_distance, out double _);

            // Normalize to [0,1] and return
            return min_distance / maxDistanceConstant;
        }

        // Gets the mountain ridge pattern from the F2-F1 Cellular Square pattern http://www.neilblevins.com/art_lessons/procedural_noise/procedural_noise.html
        public double GetMountainRidgeValueAt(int unscaledXpos, int unscaledZpos) {
            this.GetValueAt(unscaledXpos, unscaledZpos, out double F1_1stClosestPointDistance, out double F2_2ndClosestPointDistance);

            double voronoiCellularPattern = 1.0 - (F2_2ndClosestPointDistance - F1_1stClosestPointDistance);
            if (voronoiCellularPattern < 0.0)
                return (double)0.0;
            else
                return (double)voronoiCellularPattern;
        }

        private void GetValueAt(int unscaledXpos, int unscaledZpos, out double F1_1stClosestPointDistance, out double F2_2ndClosestPointDistance) {
            double xpos_full = unscaledXpos / scale;
            double zpos_full = unscaledZpos / scale;

            //Integer part of the position is the voronoi square coordinate
            int xCell = (int)xpos_full;
            int zCell = (int)zpos_full;

            //Fractional part is the location relative to the voronoi square
            double xFrac = xpos_full - xCell;
            double zFrac = zpos_full - zCell;

            F1_1stClosestPointDistance = double.MaxValue;
            F2_2ndClosestPointDistance = double.MaxValue;

            // Iterate over the voronoi square and its 8 nighbours
            for (int dx = 0; dx < 3; dx++) {
                for (int dz = 0; dz < 3; dz++) {

                    //First check whether we have forced voronoi points in this cell
                    bool forced = false;
                    for(int i = 0; i < forcedPoints.Count; i++) {
                        double forcedX = forcedPoints[i].X / scale;
                        double forcedY = forcedPoints[i].Z / scale;
                        if (xCell - 1 + dx < forcedX && xCell - 1 + dx + 1 >= forcedX
                            && zCell - 1 + dz < forcedY && zCell - 1 + dz + 1 >= forcedY)
                        {
                            double pointPosX = forcedX - xCell;
                            double pointPosZ = forcedY - zCell;
                            forced = true;
                            // Find the closest two voronoi points
                            CompareNearest(xFrac, zFrac, pointPosX, pointPosZ, ref F1_1stClosestPointDistance, ref F2_2ndClosestPointDistance);
                        }
                    }
                    // Generate a random voronoi point for the cell if none is forced
                    if(!forced)
                    {
                        long seed = PositionSeed(xCell - 1 + dx, zCell - 1 + dz);
                        double pointPosX = (NextIntLocal(ref seed, 10000) / 10000.0) - 1 + dx;
                        double pointPosZ = (NextIntLocal(ref seed, 10000) / 10000.0) - 1 + dz;
                        CompareNearest(xFrac, zFrac, pointPosX, pointPosZ, ref F1_1stClosestPointDistance, ref F2_2ndClosestPointDistance);
                    }
                }
            }
        }

        // F1 is closest, F2 is second closest
        private static void CompareNearest(double xFrac, double zFrac, double pointPosX, double pointPosZ, ref double F1, ref double F2) {
            double distance = GameMath.Sqrt(((xFrac - pointPosX) * (xFrac - pointPosX)) + ((zFrac - pointPosZ) * (zFrac - pointPosZ)));
            if (distance < F1)
            {
                F2 = F1;
                F1 = distance;
            }
            else if (distance < F2)
            {
                F2 = distance;
            }
        }

        private long PositionSeed(int xPos, int zPos) {
            long seed = mapGenSeed;
            seed *= (seed * 6364136223846793005L) + 1442695040888963407L;
            seed += xPos;
            seed *= (seed * 6364136223846793005L) + 1442695040888963407L;
            seed += zPos;
            seed *= (seed * 6364136223846793005L) + 1442695040888963407L;
            seed += xPos;
            seed *= (seed * 6364136223846793005L) + 1442695040888963407L;
            seed += zPos;

            return seed;
        }

        private int NextIntLocal(ref long seed, int max) {
            int r = (int)((seed >> 24) % (long)max);
            if (r < 0)
                r += max;
            seed *= (seed * 6364136223846793005L) + 1442695040888963407L;
            seed += mapGenSeed;

            return r;
        }
    }

    public class VoronoiRidgeNoise(VoronoiNoise voronoi) : Noise2D {
        public double getValueAt(int unscaledXpos, int unscaledZpos) => voronoi.GetMountainRidgeValueAt(unscaledXpos, unscaledZpos);
    }

    public struct XZd {

        public double X;
        public double Z;

        public XZd(double x, double z) {
            X = x;
            Z = z;
        }
    }

    public class VoronoiDataPoint {

        public XZd pos;
        public XZd[] neighbors;
        public bool distCalced = false;
        public double[] distancesToNeighbors;

        public VoronoiDataPoint(XZd point) {
            pos = point;
            neighbors = new XZd[9];
        }

        public void CalcDistToNeighbors() {
            if (!distCalced) {
                for (int i = 0; i < neighbors.Length; i++) {
                    var neighbor = neighbors[i];
                    distancesToNeighbors[i] = GameMath.Sqrt((pos.X - neighbor.X) * (pos.X - neighbor.X) + (pos.Z - neighbor.Z) * (pos.Z - neighbor.Z));
                }
                distCalced = true;
            }
        }

        public double GetMinDist(double x, double z) {
            var min_distance = Double.MaxValue;
            for (int dx = 0; dx < 3; dx++) {
                for (int dz = 0; dz < 3; dz++) {
                    var neighbor = GetNeighborByXZOffset(dx, dz);
                    var distance = GameMath.Sqrt((x - neighbor.X) * (x - neighbor.X) + (z - neighbor.Z) * (z - neighbor.Z));
                    if (min_distance > distance) {
                        min_distance = distance;
                    }
                }
            }
            return min_distance;
        }

        public XZd GetNeighborByXZOffset(int dx, int dz) { //dx should be from 0 to 2, same for dz! Just like above.
            if (dx == 1 && dz == 1) {
                return pos;
            }

            var index = (dx * 3) + dz;
            return neighbors[index];
        }

        public void SetNeighborByXZOffset(int dx, int dz, XZd neighbor) {
            if (dx == 1 && dz == 1) {
                pos = neighbor;
            }

            var index = (dx * 3) + dz;
            neighbors[index] = neighbor;
        }
    }
}
