using System.Runtime.InteropServices;

static unsafe class Posix
{
    public const int PROT_READ = 1 << 0;

    public const int PROT_WRITE = 1 << 1;

    public const int PROT_EXEC = 1 << 2;

    public const int MAP_PRIVATE = 1 << 1;

    public const int MAP_ANONYMOUS = 1 << 5;

    [DllImport("c", ExactSpelling = true, SetLastError = true)]
    public static extern void* mmap(void* addr, nuint length, int prot, int flags, int fd, uint offset);

    [DllImport("c", ExactSpelling = true, SetLastError = true)]
    public static extern int munmap(void* addr, nuint length);

    [DllImport("c", ExactSpelling = true, SetLastError = true)]
    public static extern int mprotect(void* addr, nuint len, int prot);

    public static void __builtin___clear_cache(void* begin, void* end)
    {
        // TODO: Use a native library to expose this
        _ = begin;
        _ = end;
    }
}
