using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Encounters
{
    [Serializable]
    public struct UserResponse
    {
        public string responseText;
        public List<string> context;
    }
    
    public static class UserDialogGen
    {
        private static Dictionary<CardSuit, List<UserResponse>> suitResponses = new()
        {
            // WANDS
            {
                CardSuit.WANDS, new List<UserResponse>
                {
                    new()
                    {
                        responseText = "I see you are a person of culture as well.",
                        context = new List<string> { "WANDS" }
                    },
                    new()
                    {
                        responseText = "I see you are a person of culture as well.",
                        context = new List<string> { "WANDS" }
                    },
                    new()
                    {
                        responseText = "I see you are a person of culture as well.",
                        context = new List<string> { "WANDS" }
                    }
                }
            },
            
            // SWORDS
            {
                CardSuit.SWORDS, new List<UserResponse>
                {
                    new()
                    {
                        responseText = "I see you are a person of culture as well.",
                        context = new List<string> { "SWORDS" }
                    },
                    new()
                    {
                        responseText = "I see you are a person of culture as well.",
                        context = new List<string> { "SWORDS" }
                    },
                    new()
                    {
                        responseText = "I see you are a person of culture as well.",
                        context = new List<string> { "SWORDS" }
                    }
                }
            },
            
            // CUPS
            {
                CardSuit.CUPS, new List<UserResponse>
                {
                    new()
                    {
                        responseText = "I see you are a person of culture as well.",
                        context = new List<string> { "CUPS" }
                    },
                    new()
                    {
                        responseText = "I see you are a person of culture as well.",
                        context = new List<string> { "CUPS" }
                    },
                    new()
                    {
                        responseText = "I see you are a person of culture as well.",
                        context = new List<string> { "CUPS" }
                    }
                }
            },
            
            // COINS
            {
                CardSuit.COINS, new List<UserResponse>
                {
                    new()
                    {
                        responseText = "I see you are a person of culture as well.",
                        context = new List<string> { "COINS" }
                    },
                    new()
                    {
                        responseText = "I see you are a person of culture as well.",
                        context = new List<string> { "COINS" }
                    },
                    new()
                    {
                        responseText = "I see you are a person of culture as well.",
                        context = new List<string> { "COINS" }
                    }
                }
            }

        };
        
        private static int CompareStrings(string a, string b) // returns the distance between two strings
        {
            int distance = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    distance++;
                }
            }

            return distance;
        }
        
        public static string GenerateDialog(CardSuit suit, Encounter encounter)
        {
            List<UserResponse> responses = suitResponses[suit];

            Parallel.For(0, responses.Count, i =>
            {
                responses[i].context.Sort((a, b) => CompareStrings(a, encounter.encounterName).CompareTo(CompareStrings(b, encounter.encounterName)));
            });
            
            // select the response with the lowest distance
            UserResponse bestResponse = responses[0];
            foreach (var response in responses)
            {
                if (CompareStrings(response.context[0], encounter.encounterName) < CompareStrings(bestResponse.context[0], encounter.encounterName))
                {
                    bestResponse = response;
                }
            }
            
            return bestResponse.responseText;
        }
    }
}