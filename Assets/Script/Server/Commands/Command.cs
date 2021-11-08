using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.Server.Commands
{
    [System.Serializable]
    public class Command
    {
        public object data;
        public Func<object, object> doThis;
        public bool sendToAll = false;
        public Command nextCommand;

        public Command(Func<object, object> doThis, object data = null)
        {
            this.data = data ?? throw new ArgumentNullException(nameof(data));
            this.doThis = doThis ?? throw new ArgumentNullException(nameof(doThis));
            nextCommand = null;
        }

        public void setNextCommand(Command nextCmd)
        {
            this.nextCommand = nextCmd;
        }
    }

    public class CommandFunctions
    {
        public static Func<object, object> none = (object data) =>
        {
            return data;
        };

        public static Func<object, object> log = (object data) =>
        {
            var ms = data as string;
            UiLogBehaviour.instance.addMesage(ms);
            return data;
        };
        public static Func<object, object> updateFloor1 = (object data) =>
        {
            var objs = data as object[];
            CreateRoom.instance.selectRoom(objs[0] as string);
            CreateRoom.instance.updateTileFloor((int)objs[1], (int)(objs[2]), (float[,])(objs[3]));
            return data;
        };
        public static Func<object, object> updateRoom = (object data) =>
        {
            var d = data as GENERAL.RoomData;
            CreateRoom.instance.updateRoom(d);
            return data;
        };
    }
}
