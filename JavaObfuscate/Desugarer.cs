using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaObfuscate
{
    public class Desugarer 
    {
        private int labelCounter = 1;


        public JavaAST Desugar(JavaAST ast)
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
                    JavaAST.CreateToken(Java8Lexer.LBRACE),
                    JavaAST.CreateRule(Java8Parser.RULE_blockStatements,
                        Desugar(ast.Children[2]),                                    // for loop variable declaration
                        JavaAST.CreateToken(Java8Lexer.SEMI),
                        JavaAST.CreateRule(Java8Parser.RULE_whileStatement,
                            JavaAST.CreateToken(Java8Lexer.WHILE), 
                            JavaAST.CreateToken(Java8Lexer.LPAREN),
                            Desugar(ast.Children[4]),                                // for loop condition
                            JavaAST.CreateToken(Java8Lexer.RPAREN),
                            JavaAST.CreateRule(Java8Parser.RULE_block,
                                JavaAST.CreateToken(Java8Lexer.LBRACE),
                                Desugar(ast.Children[8]),                            // for loop block
                                Desugar(ast.Children[6]),                            // for loop increment 
                                JavaAST.CreateToken(Java8Lexer.SEMI),
                                JavaAST.CreateToken(Java8Lexer.RBRACE)
                            )
                        )
                    ),
                    JavaAST.CreateToken(Java8Lexer.RBRACE)
                );
            }

            // De-sugar a do-while loop into a while loop
            //
            if (ast.Rule == Java8Parser.RULE_doStatement)
            {
                return JavaAST.CreateRule(Java8Parser.RULE_block,
                    JavaAST.CreateToken(Java8Lexer.LBRACE),
                    JavaAST.CreateRule(Java8Parser.RULE_blockStatements,
                        Desugar(ast.Children[1]),                                    // do-while block
                        JavaAST.CreateToken(Java8Lexer.SEMI),
                        JavaAST.CreateRule(Java8Parser.RULE_whileStatement,
                            JavaAST.CreateToken(Java8Lexer.WHILE),
                            JavaAST.CreateToken(Java8Lexer.LPAREN),
                            Desugar(ast.Children[4]),                                // do-while loop condition
                            JavaAST.CreateToken(Java8Lexer.RPAREN),
                            Desugar(ast.Children[1])                                 // do-while block (this will be repeated under the while block
                        )
                    ),
                    JavaAST.CreateToken(Java8Lexer.RBRACE)
                );
            }

            // Default case: recursively desugar all children.
            //
            if (ast.IsRule)
            {
                var newChildren = new List<JavaAST>();

                foreach (var child in ast.Children)
                    newChildren.Add(Desugar(child));

                return JavaAST.CreateRule(ast.Rule, newChildren.ToArray());
            }
            else
            {
                return JavaAST.CreateToken(ast.Token, ast.Text);
            }

        }
    }
}
