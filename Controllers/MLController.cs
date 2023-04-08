using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NETBuddy.ML.SentimentAnalysis.Toxicity;

namespace TrumpMcDonaldz.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MLController : ControllerBase
    {
        private const string SentimentAnalysisEndpoint = "SA";
        
        public enum SentimentAnalysisType: int
        {
            Toxicity = 1
        }

        [ModuleInitializer]
        internal static void WarmUp()
        {
            Console.WriteLine("Warming up ML.NET!");
            
            RuntimeHelpers.RunClassConstructor(typeof(ToxicitySA).TypeHandle);
            ToxicitySA.Predict(new ToxicitySA.ModelInput() { Comment_text = string.Empty });
        }
        
        [HttpGet]
        [Route(SentimentAnalysisEndpoint + "/{Type}")]
        public ValueTask<string> Get([FromRoute] SentimentAnalysisType Type, [FromQuery] string Text)
        {
            string Result;
            
            switch (Type)
            {
                default:
                    goto InvalidType;
                
                case SentimentAnalysisType.Toxicity:
                {
                    var Input = new ToxicitySA.ModelInput()
                    {
                        Comment_text = Text
                    };

                    Result = ToxicitySA.Predict(Input).PredictedLabel == "1"  ? "Toxic" : "Not Toxic";

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