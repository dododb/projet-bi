using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using projetBI.Commandes;
using projetBI.Source_de_donnees;
using System.Linq;

public class OraTest : IDataSource
{
    OracleConnection con;
    private void Connect()
    {
        con = new OracleConnection();
        con.ConnectionString = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.1.14)(PORT=1523)))(CONNECT_DATA=(SERVICE_NAME=projet))); User Id=admin; Password=Root1234;";
        con.Open();
        Console.WriteLine("Connected to Oracle" + con.ServerVersion);
    }

    private void Close()
    {
        con.Close();
        con.Dispose();
    }

    public void request()
    {
        
    }

    public IDictionary<int, string> GetBonbons_type()
    {
        IDictionary<int, string> bonbons_type = new Dictionary<int, string>();

        this.Connect();
        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "select ID, TYPE from BONBONS_TYPE";
        OracleDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            bonbons_type.Add(reader.GetInt32(0), reader.GetString(1));
        this.Close();

        return bonbons_type;
    }

    public IDictionary<int, string> GetConditionnements()
    {
        IDictionary<int, string> conditionnements = new Dictionary<int, string>();

        this.Connect();
        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "select ID, PACKAGING from PACKAGING";
        OracleDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            conditionnements.Add(reader.GetInt32(0), reader.GetString(1));
        this.Close();

        return conditionnements;
    }

    public IDictionary<int, string> GetPays()
    {
        IDictionary<int, string> pays = new Dictionary<int, string>();

        this.Connect();
        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "select ID, NOM_PAYS from PAYS";
        OracleDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            pays.Add(reader.GetInt32(0), reader.GetString(1));
        this.Close();

        return pays;
    }

    public IDictionary<int, string> GetTextures()
    {
        IDictionary<int, string> conditionnements = new Dictionary<int, string>();

        this.Connect();
        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "select ID, TEXTURE from TEXTURES";
        OracleDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            conditionnements.Add(reader.GetInt32(0), reader.GetString(1));
        this.Close();

        return conditionnements;
    }

    public IDictionary<int, string> GetCouleur()
    {
        IDictionary<int, string> conditionnements = new Dictionary<int, string>();

        this.Connect();
        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "select ID, COULEUR from COULEUR";
        OracleDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            conditionnements.Add(reader.GetInt32(0), reader.GetString(1));
        this.Close();

        return conditionnements;
    }

    public void InsertCommandes(string sql)
    {
        this.Connect();
        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
        this.Close();

    }

    public IDictionary<int, Bonbons> GetBonbons()
    {
        IDictionary<int, Bonbons> conditionnements = new Dictionary<int, Bonbons>();

        this.Connect();
        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "select ID, ID_BONBONS_TYPE, ID_PACKAGING, ID_COULEUR, ID_TEXTURES, ID_VARIANTE from BONBONS ORDER BY ID";
        OracleDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            conditionnements.Add(reader.GetInt32(0), new Bonbons(reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5)));

        this.Close();

        return conditionnements;
    }

    public IDictionary<int, string> GetVariante()
    {
        IDictionary<int, string> conditionnements = new Dictionary<int, string>();

        this.Connect();
        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "select ID, VARIANTE from VARIANTE";
        OracleDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            conditionnements.Add(reader.GetInt32(0), reader.GetString(1));
        this.Close();

        return conditionnements;
    }

    //<id command, variante, nbVarianteDansLaCommande>
    public void GetNbVarienteForEachCommand()
    {
        this.Connect();
        OracleCommand cmdCheck = con.CreateCommand();
        cmdCheck.CommandText = "SELECT ID_COMMANDES FROM DURE WHERE ID_WORKERTIME=1";
        List<int> checkL = new List<int>();
        OracleDataReader readerCheck = cmdCheck.ExecuteReader();
        while (readerCheck.Read())
            checkL.Add(readerCheck.GetInt32(0));

        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "select t1.ID As commande, t5.ID As variante, t4.id AS bonbonType, COUNT(*) AS nbBonbon " +
                            "FROM COMMANDES t1 INNER JOIN COMMANDES_LIGNES t2 ON t1.ID = t2.ID_COMMANDES " +
                            "INNER JOIN BONBONS t3 ON t2.ID_BONBONS = t3.ID " +
                            "INNER JOIN BONBONS_TYPE t4 ON t3.ID_BONBONS_TYPE = t4.ID " +
                            "INNER JOIN VARIANTE t5 ON t3.ID_VARIANTE = t5.ID " +
                            (checkL.Count > 0 ? $"WHERE t1.ID NOT IN ({string.Join(",", checkL)}) " : "") +
                            "GROUP BY t5.ID, t1.ID, t4.ID ORDER BY commande, variante, variante DESC";
        OracleDataReader reader = cmd.ExecuteReader();
        
        var data = new Dictionary<int, IDictionary<int, List<(int type, int nb)>>>();
        while (reader.Read())
        {
            (int idCommande, int idVariante, int bonbonType, int nbBonbon) = (reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
            if (!data.ContainsKey(idCommande))
                data.Add(idCommande, new Dictionary<int, List<(int type, int nb)>>());

            if (!data[idCommande].ContainsKey(idVariante))
                data[idCommande].Add(idVariante, new List<(int type, int nb)>());
            data[idCommande][idVariante].Add((bonbonType, nbBonbon));
        }
        this.Close();

        foreach (var commande in data)
        {
            var machines = new Dictionary<int, Dictionary<int, (int delai, int changement, List<(int nbType, int nbBonbonTotal)> bonbons)>>() {
                { 1, new Dictionary<int , (int delai, int changement, List<(int, int)> bonbons)>() { { 1, (750,25, new List<(int, int)>()) } } },
                { 2, new Dictionary<int , (int delai, int changement, List<(int, int)> bonbons)>() { { 2, (1230, 45, new List<(int, int)>()) } } },
                { 3, new Dictionary<int , (int delai, int changement, List<(int, int)> bonbons)>() { { 3, (625, 25, new List<(int, int)>()) } } },
                { 4, new Dictionary<int , (int delai, int changement, List<(int, int)> bonbons)>() {
                        { 2, (1230, 45, new List<(int, int)>()) },
                        { 3, (625, 25, new List<(int, int)>()) }
                    }
                }
            };
            //IDictionary<int, int> M1
            foreach (var variante in commande.Value)
            {
                var varianteWhere = machines.Where(x => x.Value.ContainsKey(variante.Key));
                varianteWhere.FirstOrDefault(x => machineGetWorkTimeVariante(x.Value) == varianteWhere.Min(y => machineGetWorkTimeVariante(y.Value))).Value[variante.Key].bonbons.Add((variante.Value.Count, variante.Value.Sum(x => x.nb)));

            }
            string sql = $"INSERT INTO DURE VALUES(1, {commande.Key}, {machines.Sum(x => machineGetWorkTimeVariante(x.Value))})";
            InsertCommandes(sql);
        }
    }

    //<id command, conditionnement, nbbonbon>
    public void  GetNbFabricationForEachCommand()
    {
        this.Connect();
        OracleCommand cmdCheck = con.CreateCommand();
        cmdCheck.CommandText = "SELECT ID_COMMANDES FROM DURE WHERE ID_WORKERTIME=2";
        List<int> checkL = new List<int>();
        OracleDataReader readerCheck = cmdCheck.ExecuteReader();
        while (readerCheck.Read())
            checkL.Add(readerCheck.GetInt32(0));

        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "select t1.ID As commande, t5.ID As packaging, t4.id AS bonbonType, COUNT(*) AS nbBonbon " +
                            "FROM COMMANDES t1 INNER JOIN COMMANDES_LIGNES t2 ON t1.ID = t2.ID_COMMANDES " +
                            "INNER JOIN BONBONS t3 ON t2.ID_BONBONS = t3.ID " +
                            "INNER JOIN BONBONS_TYPE t4 ON t3.ID_BONBONS_TYPE = t4.ID " +
                            "INNER JOIN Packaging t5 ON t3.ID_PACKAGING = t5.ID " +
                            (checkL.Count > 0 ? $"WHERE t1.ID NOT IN ({string.Join(",", checkL)}) " : "") +
                            "GROUP BY t5.ID, t1.ID, t4.ID ORDER BY commande, packaging DESC";
        OracleDataReader reader = cmd.ExecuteReader();

        var data = new Dictionary<int, IDictionary<int, List<(int type, int nb)>>>();
        while (reader.Read())
        {
            (int idCommande, int idPackaging, int bonbonType, int nbBonbon) = (reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
            if (!data.ContainsKey(idCommande))
            {
                data.Add(idCommande, new Dictionary<int, List<(int type, int nb)>>());
            }

            if (!data[idCommande].ContainsKey(idPackaging))
                data[idCommande].Add(idPackaging, new List<(int type, int nb)>());
            data[idCommande][idPackaging].Add((bonbonType, nbBonbon));
        }
        this.Close();
        
        foreach (var commande in data)
        {
            var machines = new Dictionary<int, (int packaging, int delai, int changement, List<(int, int)> bonbons)>() {
                { 1, (1, 500,15, new List<(int, int)>()) },
                { 2, (1, 500,15, new List<(int, int)>()) },
                { 3, (1, 750,25, new List<(int, int)>()) },
                { 4, (2, 200,10, new List<(int, int)>()) },
                { 5, (2, 200,10, new List<(int, int)>()) },
                { 6, (3, 150,15, new List<(int, int)>()) }
            };
            //IDictionary<int, int> M1
            foreach (var packaging in commande.Value)
            {
                var packagingWhere = machines.Where(x => x.Value.packaging == packaging.Key);
                packagingWhere.FirstOrDefault(x => machineGetWorkTime(x.Value) == packagingWhere.Min(y => machineGetWorkTime(y.Value))).Value.bonbons.Add((packaging.Value.Count, packaging.Value.Sum(x => x.nb)));
            }
            string sql = $"INSERT INTO DURE VALUES(2, {commande.Key}, {machines.Sum(x => machineGetWorkTime(x.Value))})";
            InsertCommandes(sql);
        }
    }

    public void GetNbPickingForEachCommand()
    {
        this.Connect();
        OracleCommand cmdCheck = con.CreateCommand();
        cmdCheck.CommandText = "SELECT ID_COMMANDES FROM DURE WHERE ID_WORKERTIME=3";
        List<int> checkL = new List<int>();
        OracleDataReader readerCheck = cmdCheck.ExecuteReader();
        while (readerCheck.Read())
            checkL.Add(readerCheck.GetInt32(0));

        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "SELECT t1.ID commande, t2.id_bonbons bonbons" +
                            " FROM COMMANDES t1 INNER JOIN COMMANDES_LIGNES t2 ON t1.id=t2.ID_COMMANDES" +
                            (checkL.Count > 0 ? $" WHERE t1.ID NOT IN ({string.Join(",", checkL)}) " : "") +
                            " GROUP BY t1.id, t2.id_bonbons";
        OracleDataReader reader = cmd.ExecuteReader();

        var data = new Dictionary<int, List<int>>();
        while (reader.Read())
        {
            (int idCommande, int idBonbon) = (reader.GetInt32(0), reader.GetInt32(1));
            if (!data.ContainsKey(idCommande))
            {
                data.Add(idCommande, new List<int>());
            }
            data[idCommande].Add(idBonbon);
        }
        this.Close();

        foreach (var commande in data)
        {
            
            string sql = $"INSERT INTO DURE VALUES(3, {commande.Key}, {commande.Value.Count*8})";
            InsertCommandes(sql);
        }
    }

    //<id command, variante, nbVarianteDansLaCommande>
    public void GetNbVarienteOpti()
    {
        this.Connect();
        OracleCommand cmdCheck = con.CreateCommand();
        cmdCheck.CommandText = "SELECT ID_COMMANDES FROM DURE WHERE ID_WORKERTIME=1";
        List<int> checkL = new List<int>();
        OracleDataReader readerCheck = cmdCheck.ExecuteReader();
        while (readerCheck.Read())
            checkL.Add(readerCheck.GetInt32(0));

        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "select t1.ID As commande, t5.ID As variante, t4.id AS bonbonType, COUNT(*) AS nbBonbon " +
                            "FROM COMMANDES t1 INNER JOIN COMMANDES_LIGNES t2 ON t1.ID = t2.ID_COMMANDES " +
                            "INNER JOIN BONBONS t3 ON t2.ID_BONBONS = t3.ID " +
                            "INNER JOIN BONBONS_TYPE t4 ON t3.ID_BONBONS_TYPE = t4.ID " +
                            "INNER JOIN VARIANTE t5 ON t3.ID_VARIANTE = t5.ID " +
                            (checkL.Count > 0 ? $"WHERE t1.ID NOT IN ({string.Join(",", checkL)}) " : "") +
                            "GROUP BY t5.ID, t1.ID, t4.ID ORDER BY commande, variante, variante DESC";
        OracleDataReader reader = cmd.ExecuteReader();

        var data = new Dictionary<int, IDictionary<int, List<(int type, int nb)>>>();
        while (reader.Read())
        {
            (int idCommande, int idVariante, int bonbonType, int nbBonbon) = (reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
            if (!data.ContainsKey(idCommande))
                data.Add(idCommande, new Dictionary<int, List<(int type, int nb)>>());

            if (!data[idCommande].ContainsKey(idVariante))
                data[idCommande].Add(idVariante, new List<(int type, int nb)>());
            data[idCommande][idVariante].Add((bonbonType, nbBonbon));
        }
        this.Close();
        var machines = new Dictionary<int, Dictionary<int, (int delai, int changement, List<(int nbType, int nbBonbonTotal)> bonbons)>>() {
            { 1, new Dictionary<int , (int delai, int changement, List<(int, int)> bonbons)>() { { 1, (750,25, new List<(int, int)>()) } } },
            { 2, new Dictionary<int , (int delai, int changement, List<(int, int)> bonbons)>() { { 2, (1230, 45, new List<(int, int)>()) } } },
            { 3, new Dictionary<int , (int delai, int changement, List<(int, int)> bonbons)>() { { 3, (625, 25, new List<(int, int)>()) } } },
            { 4, new Dictionary<int , (int delai, int changement, List<(int, int)> bonbons)>() {
                    { 2, (1230, 45, new List<(int, int)>()) },
                    { 3, (625, 25, new List<(int, int)>()) }
                }
            }
        };
        foreach (var commande in data)
        {
            
            //IDictionary<int, int> M1
            foreach (var variante in commande.Value)
            {
                var varianteWhere = machines.Where(x => x.Value.ContainsKey(variante.Key));
                varianteWhere.FirstOrDefault(x => machineGetWorkTimeVariante(x.Value) == varianteWhere.Min(y => machineGetWorkTimeVariante(y.Value))).Value[variante.Key].bonbons.Add((variante.Value.Count, variante.Value.Sum(x => x.nb)));

            }
            
        }

        double tempsMoyen = machines.Sum(x => machineGetWorkTimeVariante(x.Value));
        foreach (var commande in data)
        {
            string sql = $"INSERT INTO DURE VALUES(1, {commande.Key}, {tempsMoyen})";
            InsertCommandes(sql);
        }
    }

    //<id command, conditionnement, nbbonbon>
    public void GetNbFabricationOpti()
    {
        this.Connect();
        OracleCommand cmdCheck = con.CreateCommand();
        cmdCheck.CommandText = "SELECT ID_COMMANDES FROM DURE WHERE ID_WORKERTIME=2";
        List<int> checkL = new List<int>();
        OracleDataReader readerCheck = cmdCheck.ExecuteReader();
        while (readerCheck.Read())
            checkL.Add(readerCheck.GetInt32(0));

        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "select t1.ID As commande, t5.ID As packaging, t4.id AS bonbonType, COUNT(*) AS nbBonbon " +
                            "FROM COMMANDES t1 INNER JOIN COMMANDES_LIGNES t2 ON t1.ID = t2.ID_COMMANDES " +
                            "INNER JOIN BONBONS t3 ON t2.ID_BONBONS = t3.ID " +
                            "INNER JOIN BONBONS_TYPE t4 ON t3.ID_BONBONS_TYPE = t4.ID " +
                            "INNER JOIN Packaging t5 ON t3.ID_PACKAGING = t5.ID " +
                            (checkL.Count > 0 ? $"WHERE t1.ID NOT IN ({string.Join(",", checkL)}) " : "") +
                            "GROUP BY t5.ID, t1.ID, t4.ID ORDER BY commande, packaging DESC";
        OracleDataReader reader = cmd.ExecuteReader();

        var data = new Dictionary<int, IDictionary<int, List<(int type, int nb)>>>();
        while (reader.Read())
        {
            (int idCommande, int idPackaging, int bonbonType, int nbBonbon) = (reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
            if (!data.ContainsKey(idCommande))
            {
                data.Add(idCommande, new Dictionary<int, List<(int type, int nb)>>());
            }

            if (!data[idCommande].ContainsKey(idPackaging))
                data[idCommande].Add(idPackaging, new List<(int type, int nb)>());
            data[idCommande][idPackaging].Add((bonbonType, nbBonbon));
        }
        this.Close();
        var machines = new Dictionary<int, (int packaging, int delai, int changement, List<(int, int)> bonbons)>() {
                { 1, (1, 500,15, new List<(int, int)>()) },
                { 2, (1, 500,15, new List<(int, int)>()) },
                { 3, (1, 750,25, new List<(int, int)>()) },
                { 4, (2, 200,10, new List<(int, int)>()) },
                { 5, (2, 200,10, new List<(int, int)>()) },
                { 6, (3, 150,15, new List<(int, int)>()) }
            };
        foreach (var commande in data)
        {
            
            //IDictionary<int, int> M1
            foreach (var packaging in commande.Value)
            {
                var packagingWhere = machines.Where(x => x.Value.packaging == packaging.Key);
                packagingWhere.FirstOrDefault(x => machineGetWorkTime(x.Value) == packagingWhere.Min(y => machineGetWorkTime(y.Value))).Value.bonbons.Add((packaging.Value.Count, packaging.Value.Sum(x => x.nb)));
            }
            
        }

        double tempsMoyen = machines.Sum(x => machineGetWorkTime(x.Value));
        Random changementGare = new Random();
        foreach (var commande in data)
        {
            int multiplicatorChangement = changementGare.Next(1, 3888) > 3788 ? 0 : 1;
            string sql = $"INSERT INTO DURE VALUES(3, {commande.Key}, {commande.Value.Count * 8 * multiplicatorChangement})";
            InsertCommandes(sql);
        }
    }

    public void GetNbPickingOpti()
    {
        this.Connect();
        OracleCommand cmdCheck = con.CreateCommand();
        cmdCheck.CommandText = "SELECT ID_COMMANDES FROM DURE WHERE ID_WORKERTIME=3";
        List<int> checkL = new List<int>();
        OracleDataReader readerCheck = cmdCheck.ExecuteReader();
        while (readerCheck.Read())
            checkL.Add(readerCheck.GetInt32(0));

        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "SELECT t1.ID commande, t2.id_bonbons bonbons" +
                            " FROM COMMANDES t1 INNER JOIN COMMANDES_LIGNES t2 ON t1.id=t2.ID_COMMANDES" +
                            (checkL.Count > 0 ? $"WHERE t1.ID NOT IN ({string.Join(",", checkL)}) " : "") +
                            " GROUP BY t1.id, t2.id_bonbons";
        OracleDataReader reader = cmd.ExecuteReader();

        var data = new Dictionary<int, List<int>>();
        while (reader.Read())
        {
            (int idCommande, int idBonbon) = (reader.GetInt32(0), reader.GetInt32(1));
            if (!data.ContainsKey(idCommande))
            {
                data.Add(idCommande, new List<int>());
            }
            data[idCommande].Add(idBonbon);
        }
        this.Close();

        Random changementGare = new Random();
        foreach (var commande in data)
        {
            int multiplicatorChangement = changementGare.Next(1, 3888) > 3788 ? 0 : 1;
            string sql = $"INSERT INTO DURE VALUES(3, {commande.Key}, {commande.Value.Count * 8 * multiplicatorChangement})";
            InsertCommandes(sql);
        }
    }

    private double machineGetWorkTime((int packaging, int delai, int changement, List<(int type, int nb)> bonbons) machine)
    {
        return machine.bonbons.Select(x => x.nb).Sum() / machine.delai + machine.bonbons.Select(x => x.type).Count() * machine.changement;
    }

    private double machineGetWorkTimeVariante(Dictionary<int, (int delai, int changement, List<(int, int)> bonbons)> machine)
    {
        return machine.Sum(x => machineGetWorkTime(Format(x.Key, x.Value)));
    }

    private (int, int delai, int changement, List<(int, int)> bonbons) Format(int variante, (int delai, int changement, List<(int, int)> bonbons) autre)
    {
        return (variante, autre.delai, autre.changement, autre.bonbons);
    }

}