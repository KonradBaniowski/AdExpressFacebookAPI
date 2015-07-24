namespace TNS.AdExpressI.Insertions.CreativeResult
{
    public interface ICreativePopUp
    {
        /// <summary>
        /// Vérifie si fichier video trouvé
        /// </summary>
        bool IsVideoFileFound { get; set; }

        /// <summary>
        /// Vérifie si fichier audio trouvé
        /// </summary>
        bool IsAudioFileFound { get; set; }

        /// <summary>
        /// Identifiant Produit
        /// </summary>
        string IdProduct { get; set; }

        /// <summary>
        /// Indique si l'utilisateur à le droit de lire les créations
        /// </summary>
        bool HasCreationReadRights { get; set; }

        /// <summary>
        /// Indique si l'utilisateur à le droit de télécharger les créations
        /// </summary>
        bool HasCreationDownloadRights { get; set; }

        /// <summary>
        /// Get creative popup Html code
        /// </summary>
        /// <returns>Html code</returns>
        string CreativePopUpRender();

        /// <summary>
        /// Get creative popup Html code
        /// </summary>
        /// <returns>Html code</returns>
        string CreativePopUpRenderWithoutOptions(int width, int height);
    }
}