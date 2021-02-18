using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CrystalSystemAPI
{
    public class DBManager
    {

        //DT 2020-03-09 declare our sql connection
        private SqlConnection Conn;

        //DT 2020-03-09 create connection
        private void CreateConnection()
        {
            //DT 2020-03-09 declare first our configuration for our connection string
            string ConnStr = ConfigurationManager.ConnectionStrings["CerinimbusDevEntities"].ConnectionString;

            //DT 2020-03-09 since we are using an entity model, then we need to check if our meta data were set in our connection string
            if (ConnStr.ToLower().StartsWith("metadata="))
            {
                //DT 2020-03-09 then let us set our entity connection builder
                System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder efBuilder = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(ConnStr);

                //DT 2020-03-09 set our connection string from entity connection builder
                ConnStr = efBuilder.ProviderConnectionString;

            }

            //DT 2020-03-09 finally, update our sql connection from our connection string
            Conn = new SqlConnection(ConnStr);
        }

        //DT 2020-03-09 read data from database with provided sql
        public DataTable getData(string sqlString)
        {
            //DT 2020-03-09 call our database connection
            CreateConnection();

            //DT 2020-03-09 create a data adapter so we can fill the data later
            SqlDataAdapter sda = new SqlDataAdapter(sqlString, Conn);

            //DT 2020-03-09 create our data table for our output
            DataTable dt = new DataTable();
            try
            {
                //DT 2020-03-09 create our connection 
                //then fill the adapter into our data table
                Conn.Open();
                sda.Fill(dt);
            }
            catch (SqlException se)
            {
                //DT 2020-03-09 return the error
                Console.WriteLine(se.ToString());
            }
            finally
            {
                //DT 2020-03-09 close our connection when done
                Conn.Close();
            }

            //DT 2020-03-09 return our data table
            return dt;
        }
    }
}