using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace SizeNotIncluded
{
	[Serializable]
	public class Options : SingletonOptions<Options>, IOptions
	{
		public static readonly Dictionary<SizeNotIncludedMapType, SizeNotIncludedMapConfig> MapConfig = new Dictionary<SizeNotIncludedMapType, SizeNotIncludedMapConfig>()
		{
            {
				SizeNotIncludedMapType.Smallest,
				new SizeNotIncludedMapConfig(new Vector2I(4, 6), true)
            },
            {
				SizeNotIncludedMapType.Small,
				new SizeNotIncludedMapConfig(new Vector2I(5, 7), true)
            },
            {
				SizeNotIncludedMapType.Medium,
				new SizeNotIncludedMapConfig(new Vector2I(6, 9), true)
            },
            {
				SizeNotIncludedMapType.Large,
				new SizeNotIncludedMapConfig(new Vector2I(7, 10), false)
            },
            {
				SizeNotIncludedMapType.Default,
				new SizeNotIncludedMapConfig(new Vector2I(8, 12), false)
            },
            {
				SizeNotIncludedMapType.SmallestTall,
				new SizeNotIncludedMapConfig(new Vector2I(3, 7), true)
            },
            {
				SizeNotIncludedMapType.SmallTall,
				new SizeNotIncludedMapConfig(new Vector2I(4, 9), true)
            },
            {
				SizeNotIncludedMapType.MediumTall,
				new SizeNotIncludedMapConfig(new Vector2I(5, 10), true)
            },
            {
				SizeNotIncludedMapType.LargeTall,
				new SizeNotIncludedMapConfig(new Vector2I(6, 12), false)
            },
            {
				SizeNotIncludedMapType.DefaultTall,
				new SizeNotIncludedMapConfig(new Vector2I(7, 13), false)
            },
		};

		public const float MaxDensity = 2.5f;

		[Option("STRINGS.OPTIONS.MAPTYPE.NAME", "STRINGS.OPTIONS.MAPTYPE.DESC")]
		[JsonProperty]
		public SizeNotIncludedMapType MapType { get; set; } = SizeNotIncludedMapType.Smallest;

		[Option("STRINGS.OPTIONS.GEYSERCOUNT.NAME", "STRINGS.OPTIONS.GEYSERCOUNT.DESC")]
        [Limit(0, 20)]
		[JsonProperty]
		public int GeyserCount { get; set; } = 12;

		public bool IsSmall => MapConfig[MapType].IsSmall;
		public int XSize => MapConfig[MapType].XSize;
		public int YSize => MapConfig[MapType].YSize;
		public float XScale => MapConfig[SizeNotIncludedMapType.Default].MapRatio.x / (float)MapConfig[MapType].MapRatio.x;
		public float YScale => MapConfig[SizeNotIncludedMapType.Default].MapRatio.y / (float)MapConfig[MapType].MapRatio.y;
		public float Density => XScale * YScale;
		public float DensityCapped => Math.Min(Density, MaxDensity);

		public float NeutroniumBorder(ProcGenGame.Border __instance)
		{
			return Math.Min(__instance.width, MapConfig[MapType].NeutroniumBorder);
		}

		// smaller biome border so it doesn't take up most of the map
		public float BiomeBorder(ProcGenGame.Border __instance)
		{
			return Math.Min(__instance.width, MapConfig[MapType].BiomeBorder);
		}

		public override string ToString()
		{
			return $"SizeNotIncludedOptions[mode={MapType}]";
		}

		public IEnumerable<IOptionsEntry> CreateOptions() => null;

        public void OnOptionsChanged()
        {
			instance = POptions.ReadSettings<Options>() ?? new Options();
        }

        public enum SizeNotIncludedMapType : byte
		{
			[Option("STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.SMALLEST.NAME", "STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.SMALLEST.DESC")]
			Smallest,
			[Option("STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.SMALL.NAME", "STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.SMALL.DESC")]
			Small,
			[Option("STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.MEDIUM.NAME", "STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.MEDIUM.DESC")]
			Medium,
			[Option("STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.LARGE.NAME", "STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.LARGE.DESC")]
			Large,
			[Option("STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.DEFAULT.NAME", "STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.DEFAULT.DESC")]
			Default,
			[Option("STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.SMALLESTTALL.NAME", "STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.SMALLESTTALL.DESC")]
			SmallestTall,
			[Option("STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.SMALLTALL.NAME", "STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.SMALLTALL.DESC")]
			SmallTall,
			[Option("STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.MEDIUMTALL.NAME", "STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.MEDIUMTALL.DESC")]
			MediumTall,
			[Option("STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.LARGETALL.NAME", "STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.LARGETALL.DESC")]
			LargeTall,
			[Option("STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.DEFAULTTALL.NAME", "STRINGS.OPTIONS.SIZENOTINCLUDEDMAPTYPE.DEFAULTTALL.DESC")]
			DefaultTall,
		}
		public readonly struct SizeNotIncludedMapConfig
        {
			public readonly Vector2I MapRatio;
			public readonly bool IsSmall;

			public const int ChunkSize = 32;

			public SizeNotIncludedMapConfig(Vector2I ratio, bool isSmall)
            {
				MapRatio = ratio;
				IsSmall = isSmall;
            }

			public int XSize => ChunkSize * MapRatio.x;
			public int YSize => ChunkSize * MapRatio.y;
			public float NeutroniumBorder => IsSmall ? 1.5f : float.MaxValue;
			public float BiomeBorder => IsSmall ? 1.5f : float.MaxValue;
        }
	}
}
