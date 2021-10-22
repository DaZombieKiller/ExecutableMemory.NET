using System;
using System.IO;
using Iced.Intel;

public static unsafe partial class ExecutableMemory
{
    public static void* Allocate(ReadOnlySpan<byte> code)
    {
        if (OperatingSystem.IsWindows())
            return AllocateWindows(code);
        else if (OperatingSystem.IsLinux())
            return AllocateUnix(code);
        else
            throw new PlatformNotSupportedException();
    }

    public static void* Allocate(Assembler asm)
    {
        ArgumentNullException.ThrowIfNull(asm);
        using var ms = new MemoryStream();
        asm.Assemble(new StreamCodeWriter(ms), 0);
        return Allocate(ms.GetBuffer().AsSpan(0, (int)ms.Length));
    }

    public static void* Allocate(Action<Assembler> generator)
    {
        ArgumentNullException.ThrowIfNull(generator);
        var asm = new Assembler(Environment.Is64BitProcess ? 64 : 32);
        generator(asm);
        return Allocate(asm);
    }

    public static void Free(void* address)
    {
        if (OperatingSystem.IsWindows())
            FreeWindows(address);
        else if (OperatingSystem.IsLinux())
            FreeUnix(address);
        else
            throw new PlatformNotSupportedException();
    }
}
