using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

// foreach (var line in lines) {
//     var tokens = Tokenize(line);
//     var postFix = InfixToPostfix(tokens);
//     var value = EvaluatePostfix(postFix);
//     Console.Out.WriteLine($"{line}: {value}");
// }

var total = lines.Select(Tokenize).Select(InfixToPostfix).Select(EvaluatePostfix).Sum();
Console.Out.WriteLine($"Total: {total}");

List<Token> InfixToPostfix(List<Token> tokens) {

    var prec = new Dictionary<TokenType, int> {
        {TokenType.Open, 1},
        {TokenType.Plus , 3},
        {TokenType.Mult, 2}
    };

    var output = new List<Token>();
    var opStack = new Stack<Token>();
    foreach (var token in tokens) {
        switch (token.Type) {
            case TokenType.Number:
                output.Add(token);
                break;
            case TokenType.Open:
                opStack.Push(token);
                break;
            case TokenType.Close:
                var top = opStack.Pop();
                while (top.Type != TokenType.Open) {
                    output.Add(top);
                    top = opStack.Pop();
                }
                break;
            case TokenType.Mult:
            case TokenType.Plus:
               while (opStack.Count > 0 && prec[opStack.Peek().Type] >= prec[token.Type]) {
                   output.Add(opStack.Pop());
               }
                opStack.Push(token);
                break;
        }
    }
    while (opStack.Count > 0) {
        output.Add(opStack.Pop());
    }

    return output;
}

long EvaluatePostfix(List<Token> tokens) {
    var operandStack = new Stack<long>();
    foreach (var token in tokens) {
        switch (token.Type) {
            case TokenType.Number:
                operandStack.Push(token.Number.Value);
                break;
            case TokenType.Mult:
            case TokenType.Plus:
                var op1 = operandStack.Pop();
                var op2 = operandStack.Pop();
                if (token.Type == TokenType.Mult) {
                    operandStack.Push(op1 * op2);
                } else {
                    operandStack.Push(op1 + op2);
                }
                break;
        }
    }
    return operandStack.Pop();
}

List<Token> Tokenize(string line) {
    var tokens = new List<Token>();

    int p = 0;
    while (p < line.Length) {
        var c = line[p++];
        if (c == '+') {
            tokens.Add(new Token(TokenType.Plus));
        } else if (c == '*') {
            tokens.Add(new Token(TokenType.Mult));
        } else if (c == '(') {
            tokens.Add(new Token(TokenType.Open));
        } else if (c == ')') {
            tokens.Add(new Token(TokenType.Close));
        } else if (c >= '0' && c <= '9') {
            tokens.Add(new Token(c - '0'));
        }
    }
    return tokens;
}


enum TokenType {
    Number,
    Open,
    Close,
    Plus,
    Mult
}

class Token {
    public TokenType Type;
    public int? Number;

    public Token(TokenType type) {
        this.Type = type;
    }

    public Token(int number) {
        this.Number = number;
        this.Type = TokenType.Number;
    }
}



class Element {
    public int? Value;
    public Element L;
    public Op Op;
    public Element R;

    public Element(int value) {
        this.Value = value;
    }
    public Element() {
    }
}

enum Op {
    Plus,
    Mult
}
