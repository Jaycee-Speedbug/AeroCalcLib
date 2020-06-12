using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security;
using System.Globalization;



namespace AeroCalcCore
{



    /// <summary>
    /// Classe abstraite définissant les services de base offerts par les Objets dérivés 
    /// spécialisés dans la connexion aux fichiers de données textes/XML/JSON...
    /// </summary>
    // <remarks>
    // TODO: Tests unitaires à mettre en place
    // </remarks>
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

        //   T:System.ArgumentOutOfRangeException
        //     A parameter of an operation is inapplicable
        public const int FILEOP_INAPPLICABLE_PARAMETER = -3;

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
        public const string FILENAME_EXTENSION_SCRIPT = ".accsc";
        public const string FILENAME_EXTENSION_JSON = ".json";

        // Constantes utilisées dans les fichiers textes
        private const char IDENTIFIER_COMMENT_LINE = '#';



        /*
         * MEMBRES
         */

        /// <summary>
        /// Tableau contenant les lignes originales du fichier texte
        /// </summary>
        private string[] rawFileLines;

        /// <summary>
        /// List<string> des lignes du fichier texte, après traitement
        /// </summary>
        protected List<string> FileLines;


        /*
         * PROPRIETES
         */

        /// <summary>
        /// Chemin du répertoire contenant les fichiers utilisés
        /// </summary>
        public string directoryAbsolutePath { get; protected set; }

        /// <summary>
        /// Chemin du fichier utilisé en entrée
        /// </summary>
        public string inputFileAbsolutePath { get; protected set; }

        /// <summary>
        /// Chemin du fichier utilisé en sortie
        /// </summary>
        public string outputFileAbsolutePath { get; protected set; }

        /// <summary>
        /// Recoit le code de la dernière opération IO (voir constantes FILEOP)
        /// </summary>
        public int IOStatus { get; protected set; }

        /// <summary>
        /// Permet de connaitre le nombre de lignes disponibles dans la List<string> FileLines 
        /// </summary>
        public int Count { get => (FileLines == null) ? -1 : FileLines.Count; }



