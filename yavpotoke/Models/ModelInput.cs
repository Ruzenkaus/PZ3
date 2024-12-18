using Microsoft.ML.Data;

namespace yavpotoke.Models
{
    public class ModelInput
    {
        [LoadColumn(0)]
        public string Col0;

        [LoadColumn(1)]
        public float Label;
    }
}
