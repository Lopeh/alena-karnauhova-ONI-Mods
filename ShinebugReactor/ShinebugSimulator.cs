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
            //public string Name;
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
        protected static readonly Dictionary<string, ShinebugData> ShinebugValues = new Dictionary<string, ShinebugData>();

        public readonly string Name;
        public float Age;
        public ShinebugData Data
        {
            get
            {
                if (!ShinebugValues.ContainsKey(Name))
                {
                    GameObject prefab = Assets.GetPrefab(Name);
                    string ageAttributeId = Db.Get().Amounts.Age.maxAttribute.Id;
                    Trait creatureTrait = Db.Get().traits.TryGet(prefab.GetComponent<Modifiers>()
                        .initialTraits.Last());//FindLast(x => x.Contains("LightBug")));
                    float maxAge = creatureTrait.SelfModifiers.Find(x => x.AttributeId.Equals(ageAttributeId)).Value;
                    Light2D light = prefab.GetComponent<Light2D>();
                    float lux = (light?.Lux).GetValueOrDefault()/* * light?.Range*/;
                    float rad = (prefab.GetComponent<RadiationEmitter>()?.emitRads).GetValueOrDefault();
                    ShinebugValues.Add(Name, new ShinebugData(maxAge * 600f, lux, rad));
                }
                return ShinebugValues[Name];
            }
        }

        public ShinebugSimulator(string name, float age = 0.0f)
        {
            Name = name;
            Age = age;
        }

        public bool Simulate(float dt)
        {
            Age += dt;
            return Age > Data.MaxAge;
        }

        public override string ToString() =>
            $"(FakeShinebug) {Name}: {Age}s/{Data.MaxAge}s {Data.Lux} Lux {Data.Rad} Rads";
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
