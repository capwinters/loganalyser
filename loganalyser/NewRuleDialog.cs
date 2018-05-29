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
    public partial class NewRuleDialog : Form
    {
        private string selectedRegexpRuleName;
        private string regexpRule;
        private Dictionary<string, string> regexpRuleList;

        public NewRuleDialog(string selectedRuleName, Dictionary<string, string> regexpRuleList)
        {
            InitializeComponent();

            this.regexpRuleList = regexpRuleList;
            this.selectedRegexpRuleName = selectedRuleName;
            regexpRuleList.TryGetValue(selectedRuleName, out regexpRule);

            textBox1.Text = selectedRuleName;
            textBox2.Text = regexpRule;
        }

        private void RuleEditorSave_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" || textBox2.Text != "")
            {

                if (!regexpRuleList.ContainsKey(textBox1.Text))
                {
                    regexpRuleList.Add(textBox1.Text, textBox2.Text);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("This name already exists. Please give another name.");
                }
            }
            else
                MessageBox.Show("\"A life of nothing's nothing worth, \n From that first nothing ere his birth, \n To that last nothing under earth.\" Alfred Lord Tennyson");
        }

        private void RuleEditorCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
