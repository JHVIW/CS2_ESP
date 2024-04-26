using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC_Wallhacks
{
    public static class Offsets
    {
        //offsets.cs
        public static int dwViewAngles = 0x19342F0;
        public static int dwEntityList = 0x18C6268;
        public static int dwLocalPlayerPawn = 0x173A3C8;
        public static int dwViewMatrix = 0x19278B0;

        //client.dll.cs
        public static int m_hPlayerPawn = 0x7E4;
        public static int m_iHealth = 0x334;
        public static int m_vOldOrigin = 0x127C;
        public static int m_iTeamNum = 0x3CB;
        public static int m_vecViewOffset = 0xC58;
        public static int m_lifeState = 0x338;
        public static int m_modelState = 0x160;
        public static int m_pGameSceneNode = 0x318;
        public static int m_iIDEntIndex = 0x13B0;
    }
}
