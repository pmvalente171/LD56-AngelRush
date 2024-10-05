using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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
                        responseText = "You give a dramatic shrug, your eyes twinkling as you reply.",
                        context = new List<string> { "SNARK", "QUESTION", "DISAGREEMENT", "INTRODUCTION", "JOKE", "BARGAIN" }
                    },
                    new()
                    {
                        responseText = "You lean in closer, excitement in your voice, adding a hint of mystery to your words.",
                        context = new List<string> { "QUESTION", "SECRET", "FLIRT", "PLAN", "RISK", "OPPORTUNITY" }
                    },
                    new()
                    {
                        responseText = "With an exaggerated flourish, you gesture towards the direction ahead.",
                        context = new List<string> { "DIRECTION", "PLAN", "INSTRUCTION", "FIGHT", "ESCAPE", "WARNING" }
                    },
                    new()
                    {
                        responseText = "You grin widely, throwing caution to the wind, and dive right in.",
                        context = new List<string> { "FIGHT", "CHALLENGE", "OPPORTUNITY", "RISK", "ADVENTURE", "INVITATION" }
                    }
                }
            },
            
            // SWORDS
            {
                CardSuit.SWORDS, new List<UserResponse>
                {
                    new()
                    {
                        responseText = "You narrow your eyes thoughtfully, carefully considering their question before answering.",
                        context = new List<string> { "QUESTION", "DEBATE", "PROBLEM_SOLVING", "DISCUSSION", "DECISION", "INQUIRY" }
                    },
                    new()
                    {
                        responseText = "You offer a quick, sharp response, cutting through the small talk.",
                        context = new List<string> { "SNARK", "INSULT", "DEBATE", "RETORT", "ARGUMENT", "INSTRUCTION" }
                    },
                    new()
                    {
                        responseText = "You outline the plan in a cool, logical tone, ensuring no detail is missed.",
                        context = new List<string> { "PLAN", "INSTRUCTION", "STRATEGY", "DISCUSSION", "NEGOTIATION", "TEAM_MEETING" }
                    },
                    new()
                    {
                        responseText = "You adjust your glasses (real or imaginary) and proceed to dismantle their argument piece by piece.",
                        context = new List<string> { "ARGUMENT", "DEBATE", "DISCUSSION", "INTERROGATION", "EXPLANATION", "CONFRONTATION" }
                    }
                }
            },
            // CUPS
            {
                CardSuit.CUPS, new List<UserResponse>
                {
                    new()
                    {
                        responseText = "You soften your tone, nodding empathetically as you respond.",
                        context = new List<string> { "QUESTION", "CONCERN", "CONFESSION", "APOLOGY", "COMFORT", "REASSURE" }
                    },
                    new()
                    {
                        responseText = "You laugh warmly, feeling the connection as you share a moment of light-hearted humor.",
                        context = new List<string> { "SNARK", "JOKE", "LIGHTHEARTED_MOMENT", "INTRODUCTION", "GOODBYE", "CELEBRATION" }
                    },
                    new()
                    {
                        responseText = "You reach out gently, offering a kind word to reassure them.",
                        context = new List<string> { "REASSURE", "COMFORT", "SADNESS", "FEAR", "CONFESSION", "CONCERN" }
                    },
                    new()
                    {
                        responseText = "You let out a sigh, your voice carrying a gentle sadness, acknowledging the difficulty of the situation.",
                        context = new List<string> { "SADNESS", "APOLOGY", "LOSS", "REGRET", "CONFESSION", "DISAPPOINTMENT" }
                    }
                }
            },
            // COINS
            {
                CardSuit.COINS, new List<UserResponse>
                {
                    new()
                    {
                        responseText = "You cross your arms, nodding firmly as you talk about the cost-benefit of the plan.",
                        context = new List<string> { "PLAN", "NEGOTIATION", "DISCUSSION", "OPPORTUNITY", "RISK", "DEAL" }
                    },
                    new()
                    {
                        responseText = "You look at them with a knowing smirk, suggesting a practical solution.",
                        context = new List<string> { "QUESTION", "PROBLEM_SOLVING", "SUGGESTION", "NEGOTIATION", "HELP", "BARGAIN" }
                    },
                    new()
                    {
                        responseText = "You tap your finger against your wallet, making it clear what's at stake.",
                        context = new List<string> { "ARGUMENT", "NEGOTIATION", "WARNING", "PAYMENT", "DEAL", "FAVOR" }
                    },
                    new()
                    {
                        responseText = "You pull out your phone, calculating the numbers as you respond.",
                        context = new List<string> { "NEGOTIATION", "PLAN", "BARGAIN", "DEAL", "DISCUSSION", "PROBLEM_SOLVING" }
                    }
                }
            },
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
        
        public static string GenerateDialog(CardSuit suit, List<string> context)
        {
            List<UserResponse> responses = suitResponses[suit];
            UserResponse bestResponse = responses[0];
            int bestDistance = int.MaxValue;
            foreach (var response in responses)
            {
                int distance = 0;
                foreach (var contextString in context)
                {
                    foreach (var responseContext in response.context)
                    {
                        distance = Mathf.Min(distance, CompareStrings(contextString, responseContext));
                    }
                }

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestResponse = response;
                }
            }

            return bestResponse.responseText;
        }
    }
}