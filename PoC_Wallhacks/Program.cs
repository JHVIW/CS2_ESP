using PoC_Wallhacks;
using Swed64;
using System.Numerics;

Swed swed = new Swed("cs2");

IntPtr client = swed.GetModuleBase("client.dll");

Renderer renderer = new Renderer();
Thread renderThread = new Thread(new ThreadStart(renderer.Start().Wait));
renderThread.Start();

Vector2 screenSize = renderer.screenSize;

List<Entity> entities = new List<Entity>();
Entity localPlayer = new Entity();

//offsets.cs
int dwEntityList = 0x18C6268;
int dwViewMatrix = 0x19278B0;
int dwLocalPlayerPawn = 0x173A3C8;

//client.dll.cs
int m_vOldOrigin = 0x127C;
int m_iTeamNum = 0x3CB;
int m_vecViewOffset = 0xC58;

//client.dll.hpp
int m_lifeState = 0x338;
int m_hPlayerPawn = 0x7E4;


while (true)
{
    entities.Clear();
    IntPtr entityList = swed.ReadPointer(client, dwEntityList);

    IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

    IntPtr localPlayerPawn = swed.ReadPointer(client, dwLocalPlayerPawn);

    localPlayer.team = swed.ReadInt(localPlayerPawn, m_iTeamNum);

    for(int i = 0; i< 64; i++)
    {
        IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);
        if (currentController == IntPtr.Zero) continue;

        int pawnHandle = swed.ReadInt(currentController, m_hPlayerPawn);
        if(pawnHandle == 0) continue;

        IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);
        if(listEntry2 == IntPtr.Zero) continue;

        IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));
        if (currentPawn == IntPtr.Zero) continue;

        int lifeState = swed.ReadInt(currentPawn, m_lifeState);
        if (lifeState != 256) continue;

        float[] viewMatrix = swed.ReadMatrix(client + dwViewMatrix);

        Entity entity = new Entity();

        entity.team = swed.ReadInt(currentPawn, m_iTeamNum);
        entity.position = swed.ReadVec(currentPawn, m_vOldOrigin);
        entity.viewOffset = swed.ReadVec(currentPawn, m_vecViewOffset);
        entity.position2D = Calculate.WorldToScreen(viewMatrix, entity.position, screenSize);
        entity.viewPosition2D = Calculate.WorldToScreen(viewMatrix, Vector3.Add(entity.position, entity.viewOffset), screenSize);

        entities.Add(entity);

    }
    renderer.UpdateLocalPlayer(localPlayer);
    renderer.UpdateEntities(entities);

}