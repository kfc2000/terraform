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



        static void Main(string[] args)
        {
            var code = File.ReadAllText("TestFiles/MainActivity.java");

            Console.WriteLine("Parsing...");
            var stream = CharStreams.fromString(code);
            var lexer = new Java8Lexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new Java8Parser(tokens);
            parser.BuildParseTree = true;

            var cu = parser.compilationUnit();



            //Walk(cu, 0);
            Console.WriteLine("Construct AST...");
            var ast = JavaAST.ConstructAST(cu);

            //Console.WriteLine(ast.GenerateTree());

            Console.WriteLine("Desugaring...");
            var desugarer = new Desugar();
            var newast = desugarer.Run(ast);

            Console.WriteLine("Output...");
            Console.WriteLine(newast.GenerateCode());
            
        }
    }
}
