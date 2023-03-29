using System;
using System.IO;

namespace NEE.Database
{
	public class DbQueryLogger
	{
		public void Log(string component, string message, string methodName)
		{
            string docPath = "C:\\Logs";


            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "NEEApp_NorthEpirusExpatriates_WriteTextAsync.txt"), true))
			{
				outputFile.WriteLine($"{methodName} :: {message} {System.Environment.NewLine}================================================================");
			}
		}


	}
}
