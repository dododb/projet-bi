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

        private void button1_Click(object sender, EventArgs e)
        {
            OraTest ora = new OraTest();
            foreach(var toto in ora.GetBonbons_type()) Console.WriteLine(toto.Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OraTest ora = new OraTest();
            foreach (var toto in ora.GetConditionnements()) Console.WriteLine(toto.Value);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OraTest ora = new OraTest();
            foreach (var toto in ora.GetPays()) Console.WriteLine(toto.Value);
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

        }
    }
}
