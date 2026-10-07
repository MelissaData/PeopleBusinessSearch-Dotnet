using Newtonsoft.Json;
using System.Security.Cryptography;

namespace PeopleBusinessSearchDotnet
{
  /// <summary>
  /// People Business Search looks up contact records (people and businesses) that match
  /// an address and/or name, returning the matching records with their address details
  /// along with result codes that describe the outcome of the search.
  ///
  /// <para>High-level flow of this sample:</para>
  /// <list type="number">
  ///   <item><description>ARGS    - ParseArguments reads any --flag values off the command line.</description></item>
  ///   <item><description>INPUT   - CallAPI fills in whatever wasn't supplied via interactive prompts.</description></item>
  ///   <item><description>REQUEST - CallAPI builds the REST query string (license + input fields).</description></item>
  ///   <item><description>CALL    - GetContents issues the GET request and pretty-prints the JSON response.</description></item>
  /// </list>
  ///
  /// <para>This sample is a thin HTTP client: it builds a query string, sends a GET
  /// request to the People Business Search Cloud API, and prints the JSON response.</para>
  ///
  /// <para>Reference:</para>
  /// <list type="bullet">
  ///   <item><description>Documentation: https://docs.melissa.com/cloud-api/people-business-search/people-business-search-index.html</description></item>
  ///   <item><description>Release notes: https://releasenotes.melissa.com/cloud-api/people-business-search/</description></item>
  ///   <item><description>Result codes: https://docs.melissa.com/melissa/result-codes/result-codes-index.html</description></item>
  /// </list>
  /// </summary>
  static class Program
  {

    /// <summary>
    /// Entry point. Reads the optional command-line arguments, then hands control to
    /// CallAPI, which performs the actual request/response cycle.
    /// </summary>
    /// <param name="args">The raw command-line arguments.</param>
    static void Main(string[] args)
    {
      string baseServiceUrl = @"https://search.melissadata.net/";
      string serviceEndpoint = @"v5/web/contactsearch/docontactSearch";
      string license = "";
      string maxrecords = "";
      string matchlevel = "";
      string addressline1 = "";
      string locality = "";
      string administrativearea = "";
      string postal = "";
      string anyname = "";

      // Populate any values passed on the command line, then run the search.
      ParseArguments(ref license, ref maxrecords, ref matchlevel, ref addressline1, ref locality, ref administrativearea, ref postal, ref anyname, args);
      CallAPI(baseServiceUrl, serviceEndpoint, license, maxrecords, matchlevel, addressline1, locality, administrativearea, postal, anyname);
    }

    /// <summary>
    /// Reads the supported command-line options and writes each recognized value into
    /// its matching by-ref parameter. Any parameter left unset here falls back to an
    /// interactive prompt later in <see cref="CallAPI"/>.
    ///
    /// <para>Recognized flags (each followed by its value, e.g. "--anyname Melissa Data"):
    /// --license/-l, --maxrecords, --matchlevel, --addressline1, --locality,
    /// --administrativearea, --postal, --anyname.</para>
    /// </summary>
    /// <param name="license">Receives the Melissa license string, if supplied.</param>
    /// <param name="maxrecords">Receives the maximum number of records to return, if supplied.</param>
    /// <param name="matchlevel">Receives the match level for the search, if supplied.</param>
    /// <param name="addressline1">Receives the street address to search on, if supplied.</param>
    /// <param name="locality">Receives the locality (city) to search on, if supplied.</param>
    /// <param name="administrativearea">Receives the administrative area (state/province) to search on, if supplied.</param>
    /// <param name="postal">Receives the postal code to search on, if supplied.</param>
    /// <param name="anyname">Receives the individual or company name to search on, if supplied.</param>
    /// <param name="args">The raw command-line arguments to parse.</param>
    static void ParseArguments(ref string license, ref string maxrecords, ref string matchlevel, ref string addressline1, ref string locality, ref string administrativearea, ref string postal, ref string anyname, string[] args)
    {
      for (int i = 0; i < args.Length; i++)
      {
        if (args[i].Equals("--license") || args[i].Equals("-l"))
        {
          if (args[i + 1] != null)
          {
            license = args[i + 1];
          }
        }
        if (args[i].Equals("--maxrecords"))
        {
          if (args[i + 1] != null)
          {
            maxrecords = args[i + 1];
          }
        }
        if (args[i].Equals("--matchlevel"))
        {
          if (args[i + 1] != null)
          {
            matchlevel = args[i + 1];
          }
        }
        if (args[i].Equals("--addressline1"))
        {
          if (args[i + 1] != null)
          {
            addressline1 = args[i + 1];
          }
        }
        if (args[i].Equals("--locality"))
        {
          if (args[i + 1] != null)
          {
            locality = args[i + 1];
          }
        }
        if (args[i].Equals("--administrativearea"))
        {
          if (args[i + 1] != null)
          {
            administrativearea = args[i + 1];
          }
        }
        if (args[i].Equals("--postal"))
        {
          if (args[i + 1] != null)
          {
            postal = args[i + 1];
          }
        }
        if (args[i].Equals("--anyname"))
        {
          if (args[i + 1] != null)
          {
            anyname = args[i + 1];
          }
        }
      }
    }

