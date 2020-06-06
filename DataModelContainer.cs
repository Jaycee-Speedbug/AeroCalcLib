using System;
using System.Collections.Generic;
using System.Linq;


namespace AeroCalcCore
{


    /// <summary>
    /// Classe définissant le container des modèles de performances
    /// Fourni l'accès à ces modèles et les outils permettant de charger en mémoire ces modèles depuis
    /// des fichiers ou des connexions à des bases de données
    /// </summary>
    /// <remarks>
    /// Améliorations possibles:
    /// - Utiliser une classe encapsulant les objets de type PerfPile
    /// - Ajouter l'interface IEquatable aux objets PerfPile (ou leur capsule) pour assurer
    /// de meilleures performances de recherche
    /// </remarks>
    public class DataModelContainer
    {

        /*
         * Propriétés
         */

        public Units UnitsLib { get; private set; }


        /*
         * Membres
         */

        List<PerfPile> dataModels;
        //ConnectorUnitCSVFile unitsConnector;
        XMLFile xmlConnector;
        ModelCSVFile csvConnector;

        private char[] commandSeparator = { ' ' };



        /*
         * Constructeurs
         */

        /// <summary>
        /// Construit un objet DataModelContainer, Container des modèles de performances de vol
        /// </summary>
        /// 
        public DataModelContainer()
        {

            dataModels = new List<PerfPile>();
            xmlConnector = new XMLFile();
            csvConnector = new ModelCSVFile();

        }



        /*
         * Services
         */

        /// <summary>
        /// Fixe le répertoire contenant les modèles de performances à charger.
        /// Retourne True en cas de succès, False dans le cas contraire.
        /// </summary>
        /// <param name="directoryPath">Chemin du répertoire</param>
        /// <returns>
        /// 
        /// </returns>
        public bool setDataModelsDirectory(string directoryPath)
        {
            return (csvConnector.setWorkDirectory(directoryPath) & xmlConnector.setWorkDirectory(directoryPath));
        }



        /// <summary>
        /// Retourne un tableau de string contenant les Directories des différents Connectors du Container
        /// </summary>
        /// <returns></returns>
        public string[] getDataModelsDirectory()
        {
            string[] directories = new string[2];
            directories[0] = this.csvConnector.directoryAbsolutePath;
            directories[1] = this.xmlConnector.directoryAbsolutePath;
            return directories;
        }



        public void setUnitsLibrary(Units UnitsLibrary)
        {
            UnitsLib = UnitsLibrary;
        }



        /// <summary>
        /// Retourne un modèle de performances de vol à la position index dans le container
        /// </summary>
        /// <param name="index">Index de position de ce modèle de performances dans le container</param>
        /// <returns>PerfPile contenant le modèle de performances</returns>
        /// 
        public PerfPile dataModelByIndex(int index)
        {
            return dataModels.ElementAt(index);
        }



        /*
        /// <summary>
        /// Retourne le dictionnaire des unités
        /// </summary>
        /// <returns>UnitDictionnary, le dictionnaire des unités</returns>
        /// 
        public UnitDictionary unitDictionary() {
            return dataUnits;
        }
        */



