using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Serialization;

namespace loganalyser
{
    public partial class Form1 : Form
    {
        Dictionary<string, string> regexpRuleList = new Dictionary<string, string>();
        Dictionary<string, logSuperStream> loadedFiles = new Dictionary<string, logSuperStream>();
        Dictionary<string, Dictionary<string, DataTable>> layers = new Dictionary<string, Dictionary<string, DataTable>>();
        Chart chart1 = new Chart();
        DateTime xAxisStart = DateTime.MaxValue;
        DateTime xAxisStop = DateTime.MinValue;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Dock = DockStyle.Fill;
            panel3.Controls.Add(chart1);

            try
            {
                File.ReadAllText("regexpRules.dat");
                regexpRuleList = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("regexpRules.dat"));

                foreach (KeyValuePair<string, string> regexpRule in regexpRuleList)
                    comboBox1.Items.Add(regexpRule.Key);

                comboBox1.SelectedIndex = 0;
            }
            catch
            {
                regexpRuleList = new Dictionary<string, string>();
                regexpRuleList.Add("dummy rule", "[0-9]+\\:[0-9]+\\:[0-9]*\\.[0-9]{3}");
                saveRules();
                MessageBox.Show("analyser could not find regexpRules.xml, started with dummy rules");
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                //create a stream from loaded files
                logSuperStream tvpLogSuperStream = new logSuperStream(openFileDialog1.FileNames);


                string value = tvpLogSuperStream.name;

                if (InputBox("New SuperStream", "SuperStream name:", ref value) == DialogResult.OK)
                {
                    tvpLogSuperStream.name = value;
                }

                //add it to dictionary
                if (!loadedFiles.ContainsKey(tvpLogSuperStream.name) || (loadedFiles.Values.Count == 0))
                    loadedFiles.Add(tvpLogSuperStream.name, tvpLogSuperStream);
                else
                {
                    MessageBox.Show("a stream with same name exists. TODO: rename box");
                    return;
                }

                //add it to tree
                treeView1.Nodes.Add(tvpLogSuperStream.name, tvpLogSuperStream.name);
                treeView1.Nodes[tvpLogSuperStream.name].Checked = true;

                //add child nodes (individual files in stream)
                foreach (string fileName in tvpLogSuperStream.files)
                    treeView1.Nodes[tvpLogSuperStream.name].Nodes.Add(fileName);

                //just to paint the child nodes. 
                foreach (TreeNode node in treeView1.Nodes[tvpLogSuperStream.name].Nodes)
                {
                    node.ForeColor = Color.Gray;
                    node.Checked = true;
                }

                treeView1.ExpandAll();
            }
            catch
            {
                MessageBox.Show("There was an error on loading the logs. Check if files are being used by another program or same log is already loaded.");
            }
        }

        // Updates all child tree nodes recursively.
        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    this.CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // The code only executes if the user caused the checked state to change.
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(e.Node, e.Node.Checked);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void EditButton_Click(object sender, EventArgs e)
        {

        }

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewRuleDialog newRuleDialog = new NewRuleDialog("new rule", regexpRuleList);
            newRuleDialog.ShowDialog();

            comboBox1.Items.Clear();

            foreach (KeyValuePair<string, string> regexpRule in regexpRuleList)
                comboBox1.Items.Add(regexpRule.Key);

            comboBox1.SelectedIndex = 0;

            saveRules();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditRuleDialog editRuleDialog = new EditRuleDialog(regexpRuleList);
            editRuleDialog.ShowDialog();

            comboBox1.Items.Clear();

            foreach (KeyValuePair<string, string> regexpRule in regexpRuleList)
                comboBox1.Items.Add(regexpRule.Key);

            comboBox1.SelectedIndex = 0;

            saveRules();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void saveRules()
        {
            try
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter("regexpRules.dat");
                file.Write(JsonConvert.SerializeObject(regexpRuleList, Formatting.Indented));
                file.Close();
            }
            catch
            {
                MessageBox.Show("Sorry. Something went wrong in Rule manager. App will close.");
                this.Close();
            }
        }

        private void plotButton_Click(object sender, EventArgs e)
        {          
            
        }         
        
        private void loadLogsToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void deleteStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TreeNode superStream in treeView1.Nodes)
            {
                if (superStream.IsSelected)
                {
                    superStream.Remove();
                    loadedFiles.Remove(superStream.Text);
                    break;
                }

                foreach (TreeNode log in superStream.Nodes)
                {
                    if (log.IsSelected)
                    {
                        log.Remove();

                        foreach (string fileName in loadedFiles[superStream.Text].files)
                        {
                            if (fileName == log.Text)
                            {
                                loadedFiles[superStream.Text].files.Remove(fileName);
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void layerButton_Click(object sender, EventArgs e)
        {
            // read the current rule
            string currentRegexpRule;

            try {
                currentRegexpRule = regexpRuleList[comboBox1.SelectedItem.ToString()];
                FillLayersDataTables(currentRegexpRule);
                RefreshLayersTreeview();
            }                
            catch(Exception ex)
                {
                MessageBox.Show("No Rule Selected: " + ex.ToString());
                }
        }

        private void RefreshLayersTreeview()
        {
            treeView2.Nodes.Clear();

            foreach (KeyValuePair<string, Dictionary<string, DataTable>> superstream in layers)
            {
                treeView2.Nodes.Add(superstream.Key,superstream.Key);

                foreach (KeyValuePair<string, DataTable> series in superstream.Value)             
                        treeView2.Nodes[superstream.Key].Nodes.Add(series.Key);           
            }

            foreach (TreeNode parent in treeView2.Nodes)
            {
                parent.Checked = true;

                foreach (TreeNode child in parent.Nodes)
                    child.Checked = true;
            }

            treeView2.ExpandAll();
        }

        private void ResetLayersDataTables()
        {
            foreach (TreeNode superStream in treeView1.Nodes)
            {              
                if (layers.ContainsKey(superStream.Text))
                {
                    if (layers[superStream.Text].ContainsKey(comboBox1.SelectedItem.ToString()))
                        layers[superStream.Text][comboBox1.SelectedItem.ToString()].Rows.Clear();
                    else
                    {
                        DataTable dataTable = new DataTable(superStream.Text + "_" + comboBox1.SelectedItem.ToString());
                        dataTable.Columns.Add("Time", typeof(DateTime));
                        dataTable.Columns.Add("Match", typeof(string));
                        dataTable.Columns.Add("Value", typeof(double));                       
                        layers[superStream.Text].Add(comboBox1.SelectedItem.ToString(), dataTable);
                    }
                }
                else
                {   //if superstream does not have a dedicated dataTable yet
                    DataTable dataTable = new DataTable(superStream.Text + "_" + comboBox1.SelectedItem.ToString());
                    dataTable.Columns.Add("Time", typeof(DateTime));
                    dataTable.Columns.Add("Match", typeof(string));
                    dataTable.Columns.Add("Value", typeof(double));
                    Dictionary<string, DataTable> ruleData = new Dictionary<string, DataTable>();
                    ruleData.Add(comboBox1.SelectedItem.ToString(), dataTable);
                    layers.Add(superStream.Text, ruleData);
                }
            }
        }

        private void FillLayersDataTables(string regexpRule)
        {
            string line;
            DateTime time;
            double value;
            Match timeMatch;
            Match valMatch;
            Regex valRegex = new Regex(@"" + regexpRule);
            //Regex timeRegex = new Regex(@"(\d+\/\d+\/\d+\s\d+:\d+:\d+.\d+)|\[(\d+-\d+-\d+\s\d+:\d+:\d+.\d+)\||(\d+-\d+-\d+\s\d+:\d+:\d+.\d+)");
            Regex timeRegex = new Regex(@"(\d+:\d+:\d+.\d+)");
            //Regex timeRegex = new Regex(@"\[(\d+-\d+-\d+\s\d+:\d+:\d+.\d+)\|");

            //string[] timeParseFormat = new string[] { "yyyy-MM-dd HH:mm:ss.fff", "yyyy/MM/dd HH:mm:ss.fff" };         
            string timeParseFormat = "HH:mm:ss.fff";


            ResetLayersDataTables();

            foreach (TreeNode superStream in treeView1.Nodes)
            {
                if (superStream.Checked)
                {
                    foreach (TreeNode log in superStream.Nodes)
                    {
                        if (log.Checked)
                        {
                            System.IO.StreamReader file = new StreamReader(log.Text);

                            while ((line = file.ReadLine()) != null)
                            {
                                valMatch = valRegex.Match(line);

                                if (valMatch.Success)
                                {
                                    switch (valMatch.Groups.Count)
                                    {
                                        case 1:

                                            timeMatch = timeRegex.Match(line);

                                            if (timeMatch.Success)
                                            {
                                                Boolean timeParsed = false;
                                                DataRow dr = layers[superStream.Text][comboBox1.SelectedItem.ToString()].NewRow();

                                                dr["Match"] = valMatch.Groups[0];
                                                dr["Value"] = 1.0;

                                                for (int i = 1; i < timeMatch.Groups.Count; i++)
                                                {
                                                    if (DateTime.TryParseExact(timeMatch.Groups[i].Value, timeParseFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
                                                    {
                                                        dr["Time"] = time;
                                                        timeParsed = true;
                                                        break;
                                                    }
                                                }

                                                if(timeParsed)
                                                    layers[superStream.Text][comboBox1.SelectedItem.ToString()].Rows.Add(dr);
                                                else
                                                {
                                                    MessageBox.Show("Time could not be properly parsed from the log"); 
                                                    // this should not occur, if you put timeParseFormat array and time regexp correct
                                                    return;
                                                }

                                            }
                                            else
                                            {
                                                MessageBox.Show("Rule match does not have time info");
                                                return;
                                            }

                                            break;

                                        case 2:

                                            if (Double.TryParse(valMatch.Groups[1].Value, out value))
                                            {
                                                timeMatch = timeRegex.Match(line);

                                                if (timeMatch.Success)
                                                {
                                                    Boolean timeParsed = false;
                                                    DataRow dr = layers[superStream.Text][comboBox1.SelectedItem.ToString()].NewRow();
                                                    dr["Match"] = valMatch.Groups[0];
                                                    dr["Value"] = value;

                                                    for( int i = 1; i<timeMatch.Groups.Count; i++)
                                                    {
                                                        if (DateTime.TryParseExact(timeMatch.Groups[i].Value, timeParseFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
                                                        {
                                                            dr["Time"] = time;
                                                            timeParsed = true;
                                                            break;
                                                        }
                                                    }

                                                    if (timeParsed)
                                                        layers[superStream.Text][comboBox1.SelectedItem.ToString()].Rows.Add(dr);
                                                    else
                                                        {
                                                            MessageBox.Show("Time could not be properly parsed from the log"); 
                                                        // this should not occur, if you put timeParseFormat array and time regexp correct
                                                            return;
                                                        }
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Rule match does not have time info");
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("Rule match type error");
                                                return;
                                            }

                                            break;

                                        default:
                                            MessageBox.Show("Rule returns too many matches in one row. Check the rule or try seperating the matches.");
                                            return;
                                    }
                                }
                            }

                            file.Close();
                        }
                    }
               }
            }

            //check if we have empty series
            foreach (KeyValuePair<string, Dictionary<string, DataTable>> superstream in layers)
            {                
                foreach (KeyValuePair<string, DataTable> series in superstream.Value)
                {
                    if (series.Value.Rows.Count <= 0)
                    {
                        layers[superstream.Key].Remove(series.Key);
                            MessageBox.Show("layer '" + series.Key + "' for '" + superstream.Key + "' is not added, because rule does not match any row.");
                        break;
                    }
                }
            }
        }          
        
        private Title CreateTitleFrom(string text)
        {
            Title tt = new Title();
            tt.Name = text;
            tt.Text = text.ToUpper();
            return tt;
        }

        private void PrepareChartAreas()
        {
            chart1.ChartAreas.Clear();
            chart1.Titles.Clear();
            Legend legend = new Legend();
            legend.BackColor = Color.Transparent;
            legend.Docking = Docking.Bottom;
       
            chart1.Legends.Add(legend);

            //determine nr of chartareas and add them
            foreach (TreeNode node in treeView2.Nodes)
            {
                if (node.Checked)
                {
                    chart1.ChartAreas.Add(node.Text);
                    chart1.Titles.Add(CreateTitleFrom(node.Text));
                    chart1.Titles[node.Text].DockedToChartArea = node.Text;
                }
            }

            for (int i = 0; i < chart1.ChartAreas.Count; i++)
            {
                chart1.ChartAreas[i].Position.Auto = false;
                chart1.ChartAreas[i].Position.X = 0;
                chart1.ChartAreas[i].Position.Y = i * ((100 - 10) / chart1.ChartAreas.Count);
                chart1.ChartAreas[i].Position.Height = ((100 -10) / chart1.ChartAreas.Count);
                chart1.ChartAreas[i].Position.Width = 90;

                chart1.ChartAreas[i].AxisX.LabelStyle.Format = "HH:mm:ss.fff";
                chart1.ChartAreas[i].AxisX.Minimum = xAxisStart.ToOADate();
                chart1.ChartAreas[i].AxisX.Maximum = xAxisStop.ToOADate(); // xAxisStart.ToOADate() + (xAxisStop.ToOADate() - xAxisStart.ToOADate()) / 2.0;
                chart1.ChartAreas[i].AxisX.MajorGrid.LineColor = Color.LightGray;
                chart1.ChartAreas[i].AxisY.MajorGrid.LineColor = Color.LightGray;
                chart1.ChartAreas[i].AxisX.LabelStyle.Font = new Font("Consolas", 8);
                chart1.ChartAreas[i].AxisY.LabelStyle.Font = new Font("Consolas", 8);
                chart1.ChartAreas[i].AxisY.IsStartedFromZero = false;
            }

            TimeSpan frame = new TimeSpan(0, (int)numericUpDown1.Value, (int)numericUpDown2.Value, (int)numericUpDown3.Value, (int)numericUpDown4.Value);
         
            for (int i = 0; i < chart1.ChartAreas.Count; i++)
            {
                chart1.ChartAreas[i].AxisX.Minimum = xAxisStart.ToOADate();
                chart1.ChartAreas[i].AxisX.Maximum = (xAxisStart + frame).ToOADate();
            }
        }

        private void toCSV(DataTable dt, string name)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(name, sb.ToString());
        }

        private void layersContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();

            xAxisStart = DateTime.MaxValue;
            xAxisStop = DateTime.MinValue;

            foreach (TreeNode node in treeView2.Nodes)
            {
                // if a node is checked
                if (node.Checked)
                {
                    foreach (TreeNode child in node.Nodes)
                    {
                        if (child.Checked)
                        {
                            DateTime tempStart = layers[node.Text][child.Text].AsEnumerable().Select(cols => cols.Field<DateTime>("Time")).OrderBy(p => p.Ticks).FirstOrDefault();
                            DateTime tempStop = layers[node.Text][child.Text].AsEnumerable().Select(cols => cols.Field<DateTime>("Time")).OrderByDescending(p => p.Ticks).FirstOrDefault();

                            if (tempStart <= xAxisStart) xAxisStart = tempStart;
                            if (tempStop >= xAxisStop) xAxisStop = tempStop;
                        }
                    }
                }
            }

            PrepareChartAreas();

            foreach (TreeNode node in treeView2.Nodes)
            {
                // if a node is checked
                if (node.Checked)
                {
                    foreach (TreeNode child in node.Nodes)
                    {
                        if (child.Checked)
                        {
                            string seriesName = node.Text + "_" + child.Text;
                            chart1.Series.Add(seriesName);
                            chart1.Series[seriesName].Points.DataBindXY(layers[node.Text][child.Text].AsEnumerable().Select(r => r.Field<DateTime>("Time")).ToArray(),
                                                                        layers[node.Text][child.Text].AsEnumerable().Select(r => r.Field<double>("Value")).ToArray());


                            if (layers[node.Text][child.Text].Rows.Count > 0)
                            {
                                if (layers[node.Text][child.Text].AsEnumerable().Select(al => al.Field<double>("Value")).Distinct().Max() == 1.0 &&
                                    layers[node.Text][child.Text].AsEnumerable().Select(al => al.Field<double>("Value")).Distinct().Min() == 1.0)
                                {
                                    chart1.Series[seriesName].YAxisType = AxisType.Secondary;
                                    chart1.Series[seriesName].ChartType = SeriesChartType.StackedColumn;
                                    //chart1.Series[seriesName].BorderColor = Color.Red; // StackedBar100;// Bar;// FastLine;
                                }
                                else
                                {
                                    chart1.Series[seriesName].ChartType = SeriesChartType.FastLine;// StackedBar100;// Bar;// FastLine;
                                }
                            }

                            chart1.Series[seriesName].ChartArea = node.Text;
                            chart1.Series[seriesName].XValueType = ChartValueType.DateTime;
                        }
                    }
                }
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in treeView2.Nodes)
                if (node.IsSelected)                
                    layers.Remove(node.Text);
                
            foreach (TreeNode node in treeView2.Nodes)           
                    foreach (TreeNode child in node.Nodes)                 
                        if (child.IsSelected)                  
                            layers[node.Text].Remove(child.Text);
                                                                 
            RefreshLayersTreeview();
            RefreshChartRange();

            for (int i = 0; i < chart1.ChartAreas.Count; i++)
            {
                chart1.ChartAreas[i].AxisX.Minimum = xAxisStart.ToOADate();
                chart1.ChartAreas[i].AxisX.Maximum = xAxisStop.ToOADate();
            }
        }

        private void copySelectedLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //a parent and a child cannot be selected at the same time
            foreach (TreeNode parent in treeView2.Nodes)
            {
                if (parent.IsSelected)              
                    copyLayerAt(parent.Text);                         
                
                foreach (TreeNode child in parent.Nodes)
                {
                    if (child.IsSelected)                
                        copyLayerAt(parent.Text);                                
                }                       
            }

            RefreshLayersTreeview();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan frame = new TimeSpan(0, (int)numericUpDown1.Value, (int)numericUpDown2.Value, (int)numericUpDown3.Value, (int)numericUpDown4.Value);
            TimeSpan delta = new TimeSpan(hScrollBar1.Value * (xAxisStop - xAxisStart - frame).Ticks / 100);

            try
            {
                for (int i = 0; i < chart1.ChartAreas.Count; i++)
                {
                    chart1.ChartAreas[i].AxisX.Minimum = (xAxisStart + delta).ToOADate();
                    chart1.ChartAreas[i].AxisX.Maximum = (xAxisStart + frame + delta).ToOADate();
                }
            }
            catch
            {
                for (int i = 0; i < chart1.ChartAreas.Count; i++)
                {
                    chart1.ChartAreas[i].AxisX.Minimum = (xAxisStart).ToOADate();
                    chart1.ChartAreas[i].AxisX.Maximum = (xAxisStop).ToOADate();
                }
            }
        }

        private void deleteRuleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void dataTableToCSV(DataTable dt, string fileName)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field =>
                  string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                sb.AppendLine(string.Join(",", fields));
            }

            try
            {
                File.WriteAllText(fileName, sb.ToString());
                MessageBox.Show("Files Exported Sucessfully");
            }
            catch   {
                        MessageBox.Show("There has been an error in csv write. Try using a simpler superstream name and rule name than \n" + fileName.Replace(' ', '_'));
                    }
        }

        private void layersToCSV(string path)
        {
            foreach (TreeNode node in treeView2.Nodes)
            {
                // if a node is checked
                if (node.Checked)
                {
                    foreach (TreeNode child in node.Nodes)
                    {
                        if (child.Checked)
                        {
                            dataTableToCSV(layers[node.Text][child.Text], 
                                path + "\\" + 
                                node.Text.Replace(' ', '_').Replace(':', '_').Replace('/', '_') + "_" + 
                                child.Text.Replace(' ', '_').Replace(':', '_').Replace('/', '_') + ".csv");                        
                        }
                    }
                }
            }
        }
        private void exportLayersToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            layersToCSV(folderBrowserDialog1.SelectedPath);
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void RefreshChartRange()
        {
            foreach (TreeNode node in treeView2.Nodes)
            {
                // if a node is checked
                if (node.Checked)
                {
                    foreach (TreeNode child in node.Nodes)
                    {
                        if (child.Checked)
                        {
                            DateTime tempStart = layers[node.Text][child.Text].AsEnumerable().Select(cols => cols.Field<DateTime>("Time")).OrderBy(p => p.Ticks).FirstOrDefault();
                            DateTime tempStop = layers[node.Text][child.Text].AsEnumerable().Select(cols => cols.Field<DateTime>("Time")).OrderByDescending(p => p.Ticks).FirstOrDefault();

                            if (tempStart <= xAxisStart) xAxisStart = tempStart;
                            if (tempStop >= xAxisStop) xAxisStop = tempStop;
                        }
                    }
                }
            }
        }

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Turkcell Team FQA Regexp Log analyser \n Copyright Turkcell 2017 \n All Rights Reserved. ");
        }

        private void copyLayerAt(string layerName)
        {
            string name = layerName + "_Copy";

            Dictionary<string, DataTable> temp = layers[layerName];

            if (!layers.ContainsKey(name))
                layers.Add(name, temp);
            else
                layers.Add(name + DateTime.Now.ToString(), temp);
        }
    }
}
