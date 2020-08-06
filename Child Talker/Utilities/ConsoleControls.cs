using System;
using System.Diagnostics;
using System.Threading;

namespace Child_Talker.Utilities
{
    class ConsoleControls
    {
        public int[] relayValues = { 0, 0, 0 };

        public void RelayControl(int index)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = @"C:\Program Files\PuTTY\plink.exe";
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.Arguments = "-ssh ubnt@192.168.1.84 -pw ubnt ";
            cmd.StartInfo.CreateNoWindow = true;
            string arg = "echo " + relayValues[index - 1] + " > relay" + (index);

            _ = cmd.Start();
            Thread.Sleep(200);
            cmd.StandardInput.WriteLine('y');
            Thread.Sleep(200);
            cmd.StandardInput.WriteLine("cd /proc/power");
            Thread.Sleep(200);
            cmd.StandardInput.WriteLine(arg);
            Thread.Sleep(200);
            cmd.StandardInput.WriteLine("exit");
            string output = cmd.StandardOutput.ReadToEnd();

            relayValues[index - 1] = (relayValues[index - 1] == 0 ? 1 : 0);
        }

        //IR remote control send codes
        public void remoteControlSend(String remote, String remoteKey)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = @"C:\Program Files\PuTTY\plink.exe";
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.Arguments = "-ssh pi@192.168.1.11 -pw 9404CSE453 ";
            cmd.StartInfo.CreateNoWindow = true;
            _ = cmd.Start();
            Thread.Sleep(200);
            cmd.StandardInput.WriteLine('y');
            Thread.Sleep(200);
            cmd.StandardInput.WriteLine("sudo systemctl stop lircd.service");
            Thread.Sleep(200);
            cmd.StandardInput.WriteLine("irsend send_start " + remote + " " + remoteKey);
            Thread.Sleep(200);
            cmd.StandardInput.WriteLine("irsend send_stop " + remote + " " + remoteKey);
            Thread.Sleep(200);
            cmd.StandardInput.WriteLine("exit");
            string output = cmd.StandardOutput.ReadToEnd();
        }

    }
}
