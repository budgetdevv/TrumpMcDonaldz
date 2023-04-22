using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SentimentAnalysis.GoEmotion;
using SentimentAnalysis.PosNeg;
using SentimentAnalysis.Toxicity;

namespace TrumpMcDonaldz.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MLController : ControllerBase
    {
        private const string SentimentAnalysisEndpoint = "SA";
        
        public enum SentimentAnalysisType: int
        {
            PosNeg = 1,
            Emotion = 2,
            Toxicity = 3,
        }

        [ModuleInitializer]
        internal static void WarmUp()
        {
            Console.WriteLine("Warming up ML.NET!");
            
            //Running one would speed up the other models :O
            RuntimeHelpers.RunClassConstructor(typeof(PosNegModel).TypeHandle);

            PosNegModel.PredictAllLabels(string.Empty);
        }
        
        [HttpGet]
        [Route(SentimentAnalysisEndpoint + "/{Type}")]
        public ValueTask<string> Get([FromRoute] SentimentAnalysisType Type, [FromQuery] string Text, [FromQuery] bool ReplyText = true, [FromQuery] float Threshold = 0f)
        {
            string result;

            if (ReplyText)
            {
                switch (Type)
                {
                    default:
                        goto InvalidType;
                                
                    case SentimentAnalysisType.PosNeg:
                    {
                        result = PosNegModel.PredictIsNegativeText(Text, Threshold);
                                    
                        break;
                    }
                                
                    case SentimentAnalysisType.Emotion:
                    {
                        result = GoEmotion.PredictAllLabelsText(Text, Threshold);
                    
                        break;
                    }
                    
                    case SentimentAnalysisType.Toxicity:
                    {
                        result = Toxicity.PredictAllLabelsText(Text, Threshold);
                        
                        break;
                    }
                }
            }

            else
            {
                switch (Type)
                {
                    default:
                        goto InvalidType;
                                
                    case SentimentAnalysisType.PosNeg:
                    {
                        result = JsonSerializer.Serialize(PosNegModel.PredictAllLabels(Text));

                        break;
                    }
                                
                    case SentimentAnalysisType.Emotion:
                    {
                        result = JsonSerializer.Serialize(GoEmotion.PredictAllLabels(Text));
                    
                        break;
                    }
                    
                    case SentimentAnalysisType.Toxicity:
                    {
                        result = JsonSerializer.Serialize(Toxicity.PredictAllLabels(Text));

                        break;
                    }
                }
            }
            
            Ret:
            return new ValueTask<string>(result);
            
            InvalidType:
            result = "Invalid Type!";
            goto Ret;
        }
    }
}