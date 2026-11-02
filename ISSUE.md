# Regression after #172: generation time for nested `Func<T1, T>` delegates grows 4× per level instead of 2×

**Pure.DI master (adaacfe)** compared with **2.5.7**, .NET SDK 10.0.403, macOS. Repro project attached: `dotnet build` takes about 10 s with 2.5.7 and about 90 s with master, for the same generated code.

Follow-up to #172. The per-scope reuse of rewritten nodes fixes the dependency diamond from #172 (27 classes: about 70 s on 2.5.7, 1.7 s on master). For delegates with arguments nested in each other, it makes generation much slower than 2.5.7.

## Minimal code

Every view model opens the next one through two delegates with different argument types. All view models share a chain of singletons. No class consumes `ArgA` or `ArgB`.

```csharp
using Pure.DI;

var composition = new Composition();
Console.WriteLine(composition.Root.GetType().Name);

public sealed record ArgA(string Value);
public sealed record ArgB(string Value);

public sealed class S0;
public sealed class S1(S0 previous);
// ... same pattern ...
public sealed class S9(S8 previous);

public sealed class Vm0(S9 services, Func<ArgA, Vm1> openWithA, Func<ArgB, Vm1> openWithB);
// ... same pattern ...
public sealed class Vm11(S9 services, Func<ArgA, Vm12> openWithA, Func<ArgB, Vm12> openWithB);
public sealed class Vm12(S9 services);
public sealed class Top(Func<ArgA, Vm0> open);

partial class Composition
{
    private static void Setup() => DI.Setup(nameof(Composition))
        .Bind<S0>().As(Lifetime.Singleton).To<S0>()
        // ... every Sk as Singleton ...
        .Bind<S9>().As(Lifetime.Singleton).To<S9>()
        .Root<Top>("Root");
}
```

## Expected

At most the 2.5.7 generation time for the same output.

## Actual

`levels` = number of view models with two delegates; the attached project has 12. Times are full `dotnet build` runs of a fresh project.

| levels | 2.5.7 | master (adaacfe) | generated lines (identical) |
|---|---|---|---|
| 8 | 2 s | 2 s | 5,440 |
| 10 | 3 s | 7 s | 20,800 |
| 11 | | 20 s | 41,277 |
| 12 (attached) | 7–10 s | 78–95 s | 82,237 |
| 13 | 13 s | 327 s | 164,157 |

The output doubling per level is reported separately; this issue is about master being 10× to 25× slower than 2.5.7 on it, with the factor itself growing with each level.

Where the time goes on master (`dotnet-stack report` on the compiler process, three samples 5 s apart, same picture in each):

```
Microsoft.CodeAnalysis.CSharp!...SymbolDisplayVisitor.VisitNamedType
Pure.DI.Core!Pure.DI.Core.GraphOverrider+OverrideContext.AreSame
Pure.DI.Core!Pure.DI.Core.GraphOverrider+OverrideContext.Equals
Pure.DI.Core!Pure.DI.Core.GraphOverrider.RewriteNode
Pure.DI.Core!Pure.DI.Core.GraphOverrider.Override   (21 to 32 Override/RewriteNode frames)
...
```

So the time goes into comparing `OverrideContext` keys of `RewriteState.Rewritten`: dictionary lookups by `Injection` that format type symbols. The contexts are numerous here (one per chain of delegates), and `OverrideContext.GetHashCode` XORs binding ids, so we suspect many contexts share a hash and each lookup compares against many of them. We haven't verified that part.

On a real application (about 2,500 bindings, view models created through `Func<...>` factories with arguments), master doesn't finish generating either: we stopped it after more than 10 minutes, with `GraphOverrider` 65 to 80 frames deep and the compiler process at 8.9 GB.
