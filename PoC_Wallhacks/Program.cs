using PoC_Wallhacks;
using Swed64;
using System.Numerics;
using System.Runtime.InteropServices;

Swed swed = new Swed("cs2");

IntPtr client = swed.GetModuleBase("client.dll");
IntPtr forceJumpAddress = client + 0x1733750;
IntPtr forceAttack = client + 0x1733240;

Renderer renderer = new Renderer();
Thread renderThread = new Thread(new ThreadStart(renderer.Start().Wait));
renderThread.Start();

Vector2 screenSize = renderer.screenSize;

List<Entity> entities = new List<Entity>();
Entity localPlayer = new Entity();


[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int vKey);
const int SPACE_BAR = 0X20;
const uint STANDING = 65665;
const uint CROUCHING = 65667;

const uint PLUS_JUMP = 65537;
const uint MINUS_JUMP = 256;


while (true)
{
    entities.Clear();
    IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);

    IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

    IntPtr localPlayerPawn = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);

    localPlayer.team = swed.ReadInt(localPlayerPawn, Offsets.m_iTeamNum);

    for(int i = 0; i< 64; i++)
    {
        IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);
        if (currentController == IntPtr.Zero) continue;

        int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);
        if(pawnHandle == 0) continue;

        IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);
        if(listEntry2 == IntPtr.Zero) continue;

        IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));
        if (currentPawn == IntPtr.Zero) continue;

        int lifeState = swed.ReadInt(currentPawn, Offsets.m_lifeState);
        if (lifeState != 256) continue;

        float[] viewMatrix = swed.ReadMatrix(client + Offsets.dwViewMatrix);

        Entity entity = new Entity();

        entity.team = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
        entity.position = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);
        entity.viewOffset = swed.ReadVec(currentPawn, Offsets.m_vecViewOffset);
        entity.position2D = Calculate.WorldToScreen(viewMatrix, entity.position, screenSize);
        entity.viewPosition2D = Calculate.WorldToScreen(viewMatrix, Vector3.Add(entity.position, entity.viewOffset), screenSize);

        entities.Add(entity);

    }
    renderer.UpdateLocalPlayer(localPlayer);
    renderer.UpdateEntities(entities);

    uint fFlag = swed.ReadUInt(localPlayerPawn, 0x3D4);

    int entIndex = swed.ReadInt(localPlayerPawn, Offsets.m_iIDEntIndex);

    if (GetAsyncKeyState(0x12) < 0)
    {
        if (entIndex > 0)
        {
            swed.WriteInt(forceAttack, 65537);
            Thread.Sleep(1);
            swed.WriteInt(forceAttack, 256);
        }
    }
    Thread.Sleep(1);



}
