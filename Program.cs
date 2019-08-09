/*
 * Created by SharpDevelop.
 * User: adam.moseman
 * Date: 2/20/2017
 * Time: 5:54 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace filesorter2
{
	public enum SecFileType
	{
		Camera,
		HVAC,
		Rounds
	}
	
	public enum Month
	{
		NONE,
		January,
		February,
		March,
		April,
		May,
		June,
		July,
		August,
		September,
		October,
		November,
		December
	}
	
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("   MDC Papillion FileSorter v. 1.5");
			Console.WriteLine("      Written by a fellow idiot \n");
			
			DateTime TodayTime = DateTime.Today;
			bool sort = ChooseBetweenTwo("Sort New Files", "Missing/Incorrect File Names");
			if(sort == false)
			{
				CorrectFileNames();
				return;
			}
			string CameraFilePath; // Z:\Security\Camera log\MDC\{year}\{Month}\
			string HVACFilePath; // Z:\Security\HVAC log\MDC\{year}\{Month}\
			string RoundsFilePath; // Z:\Security\ROUNDS SHEETS\Midlands DC Rounds\{year}\{Month}\
			
			string CameraToSortPath; // ... \CameraFiles
			string HVACToSortPath; // ...\HVACFiles
			string RoundsToSortPath; // ...\RoundsFiles
			
			// MDC Camera MM-DD-YYYY to MM-DD-YYYY.pdf
			// MDC HVAC MM-DD-YYYY.pdf
			// MDC Rounds MM-DD-YYYY.pdf
			
			List<string> CameraFiles = new List<string>();
			List<string> HVACFiles = new List<string>();
			List<string> RoundsFiles = new List<string>();
			
			CameraFilePath = FindFilePath(SecFileType.Camera);
			HVACFilePath = FindFilePath(SecFileType.HVAC);
			RoundsFilePath = FindFilePath(SecFileType.Rounds);
			
			CameraToSortPath = FindToSortPath(SecFileType.Camera);
			HVACToSortPath = FindToSortPath(SecFileType.HVAC);
			RoundsToSortPath = FindToSortPath(SecFileType.Rounds);
			
			if(!System.IO.Directory.Exists(CameraToSortPath))
			{
				Console.WriteLine("Creating directory " + CameraToSortPath);
				System.IO.Directory.CreateDirectory(CameraToSortPath);
			}
			if(!System.IO.Directory.Exists(HVACToSortPath))
			{
				Console.WriteLine("Creating directory " + HVACToSortPath);
				System.IO.Directory.CreateDirectory(HVACToSortPath);
			}
			if(!System.IO.Directory.Exists(RoundsToSortPath))
			{
				Console.WriteLine("Creating directory " + RoundsToSortPath);
				System.IO.Directory.CreateDirectory(RoundsToSortPath);
			}
			
			CameraFiles = GatherFiles(CameraToSortPath, SecFileType.Camera);
			HVACFiles = GatherFiles(HVACToSortPath, SecFileType.HVAC);
			RoundsFiles = GatherFiles(RoundsToSortPath, SecFileType.Rounds);
			
			DateTime LastHVAC = GetLastFileDate(SecFileType.HVAC, HVACFilePath);
			DateTime LastRounds = GetLastFileDate(SecFileType.Rounds, RoundsFilePath);
			DateTime LastCamera = GetLastFileDate(SecFileType.Camera, CameraFilePath);
			
			DisplayLastDate(SecFileType.Camera, LastCamera);
			DisplayLastDate(SecFileType.HVAC, LastHVAC);
			DisplayLastDate(SecFileType.Rounds, LastRounds);
			
			if(CameraFiles.Count>0)
			{
				SortFiles(CameraFiles, CameraFilePath, LastCamera, TodayTime, SecFileType.Camera);
			}
			if(HVACFiles.Count>0)
			{
				SortFiles(HVACFiles, HVACFilePath, LastHVAC, TodayTime, SecFileType.HVAC);
			}
			if(RoundsFiles.Count>0)
			{
				SortFiles(RoundsFiles, RoundsFilePath, LastRounds, TodayTime, SecFileType.Rounds);
			}

			
			Console.WriteLine("Operation completed.");
			Console.ReadKey(true);
		}
		
		public static void CorrectFileNames()
		{
			DateTime StartDate = new DateTime(2016, 01, 01);
			DateTime Today = DateTime.Now;
			//CameraFilePath = FindFilePath(SecFileType.Camera);
			string HVACFilePath = FindFilePath(SecFileType.HVAC);
			string RoundsFilePath = FindFilePath(SecFileType.Rounds);
			
			int ToCheck = NeededFileCount(StartDate, Today, SecFileType.HVAC);
			
			for(int i=0; i<ToCheck;i++)
			{
				CorrectFileNameGenericish(SecFileType.HVAC, StartDate);
				CorrectFileNameGenericish(SecFileType.Rounds, StartDate);
				//CorrectFileNameGenericish(SecFileType.Camera, StartDate);
				StartDate = StartDate.AddDays(1);
			}
			
			Console.WriteLine("Operation completed.");
			Console.ReadKey(true);
		}
		
		public static void CorrectFileNameGenericish(SecFileType filetype, DateTime datetocheck)
		{
			string filepath = FindFilePath(filetype) + "\\" + datetocheck.Year.ToString() + "\\" + MonthToString(datetocheck.Month);;
			string filename = FormatDateToFileName(datetocheck, filetype);
			
			if(System.IO.Directory.Exists(filepath))
			{
				if(System.IO.File.Exists(filepath+"\\"+filename))
				{
					return;
				}
				Console.WriteLine("Did not find file " + filename);
				List<string> ErrorPaths = ErrorDateFormats(datetocheck, filetype);
				for(int i=0;i<ErrorPaths.Count;i++)
				{
					if(System.IO.File.Exists(filepath+"\\"+ErrorPaths[i]))
					{
						Console.WriteLine("Renaming " + ErrorPaths[i]);
						System.IO.File.Move(filepath+"\\"+ErrorPaths[i],filepath+ "\\" +filename);
						break;
					}
				}
			}
		}
		
		public static string FindFilePath(SecFileType filetype)
		{
			switch(filetype)
			{
				case SecFileType.Camera:
					return "Z:\\Security\\Camera log\\MDC";
				case SecFileType.HVAC:
					return "Z:\\Security\\HVAC log\\MDC";
				case SecFileType.Rounds:
					return "Z:\\Security\\ROUNDS SHEETS\\Midlands DC Rounds";
				default:
					throw new Exception("DONT DO WHATEVER THIS IS");
			}
		}
		
		public static List<string> GatherFiles(string path, SecFileType filetype)
		{
			string[] files = System.IO.Directory.GetFiles(path);
			List<string> ReturnValue = new List<string>();
			for(int i=0; i<files.Length;i++)
			{
				ReturnValue.Add(files[i]);
			}
			Console.WriteLine("Found " + ReturnValue.Count + " " + filetype.ToString() + " files");
			return ReturnValue;
		}
		
		public static DateTime GetLastFileDate(SecFileType filetype, string path)
		{
			DateTime date = DateTime.Now;
			if(filetype == SecFileType.Camera)
			{
				while(date.DayOfWeek != DayOfWeek.Saturday)
				{
					date = date.AddDays(-1);
				}
			}
			while(true)
			{
				string trashpath = path + "\\" + date.Year.ToString() + "\\" + MonthToString(date.Month);
				if(System.IO.Directory.Exists(trashpath))
				{
					trashpath = trashpath  + "\\" + FormatDateToFileName(date, filetype);
					if(System.IO.File.Exists(trashpath))
					{
						return date;
					}
				}
				if(filetype == SecFileType.Camera)
				{
					date = date.AddDays(-7);
				}
				else
				{
					date = date.AddDays(-1);
				}
				if(date.Year < 2016)
				{
					throw new Exception("Failed to find " + filetype + ", last path checked is " + trashpath);
				}
			}
			throw new Exception("this isn't good");
		}
		
		public static void DisplayLastDate(SecFileType filetype, DateTime lastfiledate)
		{
			Console.WriteLine("Latest {0} file is dated at {1}", filetype.ToString(), lastfiledate.ToShortDateString());
		}
		
		public static void FindFileLoadOrder(List<string> files, SecFileType filetype)
		{
			Console.WriteLine("Did you scan the {0} files newest first, or oldest first?", filetype.ToString());
			bool Newest = ChooseBetweenTwo("Newest", "Oldest");
			if(Newest)
			{
				files.Reverse();
			}
		}
		
		public static int NeededFileCount(DateTime lastfiledate, DateTime todaytime, SecFileType filetype)
		{
			if(filetype == SecFileType.Camera)
			{
				return todaytime.Subtract(lastfiledate).Days/7;
			}
			return todaytime.Subtract(lastfiledate).Days;
		}
		
		public static bool ChooseBetweenTwo(string c1, string c2)
		{
			Console.WriteLine("Press 1 or 2 to choose");
			Console.WriteLine("1: " + c1 + " 2: " + c2);
			while(true)
			{
				char input = Console.ReadKey(true).KeyChar;
				Console.WriteLine();
				//Console.WriteLine(input);
				if(input != '1' && input != '2')
				{
					continue;
				}
				if(input == '1')
				{
					return true;
				}
				return false;
			}
		}
		
		public static DateTime TodayMinusFileDays(DateTime todaytime, int filecount)
		{
			return todaytime.AddDays((double)filecount);
		}
		
		public static void MoveFiles(List<string> files, string filepath, DateTime startdate, SecFileType filetype, bool advancing)
		{
			int adddays = 1;
			if(filetype == SecFileType.Camera)
			{
				adddays = 7;
			}
			if(!advancing)
			{
				adddays = adddays*-1;
			}
			DateTime Date = startdate;
			
			Date = Date.AddDays(adddays);
			
			for(int i=0; i<files.Count;i++)
			{
				string newpath = filepath + "\\" + Date.Year.ToString() + "\\" + MonthToString(Date.Month);
				if(!System.IO.Directory.Exists(filepath))
				{
					System.IO.Directory.CreateDirectory(newpath);
					Console.WriteLine("Created directory " + newpath);
				}
				
				Console.WriteLine("Moving " + files[i] + " to " + newpath + "\\" + FormatDateToFileName(Date, filetype));
				System.IO.File.Move(files[i], newpath + "\\" + FormatDateToFileName(Date, filetype));
				Console.WriteLine("Movement succesful.");
				Date = Date.AddDays(adddays);
			}
		}
		
		public static string FormatDateToFileName(DateTime date, SecFileType filetype)
		{
			// MDC Camera MM-DD-YYYY to MM-DD-YYYY.pdf
			// MDC HVAC MM-DD-YYYY.pdf
			// MDC Rounds MM-DD-YYYY.pdf
			string mm = date.Month.ToString();
			string dd = date.Day.ToString();
			string yyyy = date.Year.ToString();
			int CamRemoveDays = -6;
			
			if(mm.Length != 2)
			{
				mm = "0" + mm;
			}
			if(dd.Length != 2)
			{
				dd = "0" + dd;
			}
			
			switch(filetype)
			{
				case SecFileType.Camera:
					string pmm = date.AddDays(CamRemoveDays).Month.ToString();
					string pdd = date.AddDays(CamRemoveDays).Day.ToString();
					string pyyyy = date.AddDays(CamRemoveDays).Year.ToString().Substring(2,2);
					yyyy = yyyy.Substring(2,2);
					if(pmm.Length != 2)
					{
						pmm = "0" + pmm;
					}
					if(pdd.Length != 2)
					{
						pdd = "0" + pdd;
					}
					return "MDC Cameras "+pmm+"-"+pdd+"-"+pyyyy+" to "+mm+"-"+dd+"-"+yyyy +".pdf";
				case SecFileType.HVAC:
					return "MDC HVAC "+mm+"-"+dd+"-"+yyyy+".pdf";
				case SecFileType.Rounds:
					return "MDC Rounds "+mm+"-"+dd+"-"+yyyy+".pdf";
				default:
					throw new Exception("WHAT");
			}
		}
		
		public static List<string> ErrorDateFormats(DateTime date, SecFileType filetype)
		{
			List<string> ReturnValue = new List<string>();
			
			string mm = date.Month.ToString();
			string dd = date.Day.ToString();
			string yyyy = date.Year.ToString();
			
			// 1-1-2016
			switch(filetype)
			{
				case SecFileType.Camera:
					throw new NotImplementedException();
				case SecFileType.HVAC:
					ReturnValue.Add("MDC HVAC "+mm+"-"+dd+"-"+yyyy+".pdf");
					break;
				case SecFileType.Rounds:
					ReturnValue.Add("MDC Rounds "+mm+"-"+dd+"-"+yyyy+".pdf");
					break;
				default:
					throw new Exception("WHAT");
			}
			
			mm = date.Month.ToString();
			dd = date.Day.ToString();
			yyyy = date.Year.ToString();
			if(mm.Length != 2)
			{
				mm = "0" + mm;
			}
			// 01-1-2016
			switch(filetype)
			{
				case SecFileType.Camera:
					throw new NotImplementedException();
				case SecFileType.HVAC:
					ReturnValue.Add("MDC HVAC "+mm+"-"+dd+"-"+yyyy+".pdf");
					break;
				case SecFileType.Rounds:
					ReturnValue.Add("MDC Rounds "+mm+"-"+dd+"-"+yyyy+".pdf");
					break;
				default:
					throw new Exception("WHAT");
			}
			
			mm = date.Month.ToString();
			dd = date.Day.ToString();
			yyyy = date.Year.ToString();

			if(dd.Length != 2)
			{
				dd = "0" + dd;
			}
			// 1-01-2016
			switch(filetype)
			{
				case SecFileType.Camera:
					throw new NotImplementedException();
				case SecFileType.HVAC:
					ReturnValue.Add("MDC HVAC "+mm+"-"+dd+"-"+yyyy+".pdf");
					break;
				case SecFileType.Rounds:
					ReturnValue.Add("MDC Rounds "+mm+"-"+dd+"-"+yyyy+".pdf");
					break;
				default:
					throw new Exception("WHAT");
			}
			
			return ReturnValue;
		}
		
		public static string FindToSortPath(SecFileType filetype)
		{
			string mylocation = AppDomain.CurrentDomain.BaseDirectory;
			mylocation += "\\"+filetype.ToString() +"Files";
			return mylocation;
		}
		
		public static void SortFiles(List<string> files, string filepath, DateTime lastfiledate, DateTime todaytime, SecFileType filetype)
		{
			FindFileLoadOrder(files, filetype);
			bool confirm = true;
			
			if(files.Count < NeededFileCount(lastfiledate, todaytime, filetype))
			{
				Console.WriteLine("You do not have enough {0} files! You have {1} but you need {2}. Would you like to add them in anyways?",
				                  filetype.ToString(), files.Count, NeededFileCount(lastfiledate, todaytime, filetype));
				confirm = ChooseBetweenTwo("Yes", "No");
			}
			if(confirm)
			{
				MoveFiles(files, filepath, lastfiledate, filetype, true);
			}
		}
		
		public static string PathToDate(DateTime date)
		{
			return date.Year.ToString() + "\\" + MonthToString(date.Month) + "\\";
		}
		
		public static string MonthToString(int month)
		{
			return ((Month)month).ToString();
		}
	}
}