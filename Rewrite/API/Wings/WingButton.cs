using UnityEngine;
using UnityEngine.UI;

namespace Blaze.API.Wings
{
    public class WingButton
    {
        public BaseWing wing;
        public TMPro.TextMeshProUGUI text;
        public Transform transform;

        public WingButton(BaseWing wing, string name, Transform parent, int pos, System.Action onClick)
        {
            this.wing = wing;

            transform = UnityEngine.Object.Instantiate(wing.ProfileButton, parent);
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(420, 144);
            transform.transform.localPosition = new Vector3(0, pos, transform.transform.localPosition.z);

            (text = transform.GetComponentInChildren<TMPro.TextMeshProUGUI>()).text = name;

            Button button = transform.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(onClick);
        }

        public WingButton(WingPage page, string name, int index, System.Action onClick)
        {
            wing = page.wing;

            Transform transform = UnityEngine.Object.Instantiate(wing.ProfileButton, page.transform);
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(420, 144);
            transform.transform.localPosition = new Vector3(0, 320 - (index * 120), transform.transform.localPosition.z);

            (text = transform.GetComponentInChildren<TMPro.TextMeshProUGUI>()).text = name;

            Button button = transform.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(onClick);
        }
    }
}
