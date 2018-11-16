using System;
using System.Collections.Generic;

namespace CustomVision
{
    public class PredictionResult
    {
        public string Id { get; set; }
        public string Project { get; set; }
        public string Iteration { get; set; }
        public DateTime Created { get; set; }
        public List<Prediction> Predictions { get; set; }
    }

    public class BoundingBox
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class Prediction
    {
        public double Probability { get; set; }
        public string TagId { get; set; }
        public string TagName { get; set; }
        public BoundingBox BoundingBox { get; set; }
    }
}