using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using projetBI.Commandes;
using projetBI.Source_de_donnees;

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

    public void InsertCommandes(object toto)
    {
        throw new NotImplementedException();
    }

    public IDictionary<int, Bonbons> GetBonbons()
    {
        IDictionary<int, Bonbons> conditionnements = new Dictionary<int, Bonbons>();

        this.Connect();
        OracleCommand cmd = con.CreateCommand();
        cmd.CommandText = "select ID, ID_BONBONS_TYPE, ID_PACKAGING, ID_COULEUR, ID_TEXTURES, ID_VARIANTE from BONBONS";
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
}