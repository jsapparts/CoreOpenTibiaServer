using System;

namespace COMMO.GameServer.World.TFSLoading {
    public enum TFSTileFlag : UInt32 {
        ProtectionZone =    0b0001, // OTBM_TILEFLAG_PROTECTIONZONE
        NoPvpZone =         0b0010, // OTBM_TILEFLAG_NOPVPZONE
        NoLogout =          0b0100, // OTBM_TILEFLAG_NOLOGOUT
        PvpZone =           0b1000  // OTBM_TILEFLAG_PVPZONE
    }
}
