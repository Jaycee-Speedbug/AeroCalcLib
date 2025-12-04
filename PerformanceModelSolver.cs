using System;
using System.Numerics;


namespace AeroCalcCore
{



    /// <summary>
    /// Classe chargée de réaliser une interpolation polynomiale de degré n
    /// </summary>
    public class PerformanceModelSolver
    {


        // Public fields


        // Private fields

        PerfSerie perfSerie;

        double[] ppX;
        double[] ppY;




        // CONSTRUCTEUR(S)


        /// <summary>
        /// Construit un objet destiné à réaliser des interpolations polynomiales
        /// </summary>
        /// <param name="ps">
        /// Série de layers de performance de vol
        /// </param>
        /// <remarks>
        /// TODO: Améliorer la gestion des exceptions (cas du système avec moins de 2 points)
        /// </remarks>
        public PerformanceModelSolver(PerfSerie ps)
        {
            this.perfSerie = ps;
            if (ps == null)
            {
                throw new ModelException(AeroCalc.E_VOID_SYSTEM, "", "", Double.NaN);
            }
            if (ps.count < 2)
            {
                throw new ModelException(AeroCalc.E_SHORT_SERIE, "", "", Double.NaN);
            }

            ppX = new double[ps.count];
            ppY = new double[ps.count];
            for (int count = 0; count < ps.count; count++)
            {
                ppX[count] = ps.pointAt(count).factorValue;
                ppY[count] = ps.pointAt(count).output;
            }
        }



        /*
         * SERVICES
         */

        /// <summary>
        /// Interpolation polynomiale de second degrés.
        /// Pour une valeur x passée en argument, sur la base des points de performance de la PerfSerie
        /// passée au constructeur, renvoie la valeur interpolée.
        /// </summary>
        /// <param name="x">Valeur pour laquelle l'interpolation est demandée</param>
        /// <returns>
        /// Renvoie la valeur prédite par interpolation, NaN en cas d'échec
        /// </returns>
        public double interpolate(double x)
        {

            double interpolation = Double.NaN;
            int[] selectedPoints = selectedPointsTable();
            int[] orderedPointsIndexes = orderedIndexesByDistance(x);

            // Cas ou aucun point n'est sélectionné
            if (selectedPoints == null)
            {
                throw new ModelException(AeroCalc.E_VOID_SYSTEM, "", "", x);
            }
            // Cas ou un seul point est sélectionné dans la série
            if (selectedPoints.Length == 1)
            {
                return perfSerie.pointAt(selectedPoints[0]).output;
            }
            else
            {
                // Cas général, plus d'un point sélectionné
                //
                double[] ptX = new double[selectedPoints.Length];
                double[] ptY = new double[selectedPoints.Length];

                for (int count = 0; count < selectedPoints.Length; count++)
                {
                    ptX[count] = perfSerie.pointAt(selectedPoints[count]).factorValue;
                    ptY[count] = perfSerie.pointAt(selectedPoints[count]).output;
                }

                // Calcul des polynomes
                double[] p = new double[selectedPoints.Length];
                double numerateur;
                double denominateur;

                for (int count = 0; count < p.Length; count++)
                {
                    numerateur = 1;
                    denominateur = 1;
                    // Calcul du numerateur
                    for (int counter = 0; counter < p.Length; counter++)
                    {
                        if (count == counter)
                        {
                            // Pas de produit à calculer
                        }
                        else
                        {
                            numerateur *= x - ptX[counter];
                        }
                    }
                    // Calcul du dénominateur
                    for (int counter = 0; counter < p.Length; counter++)
                    {
                        if (count == counter)
                        {
                            // Pas de produit à calculer
                        }
                        else
                        {
                            denominateur *= ptX[count] - ptX[counter];
                        }
                    }
                    p[count] = numerateur / denominateur;
                }
                // Somme des polynômes
                interpolation = 0;
                for (int count = 0; count < p.Length; count++)
                {
                    interpolation += ptY[count] * p[count];
                }
            }
            return interpolation;
        }



        /*
         * METHODES
         */

        /// <summary>
        /// Renvoie un tableau contenant les indexes des points de performances sélectionnés dans la série
        /// </summary>
        /// <returns>Tableau d'indexes des layers sélectionnés
        /// </returns>
        /// <remarks>
        /// REBUILD: Suppression de cette méthode et utilisation directe de la PerfSerie
        /// </remarks>
        private int[] selectedPointsTable()
        {
            if (perfSerie.selectedCount() > 0)
            {
                int[] spt = new int[perfSerie.selectedCount()];
                PerfPoint pp;
                int index = 0;
                for (int count = 0; count < perfSerie.count; count++)
                {
                    pp = perfSerie.pointAt(count);
                    if (pp.selected)
                    {
                        spt[index] = count;
                        index++;
                    }
                }
                return spt;
            }
            else
            {
                return null;
            }
        }



        private int[] orderedIndexesByDistance(double x)
        {
            int[] pts = new int[ppX.Length];
            double[] dist = distances(x);
            bool[] taken = new bool[ppX.Length];
            
            // Triage
            int shortestIndex = 0;
            double shortestDistance = dist[shortestIndex];

            for (int i = 0; i < pts.Length; i++)
            {
                shortestDistance = Double.MaxValue;
                for (int j = 0; j < dist.Length; j++)
                {
                    if (!taken[j])
                    {
                        if (dist[j] < shortestDistance)
                        {
                            shortestDistance = dist[j];
                            shortestIndex = j;
                        }
                    }
                }
                pts[i] = shortestIndex;
                taken[shortestIndex] = true;
            }
            return pts;
        }
        public int[] _A_orderedIndexesByDistance(double x)
            {
            return orderedIndexesByDistance(x);
        }



        private double[] distances(double x)
        {
            double[] distances = new double[ppX.Length];
            for (int count = 0; count < ppX.Length; count++)
            {
                distances[count] = Math.Abs(x - ppX[count]);
            }
            return distances;
        }

    }
}