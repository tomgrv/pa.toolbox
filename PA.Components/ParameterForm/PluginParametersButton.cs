using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using PA.Plugin.Components.Interfaces;

namespace PA.Plugin.Components.ParameterForm
{
    public partial class PluginParametersButton : System.Windows.Forms.Button, IPluginHelper
    {
        private PluginParametersForm form;

        public PluginParametersButton()
        {
            InitializeComponent();
            this.form = new PluginParametersForm();
        }

        #region IPluginHelper Membres

        public IPluginHelper NextHelper { get; set; }

        public bool Help<T>(T p) where T : IPlugin
        {
            this.form.Refresh<T>(p);
            this.Enabled = this.form.HasParameters;
            return true;
        }

        #endregion

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.form.ShowDialog();
        }
    }
}
