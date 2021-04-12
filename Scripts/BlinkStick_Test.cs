/******************************************************************************************************************************************************
* MIT License																																		  *
*																																					  *
* Copyright (c) 2021																																  *
* Emmanuel Badier <emmanuel.badier@gmail.com>																										  *
* 																																					  *
* Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),  *
* to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,  *
* and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:		  *
* 																																					  *
* The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.					  *
* 																																					  *
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, *
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 																							  *
* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 		  *
* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.							  *
******************************************************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace BlinkStick.Unity
{
	/// <summary>
	/// Test to set BlinkStick's color as the the color picked with mouse cursor.
	/// </summary>
	public sealed class BlinkStick_Test : MonoBehaviour
	{
		[Tooltip("If having multiple BlinkStick, you can specify the BlinkStick to connect to.")]
		public string blinkStickSerialNumber = "";
		[Tooltip("To Debug BlinkStick.GetColor()")]
		public bool logBlinkStickColor = false;
		public Button blinkStickConnect_Button;
		public GameObject colorPickerParent;
		public CUIColorPicker colorPicker;
		
		private Text _blinkStickConnect_Button_Text;
		private readonly BlinkStick _blinkStick = new BlinkStick();

		void Awake()
		{
			_blinkStickConnect_Button_Text = blinkStickConnect_Button.GetComponentInChildren<Text>(true);
			_OnBlinkStickDisconnected(false);

			colorPicker.ColorChanged += _SetBlinkStickColor;
			blinkStickConnect_Button.onClick.AddListener(_OnBlinkStickConnect_ButtonClick);
		}

		void OnDestroy()
		{
			_blinkStick.Disconnect();
		}

		private void _SetBlinkStickColor(Color pColor)
		{
			_blinkStick.SetColor(pColor);
			if(logBlinkStickColor)
			{
				Debug.Log("[BlinkStick] Color : " + _blinkStick.GetColor());
			}
		}

		private void _OnBlinkStickConnect_ButtonClick()
		{
			if(_blinkStick.Connected)
			{
				_blinkStick.Disconnect();
				_OnBlinkStickDisconnected(true);
			}
			else if(_blinkStick.Connect(blinkStickSerialNumber))
			{
				_OnBlinkStickConnected();
			}
		}

		private void _OnBlinkStickConnected()
		{
			colorPickerParent.SetActive(true);
			_blinkStickConnect_Button_Text.text = "Disconnect";
			_SetBlinkStickColor(colorPicker.Color);
			Debug.Log("[BlinkStick] connected : " + _blinkStick.ToString(true));
		}

		private void _OnBlinkStickDisconnected(bool pLog)
		{
			colorPickerParent.SetActive(false);
			_blinkStickConnect_Button_Text.text = "Connect";
			if(pLog)
			{
				Debug.Log("[BlinkStick] disconnected.");
			}
		}
	}
}