    /// <summary>
    /// Issues the GET request against the People Business Search endpoint and
    /// pretty-prints the API call and the JSON response to the console.
    /// </summary>
    /// <param name="baseServiceUrl">The People Business Search Cloud API base URL.</param>
    /// <param name="requestQuery">The endpoint path plus query string built by <see cref="CallAPI"/>.</param>
    public static async Task GetContents(string baseServiceUrl, string requestQuery)
    {
      HttpClient client = new HttpClient();
      client.BaseAddress = new Uri(baseServiceUrl);
      HttpResponseMessage response = await client.GetAsync(requestQuery);

      string text = await response.Content.ReadAsStringAsync();
      // Re-serialize with indentation so the raw response is easier to read.
      var obj = JsonConvert.DeserializeObject(text);
      var prettyResponse = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);

      // Print output
      Console.WriteLine("\n================================== OUTPUT ==================================\n");

      Console.WriteLine("API Call: ");
      string APICall = Path.Combine(baseServiceUrl, requestQuery);
      for (int i = 0; i < APICall.Length; i += 70)
      {
        if (i + 70 < APICall.Length)
        {
          Console.WriteLine(APICall.Substring(i, 70));
        }
        else
        {
          Console.WriteLine(APICall.Substring(i, APICall.Length - i));
        }
      }

