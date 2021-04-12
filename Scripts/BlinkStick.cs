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

using HidLibrary;
using System.Linq;
using UnityEngine;

namespace BlinkStick.Unity
{
	public struct BlinkStickInfos
    {
        public readonly string deviceVendorID;
        public readonly string deviceProductID;
        public readonly string deviceVersion;
		public readonly string deviceSerialNumber;
        public readonly string deviceDescription;
        public readonly string devicePath;

        public BlinkStickInfos(HidDevice pDevice)
        {
            deviceVendorID = pDevice.Attributes.VendorHexId;
            deviceProductID = pDevice.Attributes.ProductHexId;
            deviceVersion = pDevice.Attributes.Version.ToString();
            BlinkStick.TryGetSerialNumber(pDevice, out deviceSerialNumber);
            deviceDescription = pDevice.Description;
            devicePath = pDevice.DevicePath;
		}

        public string ToString(bool pShort)
		{
            if(pShort)
			{
                return string.Format("VendorID={0}, ProductID={1}, Version={2}, SN={3}",
                deviceVendorID, deviceProductID, deviceVersion, deviceSerialNumber);
            }
            else
			{
                return string.Format("VendorID={0}, ProductID={1}, Version={2}, SN={3}, Description={4}, Path={5}",
               deviceVendorID, deviceProductID, deviceVersion, deviceSerialNumber, deviceDescription, devicePath);
            }
		}
    }

	/// <summary>
	/// To use BlinkStick devices (https://blinkstick.com/) in Unity.
	/// </summary>
	public sealed class BlinkStick
	{
        public const int VendorID = 0X20A0; // 8352
        public const int ProductID = 0x41E5; // 16969
        public static readonly Color32 OffColor = Color.black;

        /// <summary>
        /// Is there a BlinkStick device connected ?
        /// </summary>
        public bool Connected { get { return _stickDevice != null; } }

        private HidDevice _stickDevice = null;
        private BlinkStickInfos? _stickInfos = null;

        /// <summary>
        /// Destructor.
        /// Ensure resources are released.
        /// </summary>
        ~BlinkStick()
		{
            Disconnect();
		}

        public string ToString(bool pShort)
		{
            string msg;
            if (Connected)
            {
                msg = _stickInfos.Value.ToString(pShort);
            }
            else
            {
                msg = "null device";
            }
            return msg;
        }

        /// <summary>
        /// Connect to the BlinkStick device with the given serial number.
        /// Returns the first available device if serial number is not specified (null or empty).
        /// </summary>
        /// <param name="pSerialNumber">The BlinkStick device serial number</param>
        public bool Connect(string pSerialNumber = "")
        {
            if (!Connected)
			{
                HidDevice[] devices = HidDevices.Enumerate(VendorID, ProductID).ToArray();
                if (devices.Length > 0)
                {
                    if(string.IsNullOrEmpty(pSerialNumber))
					{
                        // Get the first available device if serial number not specified.
                        _stickDevice = devices[0];
                    }
                    else
					{
                        // Get the device with the given serial number.
                        string sn;
                        foreach (HidDevice device in devices)
                        {
                            if (TryGetSerialNumber(device, out sn) && (pSerialNumber == sn))
                            {
                                _stickDevice = device;
                                break;
                            }
                        }
                    }

                    // Create BlinkStickInfos only if there is a device connected.
                    if (Connected)
					{
                        _stickInfos = new BlinkStickInfos(_stickDevice);
                    }
                }
            }
            return Connected;
        }

        /// <summary>
        /// Disconnect any connected BlinkStick device.
        /// </summary>
        public void Disconnect()
        {
            if (Connected)
            {
                SetColor(OffColor);
                _stickDevice.Dispose();
                _stickDevice = null;
                _stickInfos = null;
            }
        }

        /// <summary>
        /// Get the color of the connected BlinkStick device.
        /// </summary>
        public Color32 GetColor()
        {
            Color32 color = OffColor;
            if (Connected)
            {
                byte[] data;
                if(_stickDevice.ReadFeatureData(out data, 1))
				{
                    color.r = data[1];
                    color.g = data[2];
                    color.b = data[3];
                }
            }
            return color;
        }

        /// <summary>
        /// Set the color of the connected BlinkStick device.
        /// </summary>
        public void SetColor(Color32 pColor)
        {
            if (Connected)
            {
                _stickDevice.WriteFeatureData(new byte[] { 1, pColor.r, pColor.g, pColor.b });
            }
        }

        internal static bool TryGetSerialNumber(HidDevice pDevice, out string pSN)
		{
            pSN = "";
            byte[] data;
            bool success = pDevice.ReadSerialNumber(out data);
            if(success)
			{
                pSN = System.BitConverter.ToString(data).Replace("-00", "");
            }
            return success;
        }
    }
}