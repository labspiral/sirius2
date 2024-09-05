/*
 *                                                             ,--,      ,--,                              
 *              ,-.----.                                     ,---.'|   ,---.'|                              
 *    .--.--.   \    /  \     ,---,,-.----.      ,---,       |   | :   |   | :      ,---,           ,---,.  
 *   /  /    '. |   :    \ ,`--.' |\    /  \    '  .' \      :   : |   :   : |     '  .' \        ,'  .'  \ 
 *  |  :  /`. / |   |  .\ :|   :  :;   :    \  /  ;    '.    |   ' :   |   ' :    /  ;    '.    ,---.' .' | 
 *  ;  |  |--`  .   :  |: |:   |  '|   | .\ : :  :       \   ;   ; '   ;   ; '   :  :       \   |   |  |: | 
 *  |  :  ;_    |   |   \ :|   :  |.   : |: | :  |   /\   \  '   | |__ '   | |__ :  |   /\   \  :   :  :  / 
 *   \  \    `. |   : .   /'   '  ;|   |  \ : |  :  ' ;.   : |   | :.'||   | :.'||  :  ' ;.   : :   |    ;  
 *    `----.   \;   | |`-' |   |  ||   : .  / |  |  ;/  \   \'   :    ;'   :    ;|  |  ;/  \   \|   :     \ 
 *    __ \  \  ||   | ;    '   :  ;;   | |  \ '  :  | \  \ ,'|   |  ./ |   |  ./ '  :  | \  \ ,'|   |   . | 
 *   /  /`--'  /:   ' |    |   |  '|   | ;\  \|  |  '  '--'  ;   : ;   ;   : ;   |  |  '  '--'  '   :  '; | 
 *  '--'.     / :   : :    '   :  |:   ' | \.'|  :  :        |   ,/    |   ,/    |  :  :        |   |  | ;  
 *    `--'---'  |   | :    ;   |.' :   : :-'  |  | ,'        '---'     '---'     |  | ,'        |   :   /   
 *              `---'.|    '---'   |   |.'    `--''                              `--''          |   | ,'    
 *                `---`            `---'                                                        `----'   
 * 
 * 2023 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : Custom power map, verify and compensate routines
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (https://sepwind.blogspot.com)
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using SpiralLab.Sirius2.PowerMeter;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner.Rtc;
using System.Numerics;
using SpiralLab.Sirius2.PowerMap;
using SpiralLab.Sirius2;

namespace Demos
{
    /// <summary>
    /// MyPowerMap  
    /// </summary>
    public class MyPowerMap : PowerMapBase
    {
        bool isTerminated = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Index (0,1,2,...)</param>
        /// <param name="name">Name</param>
        public MyPowerMap(int index, string name)
            : base(index, name)
        {
            this.IsReady = true;
            this.IsBusy = false;
            this.IsError = false;
        }

        /// <inheritdoc/>
        public override bool CtlMapping(string[] categories, double[] xWatts)
        {
            if (this.Rtc == null || this.Laser == null || this.PowerMeter == null)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start mapping power. assign rtc, laser and powermeter at first");
                this.NotifyMappingFailed();
                return false;
            }
            if (this.IsBusy)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start mapping power. it's busy running ...");
                this.NotifyMappingFailed();
                return false;
            }
            if (null == categories || 0 == categories.Length)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start mapping power. target categories is not valid");
                this.NotifyMappingFailed();
                return false;
            }
            foreach (var category in categories)
            {
                // For example, consider category as frequency
                if (!double.TryParse(category, out double hz))
                {
                    this.IsError = true;
                    this.IsReady = false;
                    Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start mapping power. target category is not valid hz: {category}");
                    this.NotifyMappingFailed();
                    return false;
                }
            }
            if (null == xWatts || xWatts.Length < 2)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start mapping power. target watts is not valid. counts= {xWatts.Length}");
                this.NotifyMappingFailed();
                return false;
            }
            if (PowerMeter.IsError)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start mapping power. invalid powermeter status");
                this.NotifyMappingFailed();
                return false;
            }
            if (Laser.IsBusy || Laser.IsError || !Laser.IsReady)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start mapping power. invalid laser status");
                this.NotifyMappingFailed();
                return false;
            }
            if (Rtc.CtlGetStatus(RtcStatus.Busy))
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start mapping power. rtc status is invalid (busy ?)");
                this.NotifyMappingFailed();
                return false;
            }
            var powerControl = Laser as ILaserPowerControl;
            if (null == powerControl)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start mapping power. laser is not support power control function");
                this.NotifyMappingFailed();
                return false;
            }

            return this.DoPowerMapping(categories, xWatts);
        }
        /// <summary>
        /// Routine for power mapping
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="xWatts">Array of x watt(W)</param>
        /// <returns>Success or failed</returns>
        protected virtual bool DoPowerMapping(string[] categories, double[] xWatts)
        {
            bool success = true;
            var powerControl = Laser as ILaserPowerControl;

            Task.Run(() =>
            {
                this.IsBusy = true;
                this.IsReady = false;
                isTerminated = false;

                this.NotifyMappingStarted();
                success &= Rtc.CtlMoveTo(Location);
                Thread.Sleep(100);
                PowerMeter.CtlClear();
                double maxWatt = xWatts[xWatts.Length - 1];

                foreach (var category in categories)
                {
                    Logger.Log(Logger.Types.Warn, $"powermap [{this.Index}]: trying to start mapping power at target category: {category}");
                    this.Clear(category);
                    success &= PowerMeter.CtlStart(category);
                    // For example, consider category as frequency
                    var hz = double.Parse(category);
                    success &= Rtc.CtlFrequency(hz, 2);
                    var sw = Stopwatch.StartNew();
                    bool isPreHeated = false;
                    foreach (var targetWatt in xWatts)
                    {
                        if (this.isTerminated || Rtc.CtlGetStatus(RtcStatus.Aborted) || Laser.IsError || PowerMeter.IsError)
                        {
                            success &= false;
                            break;
                        }
                        success &= powerControl.CtlPower(targetWatt, string.Empty); //not mapped
                        if (!isTerminated)
                            success &= Rtc.CtlLaserOn();

                        long delayTime = Config.PowerMapHoldTimeMs;
                        if (!isPreHeated)
                        {
                            delayTime = Config.PowerMapPreHeatTimeMs;
                            isPreHeated = true;
                        }
                        sw.Restart();
                        do
                        {
                            if (isTerminated)
                            {
                                success &= false;
                                break;
                            }
                            Thread.Sleep(50);
                        } while (sw.ElapsedMilliseconds < delayTime); //use measured last data 
                        double detectedWatt = PowerMeter.MeasuredPower;
                        success &= Rtc.CtlLaserOff();
                        if (!success)
                            break;
                        if (isTerminated)
                            break;
                        double inRangeWatt = targetWatt * Config.PowerMapInRangeThreshold / 100.0f;
                        if (inRangeWatt > 0)
                        {
                            if (Math.Abs(targetWatt - detectedWatt) > inRangeWatt)
                            {
                                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: mapping out of range threshold: {Config.PowerMapInRangeThreshold:F1}%, target: {targetWatt:F3}W, detected: {detectedWatt:F3}W at category: {category}");
                                success &= false;
                                break;
                            }
                        }
                        success &= this.Update(category, targetWatt, detectedWatt);
                        Logger.Log(Logger.Types.Info, $"powermap [{this.Index}]: mapping target: {targetWatt:F3}W, detected: {detectedWatt:F3}W at category: {category}");
                        NotifyMappingProgress(category, targetWatt);
                        if (!success)
                            break;
                    }
                    if (!success)
                        break;
                }
                success &= Rtc.CtlLaserOff();
                success &= PowerMeter.CtlStop();

                Rtc.CtlMoveTo(Vector2.Zero);
                this.IsBusy = false;
                if (success && !isTerminated)
                {
                    this.IsReady = true;
                    Logger.Log(Logger.Types.Info, $"powermap [{this.Index}]: success to mapping power");
                    this.NotifyMappingFinished();
                }
                else
                {
                    this.IsError = true;
                    this.IsReady = false;
                    Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to mapping power");
                    this.NotifyMappingFailed();
                }
            });
            return success;
        }
        /// <inheritdoc/>
        public override bool CtlVerify(KeyValuePair<string, double>[] categoryAndYWatts)
        {
            if (this.Rtc == null || this.Laser == null || this.PowerMeter == null)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start verify power. assign rtc, laser and powermeter at first");
                this.NotifyVerifyFailed();
                return false;
            }
            if (this.IsBusy)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start verify power. it's busy running ...");
                this.NotifyVerifyFailed();
                return false;
            }
            if (null == categoryAndYWatts || 0 == categoryAndYWatts.Length)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start verify power. target category is not valid");
                this.NotifyVerifyFailed();
                return false;
            }
            foreach (var kv in categoryAndYWatts)
            {
                // For example, consider category as frequency
                if (!double.TryParse(kv.Key, out double hz))
                {
                    this.IsError = true;
                    this.IsReady = false;
                    Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start verify power. target category is not valid hz: {kv.Key}");
                    this.NotifyVerifyFailed();
                    return false;
                }
            }
            if (PowerMeter.IsError)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start verify power. invalid powermeter status");
                this.NotifyVerifyFailed();
                return false;
            }
            if (Laser.IsBusy || Laser.IsError || !Laser.IsReady)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start verify power. invalid laser status");
                this.NotifyVerifyFailed();
                return false;
            }
            if (Rtc.CtlGetStatus(RtcStatus.Busy))
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start verify power. rtc status is invalid (busy ?)");
                this.NotifyVerifyFailed();
                return false;
            }
            var powerControl = Laser as ILaserPowerControl;
            if (null == powerControl)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start verify power. laser is not support power control function");
                this.NotifyVerifyFailed();
                return false;
            }
            return this.DoPowerVerify(categoryAndYWatts);
        }
        /// <summary>
        /// Routine for power verification
        /// </summary>
        /// <param name="categoryAndYWatts">Array of key(category) and value(target watt(W))</param>
        /// <returns>Success or failed</returns>
        protected virtual bool DoPowerVerify(KeyValuePair<string, double>[] categoryAndYWatts)
        {
            bool success = true;
            var powerControl = Laser as ILaserPowerControl;

            Task.Run(() =>
            {
                this.IsBusy = true;
                this.IsReady = false;
                isTerminated = false;

                this.NotifyVerifyStarted();
                Rtc.CtlMoveTo(Location);
                Thread.Sleep(100);
                PowerMeter.CtlClear();

                var sw = Stopwatch.StartNew();
                var oldIsEnableLookUp = this.IsEnableLookUp;
                this.IsEnableLookUp = true;
                bool isPreHeated = false;
                foreach (var kv in categoryAndYWatts)
                {
                    Logger.Log(Logger.Types.Warn, $"powermap [{this.Index}]: trying to start power verify. target category: {kv.Key}");
                    string category = kv.Key;
                    success &= PowerMeter.CtlStart(category);
                    double targetWatt = kv.Value;
                    double detectedWatt = 0;
                    // For example, consider category as frequency
                    double hz = double.Parse(category);
                    success &= Rtc.CtlFrequency(hz, 2);
                    if (powerControl.CtlPower(targetWatt, category))
                    {
                        success &= Rtc.CtlLaserOn();
                        sw.Restart();
                        long delayTime = Config.PowerMapHoldTimeMs;
                        if (!isPreHeated)
                        {
                            delayTime = Config.PowerMapPreHeatTimeMs;
                            isPreHeated = true;
                        }
                        do
                        {
                            if (Rtc.CtlGetStatus(RtcStatus.Aborted))
                            {
                                success &= false;
                                break;
                            }
                            Thread.Sleep(50);
                        } while (sw.ElapsedMilliseconds < delayTime);
                        detectedWatt = PowerMeter.MeasuredPower;
                        success &= Rtc.CtlLaserOff();
                        if (success)
                        {
                            double inRangeWatt = targetWatt * Config.PowerMapInRangeThreshold / 100.0f;
                            if (Math.Abs(targetWatt - detectedWatt) < inRangeWatt)
                            {
                                Logger.Log(Logger.Types.Info, $"powermap [{this.Index}]: verify in range target: {targetWatt:F3} - detected: {detectedWatt:F3}W < threshold: {Config.PowerMapInRangeThreshold}% at category: {category}");
                                this.NotifyVerifyProgress(category, targetWatt);
                            }
                            else
                            {
                                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: verify out of range threshold: {Config.PowerMapInRangeThreshold:F1}%, target: {targetWatt:F3}W, detected: {detectedWatt:F3}W at category: {category}");
                                success &= false;
                            }
                        }
                        if (!success)
                            break;
                    }
                    else
                    {
                        Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to change target output power: {targetWatt:F3}W. target category: {kv.Key}");
                        success &= false;
                    }
                    if (!success)
                        break;
                }
                success &= Rtc.CtlLaserOff();
                success &= PowerMeter.CtlStop();
                Rtc.CtlMoveTo(Vector2.Zero);
                this.IsEnableLookUp = oldIsEnableLookUp;
                this.IsBusy = false;
                if (success)
                {
                    this.IsReady = true;
                    this.NotifyVerifyFinished();
                }
                else
                {
                    this.IsError = true;
                    this.IsReady = false;
                    this.NotifyVerifyFailed();
                }
            });
            return success;
        }
        /// <inheritdoc/>
        public override bool CtlCompensate(KeyValuePair<string, double>[] categoryAndYWatts)
        {
            if (this.Rtc == null || this.Laser == null || this.PowerMeter == null)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start compensate power. assign rtc, laser and powermeter at first");
                this.NotifyCompensateFailed();
                return false;
            }
            if (this.IsBusy)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start compensate power. it's busy running ...");
                this.NotifyCompensateFailed();
                return false;
            }
            if (null == categoryAndYWatts || 0 == categoryAndYWatts.Length)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start compensate power. target category is not valid");
                this.NotifyCompensateFailed();
                return false;
            }
            foreach (var kv in categoryAndYWatts)
            {
                // For example, consider category as frequency
                if (!double.TryParse(kv.Key, out double hz))
                {
                    this.IsError = true;
                    this.IsReady = false;
                    Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start compensate power. target category is not valid hz: {kv.Key}");
                    this.NotifyCompensateFailed();
                    return false;
                }
            }
            if (PowerMeter.IsError)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start compensate power. invalid powermeter status");
                this.NotifyCompensateFailed();
                return false;
            }
            if (Laser.IsBusy || Laser.IsError || !Laser.IsReady)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start compensate power. invalid laser status");
                this.NotifyCompensateFailed();
                return false;
            }
            if (Rtc.CtlGetStatus(RtcStatus.Busy))
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start compensate power. rtc status is invalid (busy ?)");
                this.NotifyCompensateFailed();
                return false;
            }
            var powerControl = Laser as ILaserPowerControl;
            if (null == powerControl)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to start compensate power. laser is not support power control function");
                this.NotifyCompensateFailed();
                return false;
            }
            return this.DoPowerCompensate(categoryAndYWatts);
        }
        /// <summary>
        /// Routine for power compensation
        /// </summary>
        /// <param name="categoryAndYWatts">Array of key(category) and value(target watt(W))</param>
        /// <returns>Success or failed</returns>
        protected virtual bool DoPowerCompensate(KeyValuePair<string, double>[] categoryAndYWatts)
        {
            bool success = true;
            var powerControl = Laser as ILaserPowerControl;

            Task.Run(() =>
            {
                this.IsBusy = true;
                this.IsReady = false;
                isTerminated = false;

                this.NotifyCompensateStarted();
                Rtc.CtlMoveTo(Location);
                Thread.Sleep(100);
                PowerMeter.CtlClear();
                var sw = Stopwatch.StartNew();
                bool isPreHeated = false;
                var oldIsEnableLookUp = this.IsEnableLookUp;
                this.IsEnableLookUp = true;
                int retryCounts = 0;
                for (int i = 0; i < categoryAndYWatts.Length; i++)
                {
                    var kv = categoryAndYWatts[i];
                    string category = kv.Key;
                    double targetWatt = kv.Value;
                    Logger.Log(Logger.Types.Warn, $"powermap [{this.Index}]: trying to start power compensate. target: {targetWatt:F3}W at category: {category}");
                    success &= PowerMeter.CtlStart(category);
                    double detectedWatt = 0;
                    // For example, consider category as frequency
                    double hz = double.Parse(category);
                    success &= Rtc.CtlFrequency(hz, 2);
                    success &= powerControl.CtlPower(targetWatt, category);
                    if (success)
                    {
                        success &= Rtc.CtlLaserOn();
                        long delayTime = Config.PowerMapHoldTimeMs;
                        if (!isPreHeated)
                        {
                            delayTime = Config.PowerMapPreHeatTimeMs;
                            isPreHeated = true;
                        }
                        sw.Restart();
                        do
                        {
                            if (Rtc.CtlGetStatus(RtcStatus.Aborted))
                            {
                                success &= false;
                                break;
                            }
                            Thread.Sleep(50);
                        } while (sw.ElapsedMilliseconds < delayTime);
                        detectedWatt = PowerMeter.MeasuredPower;
                        success &= Rtc.CtlLaserOff();
                        if (success)
                        {
                            double inRangeWatt = targetWatt * Config.PowerMapInRangeThreshold / 100.0f;
                            if (Math.Abs(targetWatt - detectedWatt) <= inRangeWatt)
                            {
                                retryCounts = 0;
                                Logger.Log(Logger.Types.Info, $"powermap [{this.Index}]: compensate in range target: {targetWatt:F3} - detected: {detectedWatt:F3}W < threshold: {Config.PowerMapInRangeThreshold}% at category: {category}");
                                this.NotifyCompensateProgress(category, targetWatt);
                            }
                            else
                            {
                                double outOfRangeWatt = targetWatt * Config.PowerMapOutOfRangeThreshold / 100.0f;
                                if (outOfRangeWatt > 0 && Math.Abs(targetWatt - detectedWatt) > outOfRangeWatt)
                                {
                                    Logger.Log(Logger.Types.Info, $"powermap [{this.Index}]: compensate out of range target: {targetWatt:F3} - detected: {detectedWatt:F3}W < threshold: {Config.PowerMapOutOfRangeThreshold}% at category: {category}");
                                    success &= false;
                                }
                                else if (retryCounts >= Config.PowerMapCompensateRetryCounts)
                                {
                                    Logger.Log(Logger.Types.Info, $"powermap [{this.Index}]: compensating but failed to retry target: {targetWatt:F3} - detected: {detectedWatt:F3}W < threshold: {Config.PowerMapInRangeThreshold}% at category: {category}");
                                    success &= false;
                                }
                                else
                                {
                                    success &= powerControl.PowerMap.LookUp(category, targetWatt, out var xWatt, out var leftXWatt, out var rightXWatt);
                                    if (!success)
                                        break;
                                    Logger.Log(Logger.Types.Warn, $"powermap [{this.Index}: compensate out of range. target: {targetWatt:F3} - detected: {detectedWatt}W < threshold: {Config.PowerMapInRangeThreshold}% at category: {category} so retry {++retryCounts} times");
                                    success &= powerControl.CtlPower(leftXWatt, string.Empty);
                                    if (success)
                                    {
                                        success &= Rtc.CtlLaserOn();
                                        sw.Restart();
                                        do
                                        {
                                            if (Rtc.CtlGetStatus(RtcStatus.Aborted))
                                            {
                                                success &= false;
                                                break;
                                            }
                                            Thread.Sleep(50);
                                        } while (sw.ElapsedMilliseconds < Config.PowerMapHoldTimeMs);
                                        var detectedLeftWatt = PowerMeter.MeasuredPower;
                                        success &= Rtc.CtlLaserOff();
                                        if (success)
                                        {
                                            Logger.Log(Logger.Types.Info, $"powermap [{this.Index}: update powermap left x: {leftXWatt:F3}, y: {detectedLeftWatt:F3}W at category: {category}");
                                            success &= powerControl.PowerMap.Update(category, leftXWatt, detectedLeftWatt);
                                            // update mapping table
                                            this.NotifyMappingProgress(category, leftXWatt);
                                        }
                                    }
                                    if (success)
                                        success &= powerControl.CtlPower(rightXWatt, string.Empty);
                                    if (success)
                                    {
                                        success &= Rtc.CtlLaserOn();
                                        sw.Restart();
                                        do
                                        {
                                            if (Rtc.CtlGetStatus(RtcStatus.Aborted))
                                            {
                                                success &= false;
                                                break;
                                            }
                                            Thread.Sleep(50);
                                        } while (sw.ElapsedMilliseconds < Config.PowerMapHoldTimeMs);
                                        var detectedRightWatt = PowerMeter.MeasuredPower;
                                        success &= Rtc.CtlLaserOff();
                                        if (success)
                                        {
                                            Logger.Log(Logger.Types.Info, $"powermap [{this.Index}: update powermap right x: {rightXWatt:F3}, y: {detectedRightWatt:F3}W at category: {category}");
                                            success &= powerControl.PowerMap.Update(category, rightXWatt, detectedRightWatt);
                                            // update mapping table
                                            this.NotifyMappingProgress(category, rightXWatt);
                                        }
                                    }
                                    if (success)
                                        i--; //to do retry target y watt again 
                                }
                            }
                        }
                        if (!success)
                            break;
                    }
                    else
                    {
                        Logger.Log(Logger.Types.Error, $"powermap [{this.Index}]: fail to change target output power: {targetWatt:F3}W. target category: {kv.Key}");
                        success &= false;
                    }
                    if (!success)
                        break;
                }
                success &= Rtc.CtlLaserOff();
                success &= PowerMeter.CtlStop();
                Rtc.CtlMoveTo(Vector2.Zero);
                this.IsBusy = false;
                this.IsEnableLookUp = oldIsEnableLookUp;
                if (success)
                {
                    this.IsReady = true;
                    this.NotifyCompensateFinished();
                }
                else
                {
                    this.IsError = true;
                    this.IsReady = false;
                    this.NotifyCompensateFailed();
                }
            });
            return success;
        }

        /// <inheritdoc/>
        public override bool CtlStop()
        {
            bool success = true;
            isTerminated = true;

            Logger.Log(Logger.Types.Warn, $"powermap [{this.Index}]: trying to stop");
            if (null != Rtc && Rtc.IsBusy)
            {
                success &= Rtc.CtlAbort();
                success &= Rtc.CtlLaserOff();
            }
            if (null != Laser && Laser.IsBusy)
                success &= Laser.CtlAbort();

            return success;
        }
        /// <inheritdoc/>
        public override bool CtlReset()
        {
            bool success = true;
            this.IsReady = true;
            this.IsError = false;

            Logger.Log(Logger.Types.Warn, $"powermap [{this.Index}]: trying to reset");
            if (null != Rtc)
                success &= Rtc.CtlReset();
            if (null != Laser)
                success &= Laser.CtlReset();

            isTerminated = false;
            return success;
        }
    }
}
