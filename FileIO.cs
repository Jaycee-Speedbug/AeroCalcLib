using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;



namespace AeroCalcCore {



    /// <summary>
    /// Classe abstraite définissant les services de base offerts par les Objets dérivés 
    /// spécialisés dans la connexion aux fichiers de données textes/XML/JSON...
    /// </summary>
    /// 
    public abstract class FileIO {

        /*
         * CONSTANTES
         */ 

        // Constantes liées aux opérations sur les fichiers
        public const int FILEOP_SUCCESSFUL = 1;


        //   T:System.NotSupportedException:
        //     path is in an invalid format.
        public const int FILEOP_UNKNOWN_ERROR = -1;


        public const int FILEOP_UNABLE_TO_CREATE_OUTPUT_FILE = -2;


        //   T:System.IO.IOException:
        //     An I/O error occurred while opening the file.
        public const int FILEOP_IO_ERROR = -4;


        //   T:System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one or more
        //     invalid characters as defined by System.IO.Path.InvalidPathChars.
        //   T:System.ArgumentNullException:
        //     path is null.
        //   T:System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum length.
        //   T:System.IO.DirectoryNotFoundException:
        //     The specified path is invalid (for example, it is on an unmapped drive).
        public const int FILEOP_INVALID_PATH = -6;


        //   T:System.UnauthorizedAccessException:
        //     path specified a file that is read-only. -or- This operation is not supported
        //     on the current platform. -or- path specified a directory. -or- The caller does
        //     not have the required permission.
        //   T:System.Security.SecurityException:
        //     The caller does not have the required permission.
        public const int FILEOP_INPUT_FILE_IS_LOCKED = -7;


        //   T:System.IO.FileNotFoundException:
        //     The file specified in path was not found.
        public const int FILEOP_FILE_DOES_NOT_EXIST = -8;


        // Constantes des types de fichiers gérés
        public const int FILE_TYPE_UNKNOWN = 0;
        public const int FILE_TYPE_XML = 1;
        public const int FILE_TYPE_CSV = 2;
        public const int FILE_TYPE_TXT = 3;
        public const int FILE_TYPE_JSON = 4;
        // Constantes de nature des fichiers gérés
        public const int FILE_TYPE_CONFIG = 100;
        public const int FILE_TYPE_DATA_MODEL = 200;
        public const int FILE_TYPE_UNITS = 300;
        public const int FILE_TYPE_SCRIPT = 400;

        // Constantes des extensions des noms de fichier
        public const string FILENAME_EXTENSION_XML = ".xml";
        public const string FILENAME_EXTENSION_CSV = ".csv";
        public const string FILENAME_EXTENSION_SCRIPT = ".acsct";
        public const string FILENAME_EXTENSION_JSON = ".json";

        // Constantes utilisées dans les fichiers textes
        private const char IDENTIFIER_COMMENT_LINE = '#';



        /*
         * MEMBRES
         */

        /// <summary>
        /// Tableau contenant les lignes du fichier texte
        /// </summary>
        protected string[] rawFileLines;

        /// <summary>
        /// List des lignes du fichier texte, après traitement
        /// </summary>
        protected List<String> fileLines;


        /*
         * PROPRIETES
         */ 

        /// <summary>
        /// Chemin du répertoire contenant les fichiers utilisés
        /// </summary>
        public string directoryAbsolutePath { get; protected set;}

        /// <summary>
        /// Chemin du fichier utilisé en entrée
        /// </summary>
        public string inputFileAbsolutePath { get; protected set; }

        /// <summary>
        /// Chemin du fichier utilisé en sortie
        /// </summary>
        public string outputFileAbsolutePath { get; protected set; }



        /*
         * CONSTRUCTEURS
         */

        public FileIO() {
            directoryAbsolutePath = "";
            inputFileAbsolutePath = "";
            outputFileAbsolutePath = "";
        }

