using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MoreLinq;
using System.Diagnostics;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 

var rules = new List<Rule>();
int lineIndex = 0;
while(true) {
    var line = lines[lineIndex++];
    if (line.Length == 0) {
        break;
    }
    var parts = line.Split(": ");

    var rule = new Rule();
    rule.Name = parts[0];
    var ranges = parts[1].Split(" or" );
    var r1 = ranges[0].Split('-');
    var r2 = ranges[1].Split('-');
    rule.R1 = (short.Parse(r1[0]), short.Parse(r1[1]));
    rule.R2 = (short.Parse(r2[0]), short.Parse(r2[1]));
    rules.Add(rule);
}

lineIndex++; // skip "your ticket"
var your = lines[lineIndex++].Split(',').Select(short.Parse).ToArray();

lineIndex++; // skip blank line
lineIndex++; // skip "nearby tickets"
var nearby = lines[lineIndex..].Select(l => l.Split(',').Select(short.Parse).ToArray());

//rules.Trace(r => $"{r.Name}: {r.R1.Item1}-{r.R1.Item2} , {r.R2.Item1}-{r.R2.Item2}").Consume();
//(new [] {your}).Trace(t => "Your: " + t.ToDelimitedString(",")).Consume();
//(nearby).Trace(t => t.ToDelimitedString(",")).Consume();

var valid = nearby.Where(ticket => ticket.All(field => rules.Any(r => r.Valid(field)))).ToArray();

Console.Out.WriteLine($"Valid ticket: {valid.Count()} of {nearby.Count()}" );

var numFields = your.Count();

var map = new bool[rules.Count(), numFields];

for (int ruleId = 0; ruleId < rules.Count(); ruleId++) {
    for(int fieldId = 0; fieldId < numFields; fieldId++) {
        map[ruleId, fieldId] = true;
        foreach (var ticket in valid) {
            if (!rules[ruleId].Valid(ticket[fieldId])) {
                map[ruleId, fieldId] = false;
                break;
            }
        }
    }
}
bool keepGoing = true;
int breaker = 100;
while (keepGoing && breaker-- > 0) {
    keepGoing = false;
    for (int ruleId = 0; ruleId < rules.Count(); ruleId++) {
        int count = 0;
        int foundFieldId = -1;
        for(int fieldId = 0; fieldId < numFields; fieldId++) {
            if (map[ruleId,fieldId]) {
                foundFieldId = fieldId;
                count++;
            }
        }
        if (count == 1) {
            for (int ruleId2 = 0; ruleId2 < rules.Count(); ruleId2++) {
                if (ruleId2 != ruleId) {
                    map[ruleId2, foundFieldId] = false;
                }
            }
        } else {
            keepGoing = true;
        }
    }
}

for (int ruleId = 0; ruleId < rules.Count(); ruleId++) {
    int count = 0;
    for(int fieldId = 0; fieldId < numFields; fieldId++) {
        if (map[ruleId,fieldId]) {
            rules[ruleId].Index = fieldId;
            count++;
        }
    }
}


var answer = rules.Where(r => r.Name.StartsWith("departure")).Select(r => your[r.Index]).Aggregate(1L, (a,b) => a * b);
Console.Out.WriteLine($"Answer: {answer}");



class Rule {
    public string Name;
    public (short, short) R1;
    public (short, short) R2;
    public bool Valid(short value) => (value >= R1.Item1 && value <= R1.Item2) || (value >= R2.Item1 && value <= R2.Item2);
    public int Index;
}

