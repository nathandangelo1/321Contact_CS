
using static System.Console;
using System.Threading;

namespace Contact_321_CSHARP {// begin namespace
    static class Program {// begin class

        static Contact[] contactsArr;

        struct Contact {
            public string firstName;
            public string lastName;
            public string address;
            public string city;
            public string state;
            public string zipCode;
            public string title;

            public static int personCount;

            public Contact(string afirstName, string aLastName, string aAddress, string aCity, string aState, string aZipCode, string aTitle) {
                firstName = afirstName;
                lastName = aLastName;
                address = aAddress;
                city = aCity;
                state = aState;
                zipCode = aZipCode;
                title = aTitle;

                personCount++;

            }// END CONSTRUCTOR
        }// END CONTACT


        #region // MAIN FUNCTIONS
        static void Main(string[] args) {// begin main
            bool running = true;

            Title = "321 Contact";

            Header("321 Contact", "Record Search/Add");

            // FILL CONTACTSARR WITH RECORDS
            PopulateContactsArr();

            // UNTIL MENU RETURNS 'FALSE', KEEP RUNNING(LOOPING)
            do {
                running = MainMenu();

            } while (running);

            // WHEN ALL SEARCHES AND/OR ADDS ARE COMPLETED, WRITE CONTACTS TO FILE
            // OVERWRITES CONTACTS.DAT
            //FileOutputWriteContacts();

        }// END MAIN

        // DISPLAYS MAIN MENU
        static bool MainMenu() {
            bool success;
            int choice;
            string subChoiceAddMode;

            // GET MENU CHOICE FROM USER
            do {
                Console.Clear();
                Header("321 Contact", "Record Search/Add");
                success = int.TryParse((GetUserInput("\n\t\tOPTIONS:\n\n\t\t1 : Search Records \n\t\t2 : Add New Record \n\t\t3 : Remove Record\n\t\t4 : Quit\n\n\t\t")), out choice);

            } while (!success || choice < 0 || choice > 5);

            // CALL CORRESPONDING FUNCTION
            if (choice == 1) {
                SearchMode();

            } else if (choice == 2) {

                // OPTION TO ADD AS MANY CONTACTS AS DESIRED
                bool carryOn;
                
                // ADD RECORD, THEN CONTINUE TO ADD RECORDS UNTIL CARRYON IS FALSE
                do {
                    carryOn = false;

                    AddMode();

                    subChoiceAddMode = (GetUserInput("\n\t\tAdd another record? (y/n)")).ToLower();

                    do {
                        if (subChoiceAddMode == "y") {
                            carryOn = true;
                        } else if (subChoiceAddMode == "n") {
                            carryOn = false;
                        }
                    } while (subChoiceAddMode != "y" && subChoiceAddMode != "n");

                } while (carryOn);

                // REWRITES(SAVE) CONTACTS.DAT FILE, INCLUDING NEW ADDITIONS
                try {
                    FileOutputWriteContacts();
                } catch (Exception e) {
                    Write(e.Message);
                }

            } else if (choice == 3) {
                bool outputSuccess = false;
                bool deleteSuccessful = DeleteMode();

                if (deleteSuccessful) {
                    // REWRITE(SAVE) CONTACTS.DAT FILE
                    outputSuccess = FileOutputWriteContacts();
                } 
                // IF DELETE AND SAVE SUCCESSFUL, WRITE 
                if (deleteSuccessful && outputSuccess) {
                    WriteLine("Contact successfully removed from file.");
                    Thread.Sleep(3000);
                }
            } else if (choice == 4) {
                return false;
            } else if (choice == 5) {
                FileOutputWriteContacts();

            }
            return true;
        }
        #endregion 


        #region // CORE IO FUNCTIONS