        /// <summary>
        /// Construit un FileConnector et prend en charge un répertoire de travail
        /// </summary>
        /// <param name="workDirectoryPath">Chemin absolu du répertoire de travail</param>
        /// 
        public FileIO(string workDirectoryPath) {
            setWorkDirectory(workDirectoryPath);
            inputFileAbsolutePath = "";
            outputFileAbsolutePath = "";
        }

        /// <summary>
        /// Construit un FileConnector et prend en charge un répertoire de travail et un fichier de sortie
        /// </summary>
        /// <param name="workDirectoryPath">Chemin absolu du répertoire de travail</param>
        /// <param name="inputFileAbsolutePath">Chemin absolu du fichier d'entrée</param>
        /// 
        public FileIO(string workDirectoryPath, string inputFileAbsolutePath) {
            setWorkDirectory(workDirectoryPath);
            setInputFileAbsolutePath(inputFileAbsolutePath);
            outputFileAbsolutePath = "";
        }

        /// <summary>
        /// Construit un FileConnector et prend en charge un répertoire de travail, un fichier de sortie
        /// et un fichier de sortie
        /// </summary>
        /// <param name="workDirectoryPath">Chemin absolu du répertoire de travail</param>
        /// <param name="inputFileAbsolutePath">Chemin absolu du fichier d'entrée</param>
        /// <param name="outputFileAbsolutePath">Chemin absolu du fichier de sortie</param>
        /// 
        public FileIO(string workDirectoryPath,
                             string inputFileAbsolutePath, 
                             string outputFileAbsolutePath) {
            setWorkDirectory(workDirectoryPath);
            setInputFileAbsolutePath(inputFileAbsolutePath);
            setOutputFileAbsolutePath(outputFileAbsolutePath);
        }



        /*
         *  SERVICES 
         */

        /// <summary>
        /// Remplace le tableau de String habituellement constitué par lecture d'un fichier par un tableau
        /// de String fourni en argument
        /// </summary>
        /// <param name="lines">Tableau de String</param>
        /// 
        public void setFileLines(string[] lines) {
            int count = 0;
            while (count < lines.Length) {
                fileLines.Add(lines[count]);
                count++;
            }
        }



        /// <summary>
        /// Retourne une liste des noms de fichiers trouvés dans le répertoire passé en argument,
        /// répondants au critère de filtre passé en argument
        /// </summary>
        /// <param name="folderAbsolutePath">Répertoire ou chercher les fichiers, s'il est différent du
        /// répertoire de travail</param>
        /// <param name="fileNameFilter">Filtre des noms de fichiers</param>
        /// <returns></returns>
        /// 
        public IEnumerable<string> filesList(string folderAbsolutePath, string fileNameFilter) {

            if (folderAbsolutePath == "") {
                folderAbsolutePath = directoryAbsolutePath;
            }
            if (Directory.Exists(folderAbsolutePath)) {
                return Directory.EnumerateFiles(folderAbsolutePath, fileNameFilter, SearchOption.AllDirectories);
            }
            else {
                return new List<string>();
            }
        }



        /// <summary>
        /// Enregistre la chaine passée en argument comme répertoire de travail (s'il existe dans la structure
        /// de fichiers) et retourne True. Renvoie False si le répertoire n'existe pas
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public bool setWorkDirectory(string absolutePath) {
            if (!string.IsNullOrEmpty(absolutePath)) {
                if (Directory.Exists(absolutePath)) {
                    // Le Directory proposé existe, on l'enregistre en temps que Directory de travail
                    directoryAbsolutePath = absolutePath;
                    return true;
                }
            }
            directoryAbsolutePath = "";
            return false;
        }



        /// <summary>
        /// Enregistre un chemin absolu comme fichier d'entrée
        /// </summary>
        /// <param name="absolutePath">Chemin absolu du fichier</param>
        /// <returns>True en cas de succès, False sinon.</returns>
        /// <remarks>
        /// 
        /// </remarks>
        /// 
        public bool setInputFileAbsolutePath(string absolutePath) {
            if (!string.IsNullOrEmpty(absolutePath)) {
                if (File.Exists(absolutePath)) {
                    // Le fichier proposé existe, on l'enregistre en temps que fichier d'entrée
                    inputFileAbsolutePath = absolutePath;
                    return true;
                }
            }
            inputFileAbsolutePath = "";
            return false;
        }



