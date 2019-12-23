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

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(Queries.GetRollPackId, connection);
                    command.Parameters.AddWithValue("RollPackNum", packNum);
                    object idResult = command.ExecuteScalar();

                    if (idResult == null || DBNull.Value.Equals(idResult))
                        return new Result<RollPack> { Message = Messages.PackageNotFound, ResultItem = null };

                    command = new SqlCommand(Queries.GetRollPackByNum, connection);
                    command.Parameters.AddWithValue("RollPackNum", packNum);

                    using (DbDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            rollPack.Rolls.Add(new Roll
                            {
                                RollNumber = (string)reader["RollNum"],
                                Quality = (QualityStatus)reader["QualityStatus"]
                            });
                        }

                        reader.Close();
                    }

                    command = new SqlCommand(Queries.GetPackSapStatus, connection);
                    command.Parameters.AddWithValue("RollPackNum", packNum);
                }

                if (rollPack.Rolls.Count(x => x.Quality == QualityStatus.Bad) > 0)
                    rollPack.QualityStatus = QualityStatus.Bad;
                else
                {
                    if (rollPack.Rolls.Count(x => x.Quality == QualityStatus.None) > 0)
                        rollPack.QualityStatus = QualityStatus.None;
                    else
                    {
                        rollPack.QualityStatus = QualityStatus.Good;
                    }
                }


                return new Result<RollPack> { Message = Messages.OK, ResultItem = rollPack };
            }
            catch (SqlException)
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

        public string GetNextRollPackNum()
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
                        return Messages.NoPackagesInQueue;

                    return (string)result;
                }
            }
            catch (SqlException)
            {
                return Messages.DatabaseError;
            }
        }

        public string DeleteRollPack(string packNum)
        {
            if (string.IsNullOrEmpty(packNum)) return Messages.ParameterIsEmpty;

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(Queries.DeleteRollPackInPackQueue, connection);
                command.Parameters.AddWithValue("RollPackNum", packNum);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    connection.Close();

                    if (result == null || DBNull.Value.Equals(result))
                        return string.Format(Messages.RowsAffected, 0);

                    return string.Format(Messages.RowsAffected, (int)result);
                }
                catch (SqlException ex)
                {
                    return Messages.DatabaseError;
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

                    if (result == null || DBNull.Value.Equals(result))
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
