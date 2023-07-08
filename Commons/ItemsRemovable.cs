using KSerialization;
using UnityEngine;

namespace Commons
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ItemsRemovable : KMonoBehaviour
    {
        [MyCmpReq]
        protected readonly Storage storage;
        [MyCmpAdd]
        private readonly CopyBuildingSettings copyBuildingSettings;
        private static readonly EventSystem.IntraObjectHandler<ItemsRemovable> OnCopySettingsDelegate
            = new EventSystem.IntraObjectHandler<ItemsRemovable>((component, data) => component.OnCopySettings(data));

        [Serialize]
        private bool allowItemRemoval = true;
        public bool AllowItemRemoval
        {
            get => allowItemRemoval;
            set
            {
                allowItemRemoval = value;
                UpdateStorageModifiers();
            }
        }

        public void UpdateStorageModifiers()
        {
            storage.allowItemRemoval = allowItemRemoval;
            storage.RenotifyAll();
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
            UpdateStorageModifiers();
        }

        protected void OnCopySettings(object data)
        {
            ItemsRemovable component = ((GameObject)data).GetComponent<ItemsRemovable>();
            if (component == null) return;
            AllowItemRemoval = component.AllowItemRemoval;
        }
    }
}