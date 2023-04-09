using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrumpMcDonaldz.Modules.ML.SentimentAnalysis.Toxicity;

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
            KaggleToxicity.Predict(new KaggleToxicity.ModelInput() { Text = string.Empty });
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
                    var Input = new KaggleToxicity.ModelInput()
                    {
                        Text = Text
                    };

                    Result = JsonSerializer.Serialize(KaggleToxicity.PredictAllLabels(Input));

                    break;
                }
                
                // case SentimentAnalysisType.Emotion:
                // {
                //     var Input = new KaggleToxicity.ModelInput()
                //     {
                //         Text = Text
                //     };
                //
                //     Result = KaggleToxicity.Predict(Input).PredictedLabel == "1"  ? "Toxic" : "Not Toxic";
                //
                //     break;
                // }
            }
            
            Ret:
            return new ValueTask<string>(Result);
            
            InvalidType:
            Result = "Invalid Type!";
            goto Ret;
        }
    }
}