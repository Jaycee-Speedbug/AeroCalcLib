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
        // XML mapping
        public const string NODE_UNITS = "Units";
        public const string NODE_UNIT = "Unit";
        public const string NODE_DIMENSION = "Dimension";
        public const string UNIT_ATTR_NAME = "name";
        public const string UNIT_ATTR_ALIAS = "alias";
        public const string UNIT_ATTR_ISREF = "isref";
        public const string UNIT_ATTR_FACTOR = "factor";
        public const string UNIT_ATTR_CONST = "constant";



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
        /// Récupère les unités utiliées dans les calculs dans le fichier xml des unités.
        /// </summary>
        /// TODO Doit on renvoyer un null en cas d'échec ???</TODO>
        public Units getUnitsFromXML(string xmlFileAbsolutePath)
        {
            // Introducing a new file
            if (xmlFileAbsolutePath != "")
            {
                if (!setInputFileAbsolutePath(xmlFileAbsolutePath))
                {
                    return null;
                }
            }
            return getUnitsFromXML();
            /*

            //Units units = new Units();
            units = getUnitsFromXML();

            // Déclenche la lecture du fichier
            if (readXmlFile() != FileIO.FILEOP_SUCCESSFUL) { return null; }

            // Node principal
            XElement mainElement = xDoc.Element(NODE_UNITS);

            foreach (XElement xe in mainElement.Elements())
            {
                if (xe.Name.LocalName == NODE_DIMENSION)
                {
                    // This is a DIMENSION node
                    string dimension = xe.Value;
                    if (dimension != null && dimension != "")
                    {
                        foreach (XElement item in xe.Elements())
                        {
                            if (item.Name.LocalName == NODE_UNIT)
                            {
                                // This is a UNIT node, let's get the data
                                string unitName = item.Attribute(ATTRIB_NAME).Value;
                                bool isRef = getBoolean(item.Attribute(UNIT_ATTR_ISREF).Value, false);
                                string alias = item.Attribute(UNIT_ATTR_ALIAS).Value;
                                double factor = getDouble(item.Attribute(UNIT_ATTR_FACTOR).Value);
                                double constant = getDouble(item.Attribute(UNIT_ATTR_CONST).Value);
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
            }
            return units;
            */
        }
        public Units getUnitsFromXML()
        {
            Units units = new Units();

            // Déclenche la lecture du fichier
            // TODO Faut-il vraiment déclencher la lecture ??? cela devrait être fait par le constructeur qui reçoit 
            //      le path du fichier....
            if (readXmlFile() != FileIO.FILEOP_SUCCESSFUL) { return null; }

            // Node principal des unités
            XElement mainElement = xDoc.Element(NODE_UNITS);

            foreach (XElement xe in mainElement.Elements())
            {
                if (xe.Name.LocalName == NODE_DIMENSION)
                {
                    // This is a DIMENSION node
                    string dimension = xe.Value;
                    if (dimension != null && dimension != "")
                    {
                        foreach (XElement item in xe.Elements())
                        {
                            if (item.Name.LocalName == NODE_UNIT)
                            {
                                // This is a UNIT node, let's get the data
                                string unitName = item.Attribute(ATTRIB_NAME).Value;
                                bool isRef = getBoolOrDefault(item.Attribute(UNIT_ATTR_ISREF).Value, false);
                                string alias = item.Attribute(UNIT_ATTR_ALIAS).Value;
                                double factor = getDoubleOrNan(item.Attribute(UNIT_ATTR_FACTOR).Value);
                                double constant = getDoubleOrNan(item.Attribute(UNIT_ATTR_CONST).Value);
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
            return new XElement(NODE_UNIT, new XAttribute(UNIT_ATTR_NAME, u.name),
                                           new XAttribute(UNIT_ATTR_ISREF, isref),
                                           new XAttribute(UNIT_ATTR_ALIAS, u.alias),
                                           new XAttribute(UNIT_ATTR_FACTOR, u.factor),
                                           new XAttribute(UNIT_ATTR_CONST, u.constant));
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
