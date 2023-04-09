using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SA_GoEmotion;
using SA_KaggleToxicity;

namespace TrumpMcDonaldz.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MLController : ControllerBase
    {
        private const string SentimentAnalysisEndpoint = "SA";
        
        public enum SentimentAnalysisType: int
        {
            Toxicity = 1,
            Emotion = 2
        }

        [ModuleInitializer]
        internal static void WarmUp()
        {
            Console.WriteLine("Warming up ML.NET!");
            
            RuntimeHelpers.RunClassConstructor(typeof(KaggleToxicity).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof(GoEmotion).TypeHandle);
            KaggleToxicity.Predict(new KaggleToxicity.ModelInput() { Text = string.Empty });
        }
        
        [HttpGet]
        [Route(SentimentAnalysisEndpoint + "/{Type}")]
        public ValueTask<string> Get([FromRoute] SentimentAnalysisType Type, [FromQuery] string Text, [FromQuery] bool ReplyText = true)
        {
            string Result;
            
            switch (Type)
            {
                default:
                    goto InvalidType;
                
                case SentimentAnalysisType.Toxicity:
                {
                    if (!ReplyText)
                    {
                        Result = JsonSerializer.Serialize(KaggleToxicity.PredictAllLabels(Text));
                    }

                    else
                    {
                        Result = KaggleToxicity.PredictAllLabelsText(Text);
                    }

                    break;
                }
                
                case SentimentAnalysisType.Emotion:
                {
                    if (!ReplyText)
                    {
                        Result = JsonSerializer.Serialize(GoEmotion.PredictAllLabels(Text));
                    }

                    else
                    {
                        Result = GoEmotion.PredictAllLabelsText(Text);
                    }

                    break;
                }
            }
            
            Ret:
            return new ValueTask<string>(Result);
            
            InvalidType:
            Result = "Invalid Type!";
            goto Ret;
        }
    }
}