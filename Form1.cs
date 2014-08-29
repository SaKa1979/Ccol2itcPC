using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCOL2iTCPC
{
    public partial class Form1 : Form
    {
        MainStart mainStart;
        TextBox sysTextBox;
        TextBox tabTextBox;
        TextBox templateBox;
        TextBox outputBox;

        public Form1(MainStart mainStart)
        {
            InitializeComponent();
            this.mainStart = mainStart;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Text = "CCOL to iTCPC";
            this.MaximizeBox = false;

            paintText();
            paintControls();
        }

        private void paintText()
        {
            Label sysText = new Label();
            sysText.Location = new Point(10, 70);
            sysText.Name = "sysText";
            sysText.Text = "Sys.h file";
            sysText.Size = TextRenderer.MeasureText(sysText.Text, sysText.Font);
            sysText.BorderStyle = BorderStyle.None;

            Label tabText = new Label();
            tabText.Location = new Point(10, 100);
            tabText.Name = "tabText";
            tabText.Text = "Tab.c file";
            tabText.Size = TextRenderer.MeasureText(tabText.Text, tabText.Font);
            tabText.BorderStyle = BorderStyle.None;

            Label outputFile = new Label();
            outputFile.Location = new Point(10, 130);
            outputFile.Name = "template file";
            outputFile.Text = "Template file";
            outputFile.Size = TextRenderer.MeasureText(outputFile.Text, outputFile.Font);
            outputFile.BorderStyle = BorderStyle.None;

            Label templateFile = new Label();
            templateFile.Location = new Point(20, 160);
            templateFile.Name = "output file";
            templateFile.Text = "Output file";
            templateFile.Size = TextRenderer.MeasureText(templateFile.Text, templateFile.Font);
            templateFile.BorderStyle = BorderStyle.None;

            this.Controls.Add(sysText);
            this.Controls.Add(tabText);
            this.Controls.Add(outputFile);
            this.Controls.Add(templateFile);
        }

        private void paintControls()
        {
            Button inputButton = new Button();
            inputButton.Location = new Point(10, 10);
            inputButton.Name = "Tab.c Button";
            inputButton.Text = "Browse for input files";
            inputButton.Size = new Size(100, 40);
            inputButton.Click += inputFiles_Click;

            Button templateButton = new Button();
            templateButton.Location = new Point(150, 10);
            templateButton.Name = "Template Button";
            templateButton.Text = "Browse for template file";
            templateButton.Size = new Size(100, 40);
            templateButton.Click += templateButton_Click;

            Button outputButton = new Button();
            outputButton.Location = new Point(300, 10);
            outputButton.Name = "Output Button";
            outputButton.Text = "Browse for output file";
            outputButton.Size = new Size(100, 40);
            outputButton.Click += outputButton_Click;
            
            sysTextBox = new TextBox();
            sysTextBox.Location = new Point(100, 70);
            sysTextBox.ReadOnly = true;
            sysTextBox.Name = "SysTextBox";
            sysTextBox.Text = Application.StartupPath;
            sysTextBox.Size = TextRenderer.MeasureText(sysTextBox.Text, sysTextBox.Font);
            sysTextBox.TextChanged += fileNameTextBox_TextChanged;
            
            tabTextBox = new TextBox();
            tabTextBox.Location = new Point(100, 100);
            tabTextBox.ReadOnly = true;
            tabTextBox.Name = "TabTextBox";
            tabTextBox.Text = Application.StartupPath;
            tabTextBox.Size = TextRenderer.MeasureText(tabTextBox.Text, tabTextBox.Font);
            tabTextBox.TextChanged += fileNameTextBox_TextChanged;

            templateBox = new TextBox();
            templateBox.Location = new Point(100, 130);
            templateBox.ReadOnly = true;
            templateBox.Name = "Output Box";
            templateBox.Size = new Size(250, 30);
            templateBox.TextChanged += fileNameTextBox_TextChanged;

            outputBox = new TextBox();
            outputBox.Location = new Point(100, 160);
            outputBox.ReadOnly = true;
            outputBox.Name = "Output Box";
            outputBox.Size = new Size(250, 30);
            outputBox.TextChanged += fileNameTextBox_TextChanged;

            Button exitButton = new Button();
            exitButton.Location = new Point(150, 200);
            exitButton.Size = new Size(60, 40);
            exitButton.Name = "Exit Button";
            exitButton.Text = "Exit";
            exitButton.Font = new Font("Times New Roman", 13);
            exitButton.BackColor = Color.Red;
            exitButton.Click += exitButton_Click;

            Button startButton = new Button();
            startButton.Location = new Point(30, 200);
            startButton.Size = new Size(70, 40);
            startButton.Name = "Start Button";
            startButton.Text = "Start";
            startButton.Font = new Font("Times New Roman", 13);
            startButton.BackColor = Color.Green;
            startButton.Click += startButton_Click;

            this.Controls.Add(inputButton);
            this.Controls.Add(sysTextBox);
            this.Controls.Add(tabTextBox);
            this.Controls.Add(exitButton);
            this.Controls.Add(startButton);
            this.Controls.Add(templateBox);
            this.Controls.Add(outputBox);
            this.Controls.Add(outputButton);
            this.Controls.Add(templateButton);
        }

        void outputButton_Click(object sender, EventArgs e)
        {
            string outputFile = mainStart.getOutputFile();
            outputBox.Text = outputFile;
        }

        void templateButton_Click(object sender, EventArgs e)
        {
            string templateFile = mainStart.getTemplateFile();
            templateBox.Text = templateFile;
        }

        void fileNameTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox senderTextBox = (TextBox)sender;
            Size size = TextRenderer.MeasureText(senderTextBox.Text, senderTextBox.Font);
            senderTextBox.Width = size.Width;
        }

        void startButton_Click(object sender, EventArgs e)
        {
            mainStart.readFiles(sysTextBox.Text, tabTextBox.Text, templateBox.Text, outputBox.Text);

            mainStart.writeFiles();
        }

        void exitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        void exitSaveButton_Click(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("lastfolder.txt"))
                writer.WriteLine(tabTextBox.Text);

            Environment.Exit(1);
        }

        void inputFiles_Click(object sender, EventArgs e)
        {
            string sysFile = String.Empty;
            string tabFile = String.Empty;
            mainStart.getInputFolder(out sysFile, out tabFile);

            sysTextBox.Text = sysFile;
            tabTextBox.Text = tabFile;
        }
    }
}
