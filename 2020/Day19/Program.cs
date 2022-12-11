using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
//Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

var rules = new Dictionary<int, Rule>();
var counter = 0;
while(true) {
    var line = lines[counter++];
    if (line.Length == 0) {
        break;
    }
    var parts = line.Split(": ");
    var ruleNum = int.Parse(parts[0]);

    Rule rule = ParseRule(parts[1]);
    rules[ruleNum] = rule;
}

var messages = lines[counter..^0];

int matches = 0;
foreach(var message in messages) {
    var consumed = Consume2(0, message);
    bool match = consumed.Any(m => m == message.Length);
    matches += match ? 1 :0;
    Console.Out.WriteLine($"{match}: {message}");
}
Console.Out.WriteLine($"Matches: {matches}");


int Consume(int ruleNum, string message) {
    var rule = rules[ruleNum];
    if (rule.Terminal.HasValue) {
        if (message.Length > 0 && message[0] == rule.Terminal.Value) {
            return 1;
        } else {
            return 0;
        }
    }
    foreach (var option in rule.Options) {
        var consumed = ConsumeSequence(option, message);
        if (consumed > 0) {
            return consumed;
        }
    }
    return 0;
}

int[] Consume2(int ruleNum, string message) {
    var rule = rules[ruleNum];
    if (rule.Terminal.HasValue) {
        if (message.Length > 0 && message[0] == rule.Terminal.Value) {
            return new int[] {1};
        } else {
            return new int[0];
        }
    }
    var consumes = new List<int>();
    foreach (var option in rule.Options) {
        var consumed = ConsumeSequence2(option, message);
        consumes.AddRange(consumed);
    }
    return consumes.ToArray();
}

int ConsumeSequence(int[] rules, string message) {
    var messagePos = 0;
    foreach (var ruleId in rules) {
        var consumed = Consume(ruleId, message[messagePos..]);
        if (consumed == 0) {
            return 0;
        }
        messagePos += consumed;
    }
    return messagePos; 
}

int[] ConsumeSequence2(int[] rules2, string message) {

var finalPossibleConsumeCounts = new List<int>();
    if (rules2.Length == 0) {
        return new int[0];
    }
    var firstRule = rules2[0];
    var possibleConsumeCountsForFirstRule = Consume2(firstRule, message);
    if (possibleConsumeCountsForFirstRule.Length == 0 || rules2.Length == 1) {
        return possibleConsumeCountsForFirstRule;
    }

    foreach (var possibleConsumeCount in possibleConsumeCountsForFirstRule) {
        var consumeCountsForRestOfRules = ConsumeSequence2(rules2[1..], message[possibleConsumeCount..]);
        finalPossibleConsumeCounts.AddRange(consumeCountsForRestOfRules.Select(c => c + possibleConsumeCount));
    }
    return finalPossibleConsumeCounts.ToArray(); 
}


Rule ParseRule(string s) {
 if (s[0] == '"') {
        return new Rule() {Terminal = s[1]};
    }
    else {
        var splits = s.Split(" | ");
        var options = new List<int[]>();
        foreach (var split in splits) {
            options.Add(split.Split(' ').Select(int.Parse).ToArray());
        }
        return new Rule {Options = options.ToArray()};
    }
}

class Rule {
    public int[][] Options;
    public char? Terminal;
}
