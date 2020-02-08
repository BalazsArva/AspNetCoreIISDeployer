using System;
using System.Globalization;

namespace AspNetCoreIISDeployer.Application.Services.IIS
{
    public struct Port : IEquatable<Port>
    {
        public const int Min = 0;
        public const int Max = 65_536;

        private const int NoPortAssignment = -1;

        public static readonly Port None = new Port(NoPortAssignment);

        public Port(int portNumber)
        {
            if (portNumber != NoPortAssignment && (portNumber < Min || portNumber > Max))
            {
                throw new ArgumentOutOfRangeException(nameof(portNumber), $"The value of '{nameof(portNumber)}' must be between {Min} and {Max} inclusive.");
            }

            PortNumber = portNumber;
        }

        public int PortNumber { get; }

        public bool Equals(Port other)
        {
            return other.PortNumber.Equals(PortNumber);
        }

        public override bool Equals(object obj)
        {
            if (obj is Port)
            {
                return Equals((Port)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return PortNumber.GetHashCode();
        }

        public override string ToString()
        {
            return PortNumber.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        public static bool operator ==(Port port1, Port port2)
        {
            return port1.Equals(port2);
        }

        public static bool operator !=(Port port1, Port port2)
        {
            return !port1.Equals(port2);
        }

        public static implicit operator Port(int port)
        {
            return new Port(port);
        }

        public static implicit operator int(Port port)
        {
            return port.PortNumber;
        }
    }
}