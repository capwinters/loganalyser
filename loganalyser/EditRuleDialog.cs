using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace loganalyser
{
    public partial class EditRuleDialog : Form
    {
        private Dictionary<string, string> regexpRuleList;

        public EditRuleDialog(Dictionary<string, string> regexpRuleList)
        {
            InitializeComponent();
            this.regexpRuleList = regexpRuleList;

            foreach (KeyValuePair<string, string> regexpRule in this.regexpRuleList)
                comboBox1.Items.Add(regexpRule.Key);

            comboBox1.SelectedIndex = 0;

            textBox2.Text = this.regexpRuleList[comboBox1.SelectedItem.ToString()];
        }

        private void EditRuleDialog_Load(object sender, EventArgs e)
        {

        }

        private void RuleEditorCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Text = this.regexpRuleList[comboBox1.SelectedItem.ToString()];
        }

        private void RuleEditorSave_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                regexpRuleList[comboBox1.SelectedItem.ToString()] = textBox2.Text;
                this.Close();
            }
            else
            {
                MessageBox.Show("Rule cannot be blank");
                this.Close();
            }
        }

        private void RuleDelete_Click(object sender, EventArgs e)
        {
            regexpRuleList.Remove(comboBox1.SelectedItem.ToString());

            comboBox1.Items.Clear();

            foreach (KeyValuePair<string, string> regexpRule in this.regexpRuleList)
                comboBox1.Items.Add(regexpRule.Key);

            comboBox1.SelectedIndex = 0;

            textBox2.Text = this.regexpRuleList[comboBox1.SelectedItem.ToString()];
        }
    }
}
