using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Klei.AI;

namespace ShinebugReactor
{
    public class ShinebugSimulator
    {
        public readonly struct ShinebugData
        {
            //public string Id;
            public readonly float MaxAge;
            public readonly float Lux;
            public readonly float Rad;

            public ShinebugData(float maxAge, float lux, float rad)
            {
                MaxAge = maxAge;
                Lux = lux;
                Rad = rad;
            }
            public override string ToString() => $"(ShinebugData) MaxAge: {MaxAge}; Lux: {Lux}; Rad: {Rad}";
        }
        protected static readonly Dictionary<Tag, ShinebugData> ShinebugValues = new Dictionary<Tag, ShinebugData>();

        public readonly Tag Id;
        public float Age;
        public ShinebugData Data
        {
            get
            {
                ShinebugData data;
                if (!ShinebugValues.TryGetValue(Id, out data))
                {
                    GameObject prefab = Assets.GetPrefab(Id);
                    var ageAttribute = Db.Get().Amounts.Age.maxAttribute;
                    float maxAge = AttributeInstance.GetTotalValue(ageAttribute,
                        prefab.GetComponent<Modifiers>().GetPreModifiers(ageAttribute));
                    Light2D light = prefab.GetComponent<Light2D>();
                    float lux = (light?.Lux).GetValueOrDefault()/* * light?.Range*/;
                    float rad = (prefab.GetComponent<RadiationEmitter>()?.emitRads).GetValueOrDefault();
                    data = new ShinebugData(maxAge * ShinebugReactor.CYCLE_LENGTH, lux, rad);
                    ShinebugValues.Add(Id, data);
                }
                return data;
            }
        }

        public ShinebugSimulator(Tag id, float age = 0.0f)
        {
            Id = id;
            Age = age;
        }

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