        // SPLITS RECORDS INTO FIELDS AND POPULATES CONTACTSARR WITH CONTACT OBJECTS
        static void PopulateContactsArr() {

            // READ DATA FROM FILE, POPULATE STRING[] 'DATA'
            string[] data = new string[2];
            FileRead(data);

            // SPLIT DATA[0] ON THE '#' TO RETRIEVE RECORD COUNT
            string[] recordCountArr = data[0].Split('#');
            bool success = int.TryParse(recordCountArr[1], out int recordCount);

            // IF PARSE RECORDCOUNT TO INT SUCCESSFUL
            //    CREATE RECORDS ARRAY WITH LENGTH OF RECORD COUNT
            string[] records;
            
            if (success) {
                records = new string[recordCount];
            
            } else {
                WriteLine("Error reading recordCount.");
            }

            // SPLIT DATA[1] ON THE [RECORD SEPERATOR] CHAR TO POPULATE RECORDS ARRAY
                // (RECORDS WILL CONTAIN 1 EMPTY RECORD ON THE END AFTER THE SPLIT
                // BUT, RECORDCOUNT IS CORRECT
            records = data[1].Split((char)30);

            // CREATE ARRAY FOR NEWLY CONSTRUCTED CONTACTS FROM RECORDS
            contactsArr = new Contact[recordCount];

            try {
                // POPULATE CONTACT ARRAY ELEMENT FIELDS
                // FOR EACH RECORD
                for (int i = 0; i < records.Length; i++) {

                    // SPLIT ON [UNIT SEPERATOR] TO RETRIEVE FIELDS
                    string[] fields = records[i].Split((char)31);

                    if (fields.Length == 7) {
                        // CONSTRUCT CONTACT AND ADD TO ARRAY AT INDEX
                        // firstName, lastName, address, city, state, zipCode, title
                        contactsArr[i] = new Contact(fields[0], fields[1], fields[2], fields[3], fields[4], fields[5], fields[6]);

                    } 
                }
            } catch (Exception e) {
                WriteLine($"HelpLink = {e.HelpLink}");
                WriteLine($"Message = {e.Message}");
                WriteLine($"Source = {e.Source}");
                WriteLine($"StackTrace = {e.StackTrace}");
                WriteLine($"TargetSite = {e.TargetSite}");
                WriteLine($"Message = {e.Message}");
            }
        }// END POPULATECONTACTARR

        // READS DATA FROM FILE- POPULATES DATA ARRAY
        static void FileRead(string[] data) {
            bool count = true;
            string path = @"C:\Users\POBOYINSAMSARA\Desktop\TEMP-DELETE\contacts.dat";

            // INSTATIATE NEW STREAMREADER OBJECT
            StreamReader fromFile = new StreamReader(path);

            // READ DATA FROM FILE. ASSUMPTIONS: .DAT FILE WITH TWO LINES, LINE 1 CONTAINING RECORD COUNT, LINE 2 CONTAINING ALL RECORDS.
            // FIELDS: firstName, lastName, address, city, state, zipCode, title
            while (fromFile.EndOfStream == false) {

                try {
                    // READ LINE FROM FILE
                    string? line = fromFile.ReadLine();

                    // IF FIRST LINE
                    if (count) {
                        count = false;
                        if (line is not null) {
                            data[0] = line;
                        }
                        // IF SECOND LINE
                    } else if (line is not null) {
                        data[1] = line;
                    } else {
                        WriteLine("ERR: Possible data read error.");
                    }
                } catch (Exception e) {
                    WriteLine(e.Message);
                }// END TRY/CATCH
            }// END WHILE

            // CLOSE STREAMREADER
            fromFile.Close();

        }// END FILEREAD
        
        // WRITES CONTACTS OUT TO FILE (REWRITES CONTACTS.DAT)
        static bool FileOutputWriteContacts() {

            try {
                string path = @"C:\Users\POBOYINSAMSARA\Desktop\TEMP-DELETE\contacts.dat";

                StreamWriter outfile = new StreamWriter(path, false);

                outfile.WriteLine($"#{contactsArr.Length}");

                foreach (Contact contact in contactsArr) {

                    // firstName, LastName, Address, City, State, ZipCode, Title
                    outfile.Write($"{contact.firstName}{(char)31}{contact.lastName}{(char)31}{contact.address}{(char)31}{contact.city}{(char)31}{contact.state}{(char)31}{contact.zipCode}{(char)31}{contact.title}{(char)30}");

                }
                outfile.Close();

            } catch (Exception e) {
                WriteLine(e.Message);
                return false;
            }

            return true;

        }// END WRITECONTACTS
        #endregion


        #region // ADD AND DELETE FUNCTIONS

