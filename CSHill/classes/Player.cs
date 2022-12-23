using System;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Ionic.Zlib;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;

public class Player
{
    public uint NetId;
    public static uint _NetId = 0; //global netid for all players
    public string Token;

    public string Socket;

    public Vector3 Position = new Vector3(0, 0, 0);
    public Vector3 Scale = new Vector3(1, 1, 1);
    public float Rotation;

    public string Name = "Player";
    public uint userId;
    public bool admin;
    public uint? membership;
    public Color ChatColor = new Color(1.0, 1.0, 1.0);

    public int Score;
    public string Speech = "";
    public uint Speed = 4;
    public uint JumpPower = 5;
    public int Health = 100;
    public int MaxHealth = 100;
    public bool Alive = true;

    public Vector3 CameraPosition = new Vector3(0, 0, 0);
    public Vector3 CameraRotation = new Vector3(0, 0, 0);
    public int CameraDistance;
    public uint CameraFOV;
    public Player CameraObject;
    public string CameraType;

    public List<uint> BlockedUsers = new();
    public List<Brick> LocalBricks = new();
    public Vector3 SpawnPosition;

    public List<Tool> Inventory = new();
    public Tool ToolEquipped;

    public Team Team;
    public Colors Colors = new();
    public Assets Assets = new();

    public Player(string _IpPort)
    {
        IpPort = _IpPort;
        _NetId++;
        NetId = _NetId;
    }
    public void Kick(string reason)
    {
        new PacketBuilder(7)
            .String("kick")
            .String(reason)
            .send(IpPort);
        Server.server.DisconnectClient(IpPort);
    }

    public void ClearMap()
    {
        new PacketBuilder(14)
            .Bool(true)
            .send(IpPort);
    }

    public void _Log(string message, bool broadcast = false)
    {
        //UNFINISHED (None of this shit exists yet)
        /*if (!Game.SystemMessages) return;

        if (Broadcast)
        {
            return Scripts.Message.MessageAll(message);
        }
        else
        {
            return Scripts.Message.MessageClient(Socket, message);
        }
        */
    }

    public void _RemovePlayer()
    {
        Game.Players.Remove(this);
        new PacketBuilder(5)
            .u32(NetId)
            .broadcastExcept(IpPort);
    }

    public void TopPrint(string message, uint seconds)
    {
        new PacketBuilder(7)
            .String("topPrint")
            .String(message)
            .u32(seconds)
            .send(IpPort);
    }

    public void CenterPrint(string message, uint seconds)
    {
        new PacketBuilder(7)
            .String("centerPrint")
            .String(message)
            .u32(seconds)
            .send(IpPort);
    }

    public void BottomPrint(string message, uint seconds)
    {
        new PacketBuilder(7)
            .String("bottomPrint")
            .String(message)
            .u32(seconds)
            .send(IpPort);
    }

    public void Prompt(string message)
    {
        new PacketBuilder(7)
            .String("prompt")
            .String(message)
            .send(IpPort);
    }

    public void Message(string message)
    {
        new PacketBuilder(6)
            .String(message)
            .send(IpPort);
    }

    public void MessageAll(string message, bool generateTitle = true)
    {
        SetSpeech(message);
        if (generateTitle) message = scripts.chat.GenerateTitle(this, message);
        new PacketBuilder(6)
            .String(message)
            .broadcast();
    }

    public void SetOutfit(Outfit outfit)
    {
        Assets = outfit.Assets;
        Colors = outfit.Colors;

        scripts.player.createPlayerIds(this, "KLMNOP").broadcast();
        //CreateAssetIds thing
    }

    public void SetHealth(int health)
    {
        if (health <= 0 && Alive) Kill();
        if (health > MaxHealth) health = MaxHealth;

        Health = health;

        scripts.player.createPlayerIds(this, "e").send(IpPort);
    }

    public void SetScore(int score)
    {
        Score = score;

        scripts.player.createPlayerIds(this, "X").broadcast();
    }

