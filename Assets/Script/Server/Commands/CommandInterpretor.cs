using Assets.Script.Server.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Server
{
    class CommandInterpretor
    {
        private static CommandInterpretor instance = null;
        public static CommandInterpretor Instance
        {
            get
            {
                if (instance == null)
                    instance = new CommandInterpretor();
                return instance;
            }
        }

        public void doCommand(Command cmd)
        {
            switch (cmd.comand)
            {
                case Command.ComandsType.LOG:
                    break;
                case Command.ComandsType.ERROR:
                    break;
            }
        }
    }
}
