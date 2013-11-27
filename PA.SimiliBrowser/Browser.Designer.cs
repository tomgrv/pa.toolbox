namespace PA.SimiliBrowser
{
    partial class Browser
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.pluginHost1 = new PA.Components.PluginLoader();
            ((System.ComponentModel.ISupportInitialize)(this.pluginHost1)).BeginInit();
            // 
            // pluginHost1
            // 
           
            this.pluginHost1.Location = null;
 
            ((System.ComponentModel.ISupportInitialize)(this.pluginHost1)).EndInit();

        }

        #endregion

        private PA.Components.PluginLoader pluginHost1;
    }
}
