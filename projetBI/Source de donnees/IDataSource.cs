using projetBI.Commandes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projetBI.Source_de_donnees
{
    interface IDataSource
    {
        IDictionary<int, Bonbons> GetBonbons();
        IDictionary<int, string> GetBonbons_type();
        IDictionary<int, string> GetConditionnements();
        IDictionary<int, string> GetTextures();
        IDictionary<int, string> GetCouleur();
        IDictionary<int, string> GetVariante();
        IDictionary<int, string> GetPays();



        void InsertCommandes(object toto);
    }
}
