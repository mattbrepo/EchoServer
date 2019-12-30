﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EchoServer
{
	/// <summary>
	/// Extension comoda per l'uso in multithreading
	/// </summary>
	public static class ControlExtensions
	{
		public static void Do<TControl>(this TControl control, Action<TControl> action) where TControl : Control
		{
			if (control.InvokeRequired) control.Invoke(action, control);
			else action(control);
		}
	}

	class Util
	{


	}
}
