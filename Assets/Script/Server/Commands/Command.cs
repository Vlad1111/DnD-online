using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Server.Commands
{
    [System.Serializable]
    public class Command
    {
        [System.Serializable]
        public enum ComandsType
        {
            LOG,
            ERROR,
            CREATE_OBJECT,
            UPDATE_OBJECT,
            DELETE_OBJECT,
            SHOW_ROOM,
            HIDE_ROOM
        }
        public ComandsType comand;
        public string name;
        public string[] args;
        public Command[] nextCommands;
    }
}