        // ADDS NEW CONTACT NEWCONTACT TO CONTACT ARRAY
        static void AddMode() {
            
            Header("321 Contact", "Record Search/Add");

            string firstName = GetUserInput("\t\tPlease enter new user's first name:  ");
            string lastName = GetUserInput("\t\tPlease enter new user's last name:  ");
            string address = GetUserInput("\t\tPlease enter new user's address:  ");
            string city = GetUserInput("\t\tPlease enter new user's city:  ");
            string state = GetUserInput("\t\tPlease enter new user's state:  ");
            string zipCode = GetUserInput("\t\tPlease enter new user's zip code:  ");
            string title = GetUserInput("\t\tPlease enter new user's title:  ");

            //  firstName, lastName, address, city, state, zipCode, title
            Contact newUser = new Contact(firstName,lastName,address,city,state,zipCode,title);

            // CREATE NEW ARRAY ONE ELEMENT LARGER THAN CONTACTSARR
            Contact[] result = new Contact[contactsArr.Length + 1];

            // COPTY CONTACTSARR TO RESULT ARRAY
            contactsArr.CopyTo(result, 0);

            // ADD NEW CONTACT TO LAST INDEX
            result[contactsArr.Length] = newUser;

            // RETURN RESULT ARRAY
            //return result;

            // SET CONTACTSARR EQUAL TO ARRAY RESULT 
            contactsArr = result;
            
        }// END ADDMODE

        // DELETES CONTACT FROM CONTACTS ARRAY
        static bool DeleteMode() {     // must search to verify contact is even in the array before deletion
            int searchIndex = 0;

            // CREATE NEW RESULT ARRAY ONE ELEMENT SMALLER THAN CONTACTSARR
            Contact[] result = new Contact[contactsArr.Length - 1]; 

            string firstLastSearch = GetUserInput("\n\tEnter first and last name for removal of contact from records. Spelling must be exact. Example. \"John Smith\"\n:");
            
            // SEARCHFOR GETS INDEX OF CONTACT MATCHING FIRST AND LAST NAME ENTERED
            searchIndex = GetIndexForDeletion(firstLastSearch);

            // IF SEARCHINDEX EQUALS -1, NO RESULTS WERE FOUND/RETURNED
            if (searchIndex != -1) {

                try {
                    // CREATE NEW RESULT ARRAY ONE ELEMENT SMALLER THAN CONTACTSARR
                    result = new Contact[contactsArr.Length - 1];

                    // SHIFT ELEMENTS TO REMOVE CONTACT AT SEARCH INDEX FROM RESULT ARRAY
                    for (int i = 0, index = 0; i < result.Length; i++, index++) {

                        // IF INDEX MATCH, SKIP INDEX
                        if (i == searchIndex) {
                            index++;
                            result[i] = contactsArr[index];
                        } else {
                            result[i] = contactsArr[index];
                        }
                    }
                    return true;

                } catch(Exception e) {
                    WriteLine(e.Message);
                    return false;
                }

                // SET CONTACTSARR EQUAL TO RESULT ARRAY
                contactsArr = result;

            } else {
                WriteLine("0 records matching search results.");
                return false;
            }

        }// END DELETEMODE

        // SEARCHES RECORDS FOR SEARCH VALUE AND RETURNS INDEX OF SEARCH VALUE FOR DELETION
        static int GetIndexForDeletion(string searchValue) {
            int index = 0;

            // FOR EACH CONTACT IN CONTACTS
            for (int i = 0; i < contactsArr.Length; i++) {

                string[] splitSearch = searchValue.Split(' ');

                // IF CONTACTSARR[I].FIRSTNAME, OR .LASTNAME, EQUALS OR CONTAINS SEARCHVALUE
                // if (contactsArr[i].firstName.Equals(searchValue, StringComparison.OrdinalIgnoreCase) || contactsArr[i].firstName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) || contactsArr[i].lastName.Equals(searchValue, StringComparison.OrdinalIgnoreCase) || contactsArr[i].lastName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) {
                if (contactsArr[i].firstName.Equals(splitSearch[0], StringComparison.OrdinalIgnoreCase) && contactsArr[i].lastName.Equals(splitSearch[1], StringComparison.OrdinalIgnoreCase)) {
                    return i;

                } else {
                    continue;

                }// END ELSE
            }// END FOR

            return -1;

        }// END SEARCHFOR

        #endregion


