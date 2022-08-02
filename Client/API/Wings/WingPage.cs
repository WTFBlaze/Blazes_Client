using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Blaze.API.Wings
{
    public class WingPage
    {
        public BaseWing wing;
        public Transform transform;
        public TMPro.TextMeshProUGUI text;
        public Button closeButton;
        public Button openButton;

        public WingPage(BaseWing wing, string name)
        {
            this.wing = wing;

            transform = UnityEngine.Object.Instantiate(wing.ProfilePage, wing.WingPages);
            Transform content = transform.Find("ScrollRect/Viewport/VerticalLayoutGroup");
            transform.gameObject.SetActive(false);

            for (int i = 0; i < content.GetChildCount(); i++)
                UnityEngine.Object.Destroy(content.GetChild(i).gameObject);
            transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = name;

            closeButton = transform.GetComponentInChildren<Button>();
            closeButton.onClick = new Button.ButtonClickedEvent();
            closeButton.onClick.AddListener(new System.Action(() =>
            {
                transform.gameObject.SetActive(false);
                wing.openedPages.RemoveAt(wing.openedPages.Count - 1);
                if (wing.openedPages.Count > 0)
                {
                    WingPage prev = wing.openedPages[wing.openedPages.Count - 1];
                    prev.transform.gameObject.SetActive(true);
                }
                else wing.WingMenu.gameObject.SetActive(true);
            }));

            Transform open = UnityEngine.Object.Instantiate(wing.ProfileButton, wing.WingButtons);
            (text = open.GetComponentInChildren<TMPro.TextMeshProUGUI>()).text = name;

            openButton = open.GetComponent<Button>();
            openButton.onClick = new Button.ButtonClickedEvent();
            openButton.onClick.AddListener(new System.Action(() => {
                transform.gameObject.SetActive(true);
                wing.openedPages.Add(this);
                if (wing.openedPages.Count > 1)
                {
                    WingPage prev = wing.openedPages[wing.openedPages.Count - 2];
                    prev.transform.gameObject.SetActive(false);
                }
                else wing.WingMenu.gameObject.SetActive(false);
            }));
        }

        public WingPage(WingPage page, string name, int index)
        {
            wing = page.wing;

            transform = UnityEngine.Object.Instantiate(wing.ProfilePage, wing.WingPages);
            Transform content = transform.Find("ScrollRect/Viewport/VerticalLayoutGroup");
            transform.gameObject.SetActive(false);

            for (int i = 0; i < content.GetChildCount(); i++)
                UnityEngine.Object.Destroy(content.GetChild(i).gameObject);
            transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = name;

            closeButton = transform.GetComponentInChildren<Button>();
            closeButton.onClick = new Button.ButtonClickedEvent();
            closeButton.onClick.AddListener(new System.Action(() =>
            {
                transform.gameObject.SetActive(false);
                wing.openedPages.RemoveAt(wing.openedPages.Count - 1);
                if (wing.openedPages.Count > 0)
                {
                    WingPage prev = wing.openedPages[wing.openedPages.Count - 1];
                    prev.transform.gameObject.SetActive(true);
                }
                else wing.WingMenu.gameObject.SetActive(true);
            }));

            Transform open = UnityEngine.Object.Instantiate(wing.ProfileButton, page.transform);
            (text = open.GetComponentInChildren<TMPro.TextMeshProUGUI>()).text = name;

            openButton = open.GetComponent<Button>();
            openButton.GetComponent<RectTransform>().sizeDelta = new Vector2(420, 144);
            openButton.transform.localPosition = new Vector3(0, 320 - (index * 120), transform.transform.localPosition.z);
            openButton.onClick = new Button.ButtonClickedEvent();
            openButton.onClick.AddListener(new System.Action(() =>
            {
                transform.gameObject.SetActive(true);
                wing.openedPages.Add(this);
                if (wing.openedPages.Count > 1)
                {
                    WingPage prev = wing.openedPages[wing.openedPages.Count - 2];
                    prev.transform.gameObject.SetActive(false);
                }
                else wing.WingMenu.gameObject.SetActive(false);
            }));
        }
    }
}
