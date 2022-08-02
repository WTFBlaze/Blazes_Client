using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blaze.API.QM
{
    public class QMToggleButton : QMButtonBase
    {
        protected TextMeshProUGUI btnTextComp;
        protected Button btnComp;
        protected Image btnImageComp;
        protected bool currentState;
        protected Action OnAction;
        protected Action OffAction;

        public QMToggleButton(QMNestedButton location, float btnXPos, float btnYPos, string btnText, Action onAction, Action offAction, string btnToolTip, bool defaultState = false)
        {
            btnQMLoc = location.GetMenuName();
            Initialize(btnXPos, btnYPos, btnText, onAction, offAction, btnToolTip, defaultState);
        }

        public QMToggleButton(string location, float btnXPos, float btnYPos, string btnText, Action onAction, Action offAction, string btnToolTip, bool defaultState = false)
        {
            btnQMLoc = location;
            Initialize(btnXPos, btnYPos, btnText, onAction, offAction, btnToolTip, defaultState);
        }

        private void Initialize(float btnXLocation, float btnYLocation, string btnText, Action onAction, Action offAction, string btnToolTip, bool defaultState)
        {
            btnType = "ToggleButton";
            button = UnityEngine.Object.Instantiate(APIStuff.SingleButtonTemplate(), GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/" + btnQMLoc).transform, true);
            RandomNumb = APIStuff.RandomNumbers();
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 176);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(-68, 796);
            btnTextComp = button.GetComponentInChildren<TextMeshProUGUI>(true);
            btnComp = button.GetComponentInChildren<Button>(true);
            btnComp.onClick = new Button.ButtonClickedEvent();
            btnComp.onClick.AddListener(new Action(HandleClick));
            btnImageComp = button.transform.Find("Icon").GetComponentInChildren<Image>(true);

            initShift[0] = 0;
            initShift[1] = 0;
            SetLocation(btnXLocation, btnYLocation);
            SetButtonText(btnText);
            SetButtonActions(onAction, offAction);
            SetToolTip(btnToolTip);
            SetActive(true);

            currentState = defaultState;
            var tmpIcon = currentState ? APIStuff.GetOnIconSprite() : APIStuff.GetOffIconSprite();
            btnImageComp.sprite = tmpIcon;
            btnImageComp.overrideSprite = tmpIcon;

            BlazesAPI.allQMToggleButtons.Add(this);
        }

        private void HandleClick()
        {
            currentState = !currentState;
            var stateIcon = currentState ? APIStuff.GetOnIconSprite() : APIStuff.GetOffIconSprite();
            btnImageComp.sprite = stateIcon;
            btnImageComp.overrideSprite = stateIcon;
            if (currentState)
            {
                OnAction.Invoke();
            }
            else
            {
                OffAction.Invoke();
            }
        }

        public void SetButtonText(string buttonText)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
        }

        public void SetButtonActions(Action onAction, Action offAction)
        {
            OnAction = onAction;
            OffAction = offAction;
        }

        public void SetToggleState(bool newState, bool shouldInvoke = false)
        {
            try
            {
                var newIcon = newState ? APIStuff.GetOnIconSprite() : APIStuff.GetOffIconSprite();
                btnImageComp.sprite = newIcon;
                btnImageComp.overrideSprite = newIcon;
                currentState = newState;

                if (shouldInvoke)
                {
                    if (newState)
                    {
                        OnAction.Invoke();
                    }
                    else
                    {
                        OffAction.Invoke();
                    }
                }
            }
            catch { }
        }

        public void ClickMe()
        {
            HandleClick();
        }

        public bool GetCurrentState()
        {
            return currentState;
        }
    }
}
