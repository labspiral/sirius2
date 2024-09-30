/*
 * 
 *                                                            ,--,      ,--,                              
 *             ,-.----.                                     ,---.'|   ,---.'|                              
 *   .--.--.   \    /  \     ,---,,-.----.      ,---,       |   | :   |   | :      ,---,           ,---,.  
 *  /  /    '. |   :    \ ,`--.' |\    /  \    '  .' \      :   : |   :   : |     '  .' \        ,'  .'  \ 
 * |  :  /`. / |   |  .\ :|   :  :;   :    \  /  ;    '.    |   ' :   |   ' :    /  ;    '.    ,---.' .' | 
 * ;  |  |--`  .   :  |: |:   |  '|   | .\ : :  :       \   ;   ; '   ;   ; '   :  :       \   |   |  |: | 
 * |  :  ;_    |   |   \ :|   :  |.   : |: | :  |   /\   \  '   | |__ '   | |__ :  |   /\   \  :   :  :  / 
 *  \  \    `. |   : .   /'   '  ;|   |  \ : |  :  ' ;.   : |   | :.'||   | :.'||  :  ' ;.   : :   |    ;  
 *   `----.   \;   | |`-' |   |  ||   : .  / |  |  ;/  \   \'   :    ;'   :    ;|  |  ;/  \   \|   :     \ 
 *   __ \  \  ||   | ;    '   :  ;;   | |  \ '  :  | \  \ ,'|   |  ./ |   |  ./ '  :  | \  \ ,'|   |   . | 
 *  /  /`--'  /:   ' |    |   |  '|   | ;\  \|  |  '  '--'  ;   : ;   ;   : ;   |  |  '  '--'  '   :  '; | 
 * '--'.     / :   : :    '   :  |:   ' | \.'|  :  :        |   ,/    |   ,/    |  :  :        |   |  | ;  
 *   `--'---'  |   | :    ;   |.' :   : :-'  |  | ,'        '---'     '---'     |  | ,'        |   :   /   
 *             `---'.|    '---'   |   |.'    `--''                              `--''          |   | ,'    
 *               `---`            `---'                                                        `----'   
 * 
 * 2023 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : my remote communication by tcp/ip
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Reflection;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Runtime.CompilerServices;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Winforms.Entity;
using SpiralLab.Sirius2.Winforms.OpenGL;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Mathematics;
using SpiralLab.Sirius2.Winforms.UI;
using SpiralLab.Sirius2.Winforms.Marker;
using SpiralLab.Sirius2.Winforms.Remote;
using OpenTK;
using SpiralLab.Sirius2.Laser;

namespace Demos
{
    /// <summary>
    /// Customized remote communication by tcp/ip
    /// <para>
    /// Used with tcp/ip server. <br/>
    /// </para>
    /// </summary>
    /// <remarks>
    /// Example commands are: <br/>
    /// 1. "Recipe;", "Recipe,FileName;" <br/>
    /// 2. "Offset;", "Offset,Count(s),X,Y,Z,AngleZ,...;" <br/>
    /// 3. "Marker,Start;", "Marker,Stop;", "Marker,Reset;" <br/>
    /// 4. "Laser,On;", "Laser,Off;" <br/>
    /// 5. "Entity,EntityName,Properties;", "Entity,EntityName,PropName;", "Entity,EntityName,PropName,PropValue;" <br/>
    /// 6. "Pen,PenName,Properties;", "Pen,PenName,PropName;", "Entity,PenName,PropName,PropValue;" <br/>
    /// 7. "FieldCorrection,Rows,Cols,Interval,X,Y,...;" <br/>
    /// 8. "Status;" <br/>
    /// </remarks>
    public class MyRemoteTcpServer
        : IRemote
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <inheritdoc/>
        public event Action<IRemote> OnConnected;
        /// <inheritdoc/>
        public event Action<IRemote> OnDisconnected;
        /// <inheritdoc/>
        public event Action<IRemote, ControlModes> OnModeChanged;
        /// <inheritdoc/>
        public event Func<IRemote, string, bool> OnReceived;

        /// <inheritdoc/>
        [Browsable(false)]
        public virtual object SyncRoot { get; protected set; } = new object();

        /// <inheritdoc/>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Basic")]
        [DisplayName("Index")]
        [Description("Index")]
        public virtual int Index { get; set; }

        /// <inheritdoc/>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Basic")]
        [DisplayName("Name")]
        [Description("Name")]
        public virtual string Name { get; set; }

        /// <inheritdoc/>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Status")]
        [DisplayName("Connected")]
        [Description("Client(or Peer) has Connected")]
        public virtual bool IsConnected
        {
            get { return isConnected; }
            protected set
            {
                var oldConnected = isConnected;
                isConnected = value;
                if (oldConnected != isConnected)
                {
                    if (isConnected)
                        NotifyConnected();
                    else
                        NotifyDisconnected();
                    NotifyPropertyChanged();
                }
            }
        }
        private bool isConnected = false;

        /// <inheritdoc/>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Status")]
        [DisplayName("Mode")]
        [Description("Mode")]
        public virtual ControlModes ControlMode
        {
            get { return mode; }
            set
            {
                var oldMode = mode;
                mode = value;
                if (oldMode != mode)
                {
                    NotifyModeChanged(mode);
                    NotifyPropertyChanged();
                    Logger.Log(Logger.Types.Info, $"remote [{Index}]: control mode has changed to {mode}");
                }
            }
        }
        private ControlModes mode = ControlModes.Local;
       
        /// <inheritdoc/>
        [Browsable(false)]
        public virtual IMarker Marker { get; protected set; }

        [Browsable(false)]
        public virtual System.Windows.Forms.Control EditorControl { get; set; }

        Thread thread;
        TcpListener listener;
        TcpClient client;
        int portNo;
        bool terminated = false;
        bool disposed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Identifier (0,1,2,...)</param>
        /// <param name="name">Name</param>
        /// <param name="marker">Target <see cref="IMarker">IMarker</see><br/>'Status,Started;', 'Status,Ended;' would be notified automatically. <br/></param>
        /// <param name="portNo">TCP server listening port no</param>
        public MyRemoteTcpServer(int index, string name, IMarker marker, int portNo)
        {
            this.Index = index;
            this.Name = name;
            this.portNo = portNo;
            Debug.Assert(marker != null);
            this.Marker = marker;
            marker.OnStarted += Marker_OnStarted;
            marker.OnEnded += Marker_OnEnded;
        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~MyRemoteTcpServer()
        {
            this.Dispose(false);
        }
        /// <inheritdoc/>
        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <inheritdoc/>
        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //this.thread?.Join(2 * 1000);
                    Marker.OnStarted -= Marker_OnStarted;
                    Marker.OnEnded -= Marker_OnEnded;
                }
                this.Stop();
                this.disposed = true;
            }
        }

        /// <summary>
        /// Implement <see cref="IMarker.OnStarted">IMarker.OnStarted</see> event to send "Status,Started;" response by remote communication
        /// </summary>
        /// <param name="marker"><see cref="IMarker">IMarker</see></param>
        void Marker_OnStarted(IMarker marker)
        {
            if (this.IsConnected)
            {
                var text = string.Format($"Status{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}Started{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                this.Send(text);
            }
        }
        /// <summary>
        /// Implement <see cref="IMarker.OnEnded">IMarker.OnEnded</see> event to send "Status,Ended;" response by remote communication
        /// </summary>
        /// <param name="marker"><see cref="IMarker">IMarker</see></param>
        /// <param name="success">Success(or failed) to mark</param>
        /// <param name="timeSpan"><c>TimeSpan</c></param>
        void Marker_OnEnded(IMarker marker, bool success, TimeSpan timeSpan)
        {
            if (this.IsConnected)
            {
                var text = string.Format($"Status{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}Ended{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                this.Send(text);
            }
        }
        /// <summary>
        /// Notify property value has change event
        /// </summary>
        /// <param name="propertyName">Property name</param>
        void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var receivers = this.PropertyChanged?.GetInvocationList();
            if (null != receivers)
                foreach (PropertyChangedEventHandler receiver in receivers)
                    receiver.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Notify communication has connected
        /// </summary>
        void NotifyConnected()
        {
            var receivers = this.OnConnected?.GetInvocationList();
            if (null != receivers)
                foreach (Action<IRemote> receiver in receivers)
                    receiver.Invoke(this);
        }
        /// <summary>
        /// Notify communication has disconnected
        /// </summary>
        void NotifyDisconnected()
        {
            var receivers = this.OnDisconnected?.GetInvocationList();
            if (null != receivers)
                foreach (Action<IRemote> receiver in receivers)
                    receiver.Invoke(this);
        }
        /// <summary>
        /// Notify <see cref="ControlModes">ControlModes</see> has changed
        /// </summary>
        /// <param name="mode"><see cref="ControlModes">ControlModes</see> </param>
        void NotifyModeChanged(ControlModes mode)
        {
            var receivers = this.OnModeChanged?.GetInvocationList();
            if (null != receivers)
                foreach (Action<IRemote, ControlModes> receiver in receivers)
                    receiver.Invoke(this, mode);
        }

        /// <inheritdoc/>
        public bool Start()
        {
            if (null != this.thread && thread.IsAlive)
            {
                Logger.Log(Logger.Types.Info, $"remote [{Index}]: tcp comm already rurnning...");
                return true;
            }

            this.terminated = false;
            try
            {
                this.listener = new TcpListener(IPAddress.Any, portNo);
                this.listener.Start();
                this.thread = new Thread(this.WorkerThread);
                this.thread.Name = $"Remote Listener Thread";
                Logger.Log(Logger.Types.Info, $"remote [{Index}]: tcp comm has started at {portNo}");
                this.thread.Start();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.Types.Error, $"remote [{Index}]: tcp comm fail to start. exception: {ex.Message}");
                return false;
            }
        }
        /// <inheritdoc/>
        public bool Stop()
        {
            if (thread == null)
                return true;
            this.terminated = true;
            this.listener?.Stop();
            this.client?.Dispose();
            this.thread?.Join(2 * 1000);
            this.thread = null;
            Logger.Log(Logger.Types.Info, $"remote [{Index}]: tcp comm has stopped at {portNo}");
            return true;
        }
        /// <inheritdoc/>
        public bool Send(byte[] bytes)
        {
            if (null == this.client)
                return false;
            if (!this.client.Connected)
                return false;
            try
            {
                var nstream = client.GetStream();
                lock (SyncRoot)
                {
                    nstream.WriteAsync(bytes, 0, bytes.Length);
                }
                Logger.Log(Logger.Types.Debug, $"remote [{Index}]: tcp comm data has sent: {Encoding.ASCII.GetString(bytes)}");
                return true;

            }
            catch (Exception ex)
            {
                Logger.Log(Logger.Types.Error, $"remote [{Index}]: tcp comm data fail to send. exception : {ex.Message}");
                return false;
            }
        }
        /// <inheritdoc/>
        public bool Send(string text)
        {
            if (null == this.client)
                return false;
            if (!this.client.Connected)
                return false;
            try
            {
                var nstream = client.GetStream();
                byte[] bytes = Encoding.ASCII.GetBytes(text);
                lock (SyncRoot)
                {
                    nstream.WriteAsync(bytes, 0, bytes.Length);
                }
                Logger.Log(Logger.Types.Debug, $"remote [{Index}]: tcp comm data has sent: {text}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.Types.Error, $"remote [{Index}]: tcp comm data fail to send. exception: {ex.Message}");
                return false;
            }
        }
        /// <inheritdoc/>
        public bool Receive(out string text)
        {
            text = string.Empty;
            if (null == this.client)
                return false;
            if (!this.client.Connected)
                return false;
            try
            {
                var nstream = client.GetStream();
                if (!nstream.CanRead)
                    return false;
                byte[] buffer = new byte[1024];
                int bytes = nstream.Read(buffer, 0, buffer.Length);
                if (bytes <= 0)
                {
                    Logger.Log(Logger.Types.Warn, $"remote [{Index}]: tcp comm received has no data. disconnecting ?");
                    return false;
                }
                text = Encoding.ASCII.GetString(buffer, 0, bytes);
                return true;
            }
            catch (Exception ex)
            {
                //Logger.Log(Logger.Type.Error, ex);
                Logger.Log(Logger.Types.Debug, $"remote [{Index}]: tcp comm fail to receive. exception: {ex.Message}");
            }
            return false;
        }
        /// <inheritdoc/>
        public bool Receive(out byte[] bytes)
        {
            bytes = null;
            if (null == this.client)
                return false;
            if (!this.client.Connected)
                return false;
            try
            {
                var nstream = client.GetStream();
                if (!nstream.CanRead)
                    return false;
                byte[] buffer = new byte[1024];
                int readBytes = nstream.Read(buffer, 0, buffer.Length);
                if (0 <= readBytes)
                {
                    Logger.Log(Logger.Types.Warn, $"remote [{Index}]: tcp comm: received has no data. disconnecting ?");
                    return false;
                }
                bytes = new byte[readBytes];
                Array.Copy(buffer, bytes, readBytes);
                return true;
            }
            catch (Exception ex)
            {
                //Logger.Log(Logger.Type.Error, ex);
                Logger.Log(Logger.Types.Debug, $"remote [{Index}]: tcp comm fail to receive. exception: {ex.Message}");
            }
            return false;
        }
        void WorkerThread()
        {
            while (!terminated)
            {
                try
                {
                    var newClient = listener.AcceptTcpClient();
                    var ipEndPoint = (IPEndPoint)newClient.Client.RemoteEndPoint;
                    this.client?.Dispose();
                    this.IsConnected = false;
                    Thread.Sleep(500);
                    Logger.Log(Logger.Types.Info, $"remote [{Index}]: tcp comm has connected new client: {ipEndPoint.Address.ToString()}:{ipEndPoint.Port}");
                    this.client = newClient;
                    this.IsConnected = true;                    
                    Task.Run(() => ClientThread(newClient));
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode == SocketError.Interrupted)
                    {

                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(Logger.Types.Error, $"remote [{Index}]: tcp comm fail to accept client. exception: {ex.Message}");
                }
            }
        }
        void ClientThread(TcpClient newClient)
        {
            var sb = new StringBuilder();
            try
            {
                var ipEndPoint = (IPEndPoint)newClient.Client.RemoteEndPoint;
                while (!terminated && null != newClient && newClient.Connected)
                {
                    if (!this.Receive(out string resp))
                    {
                        Logger.Log(Logger.Types.Warn, $"remote [{Index}]: tcp comm has disconnecting client: {ipEndPoint.Address.ToString()}:{ipEndPoint.Port}");
                        newClient?.Close();
                        break;
                    }
                    if (string.IsNullOrEmpty(resp))
                        continue;
                    sb.Append(resp);
                    for ( ; ; )
                    {
                        var buffer = sb.ToString();
                        int index = buffer.IndexOf(SpiralLab.Sirius2.Winforms.Config.RemoteTerminator);
                        if (index < 0)
                            break;
                        string part = buffer.Substring(0, index);
                        Logger.Log(Logger.Types.Debug, $"remote [{Index}]: tcp comm data has received: {part}");
                        bool success = true;
                        try
                        {
                            success &= this.ProcessFormat(part);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(Logger.Types.Error, $"remote [{Index}]: {ex.Message} at {part}");
                        }
                        if (!success)
                            Logger.Log(Logger.Types.Error, $"remote [{Index}]: tcp comm fail to parse format");
                        sb.Remove(0, index + 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.Types.Error, $"remote [{Index}]: tcp comm fail to parse. exception: {ex.Message}");
            }
            finally
            {
                Logger.Log(Logger.Types.Warn, $"remote [{Index}]: tcp comm has disconnected client");
                this.IsConnected = false;
            }
        }

        /// <summary>
        /// Parse received data
        /// </summary>
        /// <param name="data">Received data</param>
        /// <returns>Success or failed</returns>
        public virtual bool ProcessFormat(string data)
        {
            var marker = this.Marker;
            var doc = marker.Document;

            string[] tokens = data.Split(new char[] { SpiralLab.Sirius2.Winforms.Config.RemoteSeparator });
            if (0 == tokens.Length)
                return false;
            bool success = true;
            switch (tokens[0]?.Trim().ToLower())
            {
                case "recipe":
                    if (tokens.Length >= 2)
                        success = this.RecipeParse(tokens[1]);
                    else
                    {
                        this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteOk}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                        this.Send($"Recipe{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}{doc.FileName}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                    }
                    break;
                case "offset":
                    if (tokens.Length >= 2)
                        success = this.OffsetParse(tokens.Skip(1).ToArray());
                    else
                    {
                        this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteOk}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                        var sb = new StringBuilder();
                        sb.Append($"Offset{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}{marker.Offsets.Length}");
                        foreach (var offset in marker.Offsets)
                        {
                            sb.Append($"{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}");
                            sb.Append($"{offset.Dx}{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}");
                            sb.Append($"{offset.Dy}{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}");
                            sb.Append($"{offset.Dz}{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}");
                            sb.Append($"{offset.AngleZ}");
                        }
                        sb.Append($"{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                        this.Send(sb.ToString());
                    }
                    break;               
                case "marker":
                    if (tokens.Length >= 2)
                        success = this.MarkerParse(tokens[1], tokens.Skip(2).ToArray());
                    break;
                case "laser":
                    if (tokens.Length >= 2)
                        success = this.LaserParse(tokens[1], tokens.Skip(2).ToArray());
                    break;
                case "entity":
                    if (tokens.Length >= 3)
                        success = this.EntityParse(tokens[1], tokens[2], tokens.Skip(3).ToArray());
                    break;
                case "pen":
                    if (tokens.Length >= 3)
                        success = this.PenParse(tokens[1], tokens[2], tokens.Skip(3).ToArray());
                    break;
                case "fieldcorrection":
                    if (tokens.Length >= 2)
                        success = this.FieldCorrectionParse(tokens.Skip(1).ToArray());
                    break;
                case "status":
                    success = this.StatusParse();
                    break;
                default:
                    success = false;
                    Logger.Log(Logger.Types.Error, $"remote [{Index}]: unknown '{tokens[0]}' command");
                    this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                    break;
            }
            return success;
        }
        /// <summary>
        /// Change recipe file
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <returns>Success or failed</returns>
        protected virtual bool RecipeParse(string fileName)
        {
            var marker = this.Marker;
            var doc = marker.Document;
            if (ControlMode == ControlModes.Local)
            {
                Logger.Log(Logger.Types.Error, $"remote [{Index}]: fail to change recipe. control mode is local");
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                return true;
            }

            if (marker.IsBusy)
            {
                Logger.Log(Logger.Types.Error, $"remote [{Index}]: try to change recipe but marker is busy!");
                return false;
            }
            if (!File.Exists(fileName))
            {
                Logger.Log(Logger.Types.Error, $"remote [{Index}]: recipe file is not exist: {fileName}");
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                return true;
            }
            bool success = true;
            var mainControl = Application.OpenForms[0] as Control;
            mainControl.Invoke(new MethodInvoker(delegate ()
            {
                success &= doc.ActOpen(fileName);
                if (success)
                {
                    // offset will be reset at marker
                    success &= marker.Ready(doc);//, view, rtc, laser, powerMeter, remote);                  
                }
            }));

            if (success)
            {
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteOk}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
            }
            else
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
            return true;
        }
        /// <summary>
        /// Read/Write <c>IEntity</c> property
        /// </summary>
        /// <param name="entityName">Name of <c>IEntity</c></param>
        /// <param name="propName">Name of property</param>
        /// <param name="param">Array of property param</param>
        /// <returns>Success or failed</returns>
        protected virtual bool EntityParse(string entityName, string propName, string[] param)
        {
            var marker = this.Marker;
            var doc = marker.Document;
            var view = marker.View;
            var rtc = marker.Rtc;
            var laser = marker.Laser;
            var mainControl = Application.OpenForms[0] as Control;
            bool success = true;
            mainControl.Invoke(new MethodInvoker(delegate ()
            {
                if (!doc.FindByName(entityName, out var entity))
                {
                    Logger.Log(Logger.Types.Error, $"remote [{Index}]: entity name= {entityName} is not exist");
                    this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                    success = false;
                    return;
                }
                switch (propName?.Trim().ToLower())
                {
                    case "properties":
                        this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteOk}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                        var dic = RemoteHelper.QueryProperties(entity);
                        foreach (var kv in dic)
                        {
                            // one by one
                            if (null != kv.Value)
                            {
                                string s = $"{kv.Key.ToString()}{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}{kv.Value?.ToString()}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}";
                                this.Send(s);
                            }
                        }
                        success = true;
                        return;
                }

                Type type = entity.GetType();
                var propInfo = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (null == propInfo)
                {
                    //not exist
                    Logger.Log(Logger.Types.Error, $"remote [{Index}]: entity property '{propName}' is not exist");
                    this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                    success = false;
                    return;
                }

                try
                {
                    //read
                    if (null == param || param.Length == 0)
                    {
                        string resp = RemoteHelper.ReadProperty(entity, propInfo, propName);
                        this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteOk}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                        this.Send($"Entity{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}{entityName}{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}{propName}{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}{resp.ToString()}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                        success = true;
                        return;
                    }
                    else //write
                    {
                        if (ControlMode == ControlModes.Local)
                        {
                            Logger.Log(Logger.Types.Error, $"remote [{Index}]: fail to set entity property. control mode is local");
                            this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                            success = false;
                            return;
                        }
                        if (!propInfo.CanWrite)
                        {
                            //not exist
                            Logger.Log(Logger.Types.Error, $"remote [{Index}]: entity property '{propName}' is not writable");
                            this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                            success = false;
                            return;
                        }
                        success = RemoteHelper.WriteProperty(entity, propInfo, propName, param);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(Logger.Types.Error, ex);
                    this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                    success = false;
                    return;
                }

                if (success)
                {
                    entity.Regen();
                    view.Render();
                    // To refresh 
                    // editorCtrl.PropertyGridCtrl.Refresh();
                    this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteOk}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                }
                else
                    this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
            }));
            return success;
        }
        /// <summary>
        /// Read/Write <c>EntityPen</c> property
        /// </summary>
        /// <param name="penName">Name of pen entity</param>
        /// <param name="propName">Name of propert</param>
        /// <param name="param">Array of property param</param>
        /// <returns>Success or failed</returns>
        protected virtual bool PenParse(string penName, string propName, string[] param)
        {
            var marker = this.Marker;
            var doc = marker.Document;
            var view = marker.View;
            var rtc = marker.Rtc;
            var laser = marker.Laser;
            var mainControl = Application.OpenForms[0] as Control;
            bool success = true;
            EntityPen pen = null;
            mainControl.Invoke(new MethodInvoker(delegate ()
            {
                if (!doc.FindByPenName(penName, out pen))
                {
                    // Search pen by color
                    System.Drawing.Color color = System.Drawing.Color.FromName(penName);
                    if (!doc.FindByPenColor(color, out pen))
                    {
                        //not exist
                        Logger.Log(Logger.Types.Error, $"remote [{Index}]: pen name= {penName} is not exist");
                        this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                        success = false;
                        return;
                    }
                }

                switch (propName?.Trim().ToLower())
                {
                    case "properties":
                        this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteOk}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                        var dic = RemoteHelper.QueryProperties(pen);
                        foreach (var kv in dic)
                        {
                            // one by one
                            if (null != kv.Value)
                            {
                                string s = $"{kv.Key.ToString()}{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}{kv.Value?.ToString()}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}";
                                this.Send(s);
                            }
                        }
                        success = true;
                        return;
                }

                Type type = pen.GetType();
                var propInfo = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (null == propInfo)
                {
                    //not exist
                    Logger.Log(Logger.Types.Error, $"remote [{Index}]: pen property '{propName}' is not exist");
                    this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                    success = false;
                    return;
                }

                try
                {
                    //read
                    if (null == param || param.Length == 0)
                    {
                        string resp = RemoteHelper.ReadProperty(pen, propInfo, propName);
                        this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteOk}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                        this.Send($"Pen{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}{penName}{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}{propName}{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}{resp.ToString()}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                        success = true;
                        return;
                    }
                    else //write
                    {
                        if (ControlMode == ControlModes.Local)
                        {
                            Logger.Log(Logger.Types.Error, $"remote [{Index}]: fail to set pen property. control mode is local");
                            this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                            success = false;
                            return;
                        }
                        if (!propInfo.CanWrite)
                        {
                            //not exist
                            Logger.Log(Logger.Types.Error, $"remote [{Index}]: pen property '{propName}' is not writable");
                            this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                            success = false;
                            return;
                        }
                        success = RemoteHelper.WriteProperty(pen, propInfo, propName, param);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(Logger.Types.Error, ex);
                    this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                    success = false;
                    return;
                }

                if (success)
                {
                    pen.Regen();
                    view.Render();
                    // To refresh 
                    // editorCtrl.PropertyGridCtrl.Refresh();
                    this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteOk}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                }
                else
                    this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
            }));
            return success;
        }

        /// <summary>
        /// Change array of <c>Offset</c> at <c>IMarker</c>
        /// </summary>
        /// <param name="param">Counts, dx,dy,dz,dAngle,...</param>
        ///  <returns>Success or failed</returns>
        protected virtual bool OffsetParse(string[] param)
        {
            var marker = this.Marker;
            if (ControlMode == ControlModes.Local)
            {
                Logger.Log(Logger.Types.Error, $"remote [{Index}]: fail to set offset. control mode is local");
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                return true;
            }
            if (marker.IsBusy)
            {
                Logger.Log(Logger.Types.Error, $"remote [{Index}]: try to update offset but marker is busy!");
                return false;
            }
            bool success = true;
            try
            {
                int counts = int.Parse(param[0]);
                var offsets = new List<Offset>(counts);
                for (int i = 0; i < counts; i++)
                {
                    float x = float.Parse(param[i * 3 + 1]);
                    float y = float.Parse(param[i * 3 + 2]);
                    float z = float.Parse(param[i * 3 + 3]);
                    float angle = float.Parse(param[i * 3 + 4]);
                    var offset = new Offset(x, y, z, angle);
                    offsets.Add(offset);
                }
                marker.Offsets = offsets.ToArray();
            }
            catch (Exception ex)
            {
                marker.Offsets = new Offset[] { Offset.Zero };
                Logger.Log(Logger.Types.Error, ex);
                success = false;
            }
            if (success)
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteOk}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
            else
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
            return true;
        }
        /// <summary>
        /// marker commands
        /// </summary>
        /// <param name="cmd">Commands like as start, stop, reset </param>
        /// <param name="param">Array of param</param>
        /// <returns>Success of failed</returns>
        protected virtual bool MarkerParse(string cmd, string[] param)
        {
            var marker = this.Marker;
            if (ControlMode == ControlModes.Local)
            {
                Logger.Log(Logger.Types.Error, $"remote [{Index}]: fail to control marker. control mode is local");
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                return true;
            }

            bool success = true;
            switch (cmd?.Trim().ToLower())
            {
                case "start":                    
                    success &= marker.Start();
                    break;
                case "stop":
                    success &= marker.Stop();
                    break;
                case "reset":
                    success &= marker.Reset();
                    break;
                default:
                    Logger.Log(Logger.Types.Error, $"remote [{Index}]: unknown control command: {cmd}");
                    success &= false;
                    break;
            }
            if (success)
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteOk}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
            else
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
            return true;
        }
        /// <summary>
        /// laser commands
        /// </summary>
        /// <param name="cmd">Commands like as on, off</param>
        /// <param name="param">Array of param</param>
        /// <returns>Success of failed</returns>
        protected virtual bool LaserParse(string cmd, string[] param)
        {
            var marker = this.Marker;
            var rtc = marker.Rtc;
            var laser = marker.Laser;
            var laserPowerControl = laser as ILaserPowerControl;
            if (ControlMode == ControlModes.Local)
            {
                Logger.Log(Logger.Types.Error, $"remote [{Index}]: fail to control laser. control mode is local");
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                return true;
            }
            bool success = true;
            switch (cmd?.Trim().ToLower())
            {
                case "on":
                    //success &= rtc.CtlFrequency()
                    // success &= laserPowerControl.CtlPower(watt);
                    success &= rtc.CtlLaserOn(); 
                    break;
                case "off":
                    success &= rtc.CtlLaserOff();
                    break;
                default:
                    Logger.Log(Logger.Types.Error, $"remote [{Index}]: unknown control command: {cmd}");
                    success &= false;
                    break;
            }
            if (success)
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteOk}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
            else
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
            return true;
        }
        /// <summary>
        /// Query status
        /// </summary>
        /// <returns>Success or failed</returns>
        protected virtual bool StatusParse()
        {
            var marker = this.Marker;
            string text = string.Empty;
            if (marker.IsError)
                text = $"Status{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}{SpiralLab.Sirius2.Winforms.Config.RemoteError}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}";
            else if (marker.IsBusy)
                text = $"Status{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}{SpiralLab.Sirius2.Winforms.Config.RemoteBusy}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}";
            else if (marker.IsReady)
                text = $"Status{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}{SpiralLab.Sirius2.Winforms.Config.RemoteReady}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}";
            else
                text = $"Status{SpiralLab.Sirius2.Winforms.Config.RemoteSeparator}{SpiralLab.Sirius2.Winforms.Config.RemoteNotReady}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}";
            this.Send(text);
            return true;
        }
        /// <summary>
        /// Scanner field correction 2D
        /// </summary>
        /// <param name="param">Row,Col,interval, ErrX1, ErrY1, ErrX2, ErrY2, ...</param>
        /// <returns>Success or failed</returns>
        protected virtual bool FieldCorrectionParse(string[] param)
        {
            var marker = this.Marker;
            var rtc = marker.Rtc;

            if (ControlMode == ControlModes.Local)
            {
                Logger.Log(Logger.Types.Error, $"remote [{Index}]: fail to do scanner field correction. control mode is local");
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
                return true;
            }
            if (marker.IsBusy)
            {
                Logger.Log(Logger.Types.Error, $"remote [{Index}]: try to do scanner field correction but marker is busy!");
                return false;
            }
            bool success = true;
            var mainControl = Application.OpenForms[0] as Control;
            try
            {
                int rows = int.Parse(param[0]);
                int cols = int.Parse(param[1]);
                float interval = float.Parse(param[2]);
                success &= rows >= 3 && (rows % 2) == 1;
                success &= cols >= 3 && (cols % 2) == 1;
                if (success)
                {
                    float left = -interval * (float)(int)(cols / 2);
                    float top = interval * (float)(int)(rows / 2);
                    var correction2D = new RtcCorrection2D(rtc.KFactor, rows, cols, interval, interval, rtc.CorrectionFiles[(int)rtc.PrimaryHeadTable].FileName, string.Empty);
                    int index = 0;
                    for (int row = 0; row < rows; row++)
                    {
                        for (int col = 0; col < cols; col++)
                        {
                            float dx = float.Parse(param[index * 2 + 3]);
                            float dy = float.Parse(param[index * 2 + 4]);
                            correction2D.AddRelative(row, col,
                                new System.Numerics.Vector2(left + col * interval, top - row * interval),
                                new System.Numerics.Vector2(dx, dy));
                            index++;
                        }
                    }
                    var form2D = new RtcCorrection2DForm(rtc, correction2D);
                    mainControl.Invoke(new MethodInvoker(delegate ()
                    {
                        form2D.ShowDialog(mainControl);
                    }));
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.Types.Error, ex);
                success = false;
            }
            if (success)
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteOk}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
            else
                this.Send($"{SpiralLab.Sirius2.Winforms.Config.RemoteNG}{SpiralLab.Sirius2.Winforms.Config.RemoteTerminator}");
            return true;
        }
    }
}
