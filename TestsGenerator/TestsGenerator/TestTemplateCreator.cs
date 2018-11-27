﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TestsGenerator
{
	internal class TestTemplateCreator
	{
		public List<CreatedTestTemplate> GenerateTestTemplate(string code)
		{
			List<CreatedTestTemplate> result = new List<CreatedTestTemplate>();

			SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
			CompilationUnitSyntax compilationUnitSyntax = syntaxTree.GetCompilationUnitRoot();

			foreach (ClassDeclarationSyntax classInfo in compilationUnitSyntax.DescendantNodes().OfType<ClassDeclarationSyntax>())
			{
				string namespaceName = (classInfo.Parent as NamespaceDeclarationSyntax)?.Name.ToString();
				string className = classInfo.Identifier.ValueText;
				List<string> testMethodsName = new List<string>();

				foreach(MethodDeclarationSyntax method in classInfo.DescendantNodes().OfType<MethodDeclarationSyntax>())
				{
					if (method.Modifiers.Any(x => x.ValueText == "public"))
					{
						testMethodsName.Add(GetTestMethodName(testMethodsName, method.Identifier.ToString()));
					}
				}

				NamespaceDeclarationSyntax namespaceDeclarationSyntax = NamespaceDeclaration(QualifiedName(
					IdentifierName(namespaceName), IdentifierName("Tests")));

				ClassDeclarationSyntax classDeclarationSyntax = ClassDeclaration(className + "Test")
					.WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("TestClass"))))))
					.WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)));

				CompilationUnitSyntax compilationUnit = CompilationUnit()
					.WithUsings(GetUsings(namespaceName))
					.WithMembers(SingletonList<MemberDeclarationSyntax>(namespaceDeclarationSyntax
					.WithMembers(SingletonList<MemberDeclarationSyntax>(classDeclarationSyntax
					.WithMembers(CreateTestMethods(testMethodsName))))));


				
				CreatedTestTemplate resultElement = new CreatedTestTemplate();
				resultElement.Text = compilationUnit.NormalizeWhitespace().ToFullString();
				resultElement.Name = className + "Tests.cs";
				result.Add(resultElement);
			}

			return result;
		}

		private SyntaxList<UsingDirectiveSyntax> GetUsings(string namespaceName)
		{
			List<UsingDirectiveSyntax> usingDirective = new List<UsingDirectiveSyntax>();
			usingDirective.Add(UsingDirective(QualifiedName(QualifiedName(QualifiedName(
				IdentifierName("Microsoft"),
				IdentifierName("VisualStudio")),
				IdentifierName("TestTools")),
				IdentifierName("UnitTesting"))));
			usingDirective.Add(UsingDirective(IdentifierName(namespaceName)));

			return List(usingDirective);
		}

		private SyntaxList<MemberDeclarationSyntax> CreateTestMethods(List<string> methods)
		{
			List<MemberDeclarationSyntax> result = new List<MemberDeclarationSyntax>();

			foreach (string method in methods)
				result.Add(CreateTestMethod(method));

			return List(result);
		}

		private MethodDeclarationSyntax CreateTestMethod(string name)
		{
			MethodDeclarationSyntax title = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier(name + "Test"))
				.WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("TestMethod"))))))
				.WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)));

			BlockSyntax body = Block(ExpressionStatement(InvocationExpression(
				MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,IdentifierName("Assert"), IdentifierName("Fail")))
				.WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, 
				Literal("autogenerated"))))))));

			return title.WithBody(body);
		}

		private string GetTestMethodName(List<string> methodsName, string methodName, int count = 0)
		{
			string result = methodName + (count == 0 ? "" :"_"+count.ToString());

			if (methodsName.Contains(result))
				return GetTestMethodName(methodsName, methodName, ++count);

			return result;
		}
	}
}
