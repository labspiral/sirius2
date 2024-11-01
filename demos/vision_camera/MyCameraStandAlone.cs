using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Mathematics;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Vision;
using SpiralLab.Sirius2.Vision.Camera;
using SpiralLab.Sirius2.Vision.Common;
using SpiralLab.Sirius2.Vision.Process;

namespace Demos
{
    public class MyCameraStandAlone: CameraBase
    {
        /// <summary>
        /// Image width and height must be same camera.RawWidth and camera.RawHeight 
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Basic")]
        [DisplayName("Sample image")]
        [Description("Sample Image FileName")]
        public string ImageFileName { get; set; } = Path.Combine(SpiralLab.Sirius2.Vision.Config.SamplePath, "checkerboard.bmp");


        public MyCameraStandAlone(int index, int width, int height, double pixelSize, double lensMagnification, int fps, RotateFlipType rotateFlip)
            : base()
        {
            base.Index = index;
            base.Name = "MyStandAloneCamera";
            base.Width = base.RawWidth = width;
            base.Height = base.RawHeight = height;
            base.PixelSize = pixelSize;
            base.LensMagnification = lensMagnification;
            base.Fps = fps;
            base.RotateFlip = rotateFlip;
            base.ResizeWidthHeight();

            StitchRows = 0;
            StitchCols = 0;
            StitchMarginWidth = 0;
            StitchMarginHeight = 0;

            this.OnProcessLightIntensity += MyCamera_OnProcessLightIntensity;
        }

        /// <inheritdoc/>
        public override bool Initialize()
        {
            bool success = true;
            isReady = success;
            Logger.Log(Logger.Types.Debug, $"[{Index}] {Name}: initialized");
            return true;
        }

        /// <inheritdoc/>
        public override bool CtlExposureTime(double usec)
        {
            // do some control for camera exposure time
            // ...
            base.exposureTime = usec;
            return true;
        }

        /// <inheritdoc/>
        public override bool CtlGrab(IProcess process, object state = null)
        {
            if (this.IsBusy || this.IsError)
                return false;
            bool success = true;
            this.IsBusy = true;
            this.IsReady = false;
            success &= NotifyProcessLightIntensity(this, process, state);
            if (!success)
                return false;
            success &= DoGrab(out var bitmap);
            if (!success)
                return false;
            success &= NotifyImageGrabbed(this, bitmap, process, state);
            bitmap.Dispose();
            if (!success)
                return false;
            Logger.Log(Logger.Types.Debug, $"[{Index}] {Name}: grabbed");
            this.IsBusy = false;
            this.IsReady = true;
            return success;
        }

        /// <summary>
        /// Do internal grab
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        bool DoGrab(out Bitmap bitmap)
        {
            bitmap = null;
            try
            {
                bitmap = (Bitmap)Image.FromFile(ImageFileName);
                bitmap.RotateFlip(this.RotateFlip);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.Types.Error, ex);
                return false;
            }
        }
        System.Threading.Timer timerLive = null;
        public override bool CtlContinousGrab()
        {
            if (null != timerLive)
                return true;
            if (Fps == 0)
                return false;
            var msec = 1000 / Fps;
            timerLive = new System.Threading.Timer(DoLiveCallback, null, 0, msec);
            this.IsBusy = true;
            this.IsReady = false;
            Logger.Log(Logger.Types.Info, $"[{Index}] {Name}: live ...");
            return true;
        }
        void DoLiveCallback(object state)
        {
            IsReady = false;
            IsBusy = true;

            DoGrab(out var bitmap);
            NotifyImageGrabbed(this, bitmap, null);
            bitmap.Dispose();
        }

        public override bool CtlStop()
        {
            if (null != timerLive)
            {
                timerLive.Dispose();
                timerLive = null;
                Logger.Log(Logger.Types.Info, $"[{Index}] {Name}: stopped");
            }
            IsReady = true;
            IsBusy = false;
            return true;
        }

        public override bool CtlReset()
        {
            IsError = false;
            //IsReady = true;
            return true;
        }

        private bool MyCamera_OnProcessLightIntensity(ICamera camear, IProcess process, object state)
        {
            if (null == process)
                return true; //nothing to do

            bool success = true;
            var intensity = process.LightIntensity;
            // set intensity (0~100) value at light controller 
            //
            //
            Logger.Log(Logger.Types.Info, $"{process.ToString()} : intensity= {intensity}");
            return success;
        }

        #region ICameraStitchable intf
        /// <inheritdoc/>
        public override bool CtlStitchedGrab(IProcess process, object state = null)
        {
            return false;
        }
        /// <inheritdoc/>
        public override bool CtlStitchedGrab(int index, IProcess process, object state = null)
        {
            return false;
        }
        #endregion
    }
}