using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TestsGenerator
{
	public class TestGenerator
	{
		private Generator generator;
		private Reader reader;
		private Writer writer;

		public TestGenerator(int readThreads, int generationThreads, int writeThreads, string outPath)
		{
			generator = new Generator(generationThreads);
			reader = new Reader(readThreads);
			writer = new Writer(writeThreads, outPath);
		}

		public Task StartTestCreation(string inPath)
		{
			List<string> filePaths = new List<string>();
			DirectoryInfo dirInfo = new DirectoryInfo(inPath);
			foreach(var item in dirInfo.GetFiles())
			{
				filePaths.Add(item.FullName);
			}
			return writer.WriteToFiles(generator.GenerateTestClasses(reader.ReadFromFiles(filePaths)));
		}
	}
}
