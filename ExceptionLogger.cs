using System;
using System.IO;
using System.Reflection;

namespace TsMod
{
	static class ExceptionLogger
	{
		public static void LogException(Exception exception)
		{
			using (var file = new StreamWriter(GetFileName()))
			{
				try
				{
					file.WriteLine("Context:");
					file.WriteLine(
						$"Timespinner Version: {TimeSpinnerGame.Constants.GameVersion}, TsMod Version: {Assembly.GetExecutingAssembly().GetName().Version}");
					file.WriteLine();

					WriteException(exception, file);
				}
				catch (Exception e)
				{
					try
					{
						file.WriteLine("Writing ExceptionLog Failure:");
						WriteException(e, file);
					}
					catch
					{
					}
				}
			}
		}

		static void WriteException(Exception exception, StreamWriter file)
		{
			file.WriteLine("Exceptions:");

			while (exception != null)
			{
				file.WriteLine($"Exception: {exception.Message}");
				file.WriteLine(exception.StackTrace);
				file.WriteLine();

				exception = exception.InnerException;
			}
		}

		static string GetFileName()
		{
			var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var fileDateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH.mm");

			// ReSharper disable once AssignNullToNotNullAttribute
			return Path.Combine(directory, $"TsMod {fileDateTime}.txt");
		}
	}
}