using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestsGenerator;

namespace TestsGeneratorConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			TestGenerator testsGenerator = new TestGenerator(1,1,1, @"D:\GitHub\MPP-TestsGenerator\TestsGenerator\GeneratedTests");
			testsGenerator.StartTestCreation(@"D:\GitHub\MPP-TestsGenerator\TestsGenerator\Classes").Wait();
		}
	}
}
