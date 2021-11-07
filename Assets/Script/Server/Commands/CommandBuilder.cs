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
    }
}
