#pragma warning disable CS8524
#pragma warning disable CS8509
using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");
var root = Parse(lines);
ComputeSize(root);
//Part1(root);
Part2(root);


static Dir Parse(string[] lines) {
    Dir root = new Dir {Name = "/"};
    Dir current = root;
    foreach (var line in lines.Skip(1)) {
        if (line.StartsWith("$ cd ")) {
            var path = line[5..];
            if (path == "..") {
                current = current.Parent;
            } else if (path == "/") {
                current = root;
            } else {
                current = current.Dirs.Single(d => d.Name == path);
            }
        } else if (line.StartsWith("$ ls")) {

        }
        else if (line.StartsWith("dir")) {
            var path = line[4..];
            if (current.Dirs.Any(d => d.Name == path)) {
                throw new Exception($"Duplicate Dir: {path}");
            }
            current.Dirs.Add(new Dir() {Name = path, Parent = current});
        } else {
            var parts = line.Split(" ");
            var size = int.Parse(parts[0]);
            var name = parts[1];
            if (current.Files.Any(f => f.Name == name)) {
                throw new Exception($"Duplicate File: {name}");
            }
            current.Files.Add(new DataFile {Name = name, Size = size});
        }
    }
    return root;
}

static long ComputeSize(Dir dir) {
    dir.Size = dir.Files.Sum(f => f.Size) + dir.Dirs.Sum(d => ComputeSize(d));
    return dir.Size;
}

static void Part1(Dir root) {
   
    var sum = root.Descendants().Where(d => d.Size <= 100_000).Sum(d => d.Size);
   
    Console.Out.WriteLine($"Part 1 Sum: {sum}");
}

static void Part2(Dir root) {
   
    var totalFS = 70_000_000;
    var needFS = 30_000_000;
    var currentUsed = root.Size;
    
    var unusedFS = totalFS - currentUsed;
    var deleteAtLeast =needFS - unusedFS;
    Console.Out.WriteLine($"Current Unused: {unusedFS}, Delete At Least: {deleteAtLeast}");
    var sizeOfDirToDelete = root.Descendants().Where(d => d.Size >= deleteAtLeast).Min(d => d.Size);
   
    Console.Out.WriteLine($"Part 2 Size of Dir to delete: {sizeOfDirToDelete}");
}


public record Dir {
    public string Name;
    public List<Dir> Dirs = new List<Dir>();
    public List<DataFile> Files = new List<DataFile>();
    public Dir Parent;

    public long Size;

    public IEnumerable<Dir> Descendants() {
        return Dirs.Concat(Dirs.SelectMany(d => d.Descendants()));
    }
}

public record DataFile {
    public string Name;
    public int Size;
}