        /// <summary>
        /// Enregistre un chemin absolu comme fichier de sortie
        /// </summary>
        /// <param name="absolutePath">Chemin absolu du fichier</param>
        /// <returns>True en cas de succès, False sinon.</returns>
        /// <remarks>
        /// 
        /// </remarks>
        /// 
        public bool setOutputFileAbsolutePath(string absolutePath) {
            if (!string.IsNullOrEmpty(absolutePath)) {
                if (File.Exists(absolutePath)) {
                    // Le fichier proposé existe, on l'enregistre en temps que fichier d'entrée
                    outputFileAbsolutePath = absolutePath;
                    return true;
                }
            }
            outputFileAbsolutePath = "";
            return false;
        }



        /*
         *  METHODS
         */
        
        /// <summary>
        /// Retourne un int désignant le type de fichier identifié, sur la simple base de l'extension de nom
        /// </summary>
        /// <param name="fileName">Nom du fichier</param>
        /// <returns>
        /// Constante int
        /// </returns>
        /// <remarks>
        /// DEBUG: A améliorer pour inclure des vérifications plus poussées
        /// </remarks>
        /// 
        protected int fileType(string fileName) {

            string fileExt = fileName.Substring(fileName.LastIndexOf(".") + 1, fileName.Length);
            if (fileExt.Equals(FILENAME_EXTENSION_XML, StringComparison.InvariantCultureIgnoreCase)) {
                return FILE_TYPE_XML;
            }
            if (fileExt.Equals(FILENAME_EXTENSION_CSV, StringComparison.InvariantCultureIgnoreCase)) {
                return FILE_TYPE_CSV;
            }
            if (fileExt.Equals(FILENAME_EXTENSION_SCRIPT, StringComparison.InvariantCultureIgnoreCase)) {
                return FILE_TYPE_SCRIPT;
            }
            if (fileExt.Equals(FILENAME_EXTENSION_JSON, StringComparison.InvariantCultureIgnoreCase)) {
                return FILE_TYPE_JSON;
            }
            return FILE_TYPE_UNKNOWN;
        }



        /// <summary>
        /// Vérifie si le répertoire de travail existe 
        /// </summary>
        /// <returns>
        /// Bool, True si le répertoire existe, False sinon
        /// </returns>
        /// 
        protected bool workDirExists() {
            return Directory.Exists(directoryAbsolutePath);
        }



        /// <summary>
        /// Vérifie l'existance du fichier utilisé en entrée
        /// </summary>
        /// <returns>Tue si le fichier existe</returns>
        /// 
        protected bool inputFileExists() {
            return File.Exists(inputFileAbsolutePath);
        }



        /// <summary>
        /// Vérifie l'existance du fichier utilisé en entrée
        /// </summary>
        /// <returns>True si le fichier existe</returns>
        /// 
        protected bool outputFileExists() {
            return File.Exists(outputFileAbsolutePath);
        }



