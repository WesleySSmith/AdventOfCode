//using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Numerics;

public class Day15 {

    public static void Main() {
        string[] lines = File.ReadAllLines("input.txt"); 
        //string[] lines = File.ReadAllLines("sample.txt"); 
        Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
        var sw = Stopwatch.StartNew();

        var bits = lines
            .Single()
            .Select(c => byte.Parse(c.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier))
            .SelectMany(b => new [] {(b & 0b1000) > 0, (b & 0b0100) > 0 , (b & 0b0010) > 0, (b & 0b0001) > 0})
            .ToArray();
       
        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");

        //Console.Out.WriteLine(bits.Select(b => b ? '1' : '0').ToArray());
        //Part1(bits);
        Part2(bits);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static void Part1(bool[] bits) {
        var packet = ParsePacket(bits, 0);
        var sum = SumPacket(packet.packet);
        Console.Out.WriteLine($"Sum: {sum}");
    }

    static void Part2(bool[] bits) {
        var packet = ParsePacket(bits, 0);
        var value = ValuePacket(packet.packet);
        Console.Out.WriteLine($"Value: {value}");
    }

    static int SumPacket(Packet p) {
        return p.Version + (p.Packets != null ? p.Packets.Sum(p => SumPacket(p)) : 0);
    }

    static long ValuePacket(Packet p) {

        return p.Type switch { 
            0 => p.Packets.Sum(p => ValuePacket(p)),
            1 => p.Packets.Aggregate(1L, (acc, p) => acc * ValuePacket(p) ),
            2 => p.Packets.Min(p => ValuePacket(p)),
            3 => p.Packets.Max(p => ValuePacket(p)),
            4 => p.Value,
            5 => ValuePacket(p.Packets.ElementAt(0)) > ValuePacket(p.Packets.ElementAt(1)) ? 1 : 0,
            6 => ValuePacket(p.Packets.ElementAt(0)) < ValuePacket(p.Packets.ElementAt(1)) ? 1 : 0,
            7 => ValuePacket(p.Packets.ElementAt(0)) == ValuePacket(p.Packets.ElementAt(1)) ? 1 : 0,
            _ => throw new Exception("Oops")
        };
    }

    static (Packet packet, int posOffset) ParsePacket(bool[] bits, int pos) {
        int posOffset = 0;
        int version = (int)ToLong(bits[pos..(pos+3)]);
        posOffset += 3;
        int type = (int)ToLong(bits[(pos+posOffset)..(pos+posOffset+3)]);
        posOffset += 3;
        switch (type) {
            case 4:
                // literal
                var literalResult = ParseLiteralPacket(bits, pos + posOffset);
                posOffset += literalResult.posOffset;
                return (new Packet {Version = version, Type = type, Value = literalResult.value}, posOffset);
            default:
                // everything else
                var operatorResult = ParseOperatorPacket(bits, pos + posOffset);
                posOffset += operatorResult.posOffset;
                return (new Packet {Version = version, Type = type, Packets = operatorResult.packets}, posOffset);
        }
    }

    static (IEnumerable<Packet> packets, int posOffset) ParseOperatorPacket(bool[] bits, int pos) {
        int posOffset = 0;
        int lengthType = bits[pos] ? 1 : 0;
        posOffset += 1;
        var subPackets = new List<Packet>();
        if (lengthType == 0) {
            long subPacketLength = ToLong(bits[(pos+posOffset)..(pos+posOffset+15)]);
            posOffset += 15;
            while (subPacketLength > 0) {
                var subPacket = ParsePacket(bits, pos + posOffset);
                posOffset += subPacket.posOffset;
                subPackets.Add(subPacket.packet);
                subPacketLength -= subPacket.posOffset;
            }
        } 
        else {
            long subPacketCount = ToLong(bits[(pos+posOffset)..(pos+posOffset+11)]);
            posOffset += 11;
            for (var _ = 0; _ < subPacketCount; _++) {
                var subPacket = ParsePacket(bits, pos + posOffset);
                posOffset += subPacket.posOffset;
                subPackets.Add(subPacket.packet);
            }
        }
        return (subPackets, posOffset);
    }
    static (long value, int posOffset) ParseLiteralPacket(bool[] bits, int pos) {
        int posOffset = 0;
        long value = 0;
        while (true) {
            var isLast = !bits[pos+posOffset];
            var segement = ToLong(bits[(pos+posOffset+1)..(pos+posOffset+5)]);
            posOffset += 5;
            value <<= 4;
            value |= segement;
            if (isLast) {
                break;
            }
        }
        return (value, posOffset);
    }
    static long ToLong(ReadOnlySpan<bool> bits) {
        long x = 0;
        foreach (var bit in bits) {
            x <<= 1;
            x |= bit ? 1 : 0;
        }
        return x;
    }

    class Packet {
        public int Version;
        public int Type;
        public long Value;
        public IEnumerable<Packet> Packets;
    }

   
}
