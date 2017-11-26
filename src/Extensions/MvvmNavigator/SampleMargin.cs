﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Editor;

namespace MvvmNavigator
{
	/// <summary>
	/// Margin's canvas and visual definition including both size and content
	/// </summary>
	internal class SampleMargin : Canvas, IWpfTextViewMargin
	{
		/// <summary>
		/// Margin name.
		/// </summary>
		public const string MarginName = "SampleMargin";

		/// <summary>
		/// A value indicating whether the object is disposed.
		/// </summary>
		private bool isDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="SampleMargin"/> class for a given <paramref name="textView"/>.
		/// </summary>
		/// <param name="textView">The <see cref="IWpfTextView"/> to attach the margin to.</param>
		public SampleMargin(IWpfTextView textView)
		{
			this.Height = 20; // Margin height sufficient to have the label
			this.ClipToBounds = true;
			this.Background = new SolidColorBrush(Colors.LightGreen);

			// Add a green colored label that says "Hello SampleMargin"
			var label = new Label
			{
				Background = new SolidColorBrush(Colors.LightGreen),
			};
			
			var canvasParents = string.Join(", ", GetParents(this).Select(s => s.GetType().Name));
			var textViewParents = string.Join(", ", GetParents(textView as FrameworkElement).Select(s => s.GetType().Name));
			label.Content = canvasParents + ";"+ textViewParents;

			this.Children.Add(label);
		}

		private IEnumerable<DependencyObject> GetParents(FrameworkElement element)
		{
			if(element == null)
				yield break;

			yield return element;

			if (element is Grid host)
			{
				host.Margin = new Thickness(15);
				//host.BorderBrush = new SolidColorBrush(Colors.Red);
				//host.BorderThickness = new Thickness(5);
				//host.Padding = new Thickness(20);
			}
			
			if (element.Parent is FrameworkElement visual)
				foreach (var sub in GetParents(visual))
				{
					yield return sub;
				}

			if (element.Parent is IWpfTextViewHost textViewHost)
				foreach (var sub in GetParents(textViewHost.HostControl))
				{
					yield return sub;
				}
		}

		#region IWpfTextViewMargin

		/// <summary>
		/// Gets the <see cref="Sytem.Windows.FrameworkElement"/> that implements the visual representation of the margin.
		/// </summary>
		/// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
		public FrameworkElement VisualElement
		{
			// Since this margin implements Canvas, this is the object which renders
			// the margin.
			get
			{
				this.ThrowIfDisposed();
				return this;
			}
		}

		#endregion

		#region ITextViewMargin

		/// <summary>
		/// Gets the size of the margin.
		/// </summary>
		/// <remarks>
		/// For a horizontal margin this is the height of the margin,
		/// since the width will be determined by the <see cref="ITextView"/>.
		/// For a vertical margin this is the width of the margin,
		/// since the height will be determined by the <see cref="ITextView"/>.
		/// </remarks>
		/// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
		public double MarginSize
		{
			get
			{
				this.ThrowIfDisposed();

				// Since this is a horizontal margin, its width will be bound to the width of the text view.
				// Therefore, its size is its height.
				return this.ActualHeight;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the margin is enabled.
		/// </summary>
		/// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
		public bool Enabled
		{
			get
			{
				this.ThrowIfDisposed();

				// The margin should always be enabled
				return true;
			}
		}

		/// <summary>
		/// Gets the <see cref="ITextViewMargin"/> with the given <paramref name="marginName"/> or null if no match is found
		/// </summary>
		/// <param name="marginName">The name of the <see cref="ITextViewMargin"/></param>
		/// <returns>The <see cref="ITextViewMargin"/> named <paramref name="marginName"/>, or null if no match is found.</returns>
		/// <remarks>
		/// A margin returns itself if it is passed its own name. If the name does not match and it is a container margin, it
		/// forwards the call to its children. Margin name comparisons are case-insensitive.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="marginName"/> is null.</exception>
		public ITextViewMargin GetTextViewMargin(string marginName)
		{
			return string.Equals(marginName, SampleMargin.MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
		}

		/// <summary>
		/// Disposes an instance of <see cref="SampleMargin"/> class.
		/// </summary>
		public void Dispose()
		{
			if (!this.isDisposed)
			{
				GC.SuppressFinalize(this);
				this.isDisposed = true;
			}
		}

		#endregion

		/// <summary>
		/// Checks and throws <see cref="ObjectDisposedException"/> if the object is disposed.
		/// </summary>
		private void ThrowIfDisposed()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(MarginName);
			}
		}
	}
}