        /// <summary>
        /// Lit une fichier texte dans son intégralité et retourne un code signifiant le résultat
        /// </summary>
        /// <param name="absoluteFilePath">Chemin complet du fichier</param>
        /// <param name="commentFiltering">True pour filtrer les lignes de commentaires</param>
        /// <returns>Code d'erreur résultant de l'opération de lecture</returns>
        /// 
        protected int readTextFile(string absoluteFilePath, bool commentFiltering) {

            /* TODO à suppr
            // Vérification de la présence du fichier
            if (!File.Exists(absoluteFilePath)) {
                return FILEOP_FILE_DOES_NOT_EXIST;
            }
            */
            try {
                // Lecture de toutes les lignes du fichier
                rawFileLines = File.ReadAllLines(absoluteFilePath, Encoding.UTF8);
            }
            catch (Exception e) {

                if (e is ArgumentException) { return FILEOP_INVALID_PATH; }
                if (e is ArgumentNullException) { return FILEOP_INVALID_PATH; }
                if (e is PathTooLongException) { return FILEOP_INVALID_PATH; }
                if (e is DirectoryNotFoundException) { return FILEOP_INVALID_PATH; }

                if (e is IOException) { return FILEOP_IO_ERROR; }

                if (e is UnauthorizedAccessException) { return FILEOP_INPUT_FILE_IS_LOCKED; }
                if (e is System.Security.SecurityException) { return FILEOP_INPUT_FILE_IS_LOCKED; }

                if (e is System.IO.FileNotFoundException) { return FILEOP_FILE_DOES_NOT_EXIST; }

                if (e is NotSupportedException) { return FILEOP_UNKNOWN_ERROR; }
            }
            // Filtrage éventuel des commentaires du fichier source
            if (commentFiltering) {
                if (!filterComments(rawFileLines, fileLines)) {
                    return FILEOP_UNKNOWN_ERROR;
                }
            }
            return FILEOP_SUCCESSFUL;
        }



        /// <summary>
        /// Convertit la représentation sous forme de chaine d'un booléen en booléen.
        /// 0 ou False pour False,
        /// et 1 ou True pour True.
        /// </summary>
        /// <param name="word">String à convertir</param>
        /// <param name="result">Résultat de la conversion</param>
        /// <returns>True si la conversion a réussie, False dans le cas contraire</returns>
        /// 
        protected bool parseABoolean(string word, out bool result)
        {

            int parsedInt;

            if (!bool.TryParse(word, out result)) {
                // La conversion directe en booléen n'a pas fonctionné, on essaye de trouver les valeurs 0 ou 1
                if (int.TryParse(word, out parsedInt)) {
                    if (parsedInt == 0) {
                        result = false;
                        return true;
                    }
                    else if (parsedInt == 1) {
                        result = true;
                        return true;
                    }
                }
                result = false;
                return false;
            }
            else {
                return true;
            }
        }
        // Accesseur de test
        public bool __testParseABoolean(string word, out bool result)
        {
            return parseABoolean(word, out result);
        }



        /// <summary>
        /// Convertit la représentation texte d'un Double en Double.
        /// Assure une cohérence globale dans le traitement des chaines de caractères des fichiers
        /// textes utilisés comme source de données numériques en encapsulant Double.TryParse()
        /// </summary>
        /// <param name="word"></param>
        /// <param name="result"></param>
        /// <returns>True si la conversion a réussie, False dans le cas contraire</returns>
        /// 
        protected bool parseADouble(string word, out double result) {
            return Double.TryParse(word,
                                   System.Globalization.NumberStyles.Float,
                                   new CultureInfo("fr-FR"),
                                   out result);
        }
        /// Accesseur de test
        public bool __testParseADouble(string word, out double result)
        {
            return parseADouble(word, out result);
        }



        /// <summary>
        /// Expurge les lignes du fichier des commentaires
        /// </summary>
        /// <returns>True, si réussite</returns>
        /// 
        private bool filterComments(string[] originalLines, List<String> filteredLines) {

            int count = 0;
            int pos = -1;

            // Vider filteredLines
            filteredLines.Clear();
            // Lecture et filtrage des lignes
            while (count < originalLines.Length) {
                pos = originalLines[count].IndexOf(IDENTIFIER_COMMENT_LINE);
                if (pos > 0) {
                    // La ligne est partiellement commentée
                    filteredLines.Add(originalLines[count].Remove(pos));
                }
                else if (pos == 0) {
                    // La ligne commence par l'opérateur de commentaire
                }
                else {
                    // Pas d'occurence trouvée
                    filteredLines.Add(originalLines[count]);
                }
                count++;
            }
            return true;
        }

    }

}