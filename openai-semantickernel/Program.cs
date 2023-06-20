using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using openai_semantickernel;
using System.Text;


var questions = new List<string>();
questions.Add("How many invoices for PO number PO-2023-001");
questions.Add("How many invoices for supplier apple");
questions.Add("give me invoice details for unapproved invoices");
questions.Add("give me invoice numbers for supplier apple");
questions.Add("give me invoice details for supplier apple");
questions.Add("Give me PO details for Invoice INV-2023-001");

questions.Add("Give me PO details for Invoice INV-2023-001");
questions.Add("Give me all inventory details where ServiceId is available");
questions.Add("Give me inventory for PO PO-2023-004");
questions.Add("inventories null rfs date");
questions.Add("invoices against inventory with null rfs date");
questions.Add("Give me Invoice details for Invoice INV-2023-006");

questions.Add("inventory rfsdate > 2023-06-16");
questions.Add("invoices against inventory with future rfs date");

//string searchQuestion = "How many invoices for PO number PO-2023-001";
//string searchQuestion = "How many invoices for supplier apple";
//string searchQuestion = "give me invoice details for unapproved invoices";
//string searchQuestion = "give me invoice numbers for supplier apple";
//string searchQuestion = "give me invoice details for supplier apple";
//string searchQuestion = "Give me PO details for Invoice INV-2023-001";
//string searchQuestion = "Give me PO details for Invoice INV-2023-001";
//string searchQuestion = "Give me all inventory details where ServiceId is available";
//string searchQuestion = "Give me inventory for PO PO-2023-004";
//string searchQuestion = "inventories null rfs date";
//string searchQuestion = "invoices against inventory with null rfs date";
//string searchQuestion = "inventory rfsdate > 2023-06-16";
//string searchQuestion = "invoices against inventory with future rfs date";


CognitiveSearchServices cognitiveSearchService = new CognitiveSearchServices();
OpenAIServices openAIServices = new OpenAIServices();

#region cognitiveSearch selecting Indexes based on question
//string indexes = await cognitiveSearchService.getListOfIndexes();

//StringBuilder sb = new StringBuilder("Question:");
//sb.Append("\n");
//sb.Append(searchQuestion);
//sb.Append("\n");
//sb.Append("indexList:");
//sb.Append("\n");
//sb.Append(indexes);

//string indexName = openAIServices.getOpenAIResponseUsingSkill(sb.ToString(), "azuresql-index").Result;

#endregion

#region Index and skills KeyValuePair
//indexName = indexName.Substring(indexName.IndexOf(':') + 1).Trim();
//Dictionary<string, string> indexSkillDictionary = new Dictionary<string, string>();

//indexSkillDictionary.Add("azuresql-facilityindex", "Facility");
//indexSkillDictionary.Add("azuresql-index-facilityindexwithalloptionsenabled", "Facility");
//indexSkillDictionary.Add("azuresql-index", "Facility");
//indexSkillDictionary.Add("dcep-index", "Facility");
//indexSkillDictionary.Add("dpxeuseraccess-idx", "Facility");
//indexSkillDictionary.Add("hotels-sql-idx", "Facility");
//indexSkillDictionary.Add("projects-azuresql-index", "Facility");
#endregion
foreach (var searchQuestion in questions)
{
    ////if (searchQuestion != null && indexSkillDictionary.ContainsKey(indexName))
    //if (searchQuestion != null)
    //{
    
        Console.WriteLine("==========================================================================================");
        Console.WriteLine();
        Console.WriteLine($"Question: {searchQuestion}");

    #region cognitiveSearch
    var responseFromCognitiveSearch = cognitiveSearchService.getSearchResult("azuresql-poinvoices-index", searchQuestion, "semantic", "").Result;
        #endregion

        #region chatgpt and kernel

        StringBuilder s = new StringBuilder(searchQuestion);

        s.Append(responseFromCognitiveSearch);

        string openaiResult = openAIServices.getOpenAIResponseUsingSkill(s.ToString(), "InvoiceSkill").Result;
        //string openaiResult = openAIServices.getOpenAIResponse(s.ToString()).Result;
        #endregion

        var combinedResults = new
        {
            OpenAICompletion = openaiResult,
        };

        Console.WriteLine(combinedResults);
        
        Console.WriteLine();
        Console.WriteLine("==========================================================================================");
    //}
    //else if (searchQuestion != null)
    //{
    //    Console.WriteLine("Could not find the data based on the question, please add details to the question");
    //}

}
