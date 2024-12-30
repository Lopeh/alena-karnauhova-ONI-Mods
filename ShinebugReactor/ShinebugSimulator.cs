using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Klei.AI;

namespace ShinebugReactor
{
    [Serializable]
    public class ShinebugSimulator
    {
        public readonly struct ShinebugData
        {
            //public string Id;
            public readonly Tag Egg;
            public readonly Tag Baby;
            public readonly float MaxAge;
            public readonly float Lux;
            public readonly float Rad;

            public ShinebugData(Tag id) : this(Tag.Invalid, Tag.Invalid, 0, 0, 0)
            {
                GameObject prefab = Assets.GetPrefab(id);
                if (prefab == null) return;
                Egg.Name = id.Name + ShinebugReactor.EGG_POSTFIX;
                Baby.Name = id.Name + ShinebugReactor.BABY_POSTFIX;
                /*Egg = prefab.GetDef<FertilityMonitor.Def>().eggPrefab;
                Baby = Tag.Invalid;
                GameObject eggPrefab = Assets.GetPrefab(Egg);
                if (eggPrefab != null)
                {
                    var incubationDef = eggPrefab.GetDef<IncubationMonitor.Def>();
                    if (incubationDef != null)
                    {
                        Baby = incubationDef.spawnedCreature;
                    }
                }*/
                var ageAttribute = Db.Get().Amounts.Age.maxAttribute;
                MaxAge = AttributeInstance.GetTotalValue(ageAttribute,
                    prefab.GetComponent<Modifiers>().GetPreModifiers(ageAttribute))
                    * ShinebugReactor.CYCLE_LENGTH;
                Light2D light = prefab.GetComponent<Light2D>();
                Lux = (light?.Lux).GetValueOrDefault()/* * light?.Range*/;
                Rad = (prefab.GetComponent<RadiationEmitter>()?.emitRads).GetValueOrDefault();
            }
            private ShinebugData(Tag egg, Tag baby, float maxAge, float lux, float rad)
            {
                Egg = egg;
                Baby = baby;
                MaxAge = maxAge;
                Lux = lux;
                Rad = rad;
            }
            public override string ToString() => $"(ShinebugData) Egg: {Egg}; Baby: {Baby}; MaxAge: {MaxAge}; Lux: {Lux}; Rad: {Rad}";
        }
        protected static readonly Dictionary<Tag, ShinebugData> ShinebugValues = new Dictionary<Tag, ShinebugData>();

        public readonly Tag Id;
        public float Age;
        public ShinebugData Data => AddOrGetData(Id);

        public ShinebugSimulator(string id, float age = 0f)
        {
            Id.Name = id;
            //Id = id;
            Age = age;
        }

        protected static ShinebugData AddOrGetData(Tag id)
        {
            ShinebugData data;
            if (!ShinebugValues.TryGetValue(id, out data))
            {
                data = new ShinebugData(id);
                ShinebugValues.Add(id, data);
            }
            return data;
        }

        /*public static Tag GetCreatureId(GameObject go)
        {
            var id = go.PrefabID();
            if (ShinebugValues.ContainsKey(id))
            {
                return id;
            }
            var data = ShinebugValues.FirstOrDefault(x => x.Value.Egg.Equals(id) || x.Value.Baby.Equals(id));
            if (data.Key != default)
            {
                return data.Key;
            }
            if (go.GetDef<FertilityMonitor.Def>() != null)
            {
                return id;
            }
            var babyDef = go.GetDef<BabyMonitor.Def>();
            if (babyDef != null)
            {
                return babyDef.adultPrefab;
            }
            var incubationDef = go.GetDef<IncubationMonitor.Def>();
            return (incubationDef != null) ?
                GetCreatureId(Assets.GetPrefab(incubationDef.spawnedCreature)) : Tag.Invalid;
        }*/

        public bool Simulate(float dt)
        {
            Age += dt;
            return Age > Data.MaxAge;
        }

        public override string ToString() =>
            $"(FakeShinebug) {Id}: {Age}s/{Data.MaxAge}s {Data.Lux} Lux {Data.Rad} Rads";
    }
    /*public static readonly Dictionary<string, ShinebugEggData> shinebugEggValues = new Dictionary<string, ShinebugEggData>()
{
  {
    "LightBugBlackEgg",
    new ShinebugEggData()
    {
      TimeToHatch = 9000f,
      AdultLife = 45000f,
      AdultLux = 0.0f
    }
  },
  {
    "LightBugBlueEgg",
    new ShinebugEggData()
    {
      TimeToHatch = 3000f,
      AdultLife = 15000f,
      AdultLux = 1800f
    }
  },
  {
    "LightBugEgg",
    new ShinebugEggData()
    {
      TimeToHatch = 3000f,
      AdultLife = 15000f,
      AdultLux = 1800f
    }
  },
  {
    "LightBugCrystalEgg",
    new ShinebugEggData()
    {
      TimeToHatch = 9000f,
      AdultLife = 45000f,
      AdultLux = 1800f
    }
  },
  {
    "LightBugOrangeEgg",
    new ShinebugEggData()
    {
      TimeToHatch = 3000f,
      AdultLife = 15000f,
      AdultLux = 1800f
    }
  },
  {
    "LightBugPinkEgg",
    new ShinebugEggData()
    {
      TimeToHatch = 3000f,
      AdultLife = 15000f,
      AdultLux = 1800f
    }
  },
  {
    "LightBugPurpleEgg",
    new ShinebugEggData()
    {
      TimeToHatch = 3000f,
      AdultLife = 15000f,
      AdultLux = 1800f
    }
  }
};*/
}
