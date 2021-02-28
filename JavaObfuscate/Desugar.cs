using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaObfuscate
{
    public class Desugar 
    {
        private int labelCounter = 1;


        public JavaAST Run(JavaAST ast)
        {
            if (ast == null)
            {
                return null;
            }


            // De-sugar a basic for loop into a while loop.
            //
            if (ast.Rule == Java8Parser.RULE_basicForStatement)
            {
                return JavaAST.CreateRule(Java8Parser.RULE_block,
                    Run(ast.Children[2]),                                    // for loop variable declaration
                    JavaAST.CreateToken(Java8Lexer.SEMI),
                    JavaAST.CreateRule(Java8Parser.RULE_whileStatement,
                        JavaAST.CreateToken(Java8Lexer.WHILE), 
                        JavaAST.CreateToken(Java8Lexer.LPAREN),
                        Run(ast.Children[4]),                                // for loop condition
                        JavaAST.CreateToken(Java8Lexer.RPAREN),
                        JavaAST.CreateRule(Java8Parser.RULE_block,
                            JavaAST.CreateToken(Java8Lexer.LBRACE),
                            Run(ast.Children[8]),                            // for loop block
                            Run(ast.Children[6]),                            // for loop increment 
                            JavaAST.CreateToken(Java8Lexer.SEMI),
                            JavaAST.CreateToken(Java8Lexer.RBRACE)
                        )
                    )
                );
            }

            // Default case
            //
            if (ast.IsRule)
            {
                var newChildren = new List<JavaAST>();

                foreach (var child in ast.Children)
                    newChildren.Add(Run(child));

                return JavaAST.CreateRule(ast.Rule, newChildren.ToArray());
            }
            else
            {
                return JavaAST.CreateToken(ast.Token, ast.Text);
            }

        }
    }
}
