using EwECore.MSE;

namespace EwECore.Tests.MSE
{
    /// <summary>Lightweight in-memory implementation of <see cref="IMSEQuotaData"/>.</summary>
    public class FakeQuotaData : IMSEQuotaData
    {
        public FakeQuotaData(int numGroups, int numLiving, int nGear)
        {
            nGroups = numGroups;
            nLiving = numLiving;
            nFleets = nGear;
            TAC = new float[numGroups + 1];
            FixedEscapement = new float[numGroups + 1];
            FixedF = new float[numGroups + 1];
            Fopt = new float[numGroups + 1];
            Fmin = new float[numGroups + 1];
            Bbase = new float[numGroups + 1];
            Blim = new float[numGroups + 1];
            Bestimate = new float[numGroups + 1];
            CVbiomEst = new float[numGroups + 1];
            FTarget = new float[numGroups + 1];
            Quotashare = new float[nGear + 1, numGroups + 1];
            QuotaTime = new float[nGear + 1, numGroups + 1];
            CatchYearGroup = new float[numGroups + 1];
            BestimateLast = new float[numGroups + 1];
            Fish1 = new float[numGroups + 1];
            GstockPred = new float[numGroups + 1];
            RstockRatio = new float[numGroups + 1];
            KalmanGain = new float[numGroups + 1];
            BhalfT = new float[numGroups + 1];
            Rmax = new float[numGroups + 1];
            cvRec = new float[numGroups + 1];
        }

        public int nGroups { get; set; }
        public int nLiving { get; set; }
        public int nFleets { get; set; }
        public float[] TAC { get; set; }
        public float[] FixedEscapement { get; set; }
        public float[] FixedF { get; set; }
        public float[] Fopt { get; set; }
        public float[] Fmin { get; set; }
        public float[] Bbase { get; set; }
        public float[] Blim { get; set; }
        public float[] Bestimate { get; set; }
        public float[] CVbiomEst { get; set; }
        public float[] FTarget { get; set; }
        public float[,] Quotashare { get; set; }
        public float[,] QuotaTime { get; set; }
        public float[] CatchYearGroup { get; set; }
        public float[] BestimateLast { get; set; }
        public float[] Fish1 { get; set; }
        public float[] GstockPred { get; set; }
        public float[] RstockRatio { get; set; }
        public float[] KalmanGain { get; set; }
        public float[] BhalfT { get; set; }
        public float[] Rmax { get; set; }
        public float[] cvRec { get; set; }
        public float[] RHalfB0Ratio { get; set; }
        public IMSESummaryStats BioEstStats { get; set; } = null!;
    }
}
