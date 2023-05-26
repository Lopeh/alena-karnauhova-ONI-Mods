using KSerialization;

namespace ShinebugReactor
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ItemsRemovable : KMonoBehaviour
    {
        [MyCmpReq]
        protected Storage storage;
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
            UpdateStorageModifiers();
        }
    }
}