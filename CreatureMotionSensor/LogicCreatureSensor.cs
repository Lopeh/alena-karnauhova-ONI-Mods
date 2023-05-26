using HarmonyLib;
using KSerialization;
using STRINGS;
using System.Collections.Generic;

namespace Creature_Motion_Sensor
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class LogicCreatureSensor : Switch, IIntSliderControl, ISim1000ms, ISim200ms
    {
        [Serialize]
        public int pickupRange = 5;
        private readonly List<Pickupable> creatures = new List<Pickupable>();
        private List<int> reachableCells = new List<int>(100);
        [MyCmpGet]
        private KSelectable selectable;
        [MyCmpGet]
        private Rotatable rotatable;
        private bool wasOn;
        private HandleVector<int>.Handle pickupablesChangedEntry;
        private bool pickupablesDirty;
        private Extents pickupableExtents;
        [MyCmpGet]
        private readonly StationaryChoreRangeVisualizer choreRangeVisualizer;

        public string SliderTitleKey => "STRINGS.UI.STARMAP.ROCKETSTATS.TOTAL_RANGE";

        public string SliderUnits => UI.UNITSUFFIXES.DISTANCE.METER;

        public int SliderDecimalPlaces(int index) => 0;

        public float GetSliderMin(int index) => 3f;

        public float GetSliderMax(int index) => 21f;

        public float GetSliderValue(int index) => pickupRange;

        public void SetSliderValue(float percent, int index)
        {
            int num = (int)((percent + 1f) / 2f) * 2 - 1;
            if (pickupRange == num)
                return;
            pickupRange = num;
            RefreshReachableCells();
            RefreshVisualCells();
        }

        public string GetSliderTooltipKey(int index) => string.Format(STRINGS.UI.UISIDESCREENS.LOGICCREATURESENSORSIDESCREEN.TOOLTIP, pickupRange);

        public string GetSliderTooltip() => string.Format(STRINGS.UI.UISIDESCREENS.LOGICCREATURESENSORSIDESCREEN.TOOLTIP, pickupRange);

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            simRenderLoadBalance = true;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            OnToggle += OnSwitchToggled;
            UpdateLogicCircuit();
            UpdateVisualState(true);
            RefreshReachableCells();
            wasOn = switchedOn;
            RefreshVisualCells();
        }

        protected override void OnCleanUp()
        {
            GameScenePartitioner.Instance.Free(ref pickupablesChangedEntry);
            MinionGroupProber.Get().ReleaseProber(this);
            base.OnCleanUp();
        }

        public void Sim1000ms(float dt)
        {
            RefreshReachableCells();
            if (choreRangeVisualizer.width == pickupRange)
                return;
            RefreshVisualCells();
        }

        public void Sim200ms(float dt) => RefreshPickupables();

        private void RefreshVisualCells()
        {
            choreRangeVisualizer.x = -pickupRange / 2;
            choreRangeVisualizer.y = 0;
            choreRangeVisualizer.width = pickupRange;
            choreRangeVisualizer.height = pickupRange;
            if (selectable.IsSelected)
                Traverse.Create(choreRangeVisualizer).Method("UpdateVisualizers"/*, new object[0]*/).GetValue();
            Vector2I xy = Grid.CellToXY(this.NaturalBuildingCell());
            int cell = Grid.XYToCell(xy.x, xy.y + pickupRange / 2);
            if ((bool)rotatable)
            {
                CellOffset offset = new CellOffset(0, pickupRange / 2);
                CellOffset rotatedCellOffset = rotatable.GetRotatedCellOffset(offset);
                if (Grid.IsCellOffsetValid(this.NaturalBuildingCell(), rotatedCellOffset))
                    cell = Grid.OffsetCell(this.NaturalBuildingCell(), rotatedCellOffset);
            }
            pickupableExtents = new Extents(cell, pickupRange / 2);
            GameScenePartitioner.Instance.Free(ref pickupablesChangedEntry);
            pickupablesChangedEntry = GameScenePartitioner.Instance.Add("CreatureSensor.PickupablesChanged", gameObject, pickupableExtents, GameScenePartitioner.Instance.pickupablesChangedLayer, OnPickupablesChanged);
            pickupablesDirty = true;
        }

        private void RefreshReachableCells()
        {
            ListPool<int, LogicCreatureSensor>.PooledList pooledList = ListPool<int, LogicCreatureSensor>.Allocate(reachableCells);
            reachableCells.Clear();
            int x, y;
            Grid.CellToXY(this.NaturalBuildingCell(), out x, out y);
            int num = x - pickupRange / 2;
            for (int index1 = y; index1 < y + pickupRange + 1; ++index1)
            {
                for (int index2 = num; index2 < num + pickupRange + 1; ++index2)
                {
                    int cell1 = Grid.XYToCell(index2, index1);
                    if ((bool)rotatable)
                    {
                        CellOffset offset = new CellOffset(index2 - x, index1 - y);
                        offset = rotatable.GetRotatedCellOffset(offset);
                        if (Grid.IsCellOffsetValid(this.NaturalBuildingCell(), offset))
                        {
                            int cell2 = Grid.OffsetCell(this.NaturalBuildingCell(), offset);
                            Vector2I xy = Grid.CellToXY(cell2);
                            if (Grid.IsValidCell(cell2) && Grid.IsPhysicallyAccessible(x, y, xy.x, xy.y, true))
                                reachableCells.Add(cell2);
                        }
                    }
                    else if (Grid.IsValidCell(cell1) && Grid.IsPhysicallyAccessible(x, y, index2, index1, true))
                        reachableCells.Add(cell1);
                }
            }
            pooledList.Recycle();
        }

        public bool IsCellReachable(int cell) => reachableCells.Contains(cell);

        private void RefreshPickupables()
        {
            if (!pickupablesDirty) return;
            creatures.Clear();
            ListPool<ScenePartitionerEntry, LogicCreatureSensor>.PooledList pooledList = ListPool<ScenePartitionerEntry, LogicCreatureSensor>.Allocate();
            GameScenePartitioner.Instance.GatherEntries(pickupableExtents.x, pickupableExtents.y, pickupableExtents.width, pickupableExtents.height, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
            int cell = Grid.PosToCell(this);
            for (int index = 0; index < pooledList.Count; ++index)
            {
                Pickupable pickupable = pooledList[index].obj as Pickupable;
                int pickupableCell = GetPickupableCell(pickupable);
                int cellRange = Grid.GetCellRange(cell, pickupableCell);
                if (IsPickupableRelevantToMyInterestsAndReachable(pickupable) && cellRange <= pickupRange)
                    creatures.Add(pickupable);
            }
            SetState(creatures.Count > 0);
            pickupablesDirty = false;
        }

        private void OnPickupablesChanged(object data)
        {
            Pickupable pickupable = data as Pickupable;
            if (!(pickupable && IsPickupableRelevantToMyInterests(pickupable)))
                return;
            pickupablesDirty = true;
        }

        private static bool IsPickupableRelevantToMyInterests(Pickupable pickupable) => pickupable.KPrefabID.HasTag(GameTags.CreatureBrain);

        private bool IsPickupableRelevantToMyInterestsAndReachable(Pickupable pickupable) => IsPickupableRelevantToMyInterests(pickupable) && IsCellReachable(GetPickupableCell(pickupable));

        private static int GetPickupableCell(Pickupable pickupable) => pickupable.cachedCell;

        private void OnSwitchToggled(bool toggled_on)
        {
            UpdateLogicCircuit();
            UpdateVisualState();
        }

        private void UpdateLogicCircuit() => GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, switchedOn ? 1 : 0);

        private void UpdateVisualState(bool force = false)
        {
            if (!(wasOn != switchedOn || force))
                return;
            wasOn = switchedOn;
            KBatchedAnimController component = GetComponent<KBatchedAnimController>();
            component.Play(switchedOn ? "on_pre" : "on_pst");
            component.Queue(switchedOn ? "on" : "off");
        }

        protected override void UpdateSwitchStatus() => selectable.SetStatusItem(Db.Get().StatusItemCategories.Power,
            switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive);
    }
}