        /// <summary>
        /// Retourne le résultat d'un calcul multi-dimensionnel
        /// </summary>
        /// <param name="dataModelName">Nom du modèle à utiliser</param>
        /// <param name="factors">Liste des facteurs passés en argument</param>
        /// <returns>
        /// double, résultat du calcul
        /// </returns>
        /// <remarks>
        /// Fontion récursive
        /// </remarks>
        public double compute(string dataModelName, List<CommandFactor> factors)
        {

            // Recherche du modèle de performances permettant le traitement (nom + discret)
            //PerfPile Pile = dataModels.ElementAt(dataModelIndex(dataModelName, factors));
            PerfPile Pile = dataModels.Find(x => x.outputName == dataModelName);
            double result = double.NaN;

            //int commandIndex = container.dataModelIndex(Cmd.subs[0], Cmd.Factors);
            if (Pile == null)
            {
                // Aucun modèle ne répond aux critères de sélection
                return double.NaN;
            }


            // Recherche de la valeur des différents facteurs nécessaires aux calculs
            if (Pile != null)
            {
                // Le modèle est identifié
                //pp = container.dataModelByIndex(commandIndex);
                // Analyse des facteurs, paramètres et options de la commande

                //double pointFactorValue = factors.Find(x => x.name.Equals(Pile.pointFactorName)).value;
                double pointFactorValue = valueFromFactor(factors, Pile.pointFactorName);
                if (double.IsNaN(pointFactorValue))
                {
                    // serieFactorValue n'est pas disponible dans les facteurs, on tente de le calculer
                    pointFactorValue = compute(Pile.pointFactorName, factors);
                    if (double.IsNaN(pointFactorValue))
                    {
                        // Echec de la tentative du calcul
                        return double.NaN;
                    }
                }

                //double serieFactorValue = factors.Find(x => x.name.Equals(Pile.serieFactorName)).value;
                double serieFactorValue = valueFromFactor(factors, Pile.serieFactorName);
                if (double.IsNaN(serieFactorValue))
                {
                    // serieFactorValue n'est pas disponible dans les facteurs, on tente de le calculer
                    serieFactorValue = compute(Pile.serieFactorName, factors);
                    if (double.IsNaN(serieFactorValue))
                    {
                        // Echec de la tentative du calcul
                        return double.NaN;
                    }
                }

                //double layerFactorValue = factors.Find(x => x.name.Equals(Pile.layerFactorName)).value;
                double layerFactorValue = valueFromFactor(factors, Pile.layerFactorName);
                if (double.IsNaN(layerFactorValue))
                {
                    // layerFactorValue n'est pas disponible dans les facteurs, on tente de le calculer
                    serieFactorValue = compute(Pile.layerFactorName, factors);
                    if (double.IsNaN(layerFactorValue))
                    {
                        // Echec de la tentative du calcul
                        return double.NaN;
                    }
                }

                try
                {
                    result = Pile.predict(pointFactorValue, serieFactorValue, layerFactorValue);
                }
                catch (ModelException e)
                {
                    // On récupère d'une exception ou un paramètre est hors du range ou il peut être utilisé
                    // TODO les infos de l'exception sont générées directement dans predict()
                    throw;
                }
            }
            return result;
        }



        /// <summary>
        /// Renvoie l'index (clef) du modèle de performances de vol, identifié par le nom
        /// fournis en argument.
        /// Renvoie -1 si aucune correspondance, une valeur inférieure à -1 si plusieurs correspondances trouvées
        /// </summary>
        /// <param name="dataModelName">Nom du modèle de performances</param>
        /// <returns>L'index de la fonction de calcul de performances</returns>
        /// 
        public int dataModelIndex(string dataModelName)
        {
            int foundIndex = -1;
            for (int count = 0; count < this.dataModels.Count; count++)
            {
                if (dataModelName.Equals(dataModels.ElementAt(count).outputName))
                {
                    if (foundIndex > -1)
                    {
                        // Une autre occurence a déjà été découverte !
                        foundIndex = -2;
                    }
                    else if (foundIndex == -1)
                    {
                        // Première occurence !
                        foundIndex = count;
                    }
                    else
                    {
                        // Déjà plusieurs occurences
                    }
                    foundIndex--;
                }
            }
            return foundIndex;
        }



