using System;
using System.Collections.Generic;
using System.IO;
using TestsGenerator;

namespace TestsGeneratorConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			TestsCreator testsCreator = new TestsCreator(2,2,2, @"\GeneratedTests");
			testsCreator.SetReadMethod(ReadFromFile);
			testsCreator.SetWriteMethod(WriteToFile);
			testsCreator.StartTestCreation(@"D:\GitHub\MPP-TestsGenerator\TestsGenerator\Classes").Wait();
			Console.Write("Done");
			Console.ReadLine();
		}

		static private string ReadFromFile(string path)
		{
			System.Threading.Thread.Sleep(2000);
			using (StreamReader reader = new StreamReader(path))
			{
				return reader.ReadToEnd();
			}
		}

		static private void WriteToFile(List<CreatedTestTemplate> generateResults)
		{
			System.Threading.Thread.Sleep(2000);
			foreach (CreatedTestTemplate result in generateResults)
			{
				string filePath = @"D:\GitHub\MPP-TestsGenerator\TestsGenerator\GeneratedTests" + @"\" + result.Name;
				using (StreamWriter sw = new StreamWriter(filePath))
				{
					sw.Write(result.Text);
				}
			}
		}
	}
}
