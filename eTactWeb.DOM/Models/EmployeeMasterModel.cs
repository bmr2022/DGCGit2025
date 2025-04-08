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
        public string? CategoryId { get; set; }
        public IList<TextValue>? CategoryList { get; set; }
        public bool IsStaff { get; set; }
        public string? DateOfJoining { get; set; }
        public string? DOB { get; set; }
        public string? Shift { get; set; }
        public IList<TextValue>? ShiftList { get; set; }
       
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
        public decimal GrossSalary { get; set; }
        public decimal BasicSalary { get; set; }
        public string CalculatePfOn { get; set; }
        public string SalaryCalculation { get; set; }
        public decimal SalaryBasis { get; set; }
        public string PFApplicable { get; set; }
        public decimal ApplyPFFonmAmt { get; set; }
        public decimal ApplyESIFonmAmt { get; set; }
        public string OTApplicable { get; set; }
        public string LeaveApplicable { get; set; }
      
        public string ESIApplicable { get; set; }
        public string LateMarkingCalculationApplicable { get; set; }
        public decimal FixSalaryAmt { get; set; }

        //Allowance/Deduction
        public string SalaryHead { get; set; }
        public int SalaryHeadId { get; set; }
        public string AllowanceMode { get; set; }
        public decimal Percent { get; set; }
        public decimal AllowanceAmount { get; set; }
        public string AllowanceType{ get; set; }
        public string PartyPay{ get; set; }

        //job & Work Detail
        public string JoiningDate{ get; set; }
        public string JobDepartment{ get; set; }
        public int JobDepartmentId{ get; set; }
        public string JobDesignation{ get; set; }
        public int JobDesignationId{ get; set; }
        public string ReportingMg{ get; set; }
        public string EmployeeType{ get; set; }
        public string WorkLocation{ get; set; }
        public string JObShift{ get; set; }     
        public int JObShiftId{ get; set; }     
        public string EmpGrade{ get; set; }     
        public decimal JobProbationPeriod{ get; set; }     
        public string ProbationStartDate{ get; set; }     
        public string ProbationEndDate { get; set; }     
        public string DateOfConfirmation{ get; set; }     
        public string JobReference1{ get; set; }     
        public string JobReference2{ get; set; }     
        public string JoiningThrough{ get; set; }     

        //Document & IDProof
        public string PassportNo{ get; set; }     
        public string WorkPeritVisa{ get; set; }     
        public string DrivingLicenseNo { get; set; }    
        public string PanNo{ get; set; }   
        public string MedicalInsuranceDetail { get; set; }    

        //Upload Section
        public string fileUpload { get; set; }    
        public string ThumbUnPress { get; set; }    
        public string Aadhar { get; set; }   

        //Educational Qualification
        public string Qualification { get; set; }   
        public string Univercity_Sch { get; set; }   
        public string Per { get; set; }   
        public int InYear { get; set; }   
        public string Remark { get; set; }   

        //Experiance Detail
        public string CompanyName { get; set; }   
        public string CFromDate { get; set; }   
        public string CToDate { get; set; }   
        public string Desigation { get; set; }   
        public decimal Salary { get; set; }   

        //Exit Deatil
        public int NoticPeriod { get; set; }   
        public string  GratutyEligibility { get; set; }   
        public string  ResignationDate { get; set; }   
        public string  Active { get; set; }   


    }
}
