// Version 1.0 - September 24, 2020
// Written by Tyler Wood for CS3500
// Version 2.0 - October 2, 2020
// Written by Tyler Wood
// Wrote new main and helper methods, added more comments.

using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using NetworkUtil;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SS
{
    //Test for the PS5 branch
    public class Spreadsheet : AbstractSpreadsheet
    {
        //The spreadsheet itself
        Dictionary<string, Cell> sheet;

        //The dependency graph for the spreadsheet, to help with figuring out what to recalculate
        DependencyGraph dGraph = new DependencyGraph();

        string userName;

        int userID;

        SocketState theServer;

        bool firstReceive = true;

        public delegate void ErrorWMessage(string message);
        public event ErrorWMessage ErrorSend;

        public delegate void DrawSpreadsheetNames(string names);
        public event DrawSpreadsheetNames SpreadsheetNameSend;

        public delegate void UpdateCellInformation(string cellName, string cellContents);
        public event UpdateCellInformation GUICellUpdate;

        public delegate void ErrorInformation(string cellName, string message);
        public event ErrorInformation GUIErrorUpdate;

        /// <summary>
        /// A method used to determine whether the spreadsheet has been changed since it was last saved or created.
        /// </summary>
        public override bool Changed
        {
            get;
            protected set;
        }

        /// <summary>
        /// The default constructor for Spreadsheet, which uses the base validator, normalizer, and gives the version name Default.
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            sheet = new Dictionary<string, Cell>();
            Changed = false;
        }

        /// <summary>
        /// A constructor for spreadsheet which takes in and uses a Validator, Normalizer, and Version name
        /// </summary>
        /// <param name="isValid">The validator given by the user</param>
        /// <param name="normalize">The normalizer given by the user</param>
        /// <param name="version">The version information given by the user</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            sheet = new Dictionary<string, Cell>();
            Changed = false;
        }


        /// <summary>
        /// A constructor for spreadsheet that loads a previously created file, and uses a Validator, Normalizer, and Version name given to it by the user.
        /// If the version name of the file and the parameter do not match, throws an error
        /// If there are any issues with opening, writing, or closing the file, throws errors
        /// </summary>
        /// <param name="path">the path to the file</param>
        /// <param name="isValid">the user given validator</param>
        /// <param name="normalize">the user given normalizer</param>
        /// <param name="version">the user given version information</param>
        public Spreadsheet(string path, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            sheet = new Dictionary<string, Cell>();
            Changed = false;
            //load the file that was given, and throw an error if the file does not exist
            try
            {
                LoadFile(path);
            }
            catch (FileNotFoundException)
            {
                throw new SpreadsheetReadWriteException("cannot give file name that does not have a corresponding file");
            }
            catch (DirectoryNotFoundException)
            {
                throw new SpreadsheetReadWriteException("Cannot give bad directory");
            }
            catch (XmlException)
            {
                throw new SpreadsheetReadWriteException("XML file must be properly written");
            }
        }


        /// <summary>
        /// Loads the file specified from the path, has many different checks to ensure that the file is properly written
        /// </summary>
        /// <param name="path">the path to the file</param>
        private void LoadFile(string path)
        {
            using (XmlReader reader = XmlReader.Create(path))
            {
                //the name for the cell that is being checked at the time
                string cellName = null;
                //the content for the cell that is being checked at the time
                string cellContent = null;
                //a boolean to check if the cell element has been hit, to ensure that no name or content information is improperly placed
                Boolean IsInCell = false;
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            //if spreadsheet, check to ensure that the given version and the file version are the same
                            case "spreadsheet":
                                
                                if (!reader.GetAttribute("version").Equals(Version))
                                {
                                    throw new SpreadsheetReadWriteException("cannot have the version of the file be different from the given version");
                                }
                                break;

                            //if cell, make sure that there is not currently another cell being looked at
                            case "cell":
                                if (!IsInCell)
                                {
                                    IsInCell = true;
                                }
                                else
                                {
                                    throw new SpreadsheetReadWriteException("cannot have a cell nested inside a cell");
                                }
                                break;

                            //if name, then check to see if the value is a valid name and if cellname is null, and if it is, then change the cellname variable to the value
                            case "name":
                                if(object.ReferenceEquals(cellName, null))
                                {
                                    if (reader.Read() && !(String.IsNullOrEmpty(reader.Value)))
                                        cellName = reader.Value;
                                    else
                                        throw new SpreadsheetReadWriteException("cannot have a name element with no valid name");
                                }
                                else
                                    throw new SpreadsheetReadWriteException("Cannot have two names for a cell");
                                
                                break;

                            //if contents, check to see if cellcontent is null and if the content is valid. If so, then change cellcontent to the value
                            case "contents":
                                if (object.ReferenceEquals(cellContent, null))
                                {
                                    if (reader.Read() && !(String.IsNullOrEmpty(reader.Value)))
                                        cellContent = reader.Value;
                                    else
                                        throw new SpreadsheetReadWriteException("cannot have a content element with no valid content");
                                }
                                else
                                    throw new SpreadsheetReadWriteException("Cannot have two pieces of content for a cell");
                                
                                break;

                            //if the element is not specified above, then it should not be in the spreadsheet file
                            default:
                                throw new SpreadsheetReadWriteException("The only valid contents of this spreadsheet are the version information, the cell, cell name, and cell contents. Anything else is not accepted");
                                
                        }
                    }
                    else
                    {
                        //check to ensure that, when the end of the cell element is reached, that there is a name and content to use, then try to put them into the spreadsheet
                        if (reader.Name == "cell" && IsInCell && !(cellName is null) && !(cellContent is null))
                        {
                            try
                            {
                                SetContentsOfCell(cellName, cellContent);
                            }
                            catch
                            {
                                throw new SpreadsheetReadWriteException("The cell name of " + cellName + " and cell content of " + cellContent + "gave an error when trying to insert in the spreadsheet");
                            }
                            cellName = null;

                            cellContent = null;
                            IsInCell = false;
                        }

                        //check to see if the contents of the cell are in the right places, otherwise throw proper errors.
                        else if(reader.Name == "cell")
                        {
                            throw new SpreadsheetReadWriteException("The cell needs to have both a name and content inside of it");
                        }
                        else if (!IsInCell && (!(cellName is null) || !(cellContent is null)))
                            throw new SpreadsheetReadWriteException("Cannot have name or content outside of a cell");
                        
                    }
                }
            }
        }

        /// <summary>
        /// Returns the contents of the cell, unless the requested cell name is incorrect, then it will throw an InvalidNameException. If the cell is empty, returns a blank string
        /// </summary>
        /// <param name="name">The cell from which contents are being pulled</param>
        /// <returns>the contents of the cell</returns>
        public override object GetCellContents(string name)
        {
            name = Normalize(name);
            if (object.ReferenceEquals(name, null) || !Regex.IsMatch(name, "^[a-zA-Z]+[0-9]+$") || !IsValid(name))
            {
                throw new InvalidNameException();
            }

            //check to see if the sheet contains the requested cell. If not, then the error thrown is caught and an empty string is returned.
            try
            {
                return sheet[name].GetContent();
            }
            catch (KeyNotFoundException)
            {
                return "";
            }

        }

        /// <summary>
        /// Returns all of the cells in the spreadsheet that are not empty
        /// </summary>
        /// <returns>A list of every cell in the spreadsheet that is non-empty</returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (string name in sheet.Keys)
            {
                yield return name;
            }
        }


        /// <summary>
        /// Figures out whether the content is a double, formula, or string, and puts the corrected version of content into the spreadsheet, throwing errors if necessary.
        /// </summary>
        /// <param name="name">the name of the cell to be changed</param>
        /// <param name="content">the content of the cell</param>
        /// <returns></returns>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            name = Normalize(name);
            SetCellExceptionCheck(name, content);

            //figure out what type the content is, then send it to the correct SetCellContents method
            if (double.TryParse(content, out double num))
            {
                Changed = true;
                return SetCellContents(name, num);
            }
            if (content.Length >= 1 && content.Substring(0, 1).Equals("="))
            {
                Changed = true;
                return SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid));
            }
            else
            {
                Changed = true;
                return SetCellContents(name, content);
            }
        }

        /// <summary>
        /// Gets the calculated cell value of the specified cell
        /// </summary>
        /// <param name="name">the name of the cell from which value will be retrieved</param>
        /// <returns></returns>
        public override object GetCellValue(string name)
        {
            
            name = Normalize(name);
            SetCellExceptionCheck(name, 1.0);
            if (!sheet.ContainsKey(name))
            {
                return "";
            }
            return sheet[Normalize(name)].GetValue();
        }

        /// <summary>
        /// Sets the requested cell's contents to the number provided, and returns a list of the current cell and all of the cells that depend on the current cell
        /// If the name is invalid or null, throws an InvalidNameException
        /// </summary>
        /// <param name="name">The name of the cell whose contents are being set</param>
        /// <param name="number">The contents for the named cell</param>
        /// <returns>The cell name and the name of all the cells that are dependent on the named cell</returns>
        protected override IList<string> SetCellContents(string name, double number)
        {
            SetCellExceptionCheck(name, number);
            SetCellHelper(name, number);
            dGraph.ReplaceDependees(name, new HashSet<string>());

            List<string> recalcCells = new List<string>(GetCellsToRecalculate(name));
            RecalcHelper(recalcCells);
            return recalcCells;
        }


        /// <summary>
        /// Sets the requested cell's contents to the text provided, and returns a list of the current cell and all of the cells that depend on the current cell
        /// If the name is invalid or null, throws an InvalidNameException
        /// If the text provided is null, throws an ArgumentNullException
        /// </summary>
        /// <param name="name">The name of the cell whose contents are being set</param>
        /// <param name="text">The contents for the named cell</param>
        /// <returns>The cell name and the name of all the cells that are dependent on the named cell</returns>
        protected override IList<string> SetCellContents(string name, string text)
        {
            SetCellExceptionCheck(name, text);
            if (text.Equals(""))
            {
                sheet[name] = new Cell("");
                sheet[name].SetVal("");
                return new List<string>(GetCellsToRecalculate(name));
            }
            SetCellHelper(name, text);

            dGraph.ReplaceDependees(name, new HashSet<string>());

            List<string> recalcCells = new List<string>(GetCellsToRecalculate(name));
            RecalcHelper(recalcCells);

            return new List<string>(GetCellsToRecalculate(name));
        }


        /// <summary>
        /// Sets the requested cell's contents to the formula provided, and returns a list of the current cell and all of the cells that depend on the current cell
        /// If the name is invalid or null, throws an InvalidNameException
        /// If the formula provided is null, throws an ArgumentNullException
        /// If changing the cell contents to the formula causes a loop of references, throws a CircularException and does not change the spreadsheet
        /// </summary>
        /// <param name="name">The name of the cell whose contents are being set</param>
        /// <param name="formula">The contents for the named cell</param>
        /// <returns>The cell name and the name of all the cells that are dependent on the named cell</returns>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            //check for null and name exceptions
            SetCellExceptionCheck(name, formula);

            //get the old dependees that will be reset if there is a circular exception
            HashSet<string> oldDependees = new HashSet<string>(dGraph.GetDependees(name));

            //first check to see if the formula contains a reference to name, in which case throw a CircularException
            //otherwise, replace the dependees in the dependencygraph with the variables from the formula
            HashSet<string> formVars = new HashSet<string>(formula.GetVariables());
            if (formVars.Contains(name))
            {
                throw new CircularException();
            }
            else
            {
                dGraph.ReplaceDependees(name, formula.GetVariables());
            }


            //the cells that need to be recalculated
            List<string> recalcCells;

            //a try catch for the potential circular exception
            try
            {
                recalcCells = new List<string>(GetCellsToRecalculate(name));
            }
            catch (CircularException)
            {
                //if the circular exception is caught in GetCellsToRecalculate, reset the dependees with the old set, and throw the circular exception
                dGraph.ReplaceDependees(name, oldDependees);
                throw new CircularException();
            }

            //finally, put the name and cell into the spreadsheet
            SetCellHelper(name, formula);

            RecalcHelper(recalcCells);

            return recalcCells;

        }

        /// <summary>
        /// Helper method to figure out what needs to be recalculated, and then recalculate it.
        /// </summary>
        /// <param name="cells">the cells that will be recalculated</param>
        private void RecalcHelper(List<string> cells)
        {


            foreach (string cell in cells)
            {
                Object cellVal = null;
                Object cellCont = sheet[cell].GetContent();

                //check to see what type the cell content is, and calculate based on the type
                if (cellCont.GetType().Equals(typeof(double)))
                {
                    cellVal = cellCont;
                }
                if (cellCont.GetType().Equals(typeof(string)))
                {
                    cellVal = cellCont;
                }
                else if (cellCont.GetType().Equals(typeof(Formula)))
                {
                    Formula formCont = (Formula)cellCont;
                    cellVal = formCont.Evaluate(Lookup);
                }
                sheet[cell].SetVal(cellVal);
            }

            

        }

        /// <summary>
        /// the lookup to help with the evaluate method in RecalcHelper. Makes sure that the proper cell is found
        /// </summary>
        /// <param name="cellName">the cell that is being searched</param>
        /// <returns>the double value of that cell</returns>
        private double Lookup(string cellName)
        {
            if (!sheet.ContainsKey(cellName))
            {
                throw new ArgumentException();
            }

            object cellVal = sheet[cellName].GetValue();

            if(cellVal is double)
            {
                return (double)cellVal;
            }
            else
            {
                throw new ArgumentException();
            }
        }


        /// <summary>
        /// Helps the SetCellContents methods by checking to see if the cell is already filled, and either creating a new cell or replacing the content of the old one
        /// </summary>
        /// <param name="name">the selected cell to change or create info for</param>
        /// <param name="content">the content being added to the cell</param>
        private void SetCellHelper(string name, object content)
        {
            
            if (sheet.ContainsKey(name))
            {
                sheet[name].SetContent(content);
            }

            else
            {
                sheet.Add(name, new Cell(content));
            }
        }

        /// <summary>
        /// Helper method for the SetCellContents methods that checks to ensure that the content and the names of the cells are valid, and if not, throws the appropriate exceptions
        /// </summary>
        /// <param name="name">the cell being selected</param>
        /// <param name="content">the content that will fill the cell</param>
        private void SetCellExceptionCheck(string name, object content)
        {
            if (object.ReferenceEquals(content, null))
            {
                throw new ArgumentNullException();
            }

            if (object.ReferenceEquals(name, null) || !Regex.IsMatch(name, "^[a-zA-Z]+[0-9]+$") || !IsValid(name))
            {
                throw new InvalidNameException();
            }
        }


        /// <summary>
        /// Returns a list of all of the immediate dependents of the selected cell
        /// </summary>
        /// <param name="name">the name of the selected cell</param>
        /// <returns>a list of all the immediate dependents of the cell</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return dGraph.GetDependents(name);
        }



        /// <summary>
        /// Gets the version information from the requested filename, regardless of the spreadsheet it is being called from
        /// </summary>
        /// <param name="filename">the file containing the version information</param>
        /// <returns>the version information</returns>
        public override string GetSavedVersion(string filename)
        {
            string versionReturn;
            //ensure that the filename paths to a real file, then pull the version information from it if it exists. Otherwise, throw errors
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    reader.ReadToFollowing("spreadsheet");
                    versionReturn = reader.GetAttribute("version");
                    if(versionReturn is null || versionReturn == "")
                    {
                        throw new SpreadsheetReadWriteException("File must have a valid version");
                    }
                }
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Must give valid filename for GetSavedVersion");
            }
            return versionReturn;
        }


        /// <summary>
        /// Saves the file in an XML format to the specified filename, throwing exceptions if there are any issues found
        /// </summary>
        /// <param name="filename">the name of the file that will be created or updated</param>
        public override void Save(string filename)
        {
            //The settings for the xml writer
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    //check for null version information
                    if (object.ReferenceEquals(Version, null))
                    {
                        throw new SpreadsheetReadWriteException("cannot have a null version name");
                    }
                    writer.WriteAttributeString("version", Version);

                    //look through each cell, determine the type of content it carries, and add the proper element to the XML sheet from there
                    foreach (string cell in sheet.Keys)
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", cell);
                        if (sheet[cell].GetContent() is double)
                        {
                            writer.WriteElementString("contents", sheet[cell].GetContent().ToString());
                        }
                        else if (sheet[cell].GetContent() is Formula)
                        {
                            writer.WriteElementString("contents", "=" + sheet[cell].GetContent().ToString());
                        }
                        else if (sheet[cell].GetContent() is string)
                        {
                            writer.WriteElementString("contents", sheet[cell].GetContent().ToString());
                        }
                        writer.WriteEndElement();

                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (DirectoryNotFoundException)
            {
                throw new SpreadsheetReadWriteException("Cannot give a false directory");
            }
            

            //since the file has been saved, change Changed to false.
            Changed = false;
            
        }


        /*
        ///HERE IS THE NEW CONTENT
        ///
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="ip"></param>
        public void Connect(string _userName, string ip)
        {
            userName = _userName;
            Networking.ConnectToServer(OnConnect, ip, 1100);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private void OnConnect(SocketState state)
        { 
            if(state.ErrorOccured)
            {
                ErrorSend("Error occurred  when trying to connect, please verify that IP address is correct" + 
                    " and that server is online");
                return;
            }

            theServer = state;

            //Set up and send name to server
            string namePacket = userName + "\n";
            Networking.Send(theServer.TheSocket, namePacket);

            //Set up the event loop for receiving more packets
            state.OnNetworkAction = ReceiveData;
            Networking.GetData(state);

        }

        private void ReceiveData(SocketState state)
        {
            Debug.WriteLine("This is the data" + state.GetData());
            
            if (state.ErrorOccured)
            {
                ErrorSend(state.ErrorMessage);
                return;
            }
            string data = state.GetData();
            if (firstReceive)
            {
                firstReceive = false;
                SpreadsheetNameSend(state.GetData());
                
            }
            else
            {
                string[] dataArr = data.Split('\n');
                foreach(string dat in dataArr)
                {
                    try
                    {
                        JObject jsonConvert = JObject.Parse(dat);
                        ProcessData(jsonConvert, dat);
                    }
                    catch
                    {
                        //If cant json parse it is the user id INTPARSE IT
                        if(int.TryParse(dat, out int newID))
                        {
                            userID = newID;
                        }
                    }
                }
                
            }
            

            state.RemoveData(0, data.Length);

            Networking.GetData(state);
        }

        private void ProcessData(JObject jsonConvert, string data)
        {
            Debug.WriteLine("This is the jsonConvert message type: " + jsonConvert.GetValue("messageType").ToString());
            string value = jsonConvert.GetValue("messageType").ToString();

            switch (value)
            {
                case "cellUpdated":
                    Debug.WriteLine("cell update runs");
                    Cell cellJson = JsonConvert.DeserializeObject<Cell>(data);
                    SetContentsOfCell(cellJson.GetName(), (string)cellJson.GetContent());
                    GUICellUpdate(cellJson.GetName(), (string)cellJson.GetContent());
                    break;
                case "cellSelected":
                    break;
                case "disconnected":
                    break;
                case "requestError":
                    GUIErrorUpdate(jsonConvert.GetValue("cellName").ToString(), jsonConvert.GetValue("message").ToString());
                    break;
                case "serverError":
                    GUIErrorUpdate("", jsonConvert.GetValue("message").ToString());
                    break;
            }
        }

        /// <summary>
        /// Send the Network the requested spreadsheet
        /// </summary>
        /// <param name="selectedSheet">Name of spreadsheet to open/make</param>
        public void RequestSpreadsheet(string selectedSheet)
        {
            Networking.Send(theServer.TheSocket, selectedSheet + "\n");
        }

        public void SendCellEdit(string cellName, string cellContents)
        {
            Debug.WriteLine("SEND CELL");
            Cell toSend = new Cell(cellContents);
            toSend.SetName(cellName);
            toSend.SetRequest("editCell");
            string jsonString = JsonConvert.SerializeObject(toSend);
            if(!firstReceive)
                Networking.Send(theServer.TheSocket, jsonString);
        }

        public void SendCellRevert(string cellName)
        {
            Cell toSend = new Cell("");
            toSend.SetName(cellName);
            toSend.SetRequest("revertCell");
            string jsonString = JsonConvert.SerializeObject(toSend);
            JObject toRemove = JObject.Parse(jsonString);
            toRemove.Remove("contents");
            if(!firstReceive)
                Networking.Send(theServer.TheSocket, toRemove.ToString());
        }

        public void SendCellSelect(string cellName)
        {
            Cell toSend = new Cell("");
            toSend.SetName(cellName);
            toSend.SetRequest("selectCell");
            string jsonString = JsonConvert.SerializeObject(toSend);
            JObject toRemove = JObject.Parse(jsonString);
            toRemove.Remove("contents");
            if(!firstReceive)
                Networking.Send(theServer.TheSocket, toRemove.ToString());
        }

        public void SendCellUndo()
        {
            if(!firstReceive)
                Networking.Send(theServer.TheSocket, "{\"requestType\": \"undo\"}");
        }

    }

    /// <summary>
    /// A class that contains the content of a cell
    /// </summary>
    class Cell
    {

        //the cells messagetype
        [JsonProperty(PropertyName = "requestType")]
        private string requestType;

        //the cells name
        [JsonProperty(PropertyName = "cellName")]
        private string name;

        //the cell content
        [JsonProperty(PropertyName = "contents")]
        private object _content;

        //the cell value
        private object _value;

        /// <summary>
        /// The constructor for the cell, which fills the cell with the given content
        /// </summary>
        /// <param name="content">the content of the cell</param>
        public Cell(object content)
        {
            _content = content;
        }

        public string GetRequest()
        {
            return requestType;
        }

        public void SetRequest(string reqType)
        {
            requestType = reqType;
        }

        public void SetName(string _name)
        {
            name = _name;
        }
        /// <summary>
        /// getter method for the name of the cell
        /// </summary>
        /// <returns>The name of the cell</returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// getter method for the content of the cell
        /// </summary>
        /// <returns>the content of the cell</returns>
        public object GetContent()
        {
            return _content;
        }

        /// <summary>
        /// Getter method for the value of the cell
        /// </summary>
        /// <returns>the value of the cell</returns>
        public object GetValue()
        {
            return _value;
        }

        /// <summary>
        /// setter method for the value of the cell
        /// </summary>
        /// <param name="val">the soon to be value of the cell</param>
        public void SetVal(object val)
        {
            _value = val;
        }

        

        /// <summary>
        /// Set the content of the cell to something new
        /// </summary>
        /// <param name="content">the new content of the cell</param>
        public void SetContent(object content)
        {
            _content = content;
        }

        
    }
}
