using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class EmployeeMasterModel : TimeStamp
    {
        public string Branch { get; set; }
        public string? EntryDate { get; set; }
       
        public IList<EmployeeMasterModel>? EmployeeMasterList { get; set; }

        public int EmpId { get; set; }
        public string? EmpCode { get; set; }
        public string Name { get; set; }
        public string? Designation { get; set; }
        public IList<TextValue>? DesignationList { get; set; }

        public string? Department { get; set; }
        public IList<TextValue>? DepartmentList { get; set; }
        public string? Category { get; set; }
        public IList<TextValue>? CategoryList { get; set; }
        public bool IsStaff { get; set; }
        public string? DateOfJoining { get; set; }
        public string? DOB { get; set; }
        public string? Shift { get; set; }
        public IList<TextValue>? ShiftList { get; set; }
        public string? Active { get; set; }
        public string? DateOfResignation { get; set; }
        public string NatureOfDuties { get; set; }
        public int ProbationPeriod { get; set; }
        public string? Reference { get; set; }
        public bool IsNewEmployee { get; set; }
        public IList<EmployeeMasterModel>? EmployeeMasterGrid { get; set; }

        //public string AadharCardNumber { get; set; }
        //public string CompanyCardNumber { get; set; }

        //EmpDetail
        public int SrNo { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string MaritalStatus { get; set; }
        public string BloodGroup { get; set; }
        //Contact Detail
        public string MobileNo { get; set; }
        public string MobileNo2{ get; set; }
        public string EmailId{ get; set; }
        public string CurrentAddress{ get; set; }
        public string permanentAddress { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyContactRelation { get; set; }
        //Salary Detail
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string PANNo { get; set; }
        public string AdharNo { get; set; }
        public string SwiftCode { get; set; }
        public string PaymentMode { get; set; }
        public string PFNo { get; set; }
        public string ESINo { get; set; }
        public double GrossSalary { get; set; }
        public double BasicSalary { get; set; }
        public string CalculatePfOn { get; set; }
        public string SalaryCalculation { get; set; }
        public double SalaryBasis { get; set; }
        public string PFApplicable { get; set; }
        public double ApplyPFFonmAmt { get; set; }
        public double ApplyESIFonmAmt { get; set; }
        public string OTApplicable { get; set; }
        public string LeaveApplicable { get; set; }
      
        public string ESIApplicable { get; set; }
        public string LateMarkingCalculationApplicable { get; set; }
        public double FixSalaryAmt { get; set; }

        //Allowance/Deduction
        public string SalaryHead { get; set; }
        public string AllowanceMode { get; set; }
        public double Percent { get; set; }
        public double AllowanceAmount { get; set; }
        public string AllowanceType{ get; set; }
        public string PartyPay{ get; set; }

        //job & Work Detail
        public string JoiningDate{ get; set; }
        public string JobDepartment{ get; set; }
        public string JobDesignation{ get; set; }
        public string ReportingMg{ get; set; }
        public string EmployeeType{ get; set; }
        public string WorkLocation{ get; set; }
        public string JObShift{ get; set; }     
        public string EmpGrade{ get; set; }     
        public string JobProbationPeriod{ get; set; }     
        public string ProbationStartDate{ get; set; }     
        public string ProbationEndDate { get; set; }     
        public string DateOfConfirmation{ get; set; }     
        public string JobReference1{ get; set; }     
        public string JobReference2{ get; set; }     
        public string JoiningThrough{ get; set; }     


    }
}
