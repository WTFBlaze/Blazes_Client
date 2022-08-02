using Blaze.Utils.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;

namespace Blaze.API.SM
{
    internal class SMList
    {
        protected GameObject GameObject;
        protected UiVRCList UiVRCList;
        protected Text Text;
        private static GameObject PublicAvatarList = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Vertical Scroll View/Viewport/Content/Public Avatar List");

        public SMList(Transform parent, string name, int Position = 0)
        {
            Initialize(parent, name, Position);
        }

        private void Initialize(Transform parent, string name, int Position = 0)
        {
            GameObject = UnityEngine.Object.Instantiate(PublicAvatarList.gameObject, parent);
            GameObject.GetComponent<UiAvatarList>().field_Public_Category_0 = UiAvatarList.Category.SpecificList;
            UiVRCList = GameObject.GetComponent<UiVRCList>();
            Text = GameObject.transform.Find("Button").GetComponentInChildren<Text>();
            GameObject.transform.SetSiblingIndex(Position);

            UiVRCList.clearUnseenListOnCollapse = false;
            UiVRCList.usePagination = false;
            UiVRCList.hideElementsWhenContracted = false;
            UiVRCList.hideWhenEmpty = false;
            UiVRCList.field_Protected_Dictionary_2_Int32_List_1_ApiModel_0.Clear();
            UiVRCList.pickerPrefab.transform.Find("TitleText").GetComponent<Text>().supportRichText = true;

            GameObject.SetActive(true);
            //GameObject.name = $"{BlazesAPI.Identifier}-{name}";
            GameObject.name = name;
            Text.supportRichText = true;
            Text.text = name;
            BlazesAPI.allSMLists.Add(this);
        }

        public void RenderElement(Il2CppSystem.Collections.Generic.List<ApiAvatar> AvatarList)
        {
            UiVRCList.Method_Protected_Void_List_1_T_Int32_Boolean_VRCUiContentButton_0(AvatarList);
        }

        public GameObject GetGameObject()
        {
            return GameObject;
        }

        public UiVRCList GetUiVRCList()
        {
            return UiVRCList;
        }

        public Text GetText()
        {
            return Text;
        }

        public void DestroyMe()
        {
            try
            {
                UnityEngine.Object.Destroy(GameObject);
            }
            catch { }
        }
    }
}
