using System;
using System.Xml.Linq;



namespace AeroCalcCore
{



    /// <summary>
    /// Classe assurant l'interface entre un container de modèles de performance de vol et les fichiers XML
    /// permettant la persistance des modèles de calcul.
    /// 
    /// </summary>
    /// 
    public class ModelsXMLFile : XMLFile
    {
        /*
         * CONSTANTES
         */
        //! Placer les constantes de mapping XML dans la classe de base XMLFile


        /*
         * CONSTRUCTEURS
         */

        public ModelsXMLFile(string absoluteFilePath) : base("", absoluteFilePath)
        {
            // xDoc should be loaded with XML file content
        }



        /*
         *  SERVICES 
         */

        /// <summary>
        /// Sauvegarde un objet PerfPile dans un fichier XML
        /// </summary>
        /// <param name="pp">Objet PerfPile à sauvegarder</param>
        /// <param name="xmlFileAbsolutePath">Chemin complet vers le fichier XML à créer</param>
        /// <returns>Entier signalant la réussite ou l'échec de l'opération
        ///     XMLConnector.FILEOP_SUCCESSFUL : réussite
        ///     XMLConnector.FILEOP_UNABLE_TO_CREATE_OUTPUT_FILE : Impossible de créer le fichier XML
        ///     XMLConnector.FILEOP_UNKNOWN_ERROR : Erreur non identifiable
        /// </returns>
        /// <remarks>
        /// TODO: A DEVELOPPER
        /// </remarks>
        public int savePerfPileToXML(PerfPile pp, string xmlFileAbsolutePath)
        {
            XDocument xmlPerfDoc = new XDocument();
            xmlPerfDoc.AddFirst(xmlPerfPile(pp));
            xmlPerfDoc.Save(xmlFileAbsolutePath);
            return XMLFile.FILEOP_SUCCESSFUL;
        }



        /*
         *  METHODES 
         */

        /// <summary>
        /// Formate un élément XML pour refléter un objet PerfPoint
        /// </summary>
        /// <param name="pp">Objet PerfPoint</param>
        /// <returns>Un élément XML</returns>
        private XElement xmlPerfPoint(PerfPoint pp)
        {
            XElement xElem = new XElement("Point", new XAttribute("In", pp.factorValue),
                                                   new XAttribute("Out", pp.output),
                                                   new XAttribute("Break", pp.isBreak));
            return xElem;
        }



        /// <summary>
        /// Formate un élément XML pour refléter un objet PerfSerie
        /// </summary>
        /// <param name="pp">Objet PerfSerie</param>
        /// <returns>Un élément XML</returns>
        private XElement xmlPerfSerie(PerfSerie pf)
        {
            XElement xElem = new XElement("Serie", new XAttribute("Key", pf.dataBaseKey),
                                                   new XAttribute("Factor", pf.factorValue));
            XElement xColElem = new XElement("Points");
            // Ajout de chaque point de la série
            for (int count = 0; count < pf.count; count++)
            {
                xColElem.Add(xmlPerfPoint(pf.pointAt(count)));
            }
            xElem.Add(xColElem);
            return xElem;
        }



        /// <summary>
        /// Formate un élément XML pour refléter un objet PerfLayer
        /// </summary>
        /// <param name="pp">Objet PerfLayer</param>
        /// <returns>Un élément XML</returns>
        private XElement xmlPerfLayer(PerfLayer pl)
        {
            XElement xElem = new XElement("Layer", new XAttribute("Name", pl.outputName),
                                                   new XAttribute("Key", pl.dataBaseKey),
                                                   new XAttribute("Factor", pl.factorValue),
                                                   new XAttribute("PointFactorName", pl.pointFactorName),
                                                   new XAttribute("PointFactorUnitCode", pl.pointFactorUnitCode),
                                                   new XAttribute("SerieFactorName", pl.serieFactorName),
                                                   new XAttribute("SerieFactorUnitCode", pl.serieFactorUnitCode));
            XElement xColElem = new XElement("Series");
            // Ajout de chaque série de la Layer
            for (int count = 0; count < pl.count; count++)
            {
                xColElem.Add(xmlPerfSerie(pl.SerieAt(count)));
            }
            xElem.Add(xColElem);
            return xElem;
        }



        /// <summary>
        /// Formate un élément XML pour refléter un objet PerfPile
        /// </summary>
        /// <param name="pp">Objet PerfPile</param>
        /// <returns>Un élément XML</returns>
        private XElement xmlPerfPile(PerfPile pp)
        {
            XElement xElem = new XElement("Pile", new XAttribute("Name", pp.outputName),
                                                  new XAttribute("Key", pp.dataBaseKey),
                                                  new XAttribute("Hidden", (pp.hidden == true ? "1" : "0")),
                                                  new XAttribute("Factor", pp.factorValue),
                                                  new XAttribute("DiscretName", pp.discretName),
                                                  new XAttribute("DiscretValue", pp.discretValue),
                                                  new XAttribute("PointFactorName", pp.pointFactorName),
                                                  new XAttribute("PointFactorUnitCode", pp.pointFactorUnitCode),
                                                  new XAttribute("SerieFactorName", pp.serieFactorName),
                                                  new XAttribute("SerieFactorUnitCode", pp.serieFactorUnitCode),
                                                  new XAttribute("LayerFactorName", pp.layerFactorName),
                                                  new XAttribute("LayerFactorUnitCode", pp.layerFactorUnitCode));
            XElement xColElem = new XElement("Layers");
            // Ajout de chaque layer de la pile
            for (int count = 0; count < pp.count; count++)
            {
                xColElem.Add(xmlPerfLayer(pp.layerAt(count)));
            }
            xElem.Add(xColElem);
            return xElem;
        }

    }

}