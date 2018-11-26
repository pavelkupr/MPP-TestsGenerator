using System;
using TestsGenerator;

namespace TestsGeneratorConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			TestsCreator testsGenerator = new TestsCreator(1,1,1, @"D:\GitHub\MPP-TestsGenerator\TestsGenerator\GeneratedTests");
			testsGenerator.StartTestCreation(@"D:\GitHub\MPP-TestsGenerator\TestsGenerator\Classes").Wait();
			Console.Write("Done");
			Console.ReadLine();
		}
	}
}
