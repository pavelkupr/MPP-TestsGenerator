using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsGenerator;

namespace TestsGeneratorTests
{
	internal class TestMethods
	{
		internal bool testResult = false;

		internal void WriteToFileTest_1(List<CreatedTestTemplate> generateResults)
		{
			if (generateResults.Count((x) => x.Name == "DoubleGeneratorTests.cs") == 1)
				testResult = true;
		}

		internal string ReadFromFileTest_2(string path)
		{
			if (path.Contains("DoubleGenerator.cs"))
				testResult = true;
			return "TEST";
		}

		internal void WriteToFileTest_3(List<CreatedTestTemplate> generateResults)
		{
			if (generateResults.Count((x) => x.Text.Contains("class TESTTest") ) == 1)
				testResult = true;
		}

		internal string ReadFromFileTest_3(string path)
		{
			return "class TEST{}";
		}

		internal void EmptyWrite(List<CreatedTestTemplate> generateResults)
		{
		}
	}

	[TestClass]
	public class TestsCreatorTests
	{
		static private TestsCreator testsCreator;
		static private TestMethods testMethods;
		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			testMethods = new TestMethods();
			testsCreator = new TestsCreator(2, 2, 2, @"..\..\..\TestGeneratedTests");
		}

		public void Initialize()
		{
			testMethods.testResult = false;
		}

		[TestMethod]
		public void TestCreatorCreatesTemplate()
		{
			testsCreator.SetReadMethod(null);
			testsCreator.SetWriteMethod(testMethods.WriteToFileTest_1);
			testsCreator.StartTestCreation(@"..\..\..\TestClasses").Wait();
			Assert.IsTrue(testMethods.testResult);
		}

		[TestMethod]
		public void TestCreatorFindsRightPath()
		{
			testsCreator.SetReadMethod(testMethods.ReadFromFileTest_2);
			testsCreator.SetWriteMethod(testMethods.EmptyWrite);
			testsCreator.StartTestCreation(@"..\..\..\TestClasses").Wait();
			Assert.IsTrue(testMethods.testResult);
		}

		[TestMethod]
		public void TestCreatorCreatesRightClassName()
		{
			testsCreator.SetReadMethod(testMethods.ReadFromFileTest_3);
			testsCreator.SetWriteMethod(testMethods.WriteToFileTest_3);
			testsCreator.StartTestCreation(@"..\..\..\TestClasses").Wait();
			Assert.IsTrue(testMethods.testResult);
		}
	}
}