    public void SetTeam(Team team)
    {
        Team = team;

        scripts.player.createPlayerIds(this, "Y").broadcast();
    }

    public void SetCameraPosition(Vector3 position)
    {
        CameraPosition = position;

        scripts.player.createPlayerIds(this, "567").send(IpPort);
    }

    public void SetCameraRotation(Vector3 rotation)
    {
        CameraRotation = rotation;

        scripts.player.createPlayerIds(this, "89a").send(IpPort);
    }

    public void SetCameraDistance(int distance)
    {
        CameraDistance = distance;

        scripts.player.createPlayerIds(this, "4").send(IpPort);
    }

    public void SetCameraFOV(uint fov)
    {
        CameraFOV = fov;

        scripts.player.createPlayerIds(this, "3").send(IpPort);
    }

    public void SetCameraObject(Player player)
    {
        CameraObject = player;

        scripts.player.createPlayerIds(this, "c").send(IpPort);
    }

    public void SetCameraType(string cameraType)
    {
        CameraType = cameraType;

        scripts.player.createPlayerIds(this, "b").send(IpPort);
    }

    public List<Player> GetBlockedPlayers()
    {
        List<Player> players = new();

        foreach (var target in Game.Players)
        {
            if (target.BlockedUsers.Contains(userId)) players.Add(target);
        }

        return players;
    }

    public void AddTool(Tool tool)
    {
        if (Inventory.Contains(tool)) throw new Exception("Player already has tool equipped.");

        Inventory.Add(tool);

        new PacketBuilder(11)
            .Bool(true)
            .u32((uint)tool._SlotId)
            .String(tool.Name)
            .send(IpPort);
    }

    public void DeleteBricks(Brick[] bricks)
    {
        foreach (var brick in bricks)
        {
            if (LocalBricks.Contains(brick)) LocalBricks.Remove(brick);
        }

        PacketBuilder packet = new PacketBuilder(16)
            .u32((uint)bricks.Length);

        foreach (var brick in bricks) packet.u32(brick.NetId);

        packet.send(IpPort);
    }

    public void DestroyTool(Tool tool)
    {
        if (!Inventory.Contains(tool)) return;

        Inventory.Remove(tool);
        ToolEquipped = null;

        new PacketBuilder(11)
            .Bool(false)
            .u32((uint)tool._SlotId)
            .String(tool.Name)
            .send(IpPort);
    }

    public void EquipTool(Tool tool)
    {
        if (!Inventory.Contains(tool)) AddTool(tool);
        if (ToolEquipped == tool) UnequipTool(tool);
        if (ToolEquipped != null) //ToolEquipped.Emit("unequipped", this);

            ToolEquipped = tool;
        //tool.Emit(ToolEquipped, this);

        //CreateAssetIds thing
    }

    public void UnequipTool(Tool tool)
    {
        ToolEquipped = null;

        //tool.Emit("unequipped", this);

        scripts.player.createPlayerIds(this, "h").broadcast();
    }

    public void SetSpeech(string speech = "")
    {
        Speech = speech;

        scripts.player.createPlayerIds(this, "f").broadcast();
        var length = speech.Length * 500;
        Task.Delay(length > 5000 ? 5000 : length).ContinueWith((task) => {
            if (Speech == speech)
            {
                Speech = "";
                scripts.player.createPlayerIds(this, "f").broadcast();
            }
        });
    }

    public void SetSpeed(uint speedValue)
    {
        Speed = speedValue;

        scripts.player.createPlayerIds(this, "1").send(IpPort);
    }

    public void SetJumpPower(uint power)
    {
        JumpPower = power;

        scripts.player.createPlayerIds(this, "2").send(IpPort);
    }

