using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace JavaObfuscate
{
    class Program
    {
        static void GenerateAST(string filePath)
        {
            Console.WriteLine("Generating AST for " + filePath + "...");
            var ast = new Parser().Parse(filePath);
            File.WriteAllText(filePath + ".ast.txt", ast.GenerateAST());
        }


        static void GenerateCSharpAST(string filePath)
        {
            Console.WriteLine("Generating C# AST for " + filePath + "...");
            var ast = new Parser().Parse(filePath);
            File.WriteAllText(filePath + ".ast.cs.txt", ast.GenerateCSharpAST());
        }


        static void GenerateDesugaredCode(string filePath)
        {
            Console.WriteLine("Desugaring " + filePath + "...");
            var ast = new Parser().Parse(filePath);
            var desugarer = new Desugarer();
            var newast = desugarer.Desugar(ast);
            File.WriteAllText(filePath + ".dsg.txt", newast.GenerateCode());
        }


        static void Main(string[] args)
        {
            GenerateAST("TestFiles/JavaBasicForLoop.java");
            GenerateAST("TestFiles/JavaWhileLoop.java");
            GenerateAST("TestFiles/JavaDoWhileLoop.java");
            GenerateAST("TestFiles/JavaWhileSwitchCase.java");

            GenerateCSharpAST("TestFiles/JavaBasicForLoop.java");
            GenerateCSharpAST("TestFiles/JavaWhileLoop.java");
            GenerateCSharpAST("TestFiles/JavaDoWhileLoop.java");
            GenerateCSharpAST("TestFiles/JavaWhileSwitchCase.java");

            GenerateDesugaredCode("TestFiles/TestDesugarBasicForLoop.java");
            GenerateDesugaredCode("TestFiles/TestDesugarDoWhileLoop.java");



        }
    }
}
