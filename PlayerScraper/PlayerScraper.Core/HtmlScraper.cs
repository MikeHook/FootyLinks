using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace PlayerScraper.Core
{
	public class HtmlScraper
	{
		public static int MaxPlayerId = 59270;
		public static string ErrorLogPath = @"C:\_Development\Android\FootyLinks\ErrorLog\log.txt";

		public static void ScrapePlayers(string pageAddress)
		{
			for (int playerId = 10611; playerId <= MaxPlayerId; playerId++)
			{
				string filePath = string.Format(@"C:\_Development\Android\FootyLinks\PlayersSource\{0}.html", playerId);
				var htmlStream = getHtmlStream(pageAddress, playerId);
				if (htmlStream == null)
					continue;

				using (htmlStream)
				{
					writeStreamToFile(filePath, htmlStream);
				}
				if (playerId % 10 == 0)
				{
					int pauseTime = 1000;
					System.Threading.Thread.Sleep(pauseTime);
				}
			}
		}
 
		private static Stream getHtmlStream(string pageToScrape, int playerId)
		{
			string httpAddress = string.Format("{0}{1}",pageToScrape, playerId);

			WebRequest objRequest = System.Net.HttpWebRequest.Create(httpAddress);
			WebResponse objResponse;
			try
			{
				objResponse = objRequest.GetResponse();
			}
			catch(Exception ex)	
			{
				//Ignore the exception the first time and try again
			}

			try
			{
				objResponse = objRequest.GetResponse();
			}
			catch (Exception ex)
			{
				//OK, must have some big issue here, log the problem to file and carry on 
				writeErrorToFile(ErrorLogPath, httpAddress, ex);
				return null;
			}

			return objResponse.GetResponseStream();
		}

		private static void writeStreamToFile(string filePath, Stream stream)
		{			
			using (var streamReader = new StreamReader(stream))
			{
				string fileContents = streamReader.ReadToEnd();
				//Remove the carriage return, new lines
				fileContents = Regex.Replace(fileContents, "[\t\r\n]", " ");

				File.WriteAllText(filePath, fileContents);
			}
		}

		private static void writeErrorToFile(string errorFile, string httpAddress, Exception ex)
		{
			File.AppendAllText(errorFile, string.Format("{0}\r\n", httpAddress));

			if (ex.GetType() != typeof(WebException))
			{
				File.AppendAllText(errorFile, string.Format("{0}\r\n", ex.ToString()));
			}
			File.AppendAllText(errorFile, "\r\n\r\n");		
		}
	}
}
