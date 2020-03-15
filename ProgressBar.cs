using System;
using System.Text;
using System.Threading;

namespace AfterDownloadFileHandler
{
	/// <summary>
	/// An ASCII progress bar
	/// </summary>
	public class ProgressBar : IDisposable, IProgress<double>
	{
		private const int blockCount = 10;
		private readonly TimeSpan animationInterval = TimeSpan.FromSeconds(1.0 / 8);
		private const string animation = @"|/-\";

		private readonly Timer timer;

		private double currentProgress = 0;
		private string currentText = string.Empty;
		private bool disposed = false;
		private int animationIndex = 0;

		public ProgressBar()
		{
			timer = new Timer(TimerHandler);

			// A progress bar is only for temporary display in a console window.
			// If the console output is redirected to a file, draw nothing.
			// Otherwise, we'll end up with a lot of garbage in the target file.
			if (!Console.IsOutputRedirected)
			{
				ResetTimer();
			}
		}

		public void Report(double value)
		{
			// Make sure value is in [0..1] range
			value = Math.Max(0, Math.Min(1, value));
			Interlocked.Exchange(ref currentProgress, value);
		}

		private void TimerHandler(object state)
		{
			lock (timer)
			{
				if (disposed) return;

				int progressBlockCount = (int)(currentProgress * blockCount);
				int percent = (int)(currentProgress * 100);
				string text = string.Format("[{0}{1}] {2,3}% {3}",
					new string('#', progressBlockCount), new string('-', blockCount - progressBlockCount),
					percent,
					animation[animationIndex++ % animation.Length]);
				UpdateText(text);

				ResetTimer();

				//simulate infinite progress while no reporting is done withing copy
				//below if order is important
				if (currentProgress == 0.9) currentProgress = 0;
				if (currentProgress == 0.8) currentProgress = 0.9;
				if (currentProgress == 0.7) currentProgress = 0.8;
				if (currentProgress == 0.6) currentProgress = 0.7;
				if (currentProgress == 0.5) currentProgress = 0.6;
				if (currentProgress == 0.4) currentProgress = 0.5;
				if (currentProgress == 0.3) currentProgress = 0.4;
				if (currentProgress == 0.2) currentProgress = 0.3;
				if (currentProgress == 0.1) currentProgress = 0.2;
				if (currentProgress == 0) currentProgress = 0.1;
			}
		}

		private void UpdateText(string text)
		{
			// Get length of common portion
			int commonPrefixLength = 0;
			int commonLength = Math.Min(currentText.Length, text.Length);
			while (commonPrefixLength < commonLength && text[commonPrefixLength] == currentText[commonPrefixLength])
			{
				commonPrefixLength++;
			}

			// Backtrack to the first differing character
			StringBuilder outputBuilder = new StringBuilder();
			outputBuilder.Append('\b', currentText.Length - commonPrefixLength);

			// Output new suffix
			outputBuilder.Append(text.Substring(commonPrefixLength));

			// If the new text is shorter than the old one: delete overlapping characters
			int overlapCount = currentText.Length - text.Length;
			if (overlapCount > 0)
			{
				outputBuilder.Append(' ', overlapCount);
				outputBuilder.Append('\b', overlapCount);
			}

			Console.Write(outputBuilder);
			currentText = text;
		}

		private void ResetTimer()
		{
			timer.Change(animationInterval, TimeSpan.FromMilliseconds(-1));
		}

		public void Dispose()
		{
			lock (timer)
			{
				disposed = true;
				UpdateText(string.Empty);
			}
		}

	}
}
