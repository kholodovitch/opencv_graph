﻿using System;
using System.Collections.Generic;
using DataStructures;
using Emgu.CV;
using FilterImplementation.Base;

namespace FilterImplementation.Filters.Image
{
	internal class Clone : Filter
	{
		private readonly InputPin _input;
		private readonly OutputPin _originalOutput;
		private readonly List<OutputPin> _outputs = new List<OutputPin>();
		private readonly Property _countProperty;

		public Clone()
		{
			_countProperty = new Property("Count of copies", FilterPropertyType.UInteger);
			_countProperty.OnValueChanged += OnChangedCountOfCopies;
			AddProperty(_countProperty);

			_input = new InputPin("Input", PinMediaType.Image);
			AddPin(_input);
			_originalOutput = new OutputPin("Original", PinMediaType.Image);
			AddPin(_originalOutput);
		}

		#region Overrides of Filter

		public override Guid TypeGuid
		{
			get { return new Guid("27A97FFE-065B-4684-9035-80793463EB7B"); }
		}

		public override void Process()
		{
			var frame = (IImage)_input.GetData();
			lock (_outputs)
			{
				foreach (OutputPin outputPin in _outputs)
				{
					object clone = frame.Clone();
					outputPin.SetData(clone);
				}
			}
			_originalOutput.SetData(frame);
		}

		#endregion

		private void OnChangedCountOfCopies(object obj)
		{
			if (!(obj is int)) 
				return;

			var copies = (int)obj;
			if (_outputs.Count == copies) 
				return;

			lock (_outputs)
			{
				foreach (IPin outputPin in _outputs)
				{
					outputPin.Disconnect();
					RemovePin(outputPin);
				}
				_outputs.Clear();

				for (int i = 0; i < copies; i++)
				{
					var outputPin = new OutputPin("Copy № " + (i + 1), PinMediaType.Image);
					_outputs.Add(outputPin);
					AddPin(outputPin);
				}
			}
		}
	}
}