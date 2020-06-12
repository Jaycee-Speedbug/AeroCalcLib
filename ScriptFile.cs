


namespace AeroCalcCore
{



    public class ScriptFile : FileIO
    {

        /*
         * CONSTANTES
         */

        //public static string SCRIPT_FILENAME_EXTENSION = ".acscpt";

        /*
         * PROPRIETES
         */
        public int cursor { get; private set; }



        /*
         * CONSTRUCTEURS
         */
        public ScriptFile() : base() {
            cursor = -1;
        }



        /*
         * SERVICES
         */
        // TODO Une meilleure conception de la classe FileIO devrait prendre en charge readFile !!!
        /// <summary>
        /// Déclenche la lecture du fichier 'inputFile' et retourne un int indiquant le status de l'opération
        /// aussi stocké dans le membre IOStatus
        /// </summary>
        /// <returns>int</returns>
        public int readFile() {
            int status = readTextFile(inputFileAbsolutePath, true);
            if (status == FILEOP_SUCCESSFUL) { resetCursor(); }
            return status;
        }



        public void resetCursor() {
            if (FileLines != null) {
                cursor = 0;
            }
            else {
                cursor = -1;
            }
        }



        public string readNextLine() {
            if (FileLines != null && cursor > -1 && cursor < FileLines.Count) {
                string l = FileLines[cursor];
                cursor++;
                return l;
            }
            else {
                return null;
            }
        }



        /*
         *  METHODES
         */




    }

}