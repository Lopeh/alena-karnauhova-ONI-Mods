using HarmonyLib;
using MonoMod.Utils;
using System;
using UnityEngine;
using static HarmonyLib.AccessTools;

namespace Utils
{
    [SkipSaveFileSerialization]
    public class SolidConduitFilteredConsumer : SolidConduitConsumer
    {
        //[MyCmpGet]
        protected IUserControlledCapacity capacityControl;
        [MyCmpGet]
        protected TreeFilterable treeFilterable;
        [MyCmpReq]
        protected Operational operational;
        protected Traverse<int> utilityCellField;
        protected Traverse<bool> consumingField;
        protected Func<SolidConduitFlow> getConduitFlow;
        protected Func<int> getConnectedNetworkID;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            capacityControl = GetComponent<IUserControlledCapacity>();
            utilityCellField = Traverse.Create(this).Field<int>("utilityCell");
            consumingField = Traverse.Create(this).Field<bool>("consuming");
            getConduitFlow = Method(typeof(SolidConduitConsumer), "GetConduitFlow")
                .CreateDelegate<Func<SolidConduitFlow>>(this);
            getConnectedNetworkID = Method(typeof(SolidConduitConsumer), "GetConnectedNetworkID")
                .CreateDelegate<Func<int>>(this);
        }
        protected override void OnSpawn()
        {
            base.OnSpawn();
            Action<float> baseConduitUpdate = Method(typeof(SolidConduitConsumer), "ConduitUpdate")
                .CreateDelegate<Action<float>>(this);
            getConduitFlow().RemoveConduitUpdater(baseConduitUpdate);
            getConduitFlow().AddConduitUpdater(ConduitUpdate, ConduitFlowPriority.Default);
        }

        protected override void OnCleanUp()
        {
            getConduitFlow().RemoveConduitUpdater(ConduitUpdate);
            base.OnCleanUp();
        }
        protected void ConduitUpdate(float dt)
        {
            bool consumed = false;
            if (IsConnected)
            {
                SolidConduitFlow conduitFlow = getConduitFlow();
                SolidConduitFlow.ConduitContents contents = conduitFlow.GetContents(utilityCellField.Value);
                if (contents.pickupableHandle.IsValid() && (alwaysConsume || operational.IsOperational))
                {
                    float occupiedAmount;
                    float capacity;
                    if (capacityControl != null)
                    {
                        occupiedAmount = capacityControl.AmountStored;
                        capacity = capacityControl.UserMaxCapacity;
                    }
                    else {
                        occupiedAmount = capacityTag != GameTags.Any ? storage.GetMassAvailable(capacityTag) : storage.MassStored();
                        capacity = Mathf.Min(storage.capacityKg, capacityKG);
                    }
                    float spaceAvailable = Mathf.Max(0.0f, capacity - occupiedAmount);
                    if (spaceAvailable > 0.0f)
                    {
                        Pickupable pickupable1 = conduitFlow.GetPickupable(contents.pickupableHandle);
                        bool canConsume = true;
                        canConsume = (capacityControl != null) ?
                            pickupable1.TotalAmount <= spaceAvailable || pickupable1.TotalAmount > capacity
                            : pickupable1.PrimaryElement.Mass <= spaceAvailable || pickupable1.PrimaryElement.Mass > capacity;
                        if (treeFilterable)
                        {
                            canConsume &= treeFilterable.ContainsTag(pickupable1.PrefabID());
                        }
                        if (canConsume)
                        {
                            Pickupable pickupable2 = conduitFlow.RemovePickupable(utilityCellField.Value);
                            if (pickupable2)
                            {
                                storage.Store(pickupable2.gameObject, true);
                                consumed = true;
                            }
                        }
                    }
                }
            }
            if (storage != null)
                storage.storageNetworkID = getConnectedNetworkID();
            consumingField.Value = consumed;
        }
    }
}