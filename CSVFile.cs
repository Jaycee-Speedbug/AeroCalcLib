using System;
using System.Collections.Generic;
using System.Linq;



namespace AeroCalcCore
{


    /// <summary>
    /// Structure de données désignant les coordonnées d'une cellule dans un fichier CSV
    /// </summary>
    public struct CSVCoordinates
        {
        public int line;
        public int column;
        public CSVCoordinates(int l, int c) {
            line = l;
            column = c;
        }
    }



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

        // Constantes d'analyse des fichiers des modèles de calcul ou d'unités
        protected const char CELL_SEPARATOR_SEMICOLON = ';';
        protected const char CELL_SEPARATOR_TAB = '\t';
        protected const char CELL_SEPARATOR_COMMA = ',';
        protected const char CELL_SEPARATOR_PERIOD = '.';
        protected const char CELL_SEPARATOR_SPACE = ' ';

        // Constantes de balisage de la structure des données
        protected const string KWD_START_TABLE = "START_TABLE";
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
        public CSVFile() {
            // Défini le tableau de char contenant les séparateurs acceptés
            cellSeparator = new char[] { CELL_SEPARATOR_SEMICOLON, CELL_SEPARATOR_TAB };
            // Défini un tableau de String destiné à contenir les lignes du fichier CSV
            FileLines = new List<String>();
        }



        /*
         * SERVICES
         */

        /// <summary>
        /// Retourne une String présente dans la colonne fieldName, à la ligne fournie en argument
        /// dans un fichier texte CSV. Si l'argument line est -1, la ligne placée sous celle du nom de
        /// champ est utilisée.
        /// </summary>
        /// <remarks>Index en base 0</remarks>
        /// <param name="fieldName">Nom du champ au format texte</param>
        /// <param name="line">Numéro de ligne dans laquelle lire la chaine de caractères, ou -1.
        /// </param>
        /// <returns>
        /// String présente dans la même colonne que le fieldName, à la ligne désignée. Null si introuvable.
        /// </returns>
        /// 
        protected string ValueWithFieldName(string fieldName, int line)
        {

            int column = GetColumnIndex(fieldName);
            if (column > -1)
            {
                // Le champ a été trouvé, et le numéro de colonne est connu
                if (line < 0)
                {
                    // La ligne n'est pas définie, on prend la ligne sous celle du champ
                    line = GetLineIndex(fieldName);
                    if (line < 0)
                    {
                        return null;
                    }
                    // La ligne du champ a été trouvée
                    line++;
                }
                return ValueAtPosition(line, column);
            }
            return null;
        }
        // Accesseur de test
        public string _A_ValueWithFieldName(string fieldName, int line)
        {
            return ValueWithFieldName(fieldName, line);
        }



        /// <summary>
        /// Retourne la chaine de caractère à la position définie par les arguments
        /// Les index de ligne et colonne sont en base 0
        /// </summary>
        /// <remarks>CSV Structure V2</remarks>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns>String, à la position définie en arguments, Null si la colonne ou la ligne n'existe pas</returns>
        protected string ValueAtPosition(int line, int column) {
            if (line < FileLines.Count) {
                string[] subs;
                subs = FileLines[line].Split(cellSeparator, StringSplitOptions.None);
                if (column < subs.Length) {
                    return subs[column];
                }
            }
            return null;
        }
        // Accesseur de test
        public string _A_ValueAtPosition(int line, int column) {
            return ValueAtPosition(line, column);
        }



        /// <summary>
        /// Renvoie la String située dans la cellule à droite du keyword passé en argument
        /// </summary>
        /// <returns>String si existante, ou null si introuvable</returns> 
        /// <remarks>Permet de récupérer des paramètres dans un fichier CSV
        /// </remarks>
        /// <param name="keyword">string, Keyword à identifier</param>
        protected string StrRightOf(string keyword)
        {
            int line = GetLineIndex(keyword);
            int column = GetColumnIndex(keyword);

            if (line < 0 || column < 0) return null;
            return ValueAtPosition(line, column + 1);
        }
        // Accesseur de test
        public string _A_StrRightOf(string keyword)
        {
            return StrRightOf(keyword);
        }



        /// <summary>
        /// Renvoie l'index de la colonne identifiée par le keyword passé en argument
        /// (Les index de lignes et colonnes sont en base 0)
        /// </summary>
        /// <param name="keyword">string, Keyword identifiant la colonne</param>
        /// <returns>index de la colonne, si le keyword est trouvé, sinon -1</returns>
        /// 
        protected int GetColumnIndex(string keyword)
        {

            int lineOfInterest = GetLineIndex(keyword);
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
        }
        public int _A_GetColumnIndex(string keyword)
        {
            return GetColumnIndex(keyword);
        }



        /// <summary>
        /// Renvoie l'index de la ligne où apparait le keyword passé en argument
        /// </summary>
        /// <param name="keyword">string, keyword à identifier</param>
        /// <returns>index de la ligne du tableau contenant les lignes du fichier texte
        /// -1 si l'argument est null ou si l'argument est introuvable</returns>
        /// TODO : Tests
        protected int GetLineIndex(string keyword)
        {
            if (keyword==null || FileLines==null) return -1;
            try
            {
                int theLine = FileLines.FindIndex(line => line.Contains(keyword));
                return theLine;
            }
            catch (Exception) { return -1; }
        }
        public int _A_GetLineIndex(string keyword)
        {
            return GetLineIndex(keyword);
        }

    }

}