    public void _GetClients()
    {
        if (Game.Players.Count <= 1) return;

        var others = Game.Players;
        others.Remove(this);

        new PacketBuilder(3) //Send all other clients this client
            .u8(1)
            .u32(NetId)
            .String(Name)
            .u32(userId)
            .Bool(admin)
            .u8(membership)
            .broadcastExcept(IpPort);

        PacketBuilder send = new PacketBuilder(3) //Send this client to all other clients
            .u8((uint)others.Count);

        foreach (var player in others)
        {
            send.u32(player.NetId);
            send.String(player.Name);
            send.u32(player.userId);
            send.Bool(player.admin);
            send.u8(player.membership);
        }

        send.send(IpPort);
    }

    public void _UpdatePositionForOthers()
    {
        new PacketBuilder(4)
            .String("ABCF")
            .u32(NetId)
            .Float(Position.x)
            .Float(Position.y)
            .Float(Position.z)
            .Float(Rotation)
            .broadcastExcept(IpPort);
    }

    public async void NewBrick(Brick brick)
    {
        var localBrick = brick.Clone();

        localBrick.Socket = Socket;

        LocalBricks.Add(localBrick);

        PacketBuilder packet = new PacketBuilder(17)
            .u32(1);

        if (localBrick.Model != 0)
        {
            AssetData boban = await AssetDownloader.GetAssetData(localBrick.Model);
        }

        packet
            .u32(localBrick.NetId)
            .Float(localBrick.Position.x)
            .Float(localBrick.Position.y)
            .Float(localBrick.Position.z)
            .Float(localBrick.Scale.x)
            .Float(localBrick.Scale.y)
            .Float(localBrick.Scale.z)
            .u32(localBrick.Color.dec())
            .Float((float)localBrick.Visibility);

        var attributes = "";

        if (localBrick.Rotation != 0) attributes += "A";
        if (localBrick.Shape != null) attributes += "B";
        if (localBrick.LightEnabled) attributes += "D";
        if (localBrick.Collision) attributes += "F";
        if (localBrick.Clickable) attributes += "G";
        if (localBrick.Model != 0) attributes += "C";

        packet.String(attributes);

        if (localBrick.Rotation != 0) packet.u32((uint)localBrick.Rotation);
        if (localBrick.Shape != null) packet.String(localBrick.Shape);
        if (localBrick.LightEnabled) packet
                .u32(localBrick.LightColor.dec())
                .u32((uint)localBrick.LightRange);
        if (localBrick.Clickable) packet
                .Bool(localBrick.Clickable)
                .u32((uint)localBrick.ClickDistance);

        if (attributes.Contains("C")) packet.Asset(localBrick.Model);

        packet
            .deflate()
            .send(IpPort);
    }

    public void NewBricks(List<Brick> bricks)
    {
        List<Brick> localBricks = new();

        foreach (var brick in bricks)
        {
            Brick localBrick = brick.Clone();
            localBrick.Socket = Socket;
            localBricks.Add(localBrick);
            LocalBricks.Add(localBrick);
        }

        //UNFINISHED
    }

    public void SetPosition(Vector3 position)
    {
        Position = position;

        //Emit("moved", Position, Rotation);

        scripts.player.createPlayerIds(this, "ABC").broadcast();
    }

    public void SetScale(Vector3 scale)
    {
        Scale = scale;

        scripts.player.createPlayerIds(this, "GHI").broadcast();
    }
    private class _avatarData
    {
        public class _items
        {

        }
        public class _colors
        {
            [JsonProperty("head")]
            public string head { get; set; }

            [JsonProperty("torso")]
            public string torso { get; set; }

            [JsonProperty("left_arm")]
            public string left_arm { get; set; }

            [JsonProperty("left_leg")]
            public string left_leg { get; set; }

            [JsonProperty("right_arm")]
            public string right_arm { get; set; }

            [JsonProperty("right_leg")]
            public string right_leg { get; set; }
        }

