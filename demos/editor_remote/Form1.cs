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
 * Description : Example sirius2 editor
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.Winforms.Entity;
using SpiralLab.Sirius2.Winforms.Marker;

namespace Demos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            // Set language
            EditorHelper.SetLanguage();

            InitializeComponent();

            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_Load(object sender, EventArgs e)
        {   
            // Initialize sirius2 library
            EditorHelper.Initialize();

            // Create devices 
            EditorHelper.CreateDevices(out var rtc, out var laser);

            // Assign devices into usercontrol
            siriusEditorUserControl1.Rtc = rtc;
            siriusEditorUserControl1.Laser = laser;

            // Create marker
            EditorHelper.CreateMarker(out var marker);

            // Assign marker to user control
            siriusEditorUserControl1.Marker = marker;

            var document = siriusEditorUserControl1.Document;
            var view = siriusEditorUserControl1.View;
            // Create entities for test
            EditorHelper.CreateTestEntities(rtc, view, document);

            // Assign event handlers at Config
            EditorHelper.AttachEventHandlers();

            // Assign Document, View, Rtc, Laser into marker
            marker.Ready(document, view, rtc, laser);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var document = siriusEditorUserControl1.Document;
            var marker = siriusEditorUserControl1.Marker;
            var laser = siriusEditorUserControl1.Laser;
            var rtc = siriusEditorUserControl1.Rtc;

            if (document.IsModified)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to exit without save ?", "Warning", MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog(this);
                if (dialogResult == DialogResult.Yes)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }

            if (rtc.CtlGetStatus(RtcStatus.Busy) ||
                laser.IsBusy ||
                marker.IsBusy)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to exit during working on progressing... ?", "Warning", MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog(this);
                if (dialogResult == DialogResult.Yes)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }

            if (e.Cancel == false)
            {
                EditorHelper.DestroyDevices(rtc, laser, marker);
                siriusEditorUserControl1.Rtc = null;
                siriusEditorUserControl1.Laser = null;
                siriusEditorUserControl1.Marker = null;
            }
        }


        #region Control by remotely
        /// <summary>
        /// Ready status
        /// </summary>
        /// <returns>Status</returns>
        public bool IsReady
        {
            get
            {
                var marker = siriusEditorUserControl1.Marker;
                return marker.IsReady;
            }
        }
        /// <summary>
        /// Busy status
        /// </summary>
        /// <returns>Status</returns>
        public bool IsBusy
        {
            get
            {
                var marker = siriusEditorUserControl1.Marker;
                return marker.IsBusy;
            }
        }
        /// <summary>
        /// Error status
        /// </summary>
        /// <returns>Status</returns>
        public bool IsError
        {
            get
            {
                var marker = siriusEditorUserControl1.Marker;
                return marker.IsError;
            }
        }

        /// <summary>
        /// Open recipe (.sirius2 file)
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <returns>Success or failed</returns>
        public bool Open(string fileName)
        {
            if (this.IsBusy)
                return false;
            var doc = siriusEditorUserControl1.Document;
            var marker = siriusEditorUserControl1.Marker;
            bool success = true;
            this.Invoke(new MethodInvoker(delegate ()
            {
                success &= doc.ActOpen(fileName);
                success &= marker.Ready(doc, siriusEditorUserControl1.View, siriusEditorUserControl1.Rtc, siriusEditorUserControl1.Laser);
            }));
            return success;
        }
        /// <summary>
        /// Start marker
        /// </summary>
        /// <param name="offets">Array of offset</param>
        /// <returns>Success or failed</returns>
        public bool Start(SpiralLab.Sirius2.Mathematics.Offset[] offets = null)
        {
            if (!this.IsReady)
                return false;
            if (this.IsBusy)
                return false;
            if (this.IsError)
                return false;
            bool success = true;
            this.Invoke(new MethodInvoker(delegate ()
            {
                var marker = siriusEditorUserControl1.Marker;
                marker.Offsets = offets;
                success &= marker.Start();
            }));
            return success;
        }
        /// <summary>
        /// Stop marker
        /// </summary>
        ///<returns>Success or failed</returns>
        public bool Stop()
        {
            var marker = siriusEditorUserControl1.Marker;
            bool success = true;
            this.Invoke(new MethodInvoker(delegate ()
            {
                success &= marker.Stop();
            }));
            return success;
        }
        /// <summary>
        /// Reset marker status
        /// </summary>
        /// <returns>Success or failed</returns>
        public bool Reset()
        {
            var marker = siriusEditorUserControl1.Marker;
            bool success = true;
            this.Invoke(new MethodInvoker(delegate ()
            {
                success &= marker.Reset();
            }));
            return success;
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Find <c>IEntity</c> by name
        /// </summary>
        /// <param name="entityName">Entity name</param>
        /// <param name="entity">Founded <c>IEntity</c></param>
        /// <returns>Success or failed</returns>
        public bool EntityFind(string entityName, out IEntity entity)
        {
            var marker = siriusEditorUserControl1.Marker;
            var doc = marker.Document;
            Debug.Assert(doc != null);
            return doc.FindByName(entityName, out entity);
        }
        /// <summary>
        /// Find <c>EntityPen</c> by color
        /// </summary>
        /// <param name="color"><c>System.Drawing.Color</c> value at <c>Config.PensColor</c></param>
        /// <param name="entity">Founded <c>EntityPen</c></param>
        /// <returns>Success or failed</returns>
        public bool EntityFind(System.Drawing.Color color, out EntityPen entity)
        {
            var marker = siriusEditorUserControl1.Marker;
            var doc = marker.Document;
            Debug.Assert(doc != null);
            return doc.FindByPenColor(color, out entity);
        }
        /// <summary>
        /// Translate <c>IEntity</c> 
        /// </summary>
        /// <param name="entity">Target <c>IEntity</c></param>
        /// <param name="deltaXyz">Dx, Dy, Dz (mm)</param>
        /// <returns>Success or failed</returns>
        public bool EntityTranslate(IEntity entity, OpenTK.Vector3 deltaXyz)
        {
            var marker = siriusEditorUserControl1.Marker;
            var doc = marker.Document;
            Debug.Assert(doc != null);
            Debug.Assert(entity != null);
            bool success = true;
            this.Invoke(new MethodInvoker(delegate ()
            {
                success &= doc.ActTransit(new IEntity[] { entity }, deltaXyz);
            }));
            return success;
        }

        /// <summary>
        /// Query property list from <c>IEntity</c> 
        /// </summary>
        /// <remarks>
        /// Target properties are Browasable attribute is <c>True</c> only <br/>
        /// </remarks>
        /// <param name="entity">Target <c>IEntity</c> </param>
        /// <returns><c>Dictionary<string, object></c></returns>
        public Dictionary<string, object> EntityProperties(IEntity entity)
        {
            return PropertyList(entity);
            Dictionary<string, object> PropertyList(object objectType)
            {
                if (objectType == null) return new Dictionary<string, object>();
                Type t = objectType.GetType();
                PropertyInfo[] props = t.GetProperties();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (PropertyInfo prp in props)
                {
                    //Attribute [Browsable] is True only
                    if (prp.GetCustomAttributes<BrowsableAttribute>().Contains(BrowsableAttribute.Yes))
                    {
                        object value = prp.GetValue(objectType, new object[] { });
                        dic.Add(prp.Name, value);
                    }
                }
                return dic;
            }
        }
        /// <summary>
        /// Read property value at <c>IEntity</c> 
        /// </summary>
        /// <param name="entity">Target <c>IEntity</c></param>
        /// <param name="propName">Property name</param>
        /// <param name="propValue">Property value</param>
        /// <returns>Success or failed</returns>
        public bool EntityReadPropertyValue(IEntity entity, string propName, out object propValue)
        {
            propValue = null;
            Debug.Assert(entity != null);
            Type type = entity.GetType();
            var propInfo = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
            if (null == propInfo || !propInfo.CanRead)
                return false;
            propValue = propInfo.GetValue(entity);
            return true;
        }
        /// <summary>
        /// Write property value at <c>IEntity</c> 
        /// </summary>
        /// <param name="entity">Target <c>IEntity</c></param>
        /// <param name="propName">Property name</param>
        /// <param name="propValue">Property value</param>
        /// <returns>Success or failed</returns>
        public bool EntityWritePropertyValue(IEntity entity, string propName, object propValue)
        {
            Debug.Assert(entity != null);
            Type type = entity.GetType();
            var propInfo = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
            if (null == propInfo || !propInfo.CanWrite)
                return false;
            var convertedValue = Convert.ChangeType(propValue, propInfo.PropertyType);
            this.Invoke(new MethodInvoker(delegate ()
            {
                propInfo.SetValue(entity, convertedValue, null);
                // Regen data by forcily
                entity.Regen();
            }));
            return true;
        }
        #endregion
    }
}