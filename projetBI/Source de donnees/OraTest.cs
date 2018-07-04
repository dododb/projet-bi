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
        con.ConnectionString = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.133.50.10)(PORT=1523)))(CONNECT_DATA=(SERVICE_NAME=projet))); User Id=admin; Password=Root1234;";
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
    public IDictionary<int, IDictionary<int, double>> GetNbVarienteForEachCommand()
    {
        this.Connect();
        OracleCommand cmdCheck = con.CreateCommand();
        cmdCheck.CommandText = "SELECT ID_COMMANDES FROM DURE WHERE ID_WORKERTIME=1";
        List<int> checkL = new List<int>();
        OracleDataReader readerCheck = cmdCheck.ExecuteReader();
        while (readerCheck.Read())
            checkL.Add(readerCheck.GetInt32(0));

        var data = new Dictionary<int, IDictionary<int, double>>();
        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "select distinct t1.ID, t5.ID, t6.CADENCE, COUNT(*) AS Compte, t6.DELAI, COUNT(t4.id) AS CompteChangementOutils " + 
                            "FROM COMMANDES t1 INNER JOIN COMMANDES_LIGNES t2 ON t1.ID = t2.ID_COMMANDES " + 
                            "INNER JOIN BONBONS t3 ON t2.ID_BONBONS = t3.ID " + 
                            "INNER JOIN BONBONS_TYPE t4 ON t3.ID_BONBONS_TYPE = t4.ID " +
                            "INNER JOIN VARIANTE t5 ON t3.ID_VARIANTE = t5.ID " +
                            "INNER JOIN EST_FABRIQUE t6 ON t5.ID = t6.ID " +
                            "INNER JOIN FABRICATION t7 ON t6.ID_MACHINE = t7.ID_MACHINE " +
                            $"WHERE t1.ID NOT IN ({string.Join(",", checkL)})" +
                            "GROUP BY t6.CADENCE, t6.DELAI, t5.ID, t1.ID ORDER BY Compte DESC";
        OracleDataReader reader = cmd.ExecuteReader();
        bool toto = true;
        int commandes = 0;
        while (reader.Read())
        {
            if (!data.ContainsKey(reader.GetInt32(0)))
            {
                data.Add(reader.GetInt32(0), new Dictionary<int, double>());
            }
            if (toto && reader.GetInt32(1) != 1)
                data[reader.GetInt32(0)].Add(reader.GetInt32(1), (reader.GetInt32(3) / reader.GetInt32(2)) / 2 + reader.GetInt32(4) * reader.GetInt32(5));
            else data[reader.GetInt32(0)].Add(reader.GetInt32(1), reader.GetInt32(3) / reader.GetInt32(2) + reader.GetInt32(4) * reader.GetInt32(5));
        }
        this.Close();

        foreach (var d in data)
        {            
            string sql = $"INSERT INTO DURE VALUES(1, {d.Key}, {d.Value.Select(x => x.Value).Sum()})";
            InsertCommandes(sql);
        }
        return data;
    }

    public IDictionary<int, IDictionary<int, double>> GetNbFabricationForEachCommand()
    {
        this.Connect();
        OracleCommand cmdCheck = con.CreateCommand();
        var check = "SELECT ID_COMMANDES FROM DURE WHERE ID_WORKERTIME=2";
        List<int> checkL = new List<int>();
        OracleDataReader readerCheck = cmdCheck.ExecuteReader();
        while (readerCheck.Read())
            checkL.Add(readerCheck.GetInt32(0));

        var data = new Dictionary<int, IDictionary<int, double>>();
        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "select distinct t1.ID, t5.ID, t6.CADENCE, COUNT(*) AS Compte, t6.DELAI, COUNT(t4.id) AS CompteChangementOutils " +
                            "FROM COMMANDES t1 INNER JOIN COMMANDES_LIGNES t2 ON t1.ID = t2.ID_COMMANDES " +
                            "INNER JOIN BONBONS t3 ON t2.ID_BONBONS = t3.ID " +
                            "INNER JOIN BONBONS_TYPE t4 ON t3.ID_BONBONS_TYPE = t4.ID " +
                            "INNER JOIN Packaging t5 ON t3.ID_PACKAGING = t5.ID " +
                            "INNER JOIN CONDITIONNEMENT t6 ON t5.ID = t6.ID_MACHINE " +
                            $"WHERE t1.ID NOT IN ({string.Join(",", checkL)})" +
                            "GROUP BY t6.CADENCE, t6.DELAI, t5.ID, t1.ID ORDER BY Compte DESC";
        OracleDataReader reader = cmd.ExecuteReader();
        bool toto = true;
        int commandes = 0;
        while (reader.Read())
        {

        }
        this.Close();

        foreach (var d in data)
        {
            string sql = $"INSERT INTO DURE VALUES(2, {d.Key}, {d.Value.Select(x => x.Value).Sum()})";
            InsertCommandes(sql);
        }
        return data;
    }

}