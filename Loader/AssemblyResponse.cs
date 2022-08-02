namespace Blaze
{
    internal class AssemblyResponse
    {
        internal byte[] Assembly { get; private set; }
        internal Response Response { get; private set; }
        internal string Message { get; private set; }
        internal ModType ModType { get; private set; }

        internal static AssemblyResponse Error(string message)
        {
            return new AssemblyResponse
            {
                Assembly = null,
                Response = Response.Error,
                Message = message,
            };
        }

        internal static AssemblyResponse OK(byte[] assembly, string message)
        {
            return new AssemblyResponse
            {
                Assembly = assembly,
                Response = Response.OK,
                Message = message,
            };
        }
    }

    internal enum Response
    {
        Error,
        OK,
    }

    internal enum ModType
    {
        Premium,
        Free
    }
}
