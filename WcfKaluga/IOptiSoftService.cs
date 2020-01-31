using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfKaluga.Classes;
using WcfKaluga.Classes.Items;
using WcfKaluga.Classes.Results;

namespace WcfKaluga
{
    [ServiceContract]
    public interface IOptisoftService
    {
        [OperationContract]
        Result<RollPack> GetPackRolls(string packNum);

        [OperationContract]
        Result<RollList> LabQualityParam(List<string> rolls);

        [OperationContract]
        Result<RollPack> UpdateRollPackStatus(string packNum, int sapStatus);

        [OperationContract]
        Result<string> GetNextRollPackNum();

        [OperationContract]
        Result<string> DeleteRollPack(string packNum);

        [OperationContract]
        Result<string> GetRollPackNumByRollNum(string rollNum);
    }
}
