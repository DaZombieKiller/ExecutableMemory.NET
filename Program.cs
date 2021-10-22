using System;
using static Iced.Intel.AssemblerRegisters;

static unsafe class Program
{
    public static void Main()
    {
        var add = (delegate* unmanaged[SuppressGCTransition]<int, int, int>)ExecutableMemory.Allocate(static asm =>
        {
            if (OperatingSystem.IsWindows())
                asm.lea(eax, rcx + rdx);
            else
                asm.lea(eax, rdi + rsi);
            asm.ret();
        });

        try
        {
            Console.WriteLine(add(2, 2));
        }
        finally
        {
            ExecutableMemory.Free(add);
            add = null;
        }
    }
}
