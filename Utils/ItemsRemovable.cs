using KSerialization;

//namespace Utils
//{
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
//}

/*
public class Automatable : KMonoBehaviour
{
  [Serialize]
  private bool automationOnly = true;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<Automatable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Automatable>((System.Action<Automatable, object>) ((component, data) => component.OnCopySettings(data)));

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<Automatable>(-905833192, Automatable.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    Automatable component = ((GameObject) data).GetComponent<Automatable>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    this.automationOnly = component.automationOnly;
  }

  public bool GetAutomationOnly() => this.automationOnly;

  public void SetAutomationOnly(bool only) => this.automationOnly = only;

  public bool AllowedByAutomation(bool is_transfer_arm) => !this.GetAutomationOnly() | is_transfer_arm;
}
*/