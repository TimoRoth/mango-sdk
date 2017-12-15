using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Libmango
    {
        internal class SafeMangoHandle : SafeHandle
        {
            private readonly int length;

            public SafeMangoHandle(IntPtr handle, int length) : base(IntPtr.Zero, true)
            {
                this.handle = handle;
                this.length = length;
            }

            public override bool IsInvalid => handle == IntPtr.Zero;

            public int Length => length;

            public void CopyTo(byte[] buffer, int offset)
            {
                var addedRef = false;
                try
                {
                    DangerousAddRef(ref addedRef);

                    Marshal.Copy(DangerousGetHandle(), buffer, offset, length);
                }
                finally
                {
                    if (addedRef)
                    {
                        DangerousRelease();
                    }
                }
            }

            public byte[] ToArray()
            {
                var array = new byte[length];
                CopyTo(array, 0);
                return array;
            }

            protected override bool ReleaseHandle()
            {
                Marshal.FreeHGlobal(handle);
                return true;
            }
        }
    }
}