        /*
         * CONSTRUCTEURS
         */
        // TODO Revoir les constructeurs, 1 argument: doit etre un inputfilepath et non un workdirectory

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
        public FileIO(string workDirectoryPath, string inputFileAbsolutePath, string outputFileAbsolutePath) {
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
                FileLines.Add(lines[count]);
                count++;
            }
        }



        /// <summary>
        /// Retourne une liste des noms de fichiers trouvés dans le répertoire passé en argument,
        /// répondants au critère de filtre passé en argument
        /// </summary>
        /// <param name="directoryAbsolutePath">Répertoire ou chercher les fichiers, s'il est différent du
        /// répertoire de travail</param>
        /// <param name="fileNameFilter">Filtre des noms de fichiers</param>
        /// <returns></returns>
        /// TODO Améliorer le traitement des exceptions
        public List<string> filesList(string directoryAbsolutePath, string fileNameFilter) {
            List<string> files = new List<string>();

            if (string.IsNullOrEmpty(directoryAbsolutePath)) {
                // No new Models directory, using the preset
                directoryAbsolutePath = this.directoryAbsolutePath;
            }
            if (Directory.Exists(directoryAbsolutePath)) {
                try {
                    // BUG cast impossible
                    var l = Directory.EnumerateFiles(directoryAbsolutePath,
                                                     fileNameFilter,
                                                     SearchOption.AllDirectories);
                    IOStatus = FILEOP_SUCCESSFUL;
                    foreach (string item in l) {
                        files.Add(item);
                    }
                }
                catch (Exception e) {
                    setIOStatus(e);
                }
            }
            return files;
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
                    // On réinitialise le status
                    IOStatus = FILEOP_SUCCESSFUL;
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
        /// No Exception to handle
        /// </remarks>
        /// 
        public bool setInputFileAbsolutePath(string absolutePath) {
            if (!string.IsNullOrEmpty(absolutePath)) {
                if (File.Exists(absolutePath)) {
                    // Le fichier proposé existe, on l'enregistre en temps que fichier d'entrée
                    inputFileAbsolutePath = absolutePath;
                    // On réinitialise le status
                    IOStatus = FILEOP_SUCCESSFUL;
                    return true;
                }
            }
            inputFileAbsolutePath = "";
            return false;
        }



        /// <summary>
        /// Constitue le chemin absolu du fichier d'entrée sur la base du répertoire de travail, fixé au préalable,
        ///  et du nom de fichier transmis en argument.
        /// </summary>
        /// <param name="relativePath">Chemin relatif du fichier, par rapport au répertoire de travail</param>
        /// <returns>True en cas de succès, False sinon.</returns>
        /// <remarks>
        /// No Exception to handle
        /// </remarks>
        /// 
        public bool setInputFileWithRelPath(string relativePath) {
            if (workDirExists()) {
                return setInputFileAbsolutePath(directoryAbsolutePath + Path.DirectorySeparatorChar + relativePath);
            }
            else return false;
        }



        /// <summary>
        /// Enregistre un chemin absolu comme fichier de sortie
        /// </summary>
        /// <param name="absolutePath">Chemin absolu du fichier</param>
        /// <returns>True en cas de succès, False sinon.</returns>
        /// <remarks>
        /// 
        /// </remarks>
        public bool setOutputFileAbsolutePath(string absolutePath) {
            if (!string.IsNullOrEmpty(absolutePath)) {
                if (File.Exists(absolutePath)) {
                    // Le fichier proposé existe, on l'enregistre en temps que fichier d'entrée
                    outputFileAbsolutePath = absolutePath;
                    // On réinitialise le status
                    IOStatus = FILEOP_SUCCESSFUL;
                    return true;
                }
            }
            outputFileAbsolutePath = "";
            return false;
        }



        /*
         *  METHODS
         */

        protected void setIOStatus(Exception fileOperationException) {
            // Invalid path
            if (fileOperationException is ArgumentException) { IOStatus = FILEOP_INVALID_PATH; }
            if (fileOperationException is ArgumentNullException) { IOStatus = FILEOP_INVALID_PATH; }
            if (fileOperationException is PathTooLongException) { IOStatus = FILEOP_INVALID_PATH; }
            if (fileOperationException is DirectoryNotFoundException) { IOStatus = FILEOP_INVALID_PATH; }
            // Inapplicable parameter
            if (fileOperationException is ArgumentOutOfRangeException) { IOStatus = FILEOP_INAPPLICABLE_PARAMETER; }
            // IO Error
            if (fileOperationException is IOException) { IOStatus = FILEOP_IO_ERROR; }
            // Security
            if (fileOperationException is UnauthorizedAccessException) { IOStatus = FILEOP_INPUT_FILE_IS_LOCKED; }
            if (fileOperationException is SecurityException) { IOStatus = FILEOP_INPUT_FILE_IS_LOCKED; }
            // Does not exist
            if (fileOperationException is FileNotFoundException) { IOStatus = FILEOP_FILE_DOES_NOT_EXIST; }
            // Unknown
            if (fileOperationException is NotSupportedException) { IOStatus = FILEOP_UNKNOWN_ERROR; }
        }



        /// <summary>
        /// Retourne un int figurant le type de l'extension du nom de fichier passé en argument
        /// </summary>
        /// <param name="fileName">Nom du fichier</param>
        /// <returns>
        /// Constante int négative en cas d'erreur, positive en cas d'identification réussie de l'extension du nom de fichier
        /// </returns>
        /// <remarks>
        /// </remarks>
        protected int fileType(string fileName) {
            string fileExt;
            if (!string.IsNullOrEmpty(fileName)) {
                try {
                    fileExt = Path.GetExtension(fileName);
                }
                catch (Exception e) {
                    setIOStatus(e);
                    return IOStatus;
                }
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
            }
            return FILE_TYPE_UNKNOWN;
        }



        /// <summary>
        /// Vérifie si le répertoire de travail existe 
        /// </summary>
        /// <returns>
        /// Bool, True si le répertoire existe, False sinon
        /// </returns>
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
            
            try {
                setInputFileAbsolutePath(absoluteFilePath);
                // Lecture de toutes les lignes du fichier
                rawFileLines = File.ReadAllLines(absoluteFilePath, Encoding.UTF8);
                IOStatus = FILEOP_SUCCESSFUL;
            }
            catch (Exception e) {
                setIOStatus(e);
                return IOStatus;
            }
            // Filtrage et recopie dans FileLines
            int count = 0;
            int pos = -1;
            List<string> outputLines = new List<string>();
            if (commentFiltering) {
                // Filtrage des commentaires et des lignes vides
                while (count < rawFileLines.Length) {
                    if (!string.IsNullOrEmpty(rawFileLines[count])) {
                        pos = rawFileLines[count].IndexOf(IDENTIFIER_COMMENT_LINE);
                        if (pos > 0) {
                            // La ligne est partiellement commentée
                            outputLines.Add(rawFileLines[count].Remove(pos));
                        }
                        else if (pos == 0) {
                            // La ligne commence par l'opérateur de commentaire
                        }
                        else {
                            // Pas d'occurence trouvée
                            outputLines.Add(rawFileLines[count]);
                        }
                    }
                    else {
                        // Null or Empty string
                    }
                    count++;
                }
            }
            else {
                // Filtrage des lignes vides uniquement
                while (count < rawFileLines.Length) {
                    if (!string.IsNullOrEmpty(rawFileLines[count])) {
                        outputLines.Add(rawFileLines[count]);
                    }
                    else {
                        // Null or Empty string
                    }
                }
                count++;
            }
            FileLines = outputLines;
            IOStatus = FILEOP_SUCCESSFUL;
            return IOStatus;
        }



        protected int checkFile(string absoluteFilePath) {
            try {
                File.GetAttributes(absoluteFilePath);
                IOStatus = FILEOP_SUCCESSFUL;
            }
            catch (Exception e) {
                setIOStatus(e);
            }
            return IOStatus;
        }



        protected int checkPath(string pathValue) {
            try {
                Path.GetFullPath(pathValue);
                IOStatus = FILEOP_SUCCESSFUL;
            }
            catch (Exception e) {
                setIOStatus(e);
            }
            return IOStatus;
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
        protected bool parseABoolean(string word, out bool result) {
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
        public bool __testParseABoolean(string word, out bool result) {
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
        public bool __testParseADouble(string word, out double result) {
            return parseADouble(word, out result);
        }



        /// <summary>
        /// Expurge les commentaires
        /// </summary>
        /// <returns>True, si réussite</returns>
        /// TODO A déplacer dans une classe supérieure, en charge des SCRIPTS et MODELS
        /*
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
        }*/

    }

}