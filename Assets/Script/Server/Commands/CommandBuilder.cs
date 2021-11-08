using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Server.Commands
{
    class CommandBuilder
    {
        private static CommandBuilder instance = null;
        public static CommandBuilder Instance{
            get {
                if (instance == null)
                    instance = new CommandBuilder();
                return instance;
            }   
        }

        public byte[] serilize(Command cmd)
        {
            return GENERAL.SerilizeObject(cmd);
        }

        public Command deserilize(byte[] data)
        {
            return GENERAL.DeserilizeObject<Command>(data);
        }

        public Command log(string message)
        {
            var cmd = new Command(CommandFunctions.log, message);
            cmd.sendToAll = true;
            return cmd;
        }

        public Command error(string message)
        {
            return log("ERROR: " + message);
        }

        public Command connected()
        {
            return log("Player connected");
        }

        public Command updateFloor(string roomName, int x, int y, float[,] floor)
        {
            object[] objs = new object[] { roomName, x, y, floor };
            var cmd = new Command(CommandFunctions.updateFloor1, objs);
            return cmd;
        }

        public Command updateRoom(GENERAL.RoomData data)
        {
            return new Command(CommandFunctions.updateRoom, data);
        }
    }
}
