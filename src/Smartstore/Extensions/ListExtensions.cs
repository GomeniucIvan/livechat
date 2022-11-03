namespace Smartstore.Extensions
{
    public static class ListExtensions
    {
        public static IList<int> ToListOfInt(this string stringList)
        {
            if (!string.IsNullOrEmpty(stringList) && stringList.Contains("%2C"))
            {
                stringList = stringList.Replace("%2C", ",");
            }

            var returnList = new List<int>();
            if (string.IsNullOrEmpty(stringList))
            {
                return returnList;
            }

            try
            {
                return stringList.Split(',').Select(v => v.ToInt()).ToList();
            }
            catch (Exception e)
            {
                return returnList;
            }
        }

        public static IList<string> ToListOfString(this string stringList)
        {
            var returnList = new List<string>();
            if (string.IsNullOrEmpty(stringList))
            {
                return returnList;
            }

            try
            {
                return stringList.Split(',').Select(v => v).ToList();
            }
            catch (Exception e)
            {
                return returnList;
            }
        }
    }
}
