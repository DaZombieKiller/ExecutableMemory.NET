using System;
using System.Runtime.InteropServices;
using static TerraFX.Interop.Windows;

public static unsafe partial class ExecutableMemory
{
    static void* AllocateWindows(ReadOnlySpan<byte> code)
    {
        uint oldProtect;
        var buffer = VirtualAlloc(null, (uint)code.Length, MEM_COMMIT, PAGE_READWRITE);

        if (buffer == null)
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

        code.CopyTo(new Span<byte>(buffer, code.Length));

        if (VirtualProtect(buffer, (uint)code.Length, PAGE_EXECUTE_READ, &oldProtect) == 0)
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

        if (FlushInstructionCache(GetCurrentProcess(), buffer, (uint)code.Length) == 0)
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

        return buffer;
    }

    static void FreeWindows(void* address)
    {
        if (VirtualFree(address, 0, MEM_RELEASE) == 0)
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
    }
}
