

namespace DocQuery.Core
{
    public static class VectorMath
    {

        public static double CosineSimilarity(float[] a, float[] b)
        {
            if(a == null || b == null) throw new ArgumentNullException();
            if(a.Length != b.Length)
            {
                throw new ArgumentException("Vectors must be of the same length.");
            }

                var dotProduct = 0.0;
                var normA = 0.0;
                var normB = 0.0;

                for (int i = 0; i < a.Length; i++)
                {
                    dotProduct += a[i] * b[i];
                    normA += a[i] * a[i];
                    normB += b[i] * b[i];
                }

                if (normA == 0 || normB == 0)
                {
                    return 0;
                }

                return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
            }
        
    }
}
