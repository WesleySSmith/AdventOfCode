using MoreLinq;

string[] lines = File.ReadAllLines("input.txt"); 
//string[] lines = File.ReadAllLines("sample.txt"); 
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

var diags = lines;
//Part1(diags);
Part2(diags);


static void Part1(IEnumerable<string> diags) {
    int numCols = diags.First().Count();
    int gamma = 0;
    int epsilon = 0;
    for (int ii = 0; ii < numCols; ii++) {
        int count0 = 0;
        int count1 = 0;
        foreach (var diag in diags) {
            if (diag[ii] == '0') {
                count0++;
            } else {
                count1++;
            }
        }
        gamma <<= 1;
        epsilon <<= 1;
        gamma |= count0 > count1 ? 0 : 1;
        epsilon |= count0 > count1 ? 1 : 0;
    }

    Console.Out.WriteLine($"Part 1 gamma: {gamma}, epsilon: {epsilon}: answer: {gamma * epsilon}");
}

static void Part2(IEnumerable<string> diags) {
    int numCols = diags.First().Count();

    var o2Diags = diags;
    for (int ii = 0; ii < numCols; ii++) {
        int count0 = 0;
        int count1 = 0;
        foreach (var diag in o2Diags) {
            if (diag[ii] == '0') {
                count0++;
            } else {
                count1++;
            }
        }

        char filter = count0 > count1 ? '0' :'1';
        o2Diags = o2Diags.Where(d => d[ii] == filter).ToArray();
        if (o2Diags.Count() == 1) {
            break;
        }
    }
    var o2 = Convert.ToInt32(o2Diags.Single(), 2); 

    var co2Diags = diags;
    for (int ii = 0; ii < numCols; ii++) {
        int count0 = 0;
        int count1 = 0;
        foreach (var diag in co2Diags) {
            if (diag[ii] == '0') {
                count0++;
            } else {
                count1++;
            }
        }

        char filter = count0 > count1 ? '1' :'0';
        co2Diags = co2Diags.Where(d => d[ii] == filter).ToArray();
        if (co2Diags.Count() == 1) {
            break;
        }
    }

    var co2 = Convert.ToInt32(co2Diags.Single(), 2); 

    Console.Out.WriteLine($"Part 2 o2: {o2}, co2: {co2}: answer: {o2 * co2}");
}





