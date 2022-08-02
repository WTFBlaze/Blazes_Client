using Blaze.Utils.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Blaze.API.QM
{
    public class QMButtonBase
    {
        protected GameObject button;
        protected string btnQMLoc;
        protected string btnType;
        protected string btnTag;
        protected int[] initShift = { 0, 0 };
        protected Color OrigBackground;
        protected Color OrigText;
        protected int RandomNumb;

        public GameObject GetGameObject()
        {
            return button;
        }

        public void SetActive(bool state)
        {
            button.gameObject.SetActive(state);
        }

        public void SetLocation(float buttonXLoc, float buttonYLoc)
        {
            button.GetComponent<RectTransform>().anchoredPosition += Vector2.right * (232 * (buttonXLoc + initShift[0]));
            button.GetComponent<RectTransform>().anchoredPosition += Vector2.down * (210 * (buttonYLoc + initShift[1]));

            btnTag = "(" + buttonXLoc + "," + buttonYLoc + ")";
            button.name = btnQMLoc + "/" + btnType + btnTag;
            button.GetComponent<Button>().name = $"{BlazesAPI.Identifier}-{btnType}{btnTag}";
        }

        public void SetToolTip(string buttonToolTip)
        {
            button.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_0 = buttonToolTip;
            button.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_1 = buttonToolTip;
        }

        public void DestroyMe()
        {
            try
            {
                UnityEngine.Object.Destroy(button);
            }
            catch { }
        }

        internal virtual void SetTextColor(Color buttonTextColor, bool save = true) { }
    }
}
