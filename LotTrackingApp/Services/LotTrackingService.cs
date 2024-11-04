using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using LotTrackingApp.Models;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace LotTrackingApp.Services
{
    public class LotTrackingService
    {
        private readonly string _connectionString;
        private readonly IDbConnection _dbConnection;

        public LotTrackingService(IConfiguration configuration, IDbConnection dbConnection)
        {
            _connectionString = configuration.GetConnectionString("MES_ATEC_Connection");
            _dbConnection = dbConnection;
        }

        public LotTracking GetLotDetails(string lotAlias)
        {
            LotTracking lotTracking = new LotTracking();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_TRN_GetLotDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LotAlias", lotAlias);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            
                            lotTracking.LotCode = reader["LotCode"].ToString();
                            lotTracking.LotAlias = reader["LotAlias"].ToString();
                            lotTracking.CustomerID = reader["CustomerID"].ToString();
                            lotTracking.ProductID = reader["ProductID"].ToString();
                            lotTracking.MaterialID = reader["MaterialID"].ToString();
                            lotTracking.FlowID = reader["FlowID"].ToString();
                            lotTracking.PackageID = reader["PackageID"].ToString();
                            lotTracking.LeadTypeID = reader["LeadTypeID"].ToString();
                            lotTracking.StatusID = reader["StatusID"].ToString();
                            lotTracking.StoreID = reader["StoreID"].ToString();
                            lotTracking.StageID = reader["StageID"].ToString();
                            lotTracking.RecipeID = reader["RecipeID"].ToString();
                            lotTracking.CurrentQty = reader["CurrentQty"] != DBNull.Value ? Convert.ToInt32(reader["CurrentQty"]) : (int?)null;
                            lotTracking.CurrentSequence = reader["CurrentSequence"] != DBNull.Value ? Convert.ToInt32(reader["CurrentSequence"]) : (int?)null;
                            lotTracking.StatusCode = reader["StatusCode"].ToString();
                            lotTracking.StageCode = reader["StageCode"].ToString();
                            lotTracking.CustomerCode = reader["CustomerCode"].ToString();
                            int lotCode = Convert.ToInt32(lotTracking.LotCode);
                            int currentQty = lotTracking.CurrentQty.GetValueOrDefault(0); 
                            lotTracking.TotalSavedRejects = GetTotalSavedRejects(lotCode, currentQty);
                            lotTracking.TotalSavedAccounts = GetTotalSavedAccounts(lotCode, currentQty);
                            lotTracking.RecipeCode = reader["RecipeCode"].ToString();
                            lotTracking.DateCodeVerifiedBy = reader["DateCodeVerifiedBy"].ToString();
                            lotTracking.DateCode = reader["DateCode"].ToString();
                            lotTracking.POCode = reader["POCode"].ToString();
                        }
                    }
                }
            }

            return lotTracking;
        }

        private int GetTotalSavedRejects(int lotCode, int currentQty)
        {
            // Replace with actual logic to calculate total saved rejects
            // Placeholder logic:
            int totalRejects = 0;

            // You can query the database or perform calculations here
            // For example:
            // using (SqlConnection conn = new SqlConnection(_connectionString))
            // {
            //     // Logic to calculate total saved rejects based on lotCode and currentQty
            //     // e.g., execute a query to fetch total rejects from the database
            // }

            return totalRejects;
        }

        private int GetTotalSavedAccounts(int lotCode, int currentQty)
        {
            // Replace with actual logic to calculate total saved accounts
            // Placeholder logic:
            int totalAccounts = 0;

            // You can query the database or perform calculations here
            // For example:
            // using (SqlConnection conn = new SqlConnection(_connectionString))
            // {
            //     // Logic to calculate total saved accounts based on lotCode and currentQty
            //     // e.g., execute a query to fetch total accounts from the database
            // }

            return totalAccounts;
        }
        public List<TransactionDetail> GetTransactionDetails(long lotCode)
        {
            var transactionDetails = new List<TransactionDetail>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_TRN_LotTracking_GetTransactionDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LotCode", lotCode);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactionDetails.Add(new TransactionDetail
                            {
                                Sequence = (int)reader["Sequence"],
                                RecipeID = reader["RecipeID"].ToString(),
                                InQty = (int)reader["InQty"],
                                RejectQty = (int)reader["RejectQty"],
                                AccountQty = (int)reader["AccountQty"],
                                OutQty = (int)reader["OutQty"],
                                EquipmentID = reader["EquipmentID"].ToString(),
                                Remarks = reader["Remarks"].ToString()
                            });
                        }
                    }
                }
            }

            return transactionDetails;
        }
        public bool TrackIn(string lotAlias, int qtyIn, int equipmentCode, int trackInUser)
        {

            // Step 1: Retrieve the LotCode from TRN_LotStart using lotAlias
            var lotCode = _dbConnection.QuerySingleOrDefault<int>(
                @"SELECT LotCode FROM TRN_LotStart WHERE LotAlias = @LotAlias",
                new { LotAlias = lotAlias });

            // If no matching LotCode is found, return false
            if (lotCode == 0)
            {
                return false;
            }

            // Step 2: Retrieve the latest sequence with no InQty for the specified LotCode
            var lastSequenceDetail = _dbConnection.QuerySingleOrDefault<TransactionDetail>(
                @"SELECT TOP 1 * FROM TRN_Lot_Detail 
          WHERE LotCode = @LotCode AND InQty IS NULL
          ORDER BY Sequence",
                new { LotCode = lotCode });

            // If no matching sequence is found, return false
            if (lastSequenceDetail == null)
            {
                return false;
            }

            // Step 3: Update the retrieved sequence with InQty, EquipmentCode, TrackInTime, and TrackInUser
            int rowsAffected = _dbConnection.Execute(
                @"UPDATE TRN_Lot_Detail 
          SET InQty = @QtyIn, EquipmentCode = @EquipmentCode, TrackInTime = GETDATE(), TrackInUserCode = @TrackInUser
          WHERE Sequence = @Sequence AND LotCode = @LotCode",
                new
                {
                    QtyIn = qtyIn,
                    EquipmentCode = equipmentCode,
                    TrackInUser = trackInUser,
                    Sequence = lastSequenceDetail.Sequence,
                    LotCode = lotCode
                });

            return rowsAffected > 0;
        }


    }
}
