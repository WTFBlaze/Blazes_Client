using UnityEngine;

namespace Blaze.Utils.VRChat
{
    public static class InputUtils
    {
		public static bool GetKeyDown(KeyCode key, bool control = false, bool shift = false)
		{
			bool flag = !control;
			bool flag2 = !shift;
			if (control && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
			{
				flag = true;
			}
			if (shift && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
			{
				flag2 = true;
			}
			return flag && flag2 && Input.GetKeyDown(key);
		}

		public static float GetAxis(string axis, bool control = false, bool shift = false)
		{
			bool flag = !control;
			bool flag2 = !shift;
			if (control && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
			{
				flag = true;
			}
			if (shift && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
			{
				flag2 = true;
			}
			float result;
			if (flag && flag2)
			{
				result = Input.GetAxis(axis);
			}
			else
			{
				result = 0f;
			}
			return result;
		}

		public static bool GetMouseButtonDown(int button, bool control = false, bool shift = false)
		{
			bool flag = !control;
			bool flag2 = !shift;
			if (control && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
			{
				flag = true;
			}
			if (shift && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
			{
				flag2 = true;
			}
			return flag && flag2 && Input.GetMouseButtonDown(button);
		}
	}
}
