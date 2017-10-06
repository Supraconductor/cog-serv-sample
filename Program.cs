
using System;
using System.IO;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System.Collections.Generic;


// cle repertoire

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a client.
            ITextAnalyticsAPI client = new TextAnalyticsAPI();
            client.AzureRegion = AzureRegions.Eastus2;
            client.SubscriptionKey = args[0];

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string[] fileEntries = Directory.GetFiles(args[1]);

            foreach (string fileName in fileEntries)
            {
                FileStream fileStream = new FileStream(fileName, FileMode.Open);
                FileStream out1 = new FileStream(fileName + ".KP.TXT", FileMode.Create);
                FileStream out2 = new FileStream(fileName + ".SENT.TXT", FileMode.Create);

                try
                {   // Open the text file using a stream reader.
                    using (StreamReader sr = new StreamReader(fileStream))
                    {
                        String fullText = "";

                        // Read the stream to a string, and write the string to the console.
                        while (!sr.EndOfStream)
                        {
                            String line = sr.ReadLine();
                            if (!line.StartsWith("NOTE Confidence:") && !line.StartsWith("00:") && !line.StartsWith("WEBVTT"))
                                fullText = fullText + line + System.Environment.NewLine;
                        }
                        //Console.Write(fullText);

                        Console.WriteLine("\n\n===== KEY-PHRASE EXTRACTION ======");

                        KeyPhraseBatchResult result2 = client.KeyPhrases(
                            new MultiLanguageBatchInput(
                                new List<MultiLanguageInput>()
                                    {
                                        new MultiLanguageInput("", "1", fullText)
                                    }));

                        using (StreamWriter sw1 = new StreamWriter(out1))
                        {
                            // Printing keyphrases
                            foreach (var document in result2.Documents)
                            {
                                foreach (string keyphrase in document.KeyPhrases)
                                {
                                    sw1.WriteLine(keyphrase);
                                }
                                /*
                                Console.WriteLine("Document ID: {0} ", document.Id);
                                Console.WriteLine("\t Key phrases:");
                                foreach (string keyphrase in document.KeyPhrases)
                                {
                                    Console.WriteLine("\t\t" + keyphrase);
                                }
                                */
                            }
                        }

             

                        // Extracting sentiment
                        Console.WriteLine("\n\n===== SENTIMENT ANALYSIS ======");

                        SentimentBatchResult result3 = client.Sentiment(
                                new MultiLanguageBatchInput(
                                    new List<MultiLanguageInput>()
                                    {
                          new MultiLanguageInput("", "1", fullText)
                                    }));

                        using (StreamWriter sw2 = new StreamWriter(out2))
                        {
                            // Printing keyphrases

                                foreach (var document in result3.Documents)
                                {
                                    sw2.WriteLine("Sentiment Score: {0:0.00}", document.Score);
                                }

                        }
                        // Printing sentiment results
                        //foreach (var document in result3.Documents)
                       //{
                       //Console.WriteLine("Document ID: {0} , Sentiment Score: {1:0.00}", document.Id, document.Score);
                       //}
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}