#pragma warning disable CS8524
#pragma warning disable CS8509
using System.Collections;
using System.Data;
using System.Diagnostics;
using MoreLinq;

bool sample = false;

string[] lines = File.ReadAllLines(sample ? "sample.txt" : "input.txt");
Console.Out.WriteLine($"Read {lines.Length} lines from {lines.First()} to {lines.Last()}");

Stopwatch sw = Stopwatch.StartNew();

var modules = lines.Select(Module.ParseModule).ToDictionary(m => m.Name);

//Part1(modules);
Part2(modules);

Console.Out.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");


void Part1(Dictionary<string, Module> modules) 
{

    var sinks = modules.Values.SelectMany(m => m.DestinationModules).Distinct().Where(name => !modules.ContainsKey(name)).ToList();
    foreach (var sink in sinks) {
        modules.Add(sink, new Sink() {Name = sink, Type = ModuleType.Sink, DestinationModules = []});
    }

    foreach (var module in modules.Values) {
        var inputModules = modules.Values.Where(m => m.DestinationModules.Contains(module.Name)).Select(m => m.Name);
        module.SetInputModules(inputModules);
    }


    var lowPulses = 0L;
    var highPulses = 0L;

    for (var ii = 0; ii < 1000; ii++) {

        //Console.WriteLine($"\n\nButton Press {ii+1}");
        Queue<PulseEvent> q = new Queue<PulseEvent>();
        q.Enqueue(new PulseEvent {DestinationModule = "broadcaster", Pulse = Pulse.Low, SourceModule = "button"});
        while (q.TryDequeue(out var pulseEvent)) {

            //Console.WriteLine(pulseEvent.ToString());

            lowPulses += pulseEvent.Pulse == Pulse.Low ? 1 : 0;
            highPulses += pulseEvent.Pulse == Pulse.High ? 1 : 0;

            var module = modules[pulseEvent.DestinationModule];
            module.SetPulse(pulseEvent.Pulse, pulseEvent.SourceModule);
            var nextPulseEvents = module.Process();
            foreach (var nextPulseEvent in nextPulseEvents) {
                q.Enqueue(nextPulseEvent);
            }
        }
    }
    
    Console.Out.WriteLine($"Low Pulses: {lowPulses}.  High Pulses: {highPulses}.  Score: {lowPulses * highPulses}");


}

void Part2(Dictionary<string, Module> modules) 
{
 var sinks = modules.Values.SelectMany(m => m.DestinationModules).Distinct().Where(name => !modules.ContainsKey(name)).ToList();
    foreach (var sink in sinks) {
        modules.Add(sink, new Sink() {Name = sink, Type = ModuleType.Sink, DestinationModules = []});
    }

    foreach (var module in modules.Values) {
        var inputModules = modules.Values.Where(m => m.DestinationModules.Contains(module.Name)).Select(m => m.Name);
        module.SetInputModules(inputModules);
    }

    var lowPulses = 0L;
    var highPulses = 0L;

    for (var ii = 0; ii < 10_000; ii++)
    {
        PushButton(modules, ref lowPulses, ref highPulses, ii + 1);
    }
}


static void PushButton(Dictionary<string, Module> modules, ref long lowPulses, ref long highPulses, long pushNum)
{

    Queue<PulseEvent> q = new Queue<PulseEvent>();
    q.Enqueue(new PulseEvent { DestinationModule = "broadcaster", Pulse = Pulse.Low, SourceModule = "button" });
    while (q.TryDequeue(out var pulseEvent))
    {
        if (pulseEvent.SourceModule is "ft" or "qr" or "lk" or "lz" && pulseEvent.Pulse == Pulse.Low) {
            Console.WriteLine($"{pulseEvent.SourceModule} emits low on {pushNum}");
        }

        lowPulses += pulseEvent.Pulse == Pulse.Low ? 1 : 0;
        highPulses += pulseEvent.Pulse == Pulse.High ? 1 : 0;

        var module = modules[pulseEvent.DestinationModule];
        module.SetPulse(pulseEvent.Pulse, pulseEvent.SourceModule);
        var nextPulseEvents = module.Process();
        foreach (var nextPulseEvent in nextPulseEvents)
        {
            q.Enqueue(nextPulseEvent);
        }
    }
}

