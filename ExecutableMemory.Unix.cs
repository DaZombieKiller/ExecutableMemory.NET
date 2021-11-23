using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using static Posix;

public static unsafe partial class ExecutableMemory
{
    static void* AllocateUnix(ReadOnlySpan<byte> code)
    {
        var length = (nuint)(uint)sizeof(Allocation) + (uint)code.Length - 1;
        var alloc  = (Allocation*)mmap(null, length, PROT_READ | PROT_WRITE, MAP_PRIVATE | MAP_ANONYMOUS, -1, 0);

        if (alloc is null)
            ThrowExceptionForErrno(Marshal.GetLastPInvokeError());

        // Store allocation size and advance pointer to code
        alloc->Length = length;
        code.CopyTo(new Span<byte>(alloc->Buffer, code.Length));

        // TODO: Don't mark Allocation.Length as executable
        if (mprotect(alloc, length, PROT_READ | PROT_EXEC) != 0)
            ThrowExceptionForErrno(Marshal.GetLastPInvokeError());

        __builtin___clear_cache(alloc->Buffer, alloc->Buffer + code.Length);
        return alloc->Buffer;
    }

    static void FreeUnix(void* address)
    {
        var alloc = (Allocation*)((byte*)address - sizeof(nuint));

        if (munmap(alloc, alloc->Length) != 0)
        {
            ThrowExceptionForErrno(Marshal.GetLastPInvokeError());
        }
    }

    static void ThrowExceptionForErrno(int errno)
    {
        if (errno != 0)
        {
            throw new Win32Exception(errno);
        }
    }

    struct Allocation
    {
        public nuint Length;
        public fixed byte Buffer[1];
    }
}
