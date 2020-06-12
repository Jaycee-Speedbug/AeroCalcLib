using System;
using System.Collections.Generic;
using System.Linq;



namespace AeroCalcCore
{



    /// <summary>
    /// Classe abstraite permettant de construire une classe destinée à l'accès aux données
    /// textes contenues dans un fichier au format CSV.
    /// </summary>
    /// 
    abstract public class CSVFile : FileIO
    {


        /*
         * CONSTANTES
         */

        // Constantes d'analyse des fichiers des modèles de calcul, d'unités
        protected const char CELL_SEPARATOR_SEMICOLON = ';';
        protected const char CELL_SEPARATOR_TAB = '\t';
        protected const char CELL_SEPARATOR_COMMA = ',';
        protected const char CELL_SEPARATOR_PERIOD = '.';
        protected const char CELL_SEPARATOR_SPACE = ' ';

        // Constantes de balisage de la structure des données
        protected const string KWD_END_TABLE = "END_TABLE";

        protected const string KWD_START_TABLE_1 = "TABLE_1";
        protected const string KWD_END_TABLE_1 = "END_TABLE_1";

        protected const string KWD_START_TABLE_2 = "TABLE_2";
        protected const string KWD_END_TABLE_2 = "END_TABLE_2";

        protected const string KWD_START_TABLE_3 = "TABLE_3";
        protected const string KWD_END_TABLE_3 = "END_TABLE_3";

        protected const string KWD_START_TABLE_4 = "TABLE_4";
        protected const string KWD_END_TABLE_4 = "END_TABLE_4";

        protected const string KWD_START_TABLE_5 = "TABLE_5";
        protected const string KWD_END_TABLE_5 = "END_TABLE_5";



        /*
         * MEMBRES 
         */

        /// <summary>
        /// Tableau des caractères utilisés en séparateurs
        /// </summary>
        protected char[] cellSeparator;



        /*
         * CONSTRUCTEUR
         */

        /// <summary>
        /// Constructeur de la classe
        /// 
        /// </summary>
        /// 
        public CSVFile()
        {
            // Défini le tableau de char contenant les séparateurs acceptés
            cellSeparator = new char[] { CELL_SEPARATOR_SEMICOLON, CELL_SEPARATOR_TAB };
            // Défini un tableau de String destiné à contenir les lignes du fichier CSV
            FileLines = new List<String>();
        }



        /*
         * SERVICES
         */

        /// <summary>
        /// Retourne une String présente dans la cellule désignée par le nom du champ et la ligne fournis
        /// en argument, dans un fichier texte CSV. Si l'argument ligne est -1, la ligne sous celle du nom de
        /// champ est utilisée.
        /// </summary>
        /// <remarks>CSV Structure V2</remarks>
        /// <param name="fieldName">Nom du champ au format texte</param>
        /// <param name="line">Numéro de ligne dans laquelle lire la chaine, ou -1.
        /// du nom de champ</param>
        /// <returns>String présente dans la collone du champ, à la ligne désignée.</returns>
        /// 
        protected string valueWithFieldName(string fieldName, int line)
        {

            int column = getColumnIndex(fieldName);
            if (column > -1)
            {
                // Le champ a été trouvé
                if (line < 0)
                {
                    line = getLineIndex(fieldName);
                }
                return valueAtPosition(line + 1, column);
            }
            return null;
        }
        // Accesseur de test
        public string testValueWithFieldName(string fieldName, int line)
        {
            return valueWithFieldName(fieldName, line);
        }



        /// <summary>
        /// Retourne la chaine de caractère à la position définie par les arguments
        /// Les index de lignes et colonnes sont en base 0
        /// </summary>
        /// <remarks>CSV Structure V2</remarks>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns>String, à la position définie en arguments</returns>
        protected string valueAtPosition(int line, int column)
        {
            if (line < FileLines.Count)
            {
                string[] subs;
                subs = FileLines[line].Split(cellSeparator, StringSplitOptions.None);
                if (column < subs.Length)
                {
                    return subs[column];
                }
            }
            return null;
        }
        // Accesseur de test
        public string testValueAtPosition(int line, int column)
        {
            return valueAtPosition(line, column);
        }



        /// <summary>
        /// Renvoie l'index de la colonne identifiée par le keyword passé en argument
        /// Les index de lignes et colonnes sont en base 0
        /// </summary>
        /// <param name="keyword">string, Keyword identifiant la colonne</param>
        /// <returns>index de la colonne, si le keyword est trouvé, sinon -1</returns>
        /// 
        protected int getColumnIndex(string keyword)
        {

            int lineOfInterest = getLineIndex(keyword);
            if (lineOfInterest >= 0)
            {
                string[] subs = FileLines[lineOfInterest].Split(cellSeparator, StringSplitOptions.None);
                for (int counter = 0; counter < subs.Length; counter++)
                {
                    if (subs[counter].Contains(keyword))
                    {
                        return counter;
                    }
                }
            }
            return -1;


            /*
            string[] subs;

            for (int count = 0; count < FileLines.Count; count++) {

                if (FileLines[count].Contains(keyword)) {
                    subs = FileLines[count].Split(cellSeparator, StringSplitOptions.None);
                    for (int counter = 0; counter < subs.Length; counter++) {
                        if (subs[counter].Contains(keyword)) {
                            return counter;
                        }
                    }
                }
            }
            return -1;
            */
        }



        /// <summary>
        /// Renvoie l'index de la ligne où apparait le keyword passé en argument
        /// </summary>
        /// <param name="keyword">string, keyword à identifier</param>
        /// <returns>index de la ligne du tableau contenant les lignes du fichier texte</returns>
        /// 
        protected int getLineIndex(string keyword)
        {

            return FileLines.FindIndex(line => line.Contains(keyword));


            /*
            foreach (string str in FileLines)
            {
                if (str.Contains(keyword)){
                    return FileLines.IndexOf(str);
                }
            }
            */

            /*
            for (int index = 0; index < FileLines.Count; index++) {
                try {
                    if (FileLines[index].Contains(keyword)) {
                        return index;
                    }
                } catch (ArgumentNullException e) {
                    return -1;
                }
            }
            return -1;
            */
        }

    }

}