        #region // SEARCH FUNCTIONS
        // GETS USER INPUT FOR SEARCH AND CALLS SEARCH FUNCTIONS
        static void SearchMode() {// begin main
            string searchValue;

            Header("321 Contact", "Record Search");

            // GET SEARCH VALUE FROM THE USER
            WriteLine("\n\t-Search by first name, last name, or any combination seperated by a space. ");
            WriteLine("\n\tExample: John Smith -OR- Smith John -OR- J Smith -OR- Jo Sm -OR- Smith ");
            WriteLine("\n\t-The more complete your search, the more accurate and succinct the results. ");
            WriteLine(  "\n\t-Search results will be sorted by Last Name, then by First Name, in ascending alphabetical order.");
            WriteLine("\n\t-Option to save your records to a file given after the search results.");
            
            searchValue = GetUserInput("\n\nSearch our records: ");

            // CREATE LIST TO HOLD SEARCH RESULTS
            List<Contact> searchResults = new List<Contact>();

            // SEARCH RECORDS ARRAY FOR SEARCHVALUE- POPULATES LIST 'SEARCHRESULTS'
            // Search(searchValue, records, searchResults);
            bool searchSuccessful = Search(searchResults, searchValue);

            // IF SEARCH IS SUCCESSFULL, DISPLAY SEARCH RESULTS
            if (searchSuccessful) {
                // OUTPUT SEARCH RESULTS TO CONSOLE
                DisplaySearchResults(searchResults, searchValue);
            } else {
                WriteLine("Search Error.");
            }

            // /RUN AGAIN OR SAVE RESULTS TO FILE
            PostSearchMenu(searchResults);

        }// END SEARCHMODE

        // SEARCHES RECORDS FOR SEARCH VALUE AND POPULATES LIST 'SEARCHRESULTS'
        static bool Search(List<Contact> searchResults, string searchValue) {


            // FOR EACH CONTACT IN CONTACTS
            for (int i = 0; i < contactsArr.Length; i++) {

                // IF CONTACTSARR[I].FIRSTNAME, OR .LASTNAME, EQUALS OR CONTAINS SEARCHVALUE
                // if (contactsArr[i].firstName.Equals(searchValue, StringComparison.OrdinalIgnoreCase) || contactsArr[i].firstName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) || contactsArr[i].lastName.Equals(searchValue, StringComparison.OrdinalIgnoreCase) || contactsArr[i].lastName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) {
                if (FirstNameSearch(i, searchValue) || LastNameSearch(i, searchValue)) {

                    // ADD TO SEARCHRESULTS
                    searchResults.Add(contactsArr[i]);

                } else {

                    // ATTEMPT TO SPLIT STRING, THEN SEARCH USING EACH INDEX (MAX. 2) OF SPLIT STRING.
                    try {
                        string[] splitSearchValue = searchValue.Split(' ');

                        // IF SEARCHVALUE = FIRST NAME + ' ' + LAST NAME
                        if (FirstNameSearch(i, splitSearchValue[0]) && LastNameSearch(i, splitSearchValue[1])) {

                            // ADD TO SEARCHRESULTS
                            searchResults.Add(contactsArr[i]);

                        } else {
                            // IF SEARCHVALUE = LAST NAME + ' ' + FIRST NAME
                            // IF LAST NAME CONTAINS OR EQUALS PLITSEARCH[0}) AND (FIRST NAME CONTAINS OR EQUALS SPLITSEARCH[1])
                            if (LastNameSearch(i, splitSearchValue[0]) && FirstNameSearch(i, splitSearchValue[1])) {

                                // ADD TO SEARCHRESULTS
                                searchResults.Add(contactsArr[i]);
                            }
                        }

                    } catch (Exception e) {
                        WriteLine(e.Message);
                        return false;
                    }
                }// END ELSE
            }// END FOR

            return true;

        }// END SEARCH

        // SEARCHES FIRST NAME FIELDS
        static bool FirstNameSearch(int index, string searchValue) {
            if (contactsArr[index].firstName.Equals(searchValue, StringComparison.OrdinalIgnoreCase) || contactsArr[index].firstName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) {
                return true;
            } else {
                return false;
            }
        }// END FIRSTNAMESEARCH()

        // SEARCHES LAST NAME FIELDS
        static bool LastNameSearch(int index, string searchValue) {
            if (contactsArr[index].lastName.Equals(searchValue, StringComparison.OrdinalIgnoreCase) || contactsArr[index].lastName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) {
                return true;
            } else {
                return false;
            }
        }// END LASTNAMESEARCH()

