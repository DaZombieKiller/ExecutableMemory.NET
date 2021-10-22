# ExecutableMemory.NET
A sample project for writing and executing assembly in C#

```CSharp
var add = (delegate* unmanaged[SuppressGCTransition]<int, int, int>)ExecutableMemory.Allocate(static asm =>
{
    if (OperatingSystem.IsWindows())
        asm.lea(eax, rcx + rdx);
    else
        asm.lea(eax, rdi + rsi);
    asm.ret();
});

Console.WriteLine(add(2, 2));
```
