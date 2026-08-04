using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zlipacket.CoreZlipacket.Tools;

namespace Zlipacket.CoreZlipacket.System.Command
{
    public class CommandProcess
    {
        public Guid ID;
        public string name;
        public Delegate command;
        public CoroutineWrapper runningProcess;
        public string[] args;

        public UnityEvent onTerminateAction;

        public CommandProcess(Guid id, string name, Delegate command, string[] args, CoroutineWrapper runningProcess, UnityEvent onTerminateAction)
        {
            ID = id;
            this.name = name;
            this.command = command;
            this.args = args;
            this.runningProcess = runningProcess;
            this.onTerminateAction = onTerminateAction;
        }
    }
}