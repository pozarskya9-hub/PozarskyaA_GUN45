using System;

namespace Netologia.Necro
{
    [Flags]
    public enum NeighbourType : byte
    {
        None = 0,
        Left = 1,
        Right = 2,
        Forward = 4,
        Back = 8,
        ForwardLeft = 16,
        ForwardRight = 32,
        BackLeft = 64,
        BackRight = 128,

        LeftRight = Left | Right,
        ForwardBack = Forward | Back,
        Cross = Left | Right | Forward | Back,
        Diagonal = ForwardLeft | ForwardRight | BackLeft | BackRight,
        All = Cross | Diagonal
    }

    public enum Team
    {
        Player1,
        Player2
    }
}
