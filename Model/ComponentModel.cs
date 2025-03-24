using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFFSSK.Model
{
    public class ComponentModel
    {


        public ComponentModel() { }

        public ComponentModel(int branchId, string branchCode, string branchName, int stateId, string state, string contactNo, string address, string serviceTaxNo, string storeId, 
            int groupId, string groupCode, string groupName, int componentId, string componentCode, string componentName, string componentConfig)
        {
            this.BranchId = branchId;
            this.BranchCode = branchCode;
            this.BranchName = branchName;
            this.StateId = stateId;
            this.State = state;
            this.ContactNo = contactNo;
            this.Address = address;
            this.ServiceTaxNo = serviceTaxNo;
            this.StoreId = storeId;
            this.GroupId = groupId;
            this.GroupCode = groupCode;
            this.GroupName = groupName;
            this.ComponentId = componentId;
            this.ComponentCode = componentCode;
            this.ComponentName = componentName;
            this.ComponentConfig = componentConfig;
            this.NewGroupId = 0;
        }

        public int BranchId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public int StateId { get; set; }
        public string State { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public string ServiceTaxNo { get; set; }
        public string StoreId { get; set; }

        public int GroupId { get; set; }
        public int NewGroupId { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public int ComponentId { get; set; }
        public string ComponentCode { get; set; }
        public string ComponentName { get; set; }
        public string ComponentConfig { get; set; }
        

    }
}
