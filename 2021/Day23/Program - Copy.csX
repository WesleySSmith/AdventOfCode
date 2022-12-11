//using MoreLinq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Numerics;

public class Day18 {

    public static bool Debug = false;
    public static void Main() {
        //string[] lines = File.ReadAllLines("input.txt"); 
        //string[] lines = File.ReadAllLines("sample.txt"); 
        //Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
        var sw = Stopwatch.StartNew();
     
        var s = new State();

        // Input full
        s[12] = 'A';
        s[13] = 'C';
        s[14] = 'D';
        s[15] = 'D';
        s[16] = 'C';
        s[17] = 'B';
        s[18] = 'A';
        s[19] = 'B';

        // Example full
        // s[12] = 'B';
        // s[13] = 'A';
        // s[14] = 'C';
        // s[15] = 'D';
        // s[16] = 'B';
        // s[17] = 'C';
        // s[18] = 'D';
        // s[19] = 'A';



        // Example n-1
        // s[10] = 'A';
        // s[13] = 'A';
        // s[14] = 'B';
        // s[15] = 'B';
        // s[16] = 'C';
        // s[17] = 'C';
        // s[18] = 'D';
        // s[19] = 'D';

        // Example n-2
        // s[6] = 'D';
        // s[8] = 'D';
        // s[10] = 'A';
        // s[13] = 'A';
        // s[14] = 'B';
        // s[15] = 'B';
        // s[16] = 'C';
        // s[17] = 'C';

        // Example n-4
        // s[6] = 'D';
        // s[12] = 'B';
        // s[13] = 'A';
        // s[15] = 'B';
        // s[16] = 'C';
        // s[17] = 'C';
        // s[18] = 'D';
        // s[19] = 'A';

        // Example n-5
        // s[4] = 'B';
        // s[12] = 'B';
        // s[13] = 'A';
        // s[15] = 'D';
        // s[16] = 'C';
        // s[17] = 'C';
        // s[18] = 'D';
        // s[19] = 'A';


        Console.Out.WriteLine($"Parse time: {sw.ElapsedMilliseconds}");

        Part1(s);
        //Part2(instructions);

        Console.Out.WriteLine($"Total time {sw.ElapsedMilliseconds}");
    }

    static void Part1(State s) {
        int counter = 0;
        //PriorityQueue<State, int> queue = new();
        Stack<State> stack = new();
        int bestWinEnergy = int.MaxValue;
        //queue.Enqueue(s, s.CumulativeWeight);
        stack.Push(s);
        //while (queue.Count != 0) {
        while (stack.Any()) {
            //var top = queue.Dequeue();
            var top = stack.Pop();
            if (top.CumulativeWeight > bestWinEnergy) {
                continue;
            }
            if (top.IsWin) {
                
                if (top.CumulativeWeight < bestWinEnergy) {
                    Console.Out.WriteLine($"WIN with {top.CumulativeWeight}");
                    bestWinEnergy = top.CumulativeWeight;
                } else {
                    Console.Out.WriteLine($"win with {top.CumulativeWeight}");
                }
            } else {
                var next = Burrow.GetNextStates(top);
                var firstMoveIntoRoom = next.FirstOrDefault(n => n.IsMoveIntoRoom);
                if (firstMoveIntoRoom != null) {
                   stack.Push(firstMoveIntoRoom);
                } else {
                    foreach (var n in next) {
                        if (n.CumulativeWeight < bestWinEnergy) {
                            //queue.Enqueue(n, n.CumulativeWeight);
                            stack.Push(n);
                        }
                    }
                }
            }

            if (counter++ % 10000 == 0) {
                Console.Out.WriteLine($"... {counter} - best: {bestWinEnergy} - current Stack Length: {stack.Count} - energy: {top.CumulativeWeight}");
            }
        }
        Console.Out.WriteLine($"Best: {bestWinEnergy}");
    }

   
    static void Part2(State s) {
       
      
    }

    public class Cell {
        public char? Limited;
    }

