using System;
using System.Xml.Linq;



namespace AeroCalcCore
{



    /// <summary>
    /// Classe assurant l'interface entre un container de modèles de performance de vol et les fichiers textes CSV
    /// permettant la persistance des unités de mesures utilisées dans ces modèles.
    /// 
    /// Principe:
    /// Reçoit le chemin du dossier à traiter et le filtre à appliquer sur les noms de fichiers.
    /// Renvoie une liste des fichiers répondant au critère du filtre
    /// Renvoie les données des unités de mesures trouvées dans ces fichiers
    /// </summary>
    /// 
    public class UnitsXMLFile : XMLFile
    {
        /*
         * CONSTANTES
         */
        //! Placer les constantes de mapping XML dans la classe de base XMLFile



        /*
         * CONSTRUCTEURS
         */

        public UnitsXMLFile(string unitsXMLFilePath) : base("", unitsXMLFilePath)
        {
            // xDoc should be loaded with XML file content
        }



        /*
         * SERVICES
         */

        /// <summary>
        /// Sauvegarde un objet UnitDictionary dans un fichier XML
        /// </summary>
        /// <param name="ud">Objet UnitDictionary à sauvegarder</param>
        /// <param name="xmlFileAbsolutePath">Chemin absolu vers le fichier XML à créer</param>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// TODO A DEVELOPPER, il faut rajouter la branche <Dimension> dans laquelle placer les <Unit>
        /// </remarks>
        /// 
        public int saveUnitDictionaryToXML(Units ud, string xmlFileAbsolutePath)
        {
            XDocument xmlUnitsDoc = new XDocument();
            xmlUnitsDoc.AddFirst(xmlUnits(ud));
            xmlUnitsDoc.Save(xmlFileAbsolutePath);
            return XMLFile.FILEOP_SUCCESSFUL;
        }



        /// <summary>
        /// Récupère les unités utilisées dans les calculs dans le fichier XML des unités.
        /// </summary>
        /// TODO Doit on renvoyer un null en cas d'échec ???
        public Units getUnitsFromXML(string xmlFileAbsolutePath)
        {
            // Introducing a new file
            if (!string.IsNullOrEmpty(xmlFileAbsolutePath))
            {
                if (!setInputFileAbsolutePath(xmlFileAbsolutePath))
                {
                    return null;
                }
                // This new file seems fine !
                readXmlFile();
            }
            return getUnitsFromXML();
        }

        /// <summary>
        /// Récupère les unités utilisées dans les calculs dans le fichier XML des unités.
        /// </summary>
        public Units getUnitsFromXML()
        {
            Units units = new Units();

            if (xDoc != null)
            {
                foreach (XElement xe in xDoc.Descendants(NODE_DIMENSION))
                {
                    // This is a DIMENSION node
                    string dimension = xe.Attribute(ATTRIB_NAME).Value;
                    if (dimension != null && dimension != "")
                    {
                        foreach (XElement item in xe.Descendants(NODE_UNIT))
                        {
                            // This is a UNIT node, let's get the data
                            string unitName = item.Attribute(ATTRIB_NAME).Value;
                            bool isRef = getBoolOrDefault(item.Attribute(ATTRIB_ISREF).Value, false);
                            string alias = item.Attribute(ATTRIB_ALIAS).Value;
                            double factor = getDoubleOrNaN(item.Attribute(ATTRIB_FACTOR).Value);
                            double constant = getDoubleOrNaN(item.Attribute(ATTRIB_CONST).Value);
                            if (!isRef && factor == Double.NaN || constant == Double.NaN)
                            {
                                // Not a valid unit
                                break;
                            }
                            units.add(new Unit(xe.Value, unitName, alias, isRef, factor, constant));
                        }
                    }
                }
            }
            return units;
        }



        /*
         *  METHODES 
         */

        /// <summary>
        /// Retourne un XElement contenant la définition XML d'une unité de mesure
        /// </summary>
        /// <param name="u">Unité de mesure</param>
        /// <returns></returns>
        /// 
        private XElement xmlUnit(Unit u)
        {

            string isref;

            if (u.isRef)
            {
                isref = "true";
            }
            else
            {
                isref = "false";
            }
            return new XElement(NODE_UNIT, new XAttribute(ATTRIB_NAME, u.name),
                                           new XAttribute(ATTRIB_ISREF, isref),
                                           new XAttribute(ATTRIB_ALIAS, u.alias),
                                           new XAttribute(ATTRIB_FACTOR, u.factor),
                                           new XAttribute(ATTRIB_CONST, u.constant));
        }



        /// <summary>
        /// Retourne un XElement contenant l'ensemble des définitions d'unités du dictionnaire
        /// </summary>
        /// <param name="dico">Dictionnaire des unités</param>
        /// <returns></returns>
        /// 
        private XElement xmlUnits(Units dico)
        {

            XElement x = new XElement(NODE_UNITS);

            foreach (Unit u in dico.units)
            {
                x.Add(xmlUnit(u));
            }
            return x;
        }






    }

}
