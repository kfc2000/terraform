using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaObfuscate
{
    public class Parser
    {
        public JavaAST Parse(string filePath)
        {
            var code = File.ReadAllText(filePath);

            var stream = CharStreams.fromString(code);
            var lexer = new Java8Lexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new Java8Parser(tokens);
            parser.BuildParseTree = true;

            var cu = parser.compilationUnit();

            return JavaAST.ConstructAST(cu);
        }
    }
}
