using Newtonsoft.Json;
using System.Security.Cryptography;

namespace PeopleBusinessSearchDotnet
{
  static class Program
  {

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

      ParseArguments(ref license, ref maxrecords, ref matchlevel, ref addressline1, ref locality, ref administrativearea, ref postal, ref anyname, args);
      CallAPI(baseServiceUrl, serviceEndpoint, license, maxrecords, matchlevel, addressline1, locality, administrativearea, postal, anyname);
    }

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

    public static async Task GetContents(string baseServiceUrl, string requestQuery)
    {
      HttpClient client = new HttpClient();
      client.BaseAddress = new Uri(baseServiceUrl);
      HttpResponseMessage response = await client.GetAsync(requestQuery);

      string text = await response.Content.ReadAsStringAsync();
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
          inputMaxRecords = maxrecords;
          inputMatchLevel = matchlevel;
          inputAddressLine1 = addressline1;
          inputLocality = locality;
          inputAdministrativeArea = administrativearea;
          inputPostal = postal;
          inputAnyName = anyname;
        }

        while (string.IsNullOrEmpty(inputMaxRecords) || string.IsNullOrEmpty(inputMatchLevel) || string.IsNullOrEmpty(inputAddressLine1) || string.IsNullOrEmpty(inputLocality) || string.IsNullOrEmpty(inputAdministrativeArea) || string.IsNullOrEmpty(inputPostal) || string.IsNullOrEmpty(inputAnyName))
        {
          Console.WriteLine("\nFill in missing required parameter");

          if (string.IsNullOrEmpty(inputMaxRecords))
          {
            Console.Write("Pafid: ");
            inputMaxRecords = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputMatchLevel))
          {
            Console.Write("Company: ");
            inputMatchLevel = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputAddressLine1))
          {
            Console.Write("Addressline1: ");
            inputAddressLine1 = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputLocality))
          {
            Console.Write("City: ");
            inputLocality = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputAdministrativeArea))
          {
            Console.Write("State: ");
            inputAdministrativeArea = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputPostal))
          {
            Console.Write("PostalCode: ");
            inputPostal = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputAnyName))
          {
            Console.Write("Country: ");
            inputAnyName = Console.ReadLine();
          }
        }

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

        bool isValid = false;
        if (!string.IsNullOrEmpty(maxrecords + matchlevel + addressline1 + locality + administrativearea + postal + anyname))
        {
          isValid = true;
          shouldContinueRunning = false;
        }

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
