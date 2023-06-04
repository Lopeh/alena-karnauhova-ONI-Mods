using System;
using UnityEngine;
using PeterHan.PLib.UI;

//namespace Utils
//{
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
                if (targetItemsRemovable)
                {
                    RefreshUI();
                }
            });
            checkBox.CheckSize = new Vector2(26, 26);
            checkBox.Text = Utils.STRINGS.UI.UISIDESCREENS.ITEMSREMOVABLE_SIDE_SCREEN.ALLOWREMOVALBUTTON;
            checkBox.TextStyle = PUITuning.Fonts.TextDarkStyle;
            checkBox.ToolTip = Utils.STRINGS.UI.UISIDESCREENS.ITEMSREMOVABLE_SIDE_SCREEN.ALLOWREMOVALBUTTONTOOLTIP;
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
                layoutGroup.Params.Alignment = TextAnchor.MiddleLeft;
                layoutGroup.Params.Margin = new RectOffset(16, 16, 4, 4);
                //layoutGroup.Params.Spacing = 8f;
            }
            CreateUI().AddTo(gameObject, -1);
            ContentContainer = gameObject;
        }

        public override bool IsValidForTarget(GameObject target)
        {
            return target.GetComponent<ItemsRemovable>() != null;
        }

        public override void SetTarget(GameObject target)
        {
            base.SetTarget(target);
            if (target)
            {
                targetItemsRemovable = target.GetComponent<ItemsRemovable>();
                if (allowRemoveItemsToggle)
                {
                    RefreshUI();
                }
            }
        }

        public override void ClearTarget()
        {
            targetItemsRemovable = null;
            base.ClearTarget();
        }
    }
//}