        /// <summary>
        /// Renvoie l'index (clef) du premier modèle de performances de vol, identifié par son nom et le nom
        /// du discret fournis en argument.
        /// Retourne -1 si aucune correspondance n'est trouvée, un entier inférieur à -1 si plusieurs occurences
        /// sont trouvées.
        /// </summary>
        /// <param name="dataModelName">Nom du modèle de performances</param>
        /// <param name="discretName">Nom du paramètre discret associé au modèle</param>
        /// <returns>L'index de la fonction de calcul de performance</returns>
        /// 
        public int dataModelIndex(string dataModelName, string discretName)
        {
            int foundIndex = -1;
            if (discretName.Equals(""))
            {
                // Pas de nom de paramètre discret, on ne considère que le nom du modèle
                return dataModelIndex(dataModelName);
            }
            else
            {
                // Un nom de paramètre discret est fourni, on en tient compte dans la recherche
            }
            for (int count = 0; count < this.dataModels.Count; count++)
            {
                if (dataModelName.Equals(dataModels.ElementAt(count).outputName) &&
                    discretName.Equals(dataModels.ElementAt(count).discretName))
                {
                    if (foundIndex > -1)
                    {
                        // Une autre occurence a déjà été découverte !
                        foundIndex = -2;
                    }
                    else if (foundIndex == -1)
                    {
                        // Première occurence !
                        foundIndex = count;
                    }
                    else
                    {
                        // Déjà plusieurs occurence
                    }
                    foundIndex--;
                }
            }
            return foundIndex;
        }



        /// <summary>
        /// Renvoie l'index (clef) du premier modèle de performances de vol, identifié par le nom, le discret
        /// et sa valeur fournis en argument.
        /// Retourne -1 si aucune correspondance n'est trouvée, un entier inférieur à -1 si plusieurs occurences
        /// sont trouvées.
        /// </summary>
        /// <param name="dataModelName">Nom du modèle de performances</param>
        /// <param name="discretName">Nom du paramètre discret associé au modèle</param>
        /// <param name="discretValue">Valeur discrète associée au modèle</param>
        /// <returns>L'index de la fonction de calcul de performance</returns>
        /// 
        public int dataModelIndex(string dataModelName, string discretName, long discretValue)
        {
            int foundIndex = -1;
            if (discretName.Equals(""))
            {
                // Pas de nom de paramètre discret, on ne considère que le nom du modèle
                return dataModelIndex(dataModelName);
            }
            else
            {
                // Un nom de paramètre discret est fourni, on en tient compte dans la recherche
            }
            for (int count = 0; count < this.dataModels.Count; count++)
            {
                if (dataModelName.Equals(dataModels.ElementAt(count).outputName) &&
                    discretName.Equals(dataModels.ElementAt(count).discretName) &&
                    discretValue == dataModels.ElementAt(count).discretValue)
                {
                    if (foundIndex > -1)
                    {
                        // Une autre occurence a déjà été découverte !
                        foundIndex = -2;
                    }
                    else if (foundIndex == -1)
                    {
                        // Première occurence !
                        foundIndex = count;
                    }
                    else
                    {
                        // Déjà plusieurs occurence
                    }
                    foundIndex--;
                }
            }
            return foundIndex;
        }



        /// <summary>
        /// Renvoie l'index (clef) du premier modèle de performances de vol, identifié par le nom, le discret 
        /// et sa valeur fournis en argument, dans tableau de facteurs.
        /// Retourne -1 si aucune correspondance n'est trouvée, un entier inférieur à -1 si plusieurs occurences
        /// sont trouvées.
        /// </summary>
        /// <param name="dataModelName">Nom du modèle de performances</param>
        /// <param name="Factors">Tableau des facteurs passés en argument, dont un peut être le discret</param>
        /// <returns>L'index de la fonction de calcul de performances</returns>
        /// 
        public int dataModelIndex(string dataModelName, List<CommandFactor> Factors)
        {
            int foundIndex = -1;

            // Examen de tous les modèles de données contenus dans le container
            for (int count = 0; count < this.dataModels.Count; count++)
            {
                if (dataModelName.Equals(dataModels.ElementAt(count).outputName))
                {
                    // Le nom du modèle a été trouvé
                    foreach (CommandFactor factor in Factors)
                    {
                        // Recherche, pour chaque facteur, le nom du discret et sa valeur dans les caractéristiques
                        if (factor.name.Equals(dataModels.ElementAt(count).discretName) &&
                            factor.value == (double)dataModels.ElementAt(count).discretValue)
                        {
                            // Le nom du modèle, le nom du discret et la valeur du discret matchent !
                            if (foundIndex > -1)
                            {
                                // Une autre occurence a déjà été découverte !
                                foundIndex = -2;
                            }
                            else if (foundIndex == -1)
                            {
                                // Première occurence !
                                foundIndex = count;
                            }
                            else
                            {
                                // Déjà plusieurs occurence
                                foundIndex--;
                            }
                        }
                    }
                }
            }
            return foundIndex;
        }



