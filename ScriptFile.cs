


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
        public ScriptFile() : base()
        {
            cursor = -1;
        }



        /*
         * SERVICES
         */
        // TODO Une meilleure conception de la classe FileIO devrait prendre en charge readFile !!!
        public int readFile()
        {
            return readTextFile(inputFileAbsolutePath, true);
        }



        public void resetCursor()
        {
            if (fileLines != null)
            {
                cursor = 0;
            }
            else
            {
                cursor = -1;
            }
        }



        public string readNextLine()
        {
            if (fileLines != null && cursor > -1 && cursor < fileLines.Count)
            {
                string l = fileLines[cursor];
                cursor++;
                return l;
            }
            else
            {
                return null;
            }
        }



        /*
         *  METHODES
         */




    }

}