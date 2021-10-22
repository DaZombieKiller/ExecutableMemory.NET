using System;
using static Posix;

public static unsafe partial class ExecutableMemory
{
    static void* AllocateUnix(ReadOnlySpan<byte> code)
    {
        var length = (nuint)(uint)sizeof(Allocation) + (uint)code.Length - 1;
        var alloc  = (Allocation*)mmap(null, length, PROT_READ | PROT_WRITE, MAP_PRIVATE | MAP_ANONYMOUS, -1, 0);

        // Store allocation size and advance pointer to code
        alloc->Length = length;
        code.CopyTo(new Span<byte>(alloc->Buffer, code.Length));

        // TODO: Don't mark Allocation.Length as executable
        mprotect(alloc, length, PROT_READ | PROT_EXEC);

        // TODO: Native library that exports this
        //__builtin___clear_cache(buffer, buffer + code.Length);
        return alloc->Buffer;
    }

    static void FreeUnix(void* address)
    {
        var alloc = (Allocation*)((byte*)address - sizeof(nuint));
        munmap(alloc, alloc->Length);
    }

    struct Allocation
    {
        public nuint Length;
        public fixed byte Buffer[1];
    }
}
