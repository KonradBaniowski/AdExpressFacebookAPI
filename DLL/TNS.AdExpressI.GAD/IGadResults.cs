namespace TNS.AdExpressI.GAD
{
    public interface IGadResults
    {
        /// <summary>
        /// Get /Set Theme
        /// </summary>
        string Theme { get; set; }

        /// <summary>
        /// Get Gad html result
        /// </summary>
        /// <returns>Gad html result</returns>
        string GetHtml();
    }
}