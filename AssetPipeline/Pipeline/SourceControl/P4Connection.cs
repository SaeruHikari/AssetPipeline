using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Perforce.P4;

namespace AssetPipeline.Pipeline.SourceControl
{
    public class SourceControlBase
    {
        public virtual string SolutionName => "None";
        public virtual string CharacterSet => "None";
        public virtual string User => "None";
    }

    public class PerfoceConnection : SourceControlBase
    {
        public override string SolutionName => "Perforce";
        public override string CharacterSet => connection.CharacterSetName;
        public override string User => connection.UserName;

        public PerfoceConnection(string ServerIp, string UserName, string Workspace = "")
        {
            server = new Server(new ServerAddress(ServerIp));
            rep = new Repository(server);
            connection = rep.Connection;
            connection.UserName = UserName;
            connection.Client = new Client();
            connection.Client.Name = Workspace;
            connection.Connect(null);

            IList<Client> clients = rep.GetClients(new Options());
            localClients.AddRange(
                from client in clients
                where !string.IsNullOrEmpty(client.OwnerName) && client.OwnerName.Contains(User)
                select client);
        }

        public List<Client> WorkSpaces => localClients;

        public Connection connection = null;
        public Repository rep = null;
        public Server server = null;
        List<Client> localClients = new List<Client>();
    }
}
