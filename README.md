# Pure.DI master (#172): generation time for nested `Func<T1, T>` delegates grows 4× per level instead of 2×

Minimal repro for a regression in Pure.DI master (adaacfe, after #172): a chain of 13 view models, each opening the next through two delegates with different argument types, generates in about 10 s with 2.5.7 and about 90 s with master. The generated code is identical (82,237 lines). Every added level multiplies the master time by about 4, against 2 for 2.5.7.

[`ISSUE.md`](ISSUE.md) is the text to paste into a Pure.DI GitHub issue; it includes the measurements from 8 to 14 levels.

```
dotnet build                                          # 2.5.7 from nuget.org, about 10 s
dotnet build -p:PureDIVersion=<master build version> -p:RestoreAdditionalProjectSources=<folder with the package>   # about 90 s
```

The graph itself is the one of the code size issue reported with it (PureDI.UnconsumedDelegateArgumentsCodeSize), two levels deeper.