        [JsonProperty("colors")]
        public _colors colors { get; set; }
    }
    public async void SetAvatar(uint userId)
    {
        HttpResponseMessage response =  await Server.client.GetAsync($"https://api.brick-hill.com/v1/games/retrieveAvatar?id={userId}");
        _avatarData data = JsonConvert.DeserializeObject<_avatarData>(await response.Content.ReadAsStringAsync());
        Colors.Head = new Color(data.colors.head);
        Colors.Torso = new Color(data.colors.torso);
        Colors.LeftArm = new Color(data.colors.left_arm);
        Colors.LeftLeg = new Color(data.colors.left_leg);
        Colors.RightArm = new Color(data.colors.right_arm);
        Colors.RightLeg = new Color(data.colors.right_leg);

        this.Assets.tshirt
        scripts.player.createPlayerIds(this,"KLMNOP").broadcast();
        (await scripts.player.createAssetIds(this)).broadcast();
    }

    //UNFINISHED:
    //AvatarLoaded()
    //GetUserInfo()
    //OwnsAsset()

    //public async Task<> GetRankInGroup()//who tf actually uses this //people who have games where you join clan for shit idk its useful to have// whats the url for the apihttps://api.brick-hill.com/v1/clan/member?id=${groupId}&user=${user} 
    //{
    //    HttpClient client = new();
    //}

    public void Kill()
    {
        if (!Alive) return;

        Alive = false;
        Health = 0;

        new PacketBuilder(8)
            .u32(NetId)
            .Bool(true)
            .broadcast();

        scripts.player.createPlayerIds(this, "e").send(IpPort);

        //Emit("died");
    }

    public void Respawn()
    {
        Vector3 newSpawnPosition;

        if (SpawnPosition != null)
        {
            newSpawnPosition = SpawnPosition;
        } else
        {
            Random rand = new Random();
            newSpawnPosition = new Vector3((float)rand.NextDouble() * (Game.Environment.baseSize / 2), (float)rand.NextDouble() * (Game.Environment.baseSize / 2), 30);
        }

        SetPosition(newSpawnPosition);

        new PacketBuilder(8)
            .u32(NetId)
            .Bool(false)
            .broadcast();

        Alive = true;
        Health = MaxHealth;
        CameraType = "orbit";
        CameraObject = this;
        CameraPosition = new Vector3(0, 0, 0);
        CameraRotation = new Vector3(0, 0, 0);
        CameraFOV = 60;
        ToolEquipped = null;

        scripts.player.createPlayerIds(this, "ebc56789a3h").send(IpPort);

        //Emit("respawn");
    }

    public async void _CreateFigures()
    {
        scripts.player.createPlayerIds(this, "ABCDEFGHIKLMNOPXYf").broadcastExcept(IpPort);
        (await scripts.player.createAssetIds(this, "QRSTUVWg")).broadcastExcept(IpPort);

        foreach (var player in Game.Players)
        {
            if (player != this)
            {
                scripts.player.createPlayerIds(this, "ABCDEFGHIKLMNOPXYf").send(IpPort);
                (await scripts.player.createAssetIds(this, "QRSTUVWg")).send(IpPort);
            }
        }
    }

    public void _CreateTools()
    {
        foreach (var tool in Game.Tools) AddTool(tool);
    }

    public void _CreateTeams()
    {
        foreach (var team in Game.Teams)
        {
            new PacketBuilder(10)
                .u32(team.NetId)
                .String(team.Name)
                .u32(team.Color.dec())
                .send(IpPort);
        }
    }

    public void _CreateBots()
    {
        foreach (var bot in Game.Bots)
        {
            new PacketBuilder(12)
                .u32(bot.NetId)
                .String(bot.Name)

                .Float(bot.Position.x)
                .Float(bot.Position.y)
                .Float(bot.Position.z)

                .Float(bot.Rotation.x)
                .Float(bot.Rotation.y)
                .Float(bot.Rotation.z)

                .Float(bot.Scale.x)
                .Float(bot.Scale.y)
                .Float(bot.Scale.z)

                .u32(bot.Colors.Head.dec())
                .u32(bot.Colors.Torso.dec())
                .u32(bot.Colors.LeftArm.dec())
                .u32(bot.Colors.RightArm.dec())
                .u32(bot.Colors.LeftLeg.dec())
                .u32(bot.Colors.RightLeg.dec())

                .String(Color.formatHex(bot.Speech))

                .send(IpPort);
        }
    }

