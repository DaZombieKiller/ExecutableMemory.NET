using System;
using System.Runtime.InteropServices;
using static TerraFX.Interop.Windows.MEM;
using static TerraFX.Interop.Windows.PAGE;
using static TerraFX.Interop.Windows.Windows;

public static unsafe partial class ExecutableMemory
{
    static void* AllocateWindows(ReadOnlySpan<byte> code)
    {
        uint oldProtect;
        var buffer = VirtualAlloc(null, (uint)code.Length, MEM_COMMIT, PAGE_READWRITE);

        if (buffer == null)
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

        code.CopyTo(new Span<byte>(buffer, code.Length));

        if (!VirtualProtect(buffer, (uint)code.Length, PAGE_EXECUTE_READ, &oldProtect))
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

        if (!FlushInstructionCache(GetCurrentProcess(), buffer, (uint)code.Length))
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

        return buffer;
    }

    static void FreeWindows(void* address)
    {
        if (!VirtualFree(address, 0, MEM_RELEASE))
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
    }
}
