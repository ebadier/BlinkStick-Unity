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

namespace BlinkStick.Unity
{
	/// <summary>
	/// Works with the first BlinkStick device connected to this system.
	/// </summary>
	public sealed class BlinkStick
	{
		public bool Connected { get { return _blinkStick != null; } }

		private BlinkStickDotNet.BlinkStick _blinkStick = null;

		public bool Connect()
		{
			if(!Connected)
			{
				try
				{
					// Connect to the first plugged BlinkStick.
					_blinkStick = new BlinkStickDotNet.BlinkStick();
				}
				catch
				{
					// No device found.
					_blinkStick = null;
				}
			}
			return Connected;
		}

		public void Disconnect()
		{
			if(Connected)
			{
				_blinkStick.Dispose();
				_blinkStick = null;
			}
		}

		public void SetColor(Color32 pColor)
		{
			if (Connected)
			{
				_blinkStick.LedColor = System.Drawing.Color.FromArgb(pColor.a, pColor.r, pColor.g, pColor.b);
			}
		}

		public Color32 GetColor()
		{
			Color32 res = Color.black;
			if (Connected)
			{
				System.Drawing.Color color = _blinkStick.LedColor;
				res.a = color.A;
				res.r = color.R;
				res.g = color.G;
				res.b = color.B;
			}
			return res;
		}
	}
}