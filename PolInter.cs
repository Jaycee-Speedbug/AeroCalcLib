using System;



namespace AeroCalcCore {



    /// <summary>
    /// Classe chargée de réaliser une interpolation polynomiale de degré n
    /// </summary>
    public class PolInter {


        // Public fields


        // Private fields


        PerfSerie perfSerie;


        // Constructor(s)


        /// <summary>
        /// Construit un objet destiné à réaliser des interpolations polynomiales
        /// </summary>
        /// <param name="ps">
        /// Série de layers de performance de vol
        /// </param>
        public PolInter(PerfSerie ps) {
            this.perfSerie = ps;
        }



        /*
         * Services
         */


        /// <summary>
        /// Interpolation polynomiale pour une abscisse de référence, sur la base d'une série de points de
        /// performance sélectionnés, dans une série Perfserie
        /// </summary>
        /// <param name="x">Abscisse de référence</param>
        /// <returns>
        /// Renvoie la valeur prédite par interpolation
        /// </returns>
        public double interpolate(double x) {

            double interpolation = Double.NaN;
            int[] selectedPoints = selectedPointsTable();

            // Cas ou aucun point n'est sélectionné
            if (selectedPoints == null) {
                throw new ModelException(AeroCalc.E_VOID_SYSTEM, "", "", x);
            }
            // Cas ou un seul point est sélectionné dans la série
            if (selectedPoints.Length == 1) {
                return perfSerie.pointAt(selectedPoints[0]).output;
            }
            else {
                // Cas général, plus d'un point sélectionné
                //
                double[] ptX = new double[selectedPoints.Length];
                double[] ptY = new double[selectedPoints.Length];

                for (int count = 0; count < selectedPoints.Length; count++) {
                    ptX[count] = perfSerie.pointAt(selectedPoints[count]).factorValue;
                    ptY[count] = perfSerie.pointAt(selectedPoints[count]).output;
                }

                // Calcul des polynomes
                double[] p = new double[selectedPoints.Length];
                double numerateur;
                double denominateur;

                for (int count = 0; count < p.Length; count++) {
                    numerateur = 1;
                    denominateur = 1;
                    // Calcul du numerateur
                    for (int counter = 0; counter < p.Length; counter++) {
                        if (count == counter) {
                            // Pas de produit à calculer
                        }
                        else {
                            numerateur *= x - ptX[counter];
                        }
                    }
                    // Calcul du dénominateur
                    for (int counter = 0; counter < p.Length; counter++) {
                        if (count == counter) {
                            // Pas de produit à calculer
                        }
                        else {
                            denominateur *= ptX[count] - ptX[counter];
                        }
                    }
                    p[count] = numerateur / denominateur;
                }
                // Somme des polynômes
                interpolation = 0;
                for (int count = 0; count < p.Length; count++) {
                    interpolation += ptY[count] * p[count];
                }
            }
            return interpolation;
        }



        /*
         * Methods
         */

        /// <summary>
        /// Renvoie un tableau contenant les indexes des layers de performances sélectionnés dans la série
        /// </summary>
        /// <returns>Tableau d'indexes des layers sélectionnés
        /// </returns>
        private int[] selectedPointsTable() {
            if (perfSerie.selectedCount() > 0) {
                int[] spt = new int[perfSerie.selectedCount()];
                PerfPoint pp;
                int index = 0;
                for (int count = 0; count < perfSerie.count; count++) {
                    pp = perfSerie.pointAt(count);
                    if (pp.selected) {
                        spt[index] = count;
                        index++;
                    }
                }
                return spt;
            }
            else {
                return null;
            }
        }

    }

}