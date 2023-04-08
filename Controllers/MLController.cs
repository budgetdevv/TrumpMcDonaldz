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
                    var input = new ToxicitySA.ModelInput()
                    {
                        Comment_text = Text
                    };

                    Result = ToxicitySA.Predict(input).PredictedLabel == "1"  ? "Toxic" : "Not Toxic";

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