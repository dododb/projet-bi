using projetBI.Commandes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projetBI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            OraTest ora = new OraTest();
            var bonbons_type = ora.GetBonbons_type();
            var conditionnement = ora.GetConditionnements();
            var textures = ora.GetTextures();
            var couleur = ora.GetCouleur();
            var variante = ora.GetVariante();
            var pays = ora.GetPays();
            var bonbons = ora.GetBonbons();

            

            int i = 0;
            Random rad = new Random();
            Random rand = new Random();
            //int total = dico.Select(x => x.Value).Sum();
            double nbLign = (double)numericUpDown2.Value;
            double sigma_nb_command = 10;
            double mu_nb_command = nbLign;

            double sigma_bonbon = 100;
            int mu_bonbon = bonbons.Count() / 2;

            double sigma_pays = 1;
            double mu_pays = pays.Count() / 2;

            while (i++<numericUpDown1.Value)
            {
                double randGauss = RandomExtensions.NextGaussian(rand, mu_nb_command, sigma_nb_command);
                double randGaussPays = RandomExtensions.NextGaussian(rand, mu_pays, sigma_pays);
                string sql = $"INSERT INTO COMMANDES VALUES (0, {Math.Round(randGaussPays)})";
                ora.InsertCommandes(sql);
                int j = 0;
                //chaque ligne dans la commande
                while (j++ < randGauss)
                {
                    double random = RandomExtensions.NextGaussian(rad, mu_bonbon, sigma_bonbon);
                    

                    string sqlLigne = $"INSERT INTO COMMANDES_LIGNES VALUES (0,{Math.Round(random)},COMMANDES_SEQ.CURRVAL)";
                    ora.InsertCommandes(sqlLigne);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OraTest ora = new OraTest();
            ora.GetNbVarienteForEachCommand();
        }
    }
}
