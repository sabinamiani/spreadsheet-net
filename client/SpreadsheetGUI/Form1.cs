//Version 1.0
//Dylan Habersetzer and Tyler Wood

using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Form for the spreadsheet GUI
    /// </summary>
    public partial class Form1 : Form
    {
        //The spreadsheet model
        Spreadsheet sheet;
        //The cell that is currently being viewed
        string currCell;
        //A check to see if the statement Invalid Formula has already been stated before clicking off a box
        bool invalidForm;

        //Delegate for safe cross thread modification of connect button
        private delegate void DisableButtonDelegate();
        /// <summary>
        /// Constructor for the Form
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            invalidForm = false;
            
            sheet = new Spreadsheet(s1 => true, s => s.ToUpper(), "ps6");
            //setup delegates
            sheet.ErrorSend += ShowError;
            sheet.SpreadsheetNameSend += DisplaySheetNames;
            sheet.GUICellUpdate += ServerChangeTextBoxes;
            sheet.GUIErrorUpdate += DisplayErrorMessage;

            SpreadsheetPanel.SelectionChanged += RecognizeClick;
            RecognizeClick(SpreadsheetPanel);
        }

        private void CellContentTextBox_Leave(object sender, EventArgs e)
        {
            //ChangeTextBoxes();
        }

        private void RecognizeClick(SpreadsheetPanel ss)
        {
            ChangeSelectedCell();
        }

        private void CellContentTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //If the key is enter, then call ChangeTextBoxes
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ChangeTextBoxes();
            }
        }

        private void OpenButtonMenuStrip_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void loadBackgroundImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadBackGroundImage();
        }

        private void setBackgroundOpacityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetOpacity();
        }

        private void HelpButtonMenuStrip_Click(object sender, EventArgs e)
        {
            HelpDisplayMessage();
        }

        /// <summary>
        /// Helper method that opens a file and replaces the old cells with the new file materials
        /// </summary>
        private void OpenFile()
        {
            //Check to see if the sheet has been saved or not edited, if so then ask if the user wants to save
            if (sheet.Changed)
            {
                AskSave();
            }

            //Opens the file using a dialog window
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Spreadsheet files (*.sprd)|*.sprd|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Create two lists, one with the old values that need to be replaced, and one with the values that will be replacing the old
                    string filePath = openFileDialog.FileName;
                    List<string> oldnonEmpties = new List<string>(sheet.GetNamesOfAllNonemptyCells());
                    sheet = new Spreadsheet(filePath, S => true, s => s.ToUpper(), "ps6");
                    List<string> nonEmpties = new List<string>(sheet.GetNamesOfAllNonemptyCells());


                    //Go through each list and replace the cells to set them up for proper viewing, then save
                    foreach (string content in oldnonEmpties) SetPanelValues(content);
                    foreach (string content in nonEmpties) SetPanelValues(content);
                    sheet.Save(filePath);
                }
            }
        }

        /// <summary>
        /// Helper method that saves the file to a new file location
        /// </summary>
        private void SaveFile()
        {
            //Reference is "https://stackoverflow.com/a/5136341"
            SaveFileDialog savefile = new SaveFileDialog();
            //Default name for the spreadsheet, and the file filters for the view
            savefile.FileName = "spreadsheet.sprd";
            savefile.Filter = "Spreadsheet files (*.sprd)|*.sprd|All files (*.*)|*.*";

            //open dialog window and save the file using the model spreadsheet
            if (savefile.ShowDialog() == DialogResult.OK)
            {
                sheet.Save(savefile.FileName);
            }
        }


        /// <summary>
        /// Helper method that prompts the user to see if they want to save the program
        /// </summary>
        private void AskSave()
        {
            //ask the user if they want to save the program, do so if they say yes
            DialogResult askSave = MessageBox.Show("Would you like to save before data is lost?", "Save Before Loss of Data?", MessageBoxButtons.YesNo);
            if (askSave == DialogResult.Yes)
            {
                SaveFile();
            }

        }

        /// <summary>
        /// Helper method that looks to see if the spreadsheet should ask if the user wants to save changes, closes otherwise.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
        }


        /// <summary>
        /// Helper method for loading background images into the spreadsheet
        /// </summary>
        private void LoadBackGroundImage()
        {
            //request the user to pick an image from the window and then display it as the background
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image files (*.png, *.jpg, *.jpeg)|*.png; *.jpg; *.jpeg |All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    Image backImage = new Bitmap(@filePath);
                    BackgroundImage = backImage;
                }
            }
        }

        /// <summary>
        /// Helper method that changes the text boxes for values and name
        /// </summary>
        private void ChangeSelectedCell()
        {
            //find the cell that needs to be changed
            invalidForm = false;
            SpreadsheetPanel.GetSelection(out int col, out int row);
            currCell = "" + (char)(col + 65) + (row + 1);
            CellNameTextBox.Text = (currCell);

            //check to see if the cell contents are a formula, if so make sure to put an = sign first, otherwise just return the string
            if (sheet.GetCellContents(currCell) is Formula)
            {
                CellContentTextBox.Text = "=" + sheet.GetCellContents(currCell).ToString();
            }
            else
            {
                CellContentTextBox.Text = sheet.GetCellContents(currCell).ToString();
            }

            //check to see if the value is null, otherwise return the string for value
            if (sheet.GetCellValue(currCell) is null)
            {
                CellValueTextBox.Text = "";
            }
            else
            {
                CellValueTextBox.Text = sheet.GetCellValue(currCell).ToString();
            }

            sheet.SendCellSelect(currCell);
        }


        /// <summary>
        /// Helper method that changes all of the affected values on the spreadsheet and the cell value text box
        /// </summary>
        private void ChangeTextBoxes()
        {
            //Find the cell that will be changed, then send it
            SpreadsheetPanel.GetSelection(out int col, out int row);
            currCell = "" + (char)(col + 65) + (row + 1);

            sheet.SendCellEdit(currCell, CellContentTextBox.Text);
        }

        private void ServerChangeTextBoxes(string cellName, string cellContents)
        {
            string currCell = cellName;
            string oldContents = sheet.GetCellContents(currCell).ToString();
            List<string> contentsToChange = new List<string>();

            //try to use SetContentsOfCell, if that does not work then give an error message and do not commit any changes
            try
            {
                contentsToChange = new List<string>(sheet.SetContentsOfCell(currCell, cellContents));
            }
            catch
            {
                if (!invalidForm)
                {
                    MessageBox.Show("Invalid Formula");
                    invalidForm = true;
                }
                sheet.SetContentsOfCell(currCell, oldContents);
                //Debug.WriteLine("Contents of cell" + sheet.GetCellContents(currCell));
                return;
            }

            //Go through each value from GetCellContents and set the value correctly, then set the text box values of the cell
            foreach (string content in contentsToChange) SetPanelValues(content);
            string cellValue = sheet.GetCellValue(currCell).ToString();
            CellValueTextBox.Text = cellValue;



        }

        private void DisplayErrorMessage(string cellName, string message)
        {
            if(cellName.Length == 0)
            {
                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show("Error at cell " + cellName + " : " + message);
            }
        }

        /// <summary>
        /// Helper method that changes the values of the requested cells
        /// </summary>
        /// <param name="name">The name of the cell to be changed</param>
        private void SetPanelValues(string name)
        {
            object cellValue = sheet.GetCellValue(name);
            string cellVal;

            //checks to see if the cell value is a formula error, otherwise sets the string to the normal value of the string
            if (cellValue.GetType().Equals(typeof(FormulaError)))
            {
                cellVal = "Formula Error";
            }
            else
            {
                cellVal = cellValue.ToString();
            }
            
            //sets the value of the panel representing the cell
            SpreadsheetPanel.SetValue((int)name[0] - 65, int.Parse(name.Substring(1)) - 1, cellVal);

        }

        /// <summary>
        /// Helper method that sets the opacity level of the spreadsheet background
        /// </summary>
        private void SetOpacity()
        {
            ///Prompts the user to put in a requested opacity value
            string UserAnswer = Interaction.InputBox("Desired Opacity 0 - 100 ", "Set Opacity", "40");
            double alpha = 0;

            //sets the opacity value or sends an error message
            try
            {
                alpha = double.Parse(UserAnswer);
            }
            catch
            {
                MessageBox.Show("Enter valid number 0 - 100");
                return;
            }

            //converts the opacity value to a scale from 0 to 255, and clamps the bounds
            alpha = 255 * (alpha / 100);
            alpha = Math.Min(alpha, 255);
            alpha = Math.Max(alpha, 0);

            //Sets the opacity value for the spreadsheet
            Color backColor = new Color();
            backColor = Color.FromArgb((int)alpha, 255, 255, 255);
            SpreadsheetPanel.BackColor = backColor;
        }

        /// <summary>
        /// Helper method that displays a box for the help menu
        /// </summary>
        private void HelpDisplayMessage()
        {
            MessageBox.Show("Help Menu!\n\nHow to change selected Cell: Use mouse to select Cells, or use the arrow keys\n\nHow to edit Cell contents: Click on the editable text box at the top of the screen" +
                " or press the enter key!\n\nAdditional feature: Spreadsheet images and opacity\nTo use the background image feature, click edit then change background image button, and " +
                "locate an image to use as a background.\n\n" +
                "To use the background opacity feature, click edit then change opacity, and then enter a value between 0 and 100.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            HandleConnect(UsernameTextBox.Text, IPTextBox.Text);
        }

        /// <summary>
        /// Attempts to connect to server, if there are any issues then it displays another dialog box with connection issues.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="ip"></param>
        private void HandleConnect(string userName, string ip)
        {
            try
            {
                sheet.Connect(userName, ip);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            //TODO: if we do connect properly, turn off ability to edit username and ip text boxes
        }

        private void DisplaySheetNames(string names)
        {
            var method = new DisableButtonDelegate(DisableButton);
            this.Invoke(method);
            string selectedSheet = Interaction.InputBox(names);
            Debug.WriteLine("Selected Spreadsheet: " + selectedSheet);
            //tell SpreadSheet to send new data
            sheet.RequestSpreadsheet(selectedSheet);
        }

        private void DisableButton()
        {
            ConnectButton.Enabled = false;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message);
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            sheet.SendCellUndo();
        }

        private void RevertButton_Click(object sender, EventArgs e)
        {
            sheet.SendCellRevert(currCell);
        }
    }
}