    public static class Burrow {
        public static Cell[] Cells = new Cell[20];
        public static List<int>[,] Path = new List<int>[20,20];
        static Burrow()
        {
            for (var ii = 1; ii <= 19; ii++) {
                Cells[ii] = new Cell();
            }
            Cells[12].Limited = 'A';
            Cells[13].Limited = 'A';
            Cells[14].Limited = 'B';
            Cells[15].Limited = 'B';
            Cells[16].Limited = 'C';
            Cells[17].Limited = 'C';
            Cells[18].Limited = 'D';
            Cells[19].Limited = 'D';

            for (int hallPos = 1; hallPos <= 11; hallPos++) {
                if (hallPos switch {3 or 5 or 7 or 9 => true, _ => false}) {
                    continue;
                }
                for (int roomPos = 12; roomPos <= 19; roomPos++) {
                    Path[hallPos, roomPos] = HallToRoomPath(hallPos, roomPos);
                }
            }

            for (int roomPos = 12; roomPos <= 19; roomPos++) {
                for (int hallPos = 1; hallPos <= 11; hallPos++) {
                    if (hallPos switch {3 or 5 or 7 or 9 => true, _ => false}) {
                        continue;
                    }
                    Path[roomPos, hallPos] = RoomToHallPath(roomPos, hallPos);
                }
            }

            for (int roomPos1 = 12; roomPos1 <= 19; roomPos1++) {
                for (int roomPos2 = 12; roomPos2 <= 19; roomPos2++) {
                    if (roomPos1 == roomPos2) {
                        continue;
                    }
                    if (LobbyFromRoom(roomPos1) == LobbyFromRoom(roomPos2)) {
                        continue;
                    }
                    Path[roomPos1, roomPos2] = RoomToRoomPath(roomPos1, roomPos2);
                }
            }
        }

        public static List<State> GetNextStates(State s) {

            if (s.IsWin) {
                return null;
            }
            List<State> result = new();

            for (int startPos = 1; startPos <= 19; startPos++) {
                if (!s[startPos].HasValue) {
                    continue;
                }

                var c = s[startPos].Value;

                if (
                    (startPos == 13 || startPos == 15 || startPos == 17 || startPos == 19)
                    && Burrow.Cells[startPos].Limited.Value == c
                ) {
                    // Already in good bottom spot in room
                    continue;
                }

                if (
                    (startPos == 12 || startPos == 14 || startPos == 16 || startPos == 18)
                    && Burrow.Cells[startPos].Limited.Value == c
                    && s[startPos+1] == c
                ) {
                    // Already in good top spot in room
                    continue;
                }

                
                for (int endPos = 1; endPos <= 19; endPos++) {
                    var potentialPath = Path[startPos, endPos];
                    if (potentialPath == null) {
                        continue;
                    }
                    if (!PathIsClear(potentialPath, s)) {
                        continue;
                    }

                    var limited = Burrow.Cells[endPos].Limited;
                    if (limited.HasValue && limited.Value != c) {
                        continue;
                    }

                    if (IsFirstRoom(endPos)) {

                        if (s[endPos+1] == null) {
                            // don't stop at top if bottom of room is available
                            continue;
                        }
                        if (s[endPos+1] != Burrow.Cells[endPos+1].Limited.Value)
                        {
                            // don't enter a room if there's a non-eligible amphipod in
                            // the bottom
                            continue;
                        }
                    }

                    
                    var newState = new State(s);
                    newState[startPos] = null;
                    newState[endPos] = c;
                    newState.IsMoveIntoRoom = IsRoom(endPos);
                    newState.Weight = c switch {
                        'A' => 1,
                        'B' => 10,
                        'C' => 100,
                        'D' => 1000,
                        _ => throw new Exception("GGG")
                    } * potentialPath.Count();
                    newState.CumulativeWeight = s.CumulativeWeight + newState.Weight;
                    result.Add(newState);
                }
            }

            if (Debug) {
                Console.Out.WriteLine($"Next states for:\n{s}");
                foreach (var r in result) {
                    Console.Out.WriteLine(r);
                }
            }
            return result;
        }

        public static bool PathIsClear(List<int> path, State s) {
            return path.All(c => !s[c].HasValue);
        }
    }

    public class State {
        private char?[] _p = new char?[19];
        public int Weight {get; set;}
        public int CumulativeWeight {get; set;}
        public bool IsMoveIntoRoom;


        public State() {
        }

        public State(State other) {
            for (int ii = 1; ii <= 19; ii++) 
            {
                this[ii] = other[ii];
            }
        }
        public char? this[int i] {
            get {
                return _p[i-1];
            } set {
                _p[i-1] = value;
            }
        }

