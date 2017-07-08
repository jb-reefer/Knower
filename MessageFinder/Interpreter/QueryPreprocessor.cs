namespace MessageFinder
{
    public static class QueryPreprocessor
    {
        public static string Process(string query)
        {
            // Need to be able to treat less than and greater than as single, unspaced tokens.
            return query.Replace("less than", "lessthan").Replace("greater than", "greaterthan");
        }
    }
}