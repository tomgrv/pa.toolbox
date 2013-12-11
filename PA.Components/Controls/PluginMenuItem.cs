using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Reflection;
using PA.Plugin.Components.Interfaces;
using PA.Plugin.Components.Core;
using PA.Plugin.Extensions;

namespace PA.Plugin.Components.Controls
{
    [ToolboxItem(typeof(IPluginSource))]
    public partial class PluginMenuItem : ToolStripMenuItem, IPluginSource, ISupportInitialize
    {
        [ImportMany("Toolbox.Plugin.Controls/PluginMenuItem/ForceDefaultParameters")]
        [Browsable(false), ReadOnly(true)]
        public IEnumerable<string> ForceDefaultParameters { get; private set; }

        #region Plugin Management

        public enum GroupType
        {
            None,
            Category,
            CategoryDropDown
        }

        [Category("Plugin Management")]
        [Description("Specify how to group plugins")]
        [DefaultValue(GroupType.None)]
        public PluginMenuItem.GroupType GroupBy { get; set; }

        protected void BuildMenus<T>() 
            where T : IPluginOperation
        {
            this.DropDownItems.Clear();

            foreach (IPlugin o in this.Imports.OfType<T>().OrderBy(o => PluginManager.GetAttribute<PluginDescriptionAttribute>(o.GetType()).Description))
            {
                PluginToolStripItem PluginMenu = new PluginToolStripItem(o, new EventHandler<PluginEventArgs>(this.OnPluginItemClicked));

                switch (this.GroupBy)
                {
                    case PluginMenuItem.GroupType.CategoryDropDown:

                        if (PluginMenu.Plugin.GetCategory() != "")
                        {
                            if (!this.DropDownItems.ContainsKey(PluginMenu.Plugin.GetCategory()))
                            {
                                ToolStripMenuItem PluginSubMenu = new ToolStripMenuItem(PluginMenu.Plugin.GetCategory());
                                {
                                    PluginSubMenu.Name = PluginMenu.Plugin.GetCategory();
                                }
                                this.DropDownItems.Add(PluginSubMenu);
                            }

                            (this.DropDownItems[PluginMenu.Plugin.GetCategory()] as ToolStripMenuItem).DropDownItems.Add(PluginMenu);
                        }
                        else
                        {
                            this.DropDownItems.Add(PluginMenu);
                        }
                        break;

                    case PluginMenuItem.GroupType.Category:

                        if (PluginMenu.Plugin.GetCategory() != "")
                        {
                            int index = -1;

                            foreach (ToolStripItem t in this.DropDownItems)
                            {
                                if (t.Name == PluginMenu.Plugin.GetCategory())
                                {
                                    index = this.DropDownItems.IndexOf(t);
                                }
                            }

                            if (index < 0)
                            {
                                ToolStripSeparator PluginSubMenu = new ToolStripSeparator();
                                {
                                    PluginSubMenu.Name = PluginMenu.Plugin.GetCategory();
                                }
                                this.DropDownItems.Add(PluginSubMenu);
                                this.DropDownItems.Add(PluginMenu);
                            }
                            else
                            {
                                this.DropDownItems.Insert(index + 1, PluginMenu);
                            }
                        }
                        else
                        {
                            this.DropDownItems.Insert(0, PluginMenu);
                        }
                        break;

                    default:
                        this.DropDownItems.Add(PluginMenu);
                        break;

                }
            }

            this.Enabled = (this.DropDownItems.Count > 0);
        }

        #endregion

        public PluginMenuItem()
            : base()
        {
            InitializeComponent();
        }

        #region IPluginSource Members

        [Category("Plugin Management")]
        public PluginLoader Loader { get; set; }

        [Category("Plugin Management")]
        public event EventHandler<PluginEventArgs> PluginChanged;

        [Browsable(false)]
        public IPlugin Plugin { get; protected set; }

        #endregion

        #region IPartImportsSatisfiedNotification Members

        [ImportMany]
        protected virtual IEnumerable<IPluginOperation> Imports { get; set; }

        public virtual void OnImportsSatisfied()
        {
            this.BuildMenus<IPluginOperation>();
        }

        #endregion

        #region ISupportInitialize Members

        public virtual void BeginInit()
        {
            if (!this.DesignMode)
            {

            }
        }

        public virtual void EndInit()
        {
            if (!this.DesignMode)
            {
                if (this.Loader is PluginLoader)
                {
                    this.Loader.DelayedComposition(this);
                }
            }
        }

        #endregion

        protected virtual void OnPluginItemClicked(object sender, PluginEventArgs e)
        {
            if (e is PluginEventArgs && e.Plugin is IPlugin)
            {
                this.ParameterDialog.Refresh(e.Plugin);
                this.OnPluginChanged(e);
            }
        }

        protected virtual void OnPluginChanged(PluginEventArgs e)
        {
            if (this.ForceDefaultParameters.Contains(e.Plugin.GetType().FullName)
                || !this.ParameterDialog.HasParameters
                || this.ParameterDialog.ShowDialog() == DialogResult.OK)
            {
                this.Plugin = e.Plugin;

                if (this.PluginChanged != null)
                {
                    this.PluginChanged(this, e);
                }
            }
        }
    }


}