        // DISPLAYS SEARCH RESULTS
        static void DisplaySearchResults(List<Contact> searchResults, string searchValue) {

            // ORDER SEARCH RESULTS BY LAST NAME THEN BY FIRST NAME, ASCENDING
            searchResults = searchResults.OrderBy(x => x.lastName).ThenBy(x => x.firstName).ToList();

            // OUTPUT SEARCH VALUE, TOTAL RECORDS SEARCHED, AND NUMBER OF RESULTS
            Clear();
            WriteLine($"\n\n\n\n");
            WriteLine($"            Search Name:    {searchValue}");
            WriteLine($" Total records searched:    {Contact.personCount}");
            WriteLine($"      Number of results:    {searchResults.Count}");

            // OUTPUT EACH CONTACT IN SEARCH RESULTS
            foreach (Contact contact in searchResults) {

                WriteLine("");
                WriteLine($"    Name:   {contact.title}. {contact.firstName} {contact.lastName}");
                WriteLine($" Address:   {contact.address}. ");
                WriteLine($"            {contact.city}, {contact.state} {contact.zipCode}");
                WriteLine("");

            }// END FOREACH

        }// END DISPLAYRESULTS()

        // WRITES SEARCH RESULTS TO FILE
        static bool FileOutputSearchResults(List<Contact> searchResults, string? fileName) {
            string v;

            WriteLine("\nPlease enter your preferred filename or press enter to use standard \"SearchResults + DateTime\"");
            
            do {
                v = (_ = ReadLine());

            } while (v is null);

            fileName = v;

            try {

                if (String.IsNullOrEmpty(fileName)) {

                    string dt = DateTime.Now.ToString();

                    dt = dt.Replace(@"/", "");
                    dt = dt.Replace(@":", "");
                    dt = dt.Replace(@" ", "");

                    fileName = "SearchResults" + dt;
                }// ENDIF 

                string path = $@"C:\Users\POBOYINSAMSARA\Desktop\TEMP-DELETE\{fileName}.txt";

                StreamWriter outfile = new StreamWriter(path, false);

                outfile.WriteLine($"Search Results from {DateTime.Now}\n");

                foreach (Contact contact in searchResults) {

                    outfile.WriteLine("");
                    outfile.WriteLine($" Name       :   {contact.title}. {contact.firstName} {contact.lastName}");
                    outfile.WriteLine($" Address    :   {contact.address}. ");
                    outfile.WriteLine($"                {contact.city}, {contact.state} {contact.zipCode}");
                    outfile.WriteLine("");

                }
                outfile.Close();

                return true;

            } catch (Exception e) {
                WriteLine(e.Message);
                return false;
            }

        }

        // DISPLAYS MENU TO SEARCH AGAIN OR OUTPUT SEARCH RESULTS TO FILE
        static void PostSearchMenu(List<Contact> searchResults) {
            string answer;

            answer = GetUserInput($"\n\n\nSearch again (y/n). Or enter 'store' to output search results to a file on your desktop:\n");

            answer = answer.ToLower();
            string? fileName = "";

            if (answer == "y") {
                Clear();
                SearchMode();

            } else if (answer == "store") {

                bool success = FileOutputSearchResults(searchResults, fileName);

                if (success) {
                    WriteLine("Search results successfull saved to file.");
                } else {
                    WriteLine("Save failed. Try Again.");
                }
                PostSearchMenu(searchResults);
            }

        }// END POSTMENU()

#endregion


        #region // STANDARD FUNCTIONS (USERINPUT AND HEADER)
        // GETS USER INPUT(STANDARD)
        static string GetUserInput(string text) {
            string? name;

            do {
                Write(text);
                name = ReadLine();
            } while (string.IsNullOrWhiteSpace(name));

            return name;

        }// END GETUSERINPUT()
        // DISPLAYS HEADER
        public static void Header(string title, string subtitle = "") {
            Clear();
            WriteLine();
            int windowWidth = 90 - 2;
            string titleContent = String.Format("\t    ║{0," + ((windowWidth / 2) + (title.Length / 2)) + "}{1," + (windowWidth - (windowWidth / 2) - (title.Length / 2) + 1) + "}", title, "║");
            string subtitleContent = String.Format("\t    ║{0," + ((windowWidth / 2) + (subtitle.Length / 2)) + "}{1," + (windowWidth - (windowWidth / 2) - (subtitle.Length / 2) + 1) + "}", subtitle, "║");

            WriteLine("\t    ╔════════════════════════════════════════════════════════════════════════════════════════╗");
            WriteLine(titleContent);
            if (!string.IsNullOrEmpty(subtitle)) {
                WriteLine(subtitleContent);
            }
            WriteLine("\t    ╚════════════════════════════════════════════════════════════════════════════════════════╝");
        }
        #endregion


    }// end class
}// end namespace