        /// <summary>
        /// Renvoie un tableau des index des modèles de performances de vol contenus dans le container,
        /// dont le nom match le filtre passé en argument
        /// </summary>
        /// <param name="fileNameFilter">Chaine de caractère utilisée comme filtre</param>
        /// <returns>Tableau contenant les indexes des fonctions</returns>
        /// <remarks>
        /// Le délimiteur standard des noms de fonction est le point
        /// Le caractère * peut être utilisé comme carte blanche
        /// </remarks>
        /// 
        public List<int> dataModelIndexes(string fileNameFilter)
        {

            string[] functionNameSubs;
            string[] filterSubs;
            string[] originalFilterSubs;
            bool match;
            List<int> perfFunctionIndexes = new List<int>();

            char[] splitters = { AeroCalcCommand.CMD_OPERATOR_SPLITTER };
            // Découpe du nom du filtre
            originalFilterSubs = fileNameFilter.Split(splitters, StringSplitOptions.None);

            // Analyse de chaque outputName
            foreach (PerfPile pp in dataModels)
            {

                // Découpe du nom de fonction et du filtre
                functionNameSubs = pp.outputName.Split(splitters, StringSplitOptions.None);
                // Reset des mots du filtre
                filterSubs = originalFilterSubs;

                if (originalFilterSubs.Length > functionNameSubs.Length)
                {
                    // Plus de mots dans le filtre que dans le nom de la fonction, ça ne peut pas matcher
                    continue;
                }

                if (originalFilterSubs.Length < functionNameSubs.Length)
                {
                    if (fileNameFilter.Contains(AeroCalcCommand.CMD_WORD_WHITE_CARD))
                    {
                        // Le filtre comporte moins de mots que le nom de la fonction, mais au moins une WHITE_CARD
                        // Modification des mots du filtre grâce à la WHITE_CARD
                        filterSubs = expendFilter(originalFilterSubs, functionNameSubs.Length);
                    }
                    else
                    {
                        // Ca ne peut pas matcher, on passe à la suite
                        continue;
                    }
                }
                // filterSubs comporte maintenant le même nombre de mots que le nom de la fonction
                match = false;
                for (int index = 0; index < functionNameSubs.Length; index++)
                {
                    if (filterSubs[index].Contains(AeroCalcCommand.CMD_WORD_WHITE_CARD) ||
                        filterSubs[index].Equals(functionNameSubs[index], StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Ca match !
                        match = true;
                    }
                    else
                    {
                        // Ce mot ne match pas, pas la peine d'aller plus loin
                        match = false;
                        break;
                    }
                }
                if (match == true)
                {
                    // Le nom de cette fonction match le filtre
                    perfFunctionIndexes.Add(dataModels.IndexOf(pp));
                }
            }
            return perfFunctionIndexes;
        }



        /// <summary>
        /// Renvoie un tableau des index des modèles de performances de vol contenus dans le container,
        /// dont le nom match le filtre passé en argument
        /// </summary>
        /// <param name="fileNameFilter">Chaine de caractère utilisée comme filtre</param>
        /// <param name="Factors">Tableau des facteurs passés en argument, dont un peut être le discret</param>
        /// <returns>Tableau contenant les indexes des fonctions</returns>
        /// <remarks>
        /// Surcharge sur la base de List<int> dataModelIndexes(string fileNameFilter)
        /// </remarks>
        /// 
        public List<int> dataModelIndexes(string fileNameFilter, List<CommandFactor> Factors)
        {

            List<int> newIndexes = new List<int>();
            foreach (int index in dataModelIndexes(fileNameFilter))
            {
                foreach (CommandFactor factor in Factors)
                {
                    // Recherche, pour chaque facteur, le nom du discret et sa valeur dans les caractéristiques
                    if (factor.name.Equals(dataModels.ElementAt(index).discretName) &&
                        factor.value == (double)dataModels.ElementAt(index).discretValue)
                    {
                        // Le nom du discret et la valeur du discret matchent !
                        newIndexes.Add(index);
                        break;
                    }
                }
            }
            return newIndexes;
        }



        /// <summary>
        /// Charge les modèles de performances de vol en appliquant le filtre passé en argument et retourne
        /// le nombre de modèles chargés.
        /// </summary>
        /// <param name="dataModelNameFilter">Filtre des noms de modèles de performances à sélectionner</param>
        /// <returns>int, nombre de modèles chargés</returns>
        /// 
        public int loadDataModels(string dataModelNameFilter)
        {

            PerfPile pp;
            int counter = 0;

            // Recherche des fichiers à analyser dans le dossier des modèles (par défaut {App}/data )
            foreach (string fileName in csvConnector.filesList("", string.Concat(dataModelNameFilter, ".csv")))
            {
                pp = csvConnector.readFile(fileName);
                if (pp != null)
                {
                    if (dataModelIndex(pp.outputName) < 0)
                    {
                        // Ce modèle n'est pas déjà enregistré, on peut l'ajouter à la liste
                        dataModels.Add(pp);
                        if (!pp.hidden)
                        {
                            counter++;
                        }
                    }
                }
            }
            return counter;
        }



        /// <summary>
        /// Renvoie, pour affichage, les signatures de tous les modèles de performances
        /// enregistré dans le container
        /// </summary>
        /// <returns>
        /// signatures au format texte du modèle FUNCTION_NAME PARAM1 PARAM2 ...
        /// </returns>
        /// <remarks>
        /// TODO counter est un int, attention au dépassement de capacité en production
        /// </remarks>
        /// 
        public string dataModelSignatures()
        {
            string msg = "";
            int counter = 0;

            foreach (PerfPile pp in this.dataModels)
            {
                if (!pp.hidden)
                {
                    msg += modelSignature(pp.outputName) + "\n";
                    counter++;
                }
            }
            msg += String.Format("{0} modèles chargés en mémoire\n", counter);
            return msg;
        }



        /// <summary>
        /// Retourne une string contenant toutes les Pile en mémoire
        /// </summary>
        /// <returns>
        /// </returns>
        public override string ToString()
        {

            string msg = "";

            foreach (PerfPile pp in dataModels)
            {
                msg += pp.ToString();
            }
            return msg;
        }



        /*
         * METHODES
         */

        /// <summary>
        /// Retourne une string contenant la signature du modèle de calcul
        /// </summary>
        /// <param name="outputName"></param>
        /// <returns>
        /// </returns>
        private string modelSignature(string outputName)
        {
            // Recherche du nom du modèle dans la liste dataModels
            PerfPile p = this.dataModels.Find(x => x.outputName.Equals(outputName));
            if (p != null)
            {
                List<String> factorList = new List<string>();
                string factors = "";
                foreach (string word in factorsSignature(outputName).Split(commandSeparator))
                {
                    if (!string.IsNullOrEmpty(word) && factorList.Find(x => x.Equals(word)) == null)
                    {
                        // On ajoute que les noms distincts des différents facteurs 
                        factorList.Add(word);
                    }
                }
                foreach (string word in factorList)
                {
                    factors += word + " ";
                }
                return p.outputName + " " + factors;
            }
            else
            {
                // Aucun modèle de donnée ne correspond à ce nom
                return "";
            }
            // Suppression des doublons dans les facteurs

        }


        /// <summary>
        /// Retourne une string contenant les noms des facteurs d'un modèle de calcul
        /// </summary>
        /// <param name="outputName">Nom du modèle de calcul</param>
        /// <returns>
        /// </returns>
        private string factorsSignature(string outputName)
        {
            PerfPile p = this.dataModels.Find(x => x.outputName == outputName);
            string signature;

            if (p != null)
            {
                // Le modèle existe
                signature = factorsSignature(p.discretName) + " ";
                signature += factorsSignature(p.layerFactorName) + " ";
                signature += factorsSignature(p.serieFactorName) + " ";
                signature += factorsSignature(p.pointFactorName) + " ";
                return signature;
            }
            else
            {
                return outputName;
            }
        }


        /// <summary>
        /// Retourne la valeur numérique contenue dans un facteur dont le nom est donné en argument.
        /// Renvoie NaN si aucun facteur n'est reconnu dans la liste fournie
        /// Renvoie 1 si le nom est une chaine vide (absence de facteur veut dire facteur = 1)
        /// </summary>
        /// <param name="list">Liste des commandFactor</param>
        /// <param name="name">Nom du facteur</param>
        /// <returns></returns>
        private double valueFromFactor(List<CommandFactor> list, string name)
        {
            if (name.Equals("")) { return 1; }
            foreach (CommandFactor factor in list)
            {
                if (factor.name.Equals(name))
                {
                    return factor.value;
                }
            }
            return double.NaN;
        }


        /// <summary>
        /// Transforme un filtre de nom de fonction pour comporter le nombre de mots passé en argument
        /// Permet l'utilisation des caractères WHITE_CARD dans les filtres
        /// Plusieures WHITE_CARD autorisées
        /// 
        /// </summary>
        /// <param name="filterSubs">Table de mots composant le filtre</param>
        /// <param name="length">Nombre de mots que doit comporter le filtre</param>
        /// <returns>
        /// Une nouvelle table de mots composant le nouveau filtre
        /// </returns>
        /// <remarks>
        /// Format supporté:
        /// *.WORD
        /// WORD.*
        /// WORD.*.WORD
        /// 
        /// TODO Refactoring:
        /// Utiliser les List<string> 
        /// </remarks>
        /// 
        private string[] expendFilter(string[] filterSubs, int wordCount)
        {

            int index = -1;
            bool expended = false;
            string[] expendedFilterSubs = new string[wordCount];

            // Recherche de la position de la WHITE_CARDS
            for (int count = 0; count < filterSubs.Length; count++)
            {
                if (filterSubs[count].Contains(AeroCalcCommand.CMD_WORD_WHITE_CARD) && index == -1)
                {
                    // Première occurence de la WHITE_CARD
                    index = count;
                    break;
                }
            }
            // Expension du filtre
            if (index >= 0 && index <= wordCount - 1)
            {
                // La position de la première WHITE_CARD est repérée
                int filterSubsCount = 0;
                int count = 0;
                while (count < wordCount)
                {
                    // Recopie en commençant par le début, et complément des mots manquants par une/des WHITE_CARD
                    if (filterSubs[filterSubsCount].Contains(AeroCalcCommand.CMD_WORD_WHITE_CARD))
                    {
                        // Insertion(s) de WHITE_CARD
                        for (int lcount = 0; lcount <= wordCount - filterSubs.Count(); lcount++)
                        {
                            expendedFilterSubs[count] = string.Concat(AeroCalcCommand.CMD_WORD_WHITE_CARD);
                            count++;
                            // L'expension a été réalisée, elle n'est autorisée qu'une seule fois
                            // à la première occurence de white card
                            if (expended) { break; }
                        }
                        expended = true;
                    }
                    else
                    {
                        expendedFilterSubs[count] = filterSubs[filterSubsCount];
                        count++;
                    }
                    filterSubsCount++;
                }
            }
            else
            {
                // La position de la WHITE_CARD n'a pas été trouvée
                return null;
            }
            return expendedFilterSubs;
        }
        // Accesseurs de tests unitaires
        public string[] accessor_expendFilter(string[] filterSubs, int wordCount)
        {
            return expendFilter(filterSubs, wordCount);
        }

    }

}