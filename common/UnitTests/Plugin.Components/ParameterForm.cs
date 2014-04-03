using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.Plugin.Components.ParameterForm;
using PA.Plugin;

namespace UnitTests.Plugin.Components
{
    [TestClass]
    public class ParameterForm
    {
        [TestMethod]
        public void TestParameterPanel()
        {
            PluginForSpecificImportTest plugin = new PluginForSpecificImportTest();

            using (PluginParametersPanel panel = new PluginParametersPanel())
            {
                panel.Refresh<IPlugin>(plugin);

                Assert.AreEqual(3, panel.Count);
                Assert.AreEqual("Hello", (panel.Controls[0] as PluginParametersTextBox).textBox.Text);
                Assert.AreEqual("Parameter String", (panel.Controls[0] as PluginParametersTextBox).label.Text);

                Assert.AreEqual(false, (panel.Controls[1] as CheckBox).Checked);
                Assert.AreEqual("Parameter Boolean", (panel.Controls[1] as CheckBox).Text);

                Assert.AreEqual(PluginForSpecificImportTest.Mode.MODE_A.ToString(), (panel.Controls[2] as PluginParametersComboBox).comboBox.SelectedItem);
                Assert.AreEqual("Parameter Enum", (panel.Controls[2] as PluginParametersComboBox).label.Text);

                (panel.Controls[0] as PluginParametersTextBox).textBox.Text = "Bye";
                (panel.Controls[1] as CheckBox).Checked = true;
                (panel.Controls[2] as PluginParametersComboBox).comboBox.SelectedItem = (panel.Controls[2] as PluginParametersComboBox).comboBox.Items[1].ToString();

                panel.Save();
            }

            Assert.AreEqual("Bye", plugin.ParamString);
            Assert.AreEqual(true, plugin.ParamBool);
            Assert.AreEqual(PluginForSpecificImportTest.Mode.MODE_B, plugin.ParamEnum);
        }
    }
}