    //NETWORKING STUFF

    public string IpPort;
    private byte[] CurrentBytes = { };
    public void HandleBytes(byte[] newBytes)
    {

        CurrentBytes = CurrentBytes.Concat(newBytes).ToArray();//add new bytes
        var (messageSize, end) = UIntV.ReadUIntV(CurrentBytes);//get uintv stuff
        if (messageSize + end > CurrentBytes.Length) return; //return if there arent enough bytes

        if (Game.Debug.PacketInspector) Console.WriteLine("\nmessageSize: {0} | end: {1} | netId: {2} | remain: {3}", messageSize, end, NetId, CurrentBytes.Length);//debug stuff

        byte[] workBytes = new byte[messageSize]; //make new array with the size to fit the bytes we need
        Array.Copy(CurrentBytes, end, workBytes, 0, messageSize + end - 1); //add the bytes we need
        CurrentBytes = CurrentBytes.Skip(messageSize + end).ToArray(); //remove the bytes we just took from the current bytes array

        if (workBytes[0] == 120)//check for compression
            workBytes = ZlibStream.UncompressBuffer(workBytes);

        if (Game.Debug.PacketInspector)
        {
            foreach (var data in workBytes)
                Console.Write(data.ToString() + " ");
            Console.WriteLine();
        }

        if (CurrentBytes.Length > 0) HandleBytes(new byte[0]);

        //now do stuff here with workBytes
        PacketHandler packet = new(workBytes);
        switch (packet.u8())
        {
            case 1://authenitication
                {
                    if (Token != null) return;
                    //Console.WriteLine(packet.String());//version
                    try
                    {
                        var data = api.checkAuth(packet.String());
                        Name = data.Name;
                        userId = data.userId;
                        admin = data.admin;
                        membership = data.membership;
                        Token = data.token;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Kick("Token invalid!");
                    }
                    SetAvatar(userId);
                    Message(Game.MOTD);
                    Game.MessageAll($"<color:FF7A00>[SERVER] : \\c0 {Name} connected to the server.");
                    string testGameName() { if (Game.SetData == null) { return "Local"; } else { return Game.SetData.data.Name; }; }
                    new PacketBuilder(1)
                           .u32(NetId)                     //netid
                           .u32((uint)(Game.Bricks.Count)) //brickcount
                           .u32(userId)                    //userid
                           .String(Name)                   //name
                           .Bool(admin)                    //admin
                           .u8(membership)                 //membership
                           .u32(1)                         //gameid
                           .String(testGameName())          //gamename
                           .send(IpPort);

                    new PacketBuilder(3)
                        .u8(1)
                        .u32(NetId)
                        .String(Name)
                        .u32(userId)
                        .Bool(admin)
                        .u8(membership)
                        .broadcastExcept(IpPort);


                    scripts.player.SendEnv(IpPort);

                    scripts.player.SendPlayers(IpPort);

                    new Thread(() => new scripts.world.SendBRK(IpPort, NetId)).Start();
                    Respawn();

                    break;
                }
            case 2://position
                {
                    Position.x = packet.Float();
                    Position.y = packet.Float();
                    Position.z = packet.Float();
                    Rotation = packet.Float();
                    CameraRotation.x = packet.Float();
                    _UpdatePositionForOthers();
                    break;
                }
            case 3://commands and chat
                {
                    if (packet.String() != "chat") return;
                    MessageAll(packet.String());

                    break;
                }
            default:
                break;
        }


    }

}