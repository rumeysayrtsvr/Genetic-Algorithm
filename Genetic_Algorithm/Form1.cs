using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Genetic_Algorithm
{
    public partial class Form1 : Form
    {
        private Random random = new Random();
        private List<Individual> population;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnCalistir_Click(object sender, EventArgs e)
        {
            try
            {
                int populationSize = int.Parse(txtPopulationSize.Text);
                double crossoverRate = double.Parse(txtCrossoverRate.Text);
                double mutationRate = double.Parse(txtMutationRate.Text);
                double elitismRate = double.Parse(txtElitismRate.Text);
                int generationCount = int.Parse(txtGenerations.Text);

            
                if (populationSize <= 0) throw new Exception("Popülasyon boyutu pozitif bir tamsayı olmalı.");
                if (crossoverRate < 0 || crossoverRate > 1)throw new Exception("Çaprazlama oranı 0 ile 1 arasında olmalı.");
                if (mutationRate < 0 || mutationRate > 1)throw new Exception("Mutasyon oranı 0 ile 1 arasında olmalı.");
                if (elitismRate < 0 || elitismRate > 1) throw new Exception("Seçkinlik oranı 0 ile 1 arasında olmalı.");     
                if (generationCount <= 0)throw new Exception("Jenerasyon sayısı pozitif bir tamsayı olmalı.");

                    
           
                RunGeneticAlgorithm(populationSize, crossoverRate, mutationRate, elitismRate, generationCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Geçersiz Girdi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void RunGeneticAlgorithm(int populationSize, double crossoverRate, double mutationRate, double elitismRate, int generationCount)
        {
            population = InitializePopulation(populationSize);
            chart1.Series.Clear();

            Series series = new Series
            {
                ChartType = SeriesChartType.Line,
                Name = "Fitness Value"
            };
            chart1.Series.Add(series);

            for (int gen = 0; gen < generationCount; gen++)
            {
                List<Individual> newPopulation = new List<Individual>();
                int eliteCount = (int)(populationSize * elitismRate);

              
                var elites = population.OrderBy(i => i.Fitness()).Take(eliteCount).ToList();
                newPopulation.AddRange(elites);

               
                while (newPopulation.Count < populationSize)
                {
                    Individual parent1 = TournamentSelection();
                    Individual parent2 = TournamentSelection();
                    Individual offspring = Crossover(parent1, parent2, crossoverRate);
                    Mutate(offspring, mutationRate);
                    newPopulation.Add(offspring);
                }

               
                population = newPopulation.OrderBy(i => i.Fitness()).ToList();
                series.Points.AddXY(gen, population.First().Fitness());
            }

          
            Individual bestIndividual = population.First();
            listBox1.Items.Add($"{DateTime.Now:HH:mm:ss} → X = {bestIndividual.X:F4}, Y = {bestIndividual.Y:F4}, Fitness = {bestIndividual.Fitness():F4}");
            label9.Text = $" {bestIndividual.X:F4}";
            label11.Text = $" {bestIndividual.Y:F4}";
            label13.Text = $" {bestIndividual.Fitness():F4}";
            
        }

        private List<Individual> InitializePopulation(int populationSize)
        {
            List<Individual> pop = new List<Individual>();
            for (int i = 0; i < populationSize; i++)
            {
                double x = random.NextDouble() * 4 - 2; 
                double y = random.NextDouble() * 4 - 2; 
                pop.Add(new Individual(x, y));
            }
            return pop;
        }

        private Individual TournamentSelection()
        {
            int tournamentSize = 4;
            List<Individual> tournament = new List<Individual>();
            for (int i = 0; i < tournamentSize; i++)
            {
                tournament.Add(population[random.Next(population.Count)]);
            }
            return tournament.OrderBy(i => i.Fitness()).First();
        }

        private Individual Crossover(Individual parent1, Individual parent2, double crossoverRate)
        {
            if (random.NextDouble() < crossoverRate)
            {
               
                double newX = (parent1.X + parent2.X) / 2;
                double newY = (parent1.Y + parent2.Y) / 2;
                return new Individual(newX, newY);
            }
            return new Individual(parent1.X, parent1.Y);
        }

        private void Mutate(Individual individual, double mutationRate)
        {
            if (random.NextDouble() < mutationRate)
            {
               
                individual.X += random.NextDouble() * 2 - 1;
                individual.Y += random.NextDouble() * 2 - 1;

               
                individual.X = Math.Max(-2, Math.Min(2, individual.X));
                individual.Y = Math.Max(-2, Math.Min(2, individual.Y));
            }
        }
    }

    public class Individual
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Individual(double x, double y)
        {
            X = x;
            Y = y;
        }


        public double Fitness()
        {
            double term1 = 1 + Math.Pow(X + Y + 1, 2) * (19 - 14 * X + 3 * Math.Pow(X, 2) - 14 * Y + 6 * X * Y + 3 * Math.Pow(Y, 2));
            double term2 = 30 + Math.Pow(2 * X - 3 * Y, 2) * (18 - 32 * X + 12 * Math.Pow(X, 2) + 48 * Y - 36 * X * Y + 27 * Math.Pow(Y, 2));
            return term1 * term2;
        }
    }
}
