﻿namespace VehicleVendorConsole.Client
{
    using System;
    using System.IO;
    using System.Data.OleDb;
    using System.Data;
    using System.Collections.Generic;
    using VehicleVendor.Data.Repositories;
    using VehicleVendor.Models;


    public class ZipExcelLoader
    {
        /// <summary>
        /// Reads MS Excel (.xls) file through the OLE DB data provider. 
        /// </summary>
        public void LoadIntoModel(IVehicleVendorRepository repo)
        {
            string fileLocation = @"..\..\datafile.xls";

            string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";"
                + "Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";

            OleDbConnection db_Con = new OleDbConnection(connectionString);

            db_Con.Open();
            using (db_Con)
            {

                // this code is for discovering sheet names if they are not available
                DataTable tables = db_Con.GetSchema("Tables");
                var sheetNameList = new List<string>();
                for (int i = 0; i < tables.Rows.Count; i++)
                {
                    var name = tables.Rows[i][2].ToString();
                    sheetNameList.Add(name);
                }

                string sheetName = sheetNameList[0];
                Console.WriteLine(sheetName);
                string sqlString = "select * from [" + sheetName + "];";

                OleDbCommand cmdGetRows = new OleDbCommand(sqlString, db_Con);

                OleDbDataReader reader = cmdGetRows.ExecuteReader();

                using (reader)
                {
                    reader.Read();
                    Sale sale;
                    SaleDetails details;
                    var dealer = reader["DealerId"];
                    if (dealer != DBNull.Value)
                    {
                        dealer = (int)(double)dealer;
                        var saleDate = (DateTime)reader["SaleDate"];
                        Console.WriteLine(dealer + "  -  " + saleDate);
                        sale = new Sale() { DealerId = (int)(double)dealer, SaleDate = saleDate };
                        repo.Add<Sale>(sale);

                        details = this.DetailsRow(repo, reader, sale);
                        if (details != null)
                        {
                            repo.Add<SaleDetails>(details);
                        }

                        while (reader.Read())
                        {
                            if (details != null)
                            {
                                details = this.DetailsRow(repo, reader, sale);
                                repo.Add<SaleDetails>(details);
                            }
                        }
                    }
                }
            }//using db_Con
        }

        private SaleDetails DetailsRow(IVehicleVendorRepository repo, OleDbDataReader reader, Sale sale)
        {
            var detailQty = reader["Quantity"];
            if (detailQty != DBNull.Value)
            {
                detailQty = (int)(double)detailQty;
                var vehiceId = (int)(double)reader["VehicleId"];
                var details = new SaleDetails() { Quantity = (int)(double)detailQty, VehicleId = vehiceId, Sale = sale };
                return details;
            }
            return null;
        }
    }
}
