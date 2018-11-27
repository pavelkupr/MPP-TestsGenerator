using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TestsGenerator
{
	public class TestsCreator
	{
		private GeneratorBlock generator;
		private ReaderBlock reader;
		private WriterBlock writer;

		public TestsCreator(int readThreads, int generationThreads, int writeThreads, string outPath)
		{
			generator = new GeneratorBlock(generationThreads);
			reader = new ReaderBlock(readThreads);
			writer = new WriterBlock(writeThreads, outPath);
		}

		public void SetReadMethod(ReadFromFileDelegate readFunc)
		{
			reader.ReadFunc = readFunc;
		}

		public void SetWriteMethod(WriteToFileDelegate writeFunc)
		{
			writer.WriteFunc = writeFunc;
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
