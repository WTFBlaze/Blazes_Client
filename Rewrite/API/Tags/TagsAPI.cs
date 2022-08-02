using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using VRC;

namespace Blaze.API.Tags
{
    public class Tag
    {
        protected Player target;
        protected GameObject tagObj;
        protected GameObject textObj;
        protected TextMeshProUGUI textComp;
        protected ImageThreeSlice colorComp;

        public Tag(Player player, string tagText, int position, Color tagColor)
        {
            Initialize(player, tagText, position, tagColor);
        }

        private void Initialize(Player player, string tagText, int position, Color tagColor)
        {
            tagObj = UnityEngine.Object.Instantiate(player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Quick Stats").gameObject, player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents"), false);
            tagObj.name = $"{BlazesAPI.Identifier}-{APIStuff.RandomNumbers()}";
            target = player;
            tagObj.transform.localPosition = new Vector3(0f, 30 * position, 0f);
            tagObj.gameObject.SetActive(true);
            colorComp = tagObj.GetComponent<ImageThreeSlice>();
            colorComp.color = tagColor;
            for (var i = tagObj.transform.childCount; i > 0; i--)
            {
                var child = tagObj.transform.GetChild(i - 1);
                if (child.name == "Trust Text")
                {
                    textObj = child.gameObject;
                    textComp = textObj.GetComponent<TextMeshProUGUI>();
                    textComp.color = Color.white;
                    textComp.text = tagText;
                }
                else UnityEngine.Object.Destroy(child.gameObject);
            }
            BlazesAPI.allTags.Add(this);
        }
    }
}
