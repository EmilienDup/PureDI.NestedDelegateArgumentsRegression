using Pure.DI;

var composition = new Composition();
Console.WriteLine(composition.Root.GetType().Name);

public sealed record ArgA(string Value);
public sealed record ArgB(string Value);

// Shared singleton services: they consume none of the delegate arguments
public sealed class S0;
public sealed class S1(S0 previous);
public sealed class S2(S1 previous);
public sealed class S3(S2 previous);
public sealed class S4(S3 previous);
public sealed class S5(S4 previous);
public sealed class S6(S5 previous);
public sealed class S7(S6 previous);
public sealed class S8(S7 previous);
public sealed class S9(S8 previous);

// Each view model opens the next one through two delegates with different argument types; no view model consumes ArgA or ArgB
public sealed class Vm0(S9 services, Func<ArgA, Vm1> openWithA, Func<ArgB, Vm1> openWithB);
public sealed class Vm1(S9 services, Func<ArgA, Vm2> openWithA, Func<ArgB, Vm2> openWithB);
public sealed class Vm2(S9 services, Func<ArgA, Vm3> openWithA, Func<ArgB, Vm3> openWithB);
public sealed class Vm3(S9 services, Func<ArgA, Vm4> openWithA, Func<ArgB, Vm4> openWithB);
public sealed class Vm4(S9 services, Func<ArgA, Vm5> openWithA, Func<ArgB, Vm5> openWithB);
public sealed class Vm5(S9 services, Func<ArgA, Vm6> openWithA, Func<ArgB, Vm6> openWithB);
public sealed class Vm6(S9 services, Func<ArgA, Vm7> openWithA, Func<ArgB, Vm7> openWithB);
public sealed class Vm7(S9 services, Func<ArgA, Vm8> openWithA, Func<ArgB, Vm8> openWithB);
public sealed class Vm8(S9 services, Func<ArgA, Vm9> openWithA, Func<ArgB, Vm9> openWithB);
public sealed class Vm9(S9 services, Func<ArgA, Vm10> openWithA, Func<ArgB, Vm10> openWithB);
public sealed class Vm10(S9 services, Func<ArgA, Vm11> openWithA, Func<ArgB, Vm11> openWithB);
public sealed class Vm11(S9 services, Func<ArgA, Vm12> openWithA, Func<ArgB, Vm12> openWithB);
public sealed class Vm12(S9 services);
public sealed class Top(Func<ArgA, Vm0> open);

partial class Composition
{
    private static void Setup() => DI.Setup(nameof(Composition))
        .Bind<S0>().As(Lifetime.Singleton).To<S0>()
        .Bind<S1>().As(Lifetime.Singleton).To<S1>()
        .Bind<S2>().As(Lifetime.Singleton).To<S2>()
        .Bind<S3>().As(Lifetime.Singleton).To<S3>()
        .Bind<S4>().As(Lifetime.Singleton).To<S4>()
        .Bind<S5>().As(Lifetime.Singleton).To<S5>()
        .Bind<S6>().As(Lifetime.Singleton).To<S6>()
        .Bind<S7>().As(Lifetime.Singleton).To<S7>()
        .Bind<S8>().As(Lifetime.Singleton).To<S8>()
        .Bind<S9>().As(Lifetime.Singleton).To<S9>()
        .Root<Top>("Root");
}
