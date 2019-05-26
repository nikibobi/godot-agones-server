namespace GodotAgones.Server.Models
{
    public class Server
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string IpAddress { get; set; }

        public ushort Port { get; set; }

        public byte MaxPlayers { get; set; }

        public byte CurrentPlayers { get; set; }

        public string PasswordHash { get; set; }

        public bool IsPasswordProtected { get => !string.IsNullOrEmpty(PasswordHash); }

        public bool IsFull { get => CurrentPlayers == MaxPlayers; }

        public bool IsEmpty { get => CurrentPlayers == 0; }
    }
}