        public bool IsWin { 
            get {
                return this[12] == 'A' && this[13] == 'A'
                && this[14] == 'B' && this[15] == 'B'
                && this[16] == 'C' && this[17] == 'C'
                && this[18] == 'D' && this[19] == 'D';
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("#############\n#");
            for (int ii = 1; ii <= 11; ii++) {
                sb.Append(this[ii] ?? '.' );
            }
            sb.Append("#\n");
            sb.Append($"###{this[12] ?? '.'}#{this[14] ?? '.'}#{this[16] ?? '.'}#{this[18] ?? '.'}###\n");
            sb.Append($"  #{this[13] ?? '.'}#{this[15] ?? '.'}#{this[17] ?? '.'}#{this[19] ?? '.'}#\n");
            sb.Append("  #########\n");
            return sb.ToString();
        }
    }


    public static int LobbyFromRoom(int roomPos) {
        return roomPos switch {12 or 13 => 3, 14 or 15 => 5, 16 or 17 => 7, 18 or 19 => 9, _ => throw new Exception("AAA")};
    }

    public static int FirstRoomFromLobby(int lobby) {
        return lobby switch {3 => 12, 5 => 14, 7 => 16, 9 => 18, _ => throw new Exception("FFF")};
    }

    public static bool IsFirstRoom(int roomPos) {
        return roomPos switch {12 or 14 or 16 or 18 => true, _ => false};
    }

    public static bool IsRoom(int pos) {
        return pos >= 12 && pos <= 19;

    }

    public static List<int> HallToRoomPath(int hallPos, int roomPos) {
        List<int> result = new();
        int lobby = LobbyFromRoom(roomPos);
        // Move from hallPos to lobby
        if (hallPos > lobby) {
            for (var ii = hallPos - 1; ii != lobby; ii--) {
                result.Add(ii);
            }
        } else if (hallPos < lobby) {
            for (var ii = hallPos + 1; ii != lobby; ii++) {
                result.Add(ii);
            }
        } else {
            throw new Exception("BBB");
        }
        result.Add(lobby);
        //Move from lobby to room
        var firstRoom = FirstRoomFromLobby(lobby);
        result.Add(firstRoom);
        if (firstRoom != roomPos) {
            result.Add(firstRoom+1);
        }
        return result;
        
    }

     public static List<int> RoomToHallPath(int roomPos, int hallPos) {
        List<int> result = new();
        int lobby = LobbyFromRoom(roomPos);
        // Move from roomPos to lobby

        if (roomPos != FirstRoomFromLobby(lobby)) {
            result.Add(roomPos - 1);
            result.Add(lobby);
        }
        else {
            result.Add(lobby);
        }

        // Move from lobby to hall Pos

        if (hallPos > lobby) {
            for (var ii = lobby + 1; ii != hallPos; ii++) {
                result.Add(ii);
            }
        } else if (hallPos < lobby) {
            for (var ii = lobby - 1; ii != hallPos; ii--) {
                result.Add(ii);
            }
        } else {
            throw new Exception("DDD");
        }
        result.Add(hallPos);
        
        return result;
    }


     public static List<int> RoomToRoomPath(int roomPos1, int roomPos2) {
        List<int> result = new();
        int lobby1 = LobbyFromRoom(roomPos1);
        int lobby2 = LobbyFromRoom(roomPos2);
        // Move from room1Pos to lobby1

        if (roomPos1 != FirstRoomFromLobby(lobby1)) {
            result.Add(roomPos1 - 1);
            result.Add(lobby1);
        }
        else {
            result.Add(lobby1);
        }

        // Move from lobby1 to lobby2

        if (lobby1 < lobby2) {
            for (var ii = lobby1 + 1; ii != lobby2; ii++) {
                result.Add(ii);
            }
        } else if (lobby1 > lobby2) {
            for (var ii = lobby1 - 1; ii != lobby2; ii--) {
                result.Add(ii);
            }
        } else {
            throw new Exception("EEE");
        }
        result.Add(lobby2);

        // Move from lobby2 to roomPos2

        //Move from lobby to room
        var firstRoom = FirstRoomFromLobby(lobby2);
        result.Add(firstRoom);
        if (firstRoom != roomPos2) {
            result.Add(firstRoom+1);
        }
        return result;
    }
}