public abstract class Module {
    public ModuleType Type;
    public string Name;
    public List<string> DestinationModules = new List<string>();

    public IEnumerable<string> InputModules;

    public Pulse LastSent;

    public static Module ParseModule(string s) {
        var splits = s.Split(" -> ");
        var s0 = splits[0];
        var type = s0[0] switch {
            '%' => ModuleType.FlipFlop,
            '&' => ModuleType.Conjunction,
            _ => s0 == "broadcaster" ? ModuleType.Broadcaster : ModuleType.Sink

        };
        var name = type switch {
                ModuleType.FlipFlop or ModuleType.Conjunction => s0[1..],
                _ => s0
        };
        var destinationModules = splits[1].Split(',').Select(m => m.Trim()).ToList();

        return type switch {
            ModuleType.FlipFlop => new FlipFlop() {Name = name, Type = ModuleType.FlipFlop, DestinationModules = destinationModules},
            ModuleType.Conjunction => new Conjunction() {Name = name, Type = ModuleType.Conjunction, DestinationModules = destinationModules},
            ModuleType.Broadcaster => new Broadcaster() {Name = name, Type = ModuleType.Broadcaster, DestinationModules = destinationModules},
        };
    }

    public abstract IEnumerable<PulseEvent> Process();
    public abstract void SetPulse(Pulse pulse, string inputModule);

    public virtual void SetInputModules(IEnumerable<string> modules) {
        InputModules = modules;
    }
}

public class FlipFlop : Module {
    public FlipFlopState State = FlipFlopState.Off;

    private Pulse PulseReceived;

    public override void SetPulse(Pulse pulse, string inputModule) {
        PulseReceived = pulse;
    }

    public override IEnumerable<PulseEvent> Process() {
        if (PulseReceived == Pulse.High) {
            // Do nothing
            return [];
        } else {
            State = State == FlipFlopState.On ? FlipFlopState.Off : FlipFlopState.On;
            LastSent = State == FlipFlopState.On ? Pulse.High : Pulse.Low;
            return DestinationModules.Select(dm => new PulseEvent {
                Pulse = LastSent,
                DestinationModule = dm,
                SourceModule = Name});
        }
    }
}

public class Conjunction : Module {

    private Dictionary<string, Pulse> PulsesReceived;

    

    public override void SetInputModules(IEnumerable<string> modules)
    {
        base.SetInputModules(modules);
        PulsesReceived = modules.ToDictionary(m => m, m => Pulse.Low);
    }

    public override void SetPulse(Pulse pulse, string inputModule) {
        PulsesReceived[inputModule] = pulse;
    }

    public override IEnumerable<PulseEvent> Process() {
        LastSent = PulsesReceived.Values.All(p => p == Pulse.High) ? Pulse.Low : Pulse.High;
        return DestinationModules.Select(dm => new PulseEvent {Pulse = LastSent, DestinationModule = dm, SourceModule = Name});
    }
}

public class Broadcaster : Module
{
    private Pulse PulseReceived;
    public override IEnumerable<PulseEvent> Process()
    {
        return DestinationModules.Select(dm => new PulseEvent {Pulse = PulseReceived, DestinationModule = dm, SourceModule = Name});
    }

    public override void SetPulse(Pulse pulse, string inputModule)
    {
        PulseReceived = pulse;
    }
}

public class Sink : Module
{
    public override IEnumerable<PulseEvent> Process()
    {
        return [];
    }

    public override void SetPulse(Pulse pulse, string inputModule)
    {
    }
}

public record PulseEvent {
    public Pulse Pulse;
    public string DestinationModule;
    public string SourceModule;

    public override string ToString() {
        return $"{SourceModule} -{(Pulse == Pulse.Low ? "low" : "high")}-> {DestinationModule}";
    }
}
public enum FlipFlopState {
    On, Off
}

public enum ModuleType {
    FlipFlop,
    Conjunction,
    Broadcaster,
    Sink
}

public enum Pulse {
    Low, High
}
