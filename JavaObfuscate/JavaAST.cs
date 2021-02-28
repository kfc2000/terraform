﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace JavaObfuscate
{
    public class JavaAST
    {
        private bool isRule;
        private int type;
        private string text;

        private List<JavaAST> children = new List<JavaAST>();
        static int[] literals = new int[] { Java8Lexer.FloatingPointLiteral, Java8Lexer.IntegerLiteral, Java8Lexer.StringLiteral, Java8Lexer.CharacterLiteral, Java8Lexer.BooleanLiteral, Java8Lexer.Identifier };

        public bool IsRule
        {
            get
            {
                return isRule;
            }
        }

        public int Rule
        {
            get
            {
                if (isRule)
                    return type;
                else
                    return -1;
            }
        }

        public int Token
        {
            get
            {
                if (!isRule)
                    return type;
                else
                    return -1;
            }
        }

        public string Text
        {
            get
            {
                return text;
            }
        }

        public List<JavaAST> Children
        {
            get { return children; }
        }

        public static JavaAST CreateToken(int type, string text = "")
        {
            JavaAST ast = new JavaAST();
            ast.isRule = false;
            ast.type = type;

            if (type == -1)
            {
                ast.text = "";
            }
            else
            {
                var literalText = Java8Lexer._LiteralNames[type] ?? "";
                if (literalText != "")
                    ast.text = literalText.Substring(1, literalText.Length - 2);
                else
                    ast.text = text;
            }
            return ast;
        }

        public static JavaAST CreateRule(int type, params JavaAST[] children)
        {
            JavaAST ast = new JavaAST();
            ast.isRule = true;
            ast.type = type;
            ast.text = "";
            if (children != null)
                ast.children.AddRange(children);
            return ast;
        }


        private static int codeIndent = 0;

        private static bool codeNewLine = true;

        private static bool inBasicForStatement = false;


        /// <summary>
        /// Generates the actual Java code recurively.
        /// </summary>
        /// <param name="sb"></param>
        private void GenerateCode(StringBuilder sb)
        {
            if (!this.isRule && this.text != "<EOF>")
            {
                if (this.Token == Java8Lexer.LBRACE)
                    codeIndent = codeIndent + 1;
                if (this.Token == Java8Lexer.RBRACE)
                    codeIndent = codeIndent - 1;
            }
            
            if (codeNewLine)
            {
                sb.Append(new String(' ', codeIndent * 2));
                codeNewLine = false;
            }

            if (this.isRule && this.Rule == Java8Parser.RULE_basicForStatement)
            {
                inBasicForStatement = true;
            }

            if (!this.isRule && this.text != "<EOF>")
            {
                //sb.Append(codeIndent.ToString());
                sb.Append(this.text + " ");

                if (this.Token == Java8Lexer.LBRACE ||
                    this.Token == Java8Lexer.RBRACE ||
                    this.Token == Java8Lexer.SEMI)
                {
                    if (!inBasicForStatement)
                    {
                        sb.Append("\n");
                        codeNewLine = true;
                    }
                }
            }

            foreach (var child in this.children)
            {
                child.GenerateCode(sb);

                if (!child.isRule && child.Token == Java8Lexer.RPAREN)
                {
                    inBasicForStatement = false;
                }
            }



        }

        /// <summary>
        /// Generates the actual Java code recurively.
        /// </summary>
        public string GenerateCode()
        {
            StringBuilder sb = new StringBuilder();
            codeNewLine = true;
            codeIndent = 0;

            this.GenerateCode(sb);
            return sb.ToString();
        }



        /// <summary>
        /// Generates the AST recursively in an easy-to-read structure.
        /// 
        /// This is useful for understanding and analyzing the AST structure generated by ANTLR.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="childNo"></param>
        /// <param name="indent"></param>
        private void GenerateAST(StringBuilder sb, int childNo, int indent)
        {
            string padding = new String(' ', indent * 2);
            if (this.isRule && this.Rule >= 0)
                sb.Append(String.Format("{0,-50}   {1,-10} {2,2}\n",
                    padding + Java8Parser.ruleNames[this.Rule], "", padding + childNo.ToString()));
            else if (!this.isRule && this.Token >= 0)
                sb.Append(String.Format("{0,-50} | {1,-10} {2,2}\n",
                    padding + "#" + (Java8Lexer._SymbolicNames[this.Token] ?? ""), this.Text, padding + childNo.ToString()));

            for (int i = 0; i < this.children.Count; i++)
            {
                this.children[i].GenerateAST(sb, i, indent + 1);
            }
        }


        /// <summary>
        /// Generates the AST recursively in an easy-to-read structure.
        /// 
        /// This is useful for understanding and analyzing the AST structure generated by ANTLR.
        /// </summary>

        public string GenerateAST()
        {
            StringBuilder sb = new StringBuilder();
            this.GenerateAST(sb, 0, 0);
            return sb.ToString();
        }


        /// <summary>
        /// Generates the AST in a C# code output that you can use it to programmatically
        /// create the Java AST.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="childNo"></param>
        /// <param name="childCount"></param>
        /// <param name="indent"></param>
        private void GenerateCSharpAST(StringBuilder sb, int childNo, int childCount, int indent)
        {
            bool isLastChild = (childNo == childCount - 1);
            string endingComma = isLastChild ? "" : ",";

            string padding = new String(' ', indent * 2);
            if (this.isRule && this.Rule >= 0)
                sb.Append(String.Format("{0,-90} // {1,2}\n",
                    padding + "JavaAST.CreateRule(Java8Parser.RULE_" + Java8Parser.ruleNames[this.Rule] + endingComma, "", childNo.ToString()));
            else if (!this.isRule && this.Token >= 0)
            {
                if (literals.Contains(this.Token))
                {
                    sb.Append(String.Format("{0,-90} // {1,2}\n",
                        padding + "JavaAST.CreateToken(Java8Lexer." + (Java8Lexer._SymbolicNames[this.Token] ?? "") + ", \"" + this.Text + "\")" + endingComma, padding + childNo.ToString()));
                }
                else
                {
                    sb.Append(String.Format("{0,-90} // {1,2}\n",
                        padding + "JavaAST.CreateToken(Java8Lexer." + (Java8Lexer._SymbolicNames[this.Token] ?? "") + ")" + endingComma, padding + childNo.ToString()));

                }
            }

            for (int i = 0; i < this.children.Count; i++)
            {
                this.children[i].GenerateCSharpAST(sb, i, this.children.Count, indent + 1);
            }

            if (this.isRule && this.Rule >= 0)
                sb.Append(padding + ")\n");
        }



        /// <summary>
        /// Generates the AST in a C# code output that you can use it to programmatically
        /// create the Java AST.
        /// </summary>
        /// <returns></returns>
        public string GenerateCSharpAST()
        {
            StringBuilder sb = new StringBuilder();
            this.GenerateCSharpAST(sb, 0, 1, 0);
            return sb.ToString();
        }


        /// <summary>
        /// Constructs the AST recursively from the ANTLR Java Parser classes.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static JavaAST ConstructAST(TerminalNodeImpl ctx)
        {


            if (literals.Contains(ctx.Symbol.Type))
            {
                return JavaAST.CreateToken(ctx.Symbol.Type, ctx.GetText());
            }
            else
            {
                return JavaAST.CreateToken(ctx.Symbol.Type);
            }
        }


        /// <summary>
        /// Constructs the AST recursively from the ANTLR Java Parser classes.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static JavaAST ConstructAST(ParserRuleContext ctx)
        {
            if (ctx.children != null)
            {

                if (ctx.children.Count > 1)
                {
                    var ast = JavaAST.CreateRule(ctx.RuleIndex);

                    foreach (var child in ctx.children)
                    {
                        var childCtx = child;

                        if (childCtx is ParserRuleContext)
                        {
                            var childAst = ConstructAST(childCtx as ParserRuleContext);
                            if (childAst != null)
                                ast.Children.Add(childAst);
                        }
                        if (childCtx is TerminalNodeImpl)
                        {
                            var childAst = ConstructAST(childCtx as TerminalNodeImpl);
                            if (childAst != null)
                                ast.Children.Add(childAst);
                        }
                    }

                    return ast;
                }
                else if (ctx.children.Count == 1)
                {
                    if (ctx.children[0] is ParserRuleContext)
                        return ConstructAST(ctx.children[0] as ParserRuleContext);
                    else
                        return ConstructAST(ctx.children[0] as TerminalNodeImpl);
                }
                else
                {
                    return null;
                }
            }
            else
                return null;
        }

    }
}
