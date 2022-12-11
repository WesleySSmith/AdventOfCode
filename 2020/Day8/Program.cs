using System;
using System.IO;
using System.Linq;

namespace Day8
{
    class Program
    {
        enum Op {
            Nop,
            Acc,
            Jmp
        }

        readonly struct Instruction {
            public Op Op { get; init; }
            public int Arg { get; init; }
        }

        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");
            //string[] lines = File.ReadAllLines("sample.txt");
            Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

            var instructions = lines
                .Select(l => l.Split(' '))
                .Select(parts => new Instruction {
                    Op = (Op)Enum.Parse(typeof(Op), parts[0], ignoreCase: true),
                    Arg = int.Parse(parts[1]) 
                })
                .ToArray();
// Part 1
            var result = Run1(instructions);
            Console.Out.WriteLine($"Accumulator: {result.Accumulator}, Exited: {result.Exited}");

//Part 2 
           
            result = Run2(instructions);
            Console.Out.WriteLine($"Accumulator: {result.Accumulator}, Exited: {result.Exited}");
        }

        private static (int Accumulator, bool Exited) Run2(Instruction[] instructions) {
           
             for (int ii = 0; ii < instructions.Length; ii++) {
                var instruction = instructions[ii];
                var op = instruction.Op;
                if (op == Op.Acc) {
                    continue;
                }
                var switchedOp = op == Op.Jmp ? Op.Nop : Op.Jmp;
                var switchedInstruction = new Instruction {Op = switchedOp, Arg = instruction.Arg};
                instructions[ii] = switchedInstruction;
                var result = Run1(instructions);
                if (result.Exited) {
                    Console.Out.WriteLine($"***Fixed program by modifying line {ii}");
                    return result;
                }
                instructions[ii] = instruction;
            }
            Console.Out.WriteLine($"***Failed to fix program");
            return (0, false);
        }

        private static (int Accumulator, bool Exited) Run1(Instruction[] instructions) {
            bool[] visited = new bool[instructions.Length];
            var pc = 0;
            var acc = 0;
            while(pc < instructions.Length) {
                var instruction = instructions[pc];
                if (visited[pc]) {
                    break;
                }
                visited[pc] = true;
                switch (instruction.Op) {
                    case Op.Acc:
                        acc += instruction.Arg;
                        pc++;
                        break;
                    case Op.Jmp:
                        pc += instruction.Arg;
                        break;
                    case Op.Nop:
                        pc++;
                        break;
                }
            }
            return (acc, pc == instructions.Length);
        }

        
    }
}
