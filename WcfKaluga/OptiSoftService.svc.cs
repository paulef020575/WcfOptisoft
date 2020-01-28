using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Configuration;
using System.Web.WebSockets;
using WcfKaluga.Classes;
using WcfKaluga.Classes.Items;
using WcfKaluga.Classes.Results;
using WcfKaluga.Resources;

namespace WcfKaluga
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "OptisoftService" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы OptisoftService.svc или OptisoftService.svc.cs в обозревателе решений и начните отладку.
    public class OptisoftService : IOptisoftService
    {
        public Result<RollPack> GetPackRolls(string packNum)
        {
            if (string.IsNullOrEmpty(packNum))
                return new Result<RollPack> { Message = Messages.ParameterIsEmpty, ResultItem = null};

            RollPack rollPack = new RollPack { RollPackNum = packNum, Rolls = new List<Roll>() };
            long rollPackId = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(Queries.GetRollPackId, connection);
                    command.Parameters.AddWithValue("RollPackNum", packNum);

                    using (DbDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                    {
                        if (reader.Read())
                        {
                            rollPackId = (long) reader["id"];
                            rollPack.MaterialCode = (string) reader["MaterialCode"];
                            rollPack.SapStatus = (int) reader["SapStatus"];
                            rollPack.WeightGross = (DBNull.Value.Equals(reader["WeightGross"]) ? 0m : (decimal)reader["WeightGross"]);
                            rollPack.WeightNet =  (DBNull.Value.Equals(reader["WeightNet"]) ? 0m : (decimal)reader["WeightNet"]);
                            rollPack.Brutto1 = (DBNull.Value.Equals(reader["Brutto1"]) ? 0m : (int)reader["Brutto1"]);
                            rollPack.QualityStatus = ((bool)reader["IsWaste"] ? QualityStatus.Bad : QualityStatus.Good);

                            reader.Close();
                        }
                        else
                        {
                            reader.Close();
                            connection.Close();
                            return new Result<RollPack> { Message = Messages.PackageNotFound, ResultItem = null };
                        }
                    }

                    rollPack.Properties = new List<Property>();
                    SqlCommand command2 = new SqlCommand(Queries.GetRollPackProperties, connection);
                    command2.Parameters.AddWithValue("Id", rollPackId);

                    using (DbDataReader reader = command2.ExecuteReader(CommandBehavior.Default))
                    {
                        while (reader.Read())
                        {
                            rollPack.Properties.Add(new Property
                            {
                                Code = (string) reader["code"],
                                Value = (string) reader["value"]
                            });
                        }

                        reader.Close();
                    }

                    SqlCommand command1 = new SqlCommand(Queries.GetRollPackByNum, connection);
                    command1.Parameters.AddWithValue("RollPackNum", packNum);

                    using (DbDataReader reader = command1.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Roll roll = new Roll
                            {
                                RollNumber = (string) reader["RollNum"],
                                Quality = (QualityStatus) reader["QualityStatus"],
                                WeightGross = (decimal)reader["WeightGross"],
                                WeightNet = (decimal)reader["WeightNet"],
                                Brutto1 = (int)reader["Brutto1"]
                            };

                            rollPack.Rolls.Add(roll);

                            if (roll.Quality == QualityStatus.Bad)
                                rollPack.QualityStatus = QualityStatus.Bad;
                            if (roll.Quality == QualityStatus.None && rollPack.QualityStatus == QualityStatus.Good)
                                rollPack.QualityStatus = QualityStatus.None;
                        }

                        reader.Close();
                    }
                }

                return new Result<RollPack> { Message = Messages.OK, ResultItem = rollPack };
            }
            catch (SqlException exc)
            {
                return new Result<RollPack> { Message = Messages.DatabaseError, ResultItem = null };
            }
        }

        public Result<RollList> LabQualityParam(List<string> rolls)
        {
            if (rolls == null || rolls.Count() == 0)
                return new Result<RollList> {Message = Messages.ParameterIsEmpty, ResultItem = null};
            RollList rollList = new RollList { Items = new List<RollQuality>()};

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    SqlCommand command = new SqlCommand(Queries.GetRollQuality, connection);
                    command.Parameters.AddWithValue("RollNum", "");

                    connection.Open();
                    foreach (string rollNum in rolls)
                    {
                        command.Parameters["RollNum"].Value = rollNum;
                        RollQuality roll = new RollQuality { RollNumber = rollNum, QualityParams = new List<QualityParam>() };
                        using (DbDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                        {
                            while (reader.Read())
                                roll.QualityParams.Add(new QualityParam
                                {
                                    Code = (string)reader["Code"],
                                    Name = (string)reader["Name"],
                                    Value = (decimal?)(DBNull.Value.Equals(reader["Value"]) ? null : reader["Value"])
                                });

                            reader.Close();
                        }

                        rollList.Items.Add(roll);
                    }
                    connection.Close();
                }

                return new Result<RollList> { Message = Messages.OK, ResultItem = rollList };
            }
            catch (SqlException)
            {
                return new Result<RollList> { Message = Messages.DatabaseError, ResultItem = null };
            }
        }

        public Result<RollPack> UpdateRollPackStatus(string packNum, int sapStatus)
        {
            if (string.IsNullOrEmpty(packNum))
                return new Result<RollPack> { Message = Messages.ParameterIsEmpty, ResultItem = null };

            if (sapStatus < 0 || sapStatus > 3)
                return new Result<RollPack> { Message = Messages.InvalidSapStatus, ResultItem = null};

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(Queries.UpdateRollPackStatus, connection);
                    command.Parameters.AddWithValue("RollPackNum", packNum);
                    command.Parameters.AddWithValue("SapStatus", sapStatus);

                    int result = (int)command.ExecuteScalar();
                    connection.Close();

                    if (result == 0)
                        return new Result<RollPack> { Message = "Package not found", ResultItem = null };

                    return new Result<RollPack>
                    {
                        Message = Messages.OK,
                        ResultItem = new RollPack { RollPackNum = packNum, SapStatus = sapStatus }
                    };
                }
            }
            catch (SqlException)
            {
                return new Result<RollPack> {Message = Messages.DatabaseError, ResultItem = null};
            }
        }

        public Result<string> GetNextRollPackNum()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    SqlCommand command = new SqlCommand(Queries.GetNextRollPackNum, connection);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    connection.Close();

                    if (result == null || DBNull.Value.Equals(result))
                        return new Result<string> {Message = Messages.NoPackagesInQueue, ResultItem = ""};

                    return new Result<string> {Message = "", ResultItem = result.ToString()};
                }
            }
            catch (SqlException)
            {
                return new Result<string> {Message = Messages.DatabaseError, ResultItem = ""};
            }
        }

        public Result<string> DeleteRollPack(string packNum)
        {
            if (string.IsNullOrEmpty(packNum)) return new Result<string> {Message = Messages.ParameterIsEmpty, ResultItem = ""};

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(Queries.DeleteRollPackInPackQueue, connection);
                command.Parameters.AddWithValue("RollPackNum", packNum);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    connection.Close();

                    if (result == null || DBNull.Value.Equals(result) || (int)result == 0)
                        return new Result<string> {Message = string.Format(Messages.RowsAffected, 0), ResultItem = ""};

                    return new Result<string> { Message = "", ResultItem = result.ToString() };
                }
                catch (SqlException ex)
                {
                    return new Result<string> {Message = Messages.DatabaseError, ResultItem = ""};
                }
            }
        }

        public Result<RollPack> GetRollPackNumByRollNum(string rollNum)
        {
            if (string.IsNullOrEmpty(rollNum)) return new Result<RollPack> { Message = Messages.ParameterIsEmpty, ResultItem = null};
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    SqlCommand command = new SqlCommand(Queries.GetRollPackNumByRollNum, connection);
                    command.Parameters.AddWithValue("rollNum", rollNum);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    connection.Close();

                    if (result == null || DBNull.Value.Equals(result) || string.IsNullOrEmpty((string)result))
                        return new Result<RollPack> { Message = Messages.PackageNotFound, ResultItem = null };

                    return new Result<RollPack> { Message = Messages.OK, ResultItem = new RollPack { RollPackNum = (string)result } };
                }
            }
            catch (SqlException)
            {
                return new Result<RollPack> { Message = Messages.DatabaseError, ResultItem = null };
            }
        }

        private string GetConnectionString()
        {
            return WebConfigurationManager.ConnectionStrings["KalugaConnection"].ConnectionString;
        }
    }
}
