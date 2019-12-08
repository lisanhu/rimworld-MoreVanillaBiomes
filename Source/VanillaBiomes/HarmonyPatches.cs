﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using Harmony;
using RimWorld.Planet;

namespace VanillaBiomes
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {

        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.vanillabiomes");
            MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.Planet.World), "CoastDirectionAt");
            HarmonyMethod prefixmethod = new HarmonyMethod(typeof(VanillaBiomes.HarmonyPatches).GetMethod("CoastDirectionAt_Prefix"));

            // patch the targetmethod, by calling prefixmethod before it runs, with no postfixmethod (i.e. null)
            harmony.Patch(targetmethod, prefixmethod, null);

            AddPlantsToBiomes();
            AddAnimalsToBiomes();

        }


        // from RF-Archipelagos
        public static bool CoastDirectionAt_Prefix(int tileID, ref Rot4 __result, ref World __instance)
        {
            var world = Traverse.Create(__instance);
            WorldGrid worldGrid = world.Field("grid").GetValue<WorldGrid>();
            if (worldGrid[tileID].biome.defName.Contains("NoBeach"))
            {
                __result = Rot4.Invalid;
                return false;
            }
            return true;
        }


        // adapted from RF-Archipelagos

        private static void AddPlantsToBiomes()
        {

            foreach (ThingDef current in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if(current.plant != null)
                {

                    if (current.plant.wildBiomes != null)
                    {
                        for (int j = 0; j < current.plant.wildBiomes.Count; j++)
                        {
                            // icebergs
                            if (current.plant.wildBiomes[j].biome.defName == "SeaIce")
                            {
                                PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_Iceberg_NoBeach");
                                newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                current.plant.wildBiomes.Add(newRecord1);
                            }

                            //sandbar
                            if (current.plant.wildBiomes[j].biome.defName == "Desert")
                            {
                                PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_Sandbar_NoBeach");
                                newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                current.plant.wildBiomes.Add(newRecord1);
                            }
                            if (current.plant.wildBiomes[j].biome.defName == "ExtremeDesert")
                            {
                                PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_Sandbar_NoBeach");
                                newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                current.plant.wildBiomes.Add(newRecord1);
                            }

                            //Meadow
                            if (current.plant.wildBiomes[j].biome.defName == "BorealForest")
                            {
                                PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_AlpineMeadow");
                                newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                if (current.plant.IsTree)
                                {
                                    newRecord1.commonality *= 0.4f;
                                }
                                else if (current.plant.purpose == PlantPurpose.Beauty)
                                {
                                    newRecord1.commonality *= 2f;
                                }


                                current.plant.wildBiomes.Add(newRecord1);
                            }

                            //Grasslands
                            if (current.plant.wildBiomes[j].biome.defName == "AridShubland")
                            {
                                PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_Grasslands");
                                newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                if (current.plant.IsTree)
                                {
                                    newRecord1.commonality *= 0.4f;
                                }
                                current.plant.wildBiomes.Add(newRecord1);
                            }

                            //Dunes, Oasis
                            if (current.plant.wildBiomes[j].biome.defName == "AridShubland")
                            {
                                if (current.plant.purpose == PlantPurpose.Food)
                                {
                                    PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                    newRecord1.biome = BiomeDef.Named("ZBiome_CoastalDunes");
                                    newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                    current.plant.wildBiomes.Add(newRecord1);

                                    PlantBiomeRecord newRecord2 = new PlantBiomeRecord();
                                    newRecord2.biome = BiomeDef.Named("ZBiome_DesertOasis");
                                    newRecord2.commonality = current.plant.wildBiomes[j].commonality;
                                    current.plant.wildBiomes.Add(newRecord2);
                                }
                            }

                            if (current.plant.wildBiomes[j].biome.defName == "TropicalRainforest")
                            {
                                if (current.plant.purpose != PlantPurpose.Food)
                                {
                                    PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                    newRecord1.biome = BiomeDef.Named("ZBiome_CoastalDunes");
                                    newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                    current.plant.wildBiomes.Add(newRecord1);

                                    PlantBiomeRecord newRecord2 = new PlantBiomeRecord();
                                    newRecord2.biome = BiomeDef.Named("ZBiome_DesertOasis");
                                    newRecord2.commonality = current.plant.wildBiomes[j].commonality;
                                    current.plant.wildBiomes.Add(newRecord2);
                                }

                            }

                        }

                    }
                }
            }
        }



        private static void AddAnimalsToBiomes()
        {
            //foreach (PawnKindDef current in DefDatabase<PawnKindDef>.AllDefs)
            //{
            //    if (current.RaceProps.wildBiomes != null && current.defName != "Cobra")
            //    {
            //        for (int j = 0; j < current.RaceProps.wildBiomes.Count; j++)
            //        {
            //            if (current.RaceProps.wildBiomes[j].biome.defName == "AridShrubland")
            //            {
            //                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
            //                AnimalBiomeRecord newRecord2 = new AnimalBiomeRecord();
            //                newRecord1.biome = BiomeDef.Named("DesertArchipelago");
            //                newRecord2.biome = BiomeDef.Named("DesertArchipelago_Fresh");
            //                newRecord1.commonality = current.RaceProps.wildBiomes[j].commonality;
            //                newRecord2.commonality = current.RaceProps.wildBiomes[j].commonality;
            //                current.RaceProps.wildBiomes.Add(newRecord1);
            //                current.RaceProps.wildBiomes.Add(newRecord2);
            //            }
            //        }
            //    }
            //}
        }

    }
}