      Console.WriteLine("\nAPI Response:");
      Console.WriteLine(prettyResponse);
    }
    /// <summary>
    /// Drives the interactive/CLI loop: gathers the required search fields, builds and
    /// submits the REST query, prints the result, and optionally repeats for another record.
    ///
    /// <para>In interactive mode (no search args) it loops, asking for a new record each pass
    /// until the user answers "N". In one-shot mode (search args supplied) it runs a single
    /// pass and exits.</para>
    /// </summary>
    /// <param name="baseServiceUrl">The People Business Search Cloud API base URL.</param>
    /// <param name="serviceEndPoint">The specific People Business Search endpoint path to call.</param>
    /// <param name="license">The Melissa license string sent with every request.</param>
    /// <param name="maxrecords">The maximum number of records to return in one-shot mode; if all search fields are empty, the program prompts interactively.</param>
    /// <param name="matchlevel">The match level to use in one-shot mode.</param>
    /// <param name="addressline1">A street address to search on in one-shot mode.</param>
    /// <param name="locality">A locality (city) to search on in one-shot mode.</param>
    /// <param name="administrativearea">An administrative area (state/province) to search on in one-shot mode.</param>
    /// <param name="postal">A postal code to search on in one-shot mode.</param>
    /// <param name="anyname">An individual or company name to search on in one-shot mode.</param>
    static void CallAPI(string baseServiceUrl, string serviceEndPoint, string license, string maxrecords, string matchlevel, string addressline1, string locality, string administrativearea, string postal, string anyname)
    {
      Console.WriteLine("\n============ WELCOME TO MELISSA PEOPLE BUSINESS SEARCH CLOUD API ===========\n");

      bool shouldContinueRunning = true;

      while (shouldContinueRunning)
      {
        string inputMaxRecords = "";
        string inputMatchLevel = "";
        string inputAddressLine1 = "";
        string inputLocality = "";
        string inputAdministrativeArea = "";
        string inputPostal = "";
        string inputAnyName = "";

        // No values were supplied via command line, so prompt for every field.
        if (string.IsNullOrEmpty(maxrecords) && string.IsNullOrEmpty(matchlevel) && string.IsNullOrEmpty(addressline1) && string.IsNullOrEmpty(locality) && string.IsNullOrEmpty(administrativearea) && string.IsNullOrEmpty(postal) && string.IsNullOrEmpty(anyname))
        {
          Console.WriteLine("\nFill in each value to see results");

          Console.Write("Max Records: ");
          inputMaxRecords = Console.ReadLine();

          Console.Write("Match Level: ");
          inputMatchLevel = Console.ReadLine();

          Console.Write("Addressline1: ");
          inputAddressLine1 = Console.ReadLine();

          Console.Write("Locality: ");
          inputLocality = Console.ReadLine();

          Console.Write("Administrative Area: ");
          inputAdministrativeArea = Console.ReadLine();

          Console.Write("Postal: ");
          inputPostal = Console.ReadLine();

          Console.Write("Any Name: ");
          inputAnyName = Console.ReadLine();
        }
        else
        {
          // At least one field was supplied via command line; use those values as-is.
          inputMaxRecords = maxrecords;
          inputMatchLevel = matchlevel;
          inputAddressLine1 = addressline1;
          inputLocality = locality;
          inputAdministrativeArea = administrativearea;
          inputPostal = postal;
          inputAnyName = anyname;
        }

        // Prompt individually for any still-missing required field (all seven are required).
        while (string.IsNullOrEmpty(inputMaxRecords) || string.IsNullOrEmpty(inputMatchLevel) || string.IsNullOrEmpty(inputAddressLine1) || string.IsNullOrEmpty(inputLocality) || string.IsNullOrEmpty(inputAdministrativeArea) || string.IsNullOrEmpty(inputPostal) || string.IsNullOrEmpty(inputAnyName))
        {
          Console.WriteLine("\nFill in missing required parameter");

          if (string.IsNullOrEmpty(inputMaxRecords))
          {
            Console.Write("Max Records: ");
            inputMaxRecords = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputMatchLevel))
          {
            Console.Write("Match Level: ");
            inputMatchLevel = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputAddressLine1))
          {
            Console.Write("Addressline1: ");
            inputAddressLine1 = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputLocality))
          {
            Console.Write("Locality: ");
            inputLocality = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputAdministrativeArea))
          {
            Console.Write("Administrative Area: ");
            inputAdministrativeArea = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputPostal))
          {
            Console.Write("Postal: ");
            inputPostal = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputAnyName))
          {
            Console.Write("Any Name: ");
            inputAnyName = Console.ReadLine();
          }
        }

        // Map input fields to the API's expected query parameter names and
        // request a JSON response.
        Dictionary<string, string> inputs = new Dictionary<string, string>()
                {
                    { "format", "json" },
                    { "maxrecords", inputMaxRecords },
                    { "matchlevel", inputMatchLevel },
                    { "a1", inputAddressLine1 },
                    { "loc", inputLocality },
                    { "adminarea", inputAdministrativeArea },
                    { "postal", inputPostal },
                    { "anyname", inputAnyName }
                };

        Console.WriteLine("\n================================== INPUTS ==================================\n");
        Console.WriteLine($"\t   Base Service Url: {baseServiceUrl}");
        Console.WriteLine($"\t  Service End Point: {serviceEndPoint}");
        Console.WriteLine($"\t        Max Records: {inputMaxRecords}");
        Console.WriteLine($"\t        Match Level: {inputMatchLevel}");
        Console.WriteLine($"\t       AddressLine1: {inputAddressLine1}");
        Console.WriteLine($"\t           Locality: {inputLocality}");
        Console.WriteLine($"\t AdministrativeArea: {inputAdministrativeArea}");
        Console.WriteLine($"\t             Postal: {inputPostal}");
        Console.WriteLine($"\t           Any Name: {inputAnyName}");

        // Create Service Call
        // Set the License String in the Request
        string RESTRequest = "";

        RESTRequest += @"&id=" + Uri.EscapeDataString(license);

        // Set the Input Parameters
        foreach (KeyValuePair<string, string> kvp in inputs)
          RESTRequest += @"&" + kvp.Key + "=" + Uri.EscapeDataString(kvp.Value);

        // Build the final REST String Query
        RESTRequest = serviceEndPoint + @"?" + RESTRequest;

        // Submit to the Web Service. 
        bool success = false;
        int retryCounter = 0;

        do
        {
          try //retry just in case of network failure
          {
            GetContents(baseServiceUrl, $"{RESTRequest}").Wait();
            Console.WriteLine();
            success = true;
          }
          catch (Exception ex)
          {
            retryCounter++;
            Console.WriteLine(ex.ToString());
            return;
          }
        } while ((success != true) && (retryCounter < 5));

        // If any search field came from the command line, treat this as a one-shot
        // run rather than looping for additional records.
        bool isValid = false;
        if (!string.IsNullOrEmpty(maxrecords + matchlevel + addressline1 + locality + administrativearea + postal + anyname))
        {
          isValid = true;
          shouldContinueRunning = false;
        }

        // Otherwise ask whether to test another record. Keep prompting until we get a
        // valid Y/N. "N" ends the program; "Y" falls through to another pass.
        while (!isValid)
        {
          Console.WriteLine("\nTest another record? (Y/N)");
          string testAnotherResponse = Console.ReadLine();

          if (!string.IsNullOrEmpty(testAnotherResponse))
          {
            testAnotherResponse = testAnotherResponse.ToLower();
            if (testAnotherResponse == "y")
            {
              isValid = true;
            }
            else if (testAnotherResponse == "n")
            {
              isValid = true;
              shouldContinueRunning = false;
            }
            else
            {
              Console.Write("Invalid Response, please respond 'Y' or 'N'");
            }
          }
        }
      }

      Console.WriteLine("\n================== THANK YOU FOR USING MELISSA CLOUD API ===================\n");
    }
  }
}
