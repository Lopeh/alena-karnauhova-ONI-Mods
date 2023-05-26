using System;
using UnityEngine;
using PeterHan.PLib.UI;

namespace Utils
{
    public class ItemsRemovableSideScreen : SideScreenContent
    {
        protected MultiToggle allowRemoveItemsToggle;
        protected ItemsRemovable targetItemsRemovable;

        public void RefreshUI()
        {
            int state = targetItemsRemovable.AllowItemRemoval ?
                PCheckBox.STATE_CHECKED : PCheckBox.STATE_UNCHECKED;
            allowRemoveItemsToggle.ChangeState(state);
        }

        protected IUIComponent CreateUI()
        {
            PCheckBox checkBox = new PCheckBox("ItemsRemovableCheckbox").AddOnRealize(s =>
            {
                allowRemoveItemsToggle = s.GetComponent<MultiToggle>();
                RefreshUI();
            });
            checkBox.CheckSize = new Vector2(26, 26);
            checkBox.Text = STRINGS.UI.UISIDESCREENS.ITEMSREMOVABLE_SIDE_SCREEN.ALLOWREMOVALBUTTON;
            checkBox.TextStyle = PUITuning.Fonts.TextDarkStyle;
            checkBox.ToolTip = STRINGS.UI.UISIDESCREENS.ITEMSREMOVABLE_SIDE_SCREEN.ALLOWREMOVALBUTTONTOOLTIP;
            checkBox.OnChecked += (s, state) =>
            {
                targetItemsRemovable.AllowItemRemoval = !targetItemsRemovable.AllowItemRemoval;
                RefreshUI();
            };
            return checkBox;
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            BoxLayoutGroup layoutGroup = gameObject.GetComponent<BoxLayoutGroup>();
            if (layoutGroup)
            {
                layoutGroup.Params = new BoxLayoutParams()
                {
                    Alignment = TextAnchor.MiddleLeft,
                    Margin = new RectOffset(16, 16, 4, 4),
                    //Spacing = 8f,
                };
                layoutGroup.Params.Alignment = TextAnchor.MiddleLeft;
            }
            CreateUI().AddTo(gameObject, 0);
            ContentContainer = gameObject;
        }

        public override bool IsValidForTarget(GameObject target)
        {
            return target.GetComponent<ItemsRemovable>() != null;
        }

        public override void SetTarget(GameObject target)
        {
            base.SetTarget(target);
            if (target != null)
            {
                targetItemsRemovable = target.GetComponent<ItemsRemovable>();
                //RefreshUI();
            }
        }
    }
}