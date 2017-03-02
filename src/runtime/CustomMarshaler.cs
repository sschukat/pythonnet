using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Python.Runtime
{
    /// <summary>
    /// Abstract class defining boiler plate methods that
    /// Custom Marshalers will use.
    /// </summary>
    public abstract class MarshalerBase : ICustomMarshaler
    {
        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            throw new NotImplementedException();
        }

        public abstract IntPtr MarshalManagedToNative(object managedObj);

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        public void CleanUpManagedData(object managedObj)
        {
            // Let GC deal with it
        }

        public int GetNativeDataSize()
        {
            return IntPtr.Size;
        }
    }


    /// <summary>
    /// Custom Marshaler to deal with Managed String to Native
    /// conversion differences on UCS2/UCS4.
    /// </summary>
    public class StrMarshaler : MarshalerBase
    {
        private static readonly MarshalerBase Instance = new StrMarshaler();
        private static readonly Encoding PyEncoding = Runtime.PyEncoding;

        public override IntPtr MarshalManagedToNative(object managedObj)
        {
            var s = managedObj as string;

            if (s == null)
            {
                return IntPtr.Zero;
            }

            byte[] bStr = PyEncoding.GetBytes(s + "\0");
            IntPtr mem = Marshal.AllocHGlobal(bStr.Length);
            try
            {
                Marshal.Copy(bStr, 0, mem, bStr.Length);
            }
            catch (Exception)
            {
                Marshal.FreeHGlobal(mem);
                throw;
            }

            return mem;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return Instance;
        }
    }


    /// <summary>
    /// Custom Marshaler to deal with Managed String Arrays to Native
    /// conversion differences on UCS2/UCS4.
    /// </summary>
    public class StrArrayMarshaler : MarshalerBase
    {
        private static readonly MarshalerBase Instance = new StrArrayMarshaler();
        private static readonly Encoding PyEncoding = Runtime.PyEncoding;

        public override IntPtr MarshalManagedToNative(object managedObj)
        {
            var argv = managedObj as string[];

            if (argv == null)
            {
                return IntPtr.Zero;
            }

            int totalStrLength = argv.Sum(arg => arg.Length + 1);
            int memSize = argv.Length * IntPtr.Size + totalStrLength * Runtime.UCS;

            IntPtr mem = Marshal.AllocHGlobal(memSize);
            try
            {
                // Preparing array of pointers to strings
                IntPtr curStrPtr = mem + argv.Length * IntPtr.Size;
                for (var i = 0; i < argv.Length; i++)
                {
                    byte[] bStr = PyEncoding.GetBytes(argv[i] + "\0");
                    Marshal.Copy(bStr, 0, curStrPtr, bStr.Length);
                    Marshal.WriteIntPtr(mem + i * IntPtr.Size, curStrPtr);
                    curStrPtr += bStr.Length;
                }
            }
            catch (Exception)
            {
                Marshal.FreeHGlobal(mem);
                throw;
            }

            return mem;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return Instance;
        }
    }


    /// <summary>
    /// Custom Marshaler to deal with Managed String to Native
    /// conversion on UTF-8. Use on functions that expect UTF-8 encoded
    /// strings like `PyUnicode_FromStringAndSize`
    /// </summary>
    /// <remarks>
    /// If instead we used `MarshalAs(UnmanagedType.LPWStr)` the output to
    /// `foo` would be `f\x00o\x00o\x00`.
    /// </remarks>
    public class Utf8Marshaler : MarshalerBase
    {
        private static readonly MarshalerBase Instance = new Utf8Marshaler();
        private static readonly Encoding PyEncoding = Encoding.UTF8;

        public override IntPtr MarshalManagedToNative(object managedObj)
        {
            var s = managedObj as string;

            if (s == null)
            {
                return IntPtr.Zero;
            }

            byte[] bStr = PyEncoding.GetBytes(s + "\0");
            IntPtr mem = Marshal.AllocHGlobal(bStr.Length);
            try
            {
                Marshal.Copy(bStr, 0, mem, bStr.Length);
            }
            catch (Exception)
            {
                Marshal.FreeHGlobal(mem);
                throw;
            }

            return mem;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return Instance;
        }
    }
}