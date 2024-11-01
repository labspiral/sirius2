using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
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
    /// <summary>
    /// Co-axial camera = scanner + camera adapter + vision
    /// </summary>
    public class MyCamera : CameraBase
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


        public MyCamera(int index, int width, int height, double pixelSize, double lensMagnification, int fps, RotateFlipType rotateFlip, int rows, int cols, int marginWidth, int marginHeight)
            : base()
        {
            base.Index = index;
            base.Name = "MyCoaxialCamera";
            base.Width = base.RawWidth = width;
            base.Height = base.RawHeight = height;
            base.PixelSize = pixelSize;
            base.LensMagnification = lensMagnification;
            base.Fps = fps;
            base.RotateFlip = rotateFlip;
            base.ResizeWidthHeight();
            
            // If using stitched camera (aka. co-axial camera at behind scanner)
            StitchRows = rows;
            StitchCols = cols;
            // Margin means, cut-off pixels at edges 
            StitchMarginWidth = marginWidth;
            StitchMarginHeight = marginHeight;       

            this.OnProcessLightIntensity += MyCamera_OnProcessLightIntensity;
            this.OnStitchedImageGrabStarted += MyCamera_OnStitchedImageGrabStarted;
            this.OnStitchedImageIndexBefore += MyCamera_OnStitchedImageIndexBefore;
            this.OnStitchedImageIndexAfter += MyCamera_OnStitchedImageIndexAfter;
            this.OnStitchedImageGrabEnded += MyCamera_OnStitchedImageGrabEnded;
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
                // Simulated grab(or acquire) image from camera by file
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
            if (null == StitchCalibrator || StitchCalibrator.Cells.Count != this.StitchRows * this.StitchCols)
            {
                Logger.Log(Logger.Types.Error, $"[{Index}] {Name}: stitch positions are not exist for {this.StitchRows}x{this.StitchCols}");
                return false;
            }
            if (this.IsBusy || this.IsError)
            {
                Logger.Log(Logger.Types.Error, $"[{Index}] {Name}: busy or error status");
                return false;
            }
            if (null == this.Rtc || this.Rtc.IsBusy)
            {
                Logger.Log(Logger.Types.Error, $"[{Index}] {Name}: scanner is invalid or busy");
                return false;
            }

            Task.Run(() =>
            {
                bool success = true;
                this.IsBusy = true;
                this.IsReady = false;
                try
                {
                    success &= NotifyStitchedImageGrabStarted(this, process, state);
                    int index = 0;
                    for (int row = 0; row < this.StitchRows; row++)
                    {
                        for (int col = 0; col < this.StitchCols; col++)
                        {
                            success &= NotifyStitchedImageIndexGrabBefore(this, index, process, state);
                            if (!success)
                                break;
                            success &= NotifyProcessLightIntensity(this, process, state);
                            if (!success)
                                break;
                            success &= DoGrab(out var bitmap);
                            if (!success)
                                break;
                            success &= NotifyStitchedImageIndexGrabbed(this, index, bitmap, process, state);
                            bitmap.Dispose();
                            if (!success)
                                break;
                            Logger.Log(Logger.Types.Debug, $"[{Index}] {Name}: stitched grabbed at {index}");
                            index++;
                            success &= NotifyStitchedImageIndexGrabAfter(this, index, process, state);
                            if (!success)
                                break;
                            Thread.Sleep(1000 / this.Fps);
                            //or
                            //Application.DoEvents();
                        }
                        if (!success)
                            break;
                    }
                }
                finally
                {
                    this.IsBusy = false;
                    this.IsReady = success;
                    this.IsError = !success;
                    NotifyStitchedImageGrabEnded(this, process, state);
                }
            });
            return true;
        }
        /// <inheritdoc/>
        public override bool CtlStitchedGrab(int index, IProcess process, object state = null)
        {
            if (null == StitchCalibrator || StitchCalibrator.Cells.Count != this.StitchRows * this.StitchCols)
            {
                Logger.Log(Logger.Types.Error, $"[{Index}] {Name}: stitch positions are not exist for {this.StitchRows}x{this.StitchCols}");
                return false;
            }
            if (this.IsBusy || this.IsError)
                return false;
            this.IsBusy = true;
            this.IsReady = false;

            NotifyStitchedImageIndexGrabBefore(this, index, process, state);
            DoGrab(out var bitmap);
            NotifyStitchedImageIndexGrabbed(this, index, bitmap, process, state);
            NotifyStitchedImageIndexGrabAfter(this, index, process, state);
            Logger.Log(Logger.Types.Debug, $"[{Index}] {Name}: stitched grabbed at {index}");
            bitmap.Dispose();
            
            this.IsBusy = false;
            this.IsReady = true;
            return true;
        }

        private bool MyCamera_OnStitchedImageGrabStarted(ICameraStitched cameraStitched, IProcess process, object state)
        {
            return true;
        }
        private bool MyCamera_OnStitchedImageIndexBefore(ICameraStitched cameraStitched, int index, IProcess process, object state)
        {
            bool success = true;
            var cell = cameraStitched.StitchCalibrator.Cells[index];
            success &= Rtc.CtlMoveTo(cell.Position, SpiralLab.Sirius2.Vision.Config.JumpSpeed);
            Thread.Sleep(2);
            return success;
        }
        private bool MyCamera_OnStitchedImageIndexAfter(ICameraStitched cameraStitched, int index, IProcess process, object state)
        {
            return true;
        }
        private bool MyCamera_OnStitchedImageGrabEnded(ICameraStitched cameraStitched, IProcess process, object state)
        {
            // Revert original correction table
            bool success = true;
            success &= Rtc.CtlMoveTo(Vector2.Zero, SpiralLab.Sirius2.Vision.Config.JumpSpeed);
            return success;
        }
        #endregion
    }
}