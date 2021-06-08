namespace SystemProcedure.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using SystemEntities.GeneralModels;
    using SystemEntities.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<SystemProcedure.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SystemProcedure.ApplicationDbContext context)
        {
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var userStore = new UserStore<ApplicationUser>(context);
            var UserManager = new UserManager<ApplicationUser>(userStore);





            Branch davao = new Branch() { Address = "Davao City", AssetTag = "DVO", Name = "Davao Branch" };
            Branch HO = new Branch() { Address = "Mindanao Ave.", AssetTag = "", Name = "Head Office" };
            Branch SURIGAO = new Branch() { Address = "Surigao City", AssetTag = "SUR", Name = "Surigao Branch" };
            Branch Cebu = new Branch() { Address = "Cebu City", AssetTag = "CEB", Name = "Cebu Branch" };
            Branch CDO = new Branch() { Address = "Cagayan De Oro City", AssetTag = "CGY", Name = "Cagayan Branch" };

            Branch Benguet = new Branch() { Address = "Benguet City", AssetTag = "BGUET", Name = "Benguet Branch" };
            Branch Palawan = new Branch() { Address = "Palawan City", AssetTag = "PAL", Name = "Palawan Branch" };
            Branch IsaBela = new Branch() { Address = "Isabela City", AssetTag = "ISA", Name = "Isabela Branch" };
            context.Branches.Add(davao);
            context.Branches.Add(HO);
            context.Branches.Add(SURIGAO);
            context.Branches.Add(Cebu);
            context.Branches.Add(CDO);
            context.Branches.Add(Benguet);
            context.Branches.Add(Palawan);
            context.Branches.Add(IsaBela);

            context.SaveChanges();

            LookUpList list1 = new LookUpList() { Description = "M", GroupOf = "AssetItemTypes", Name = "Monitor" };
            LookUpList list35 = new LookUpList() { Description = "D", GroupOf = "AssetItemTypes", Name = "Desktop" };
            LookUpList list2 = new LookUpList() { Description = "CAS", GroupOf = "AssetItemTypes", Name = "Casing" };
            LookUpList list3 = new LookUpList() { Description = "PRO", GroupOf = "AssetItemTypes", Name = "Processor" };
            LookUpList list4 = new LookUpList() { Description = "RAM", GroupOf = "AssetItemTypes", Name = "Memory-Desktop" };
            LookUpList list50 = new LookUpList() { Description = "LAPRAM", GroupOf = "AssetItemTypes", Name = "Memory-Laptop" };
            LookUpList list51 = new LookUpList() { Description = "FW", GroupOf = "AssetItemTypes", Name = "Firewall Appliance" };
            LookUpList list5 = new LookUpList() { Description = "HDD", GroupOf = "AssetItemTypes", Name = "Hard Disk Drive" };
            LookUpList list6 = new LookUpList() { Description = "MB", GroupOf = "AssetItemTypes", Name = "Motherboard" };
            LookUpList list7 = new LookUpList() { Description = "PS", GroupOf = "AssetItemTypes", Name = "Power Supply" };
            LookUpList list8 = new LookUpList() { Description = "ODD", GroupOf = "AssetItemTypes", Name = "Optical Disk Drive" };
            LookUpList list9 = new LookUpList() { Description = "OTH", GroupOf = "AssetItemTypes", Name = "VGA Cable" };
            LookUpList list10 = new LookUpList() { Description = "OTH", GroupOf = "AssetItemTypes", Name = "Power Cable" };
            LookUpList list11 = new LookUpList() { Description = "UPSBATT", GroupOf = "AssetItemTypes", Name = "UPS Battery" };
            LookUpList list12 = new LookUpList() { Description = "FDD", GroupOf = "AssetItemTypes", Name = "Floppy Disk Drive" };
            LookUpList list13 = new LookUpList() { Description = "MSE", GroupOf = "AssetItemTypes", Name = "Mouse" };
            LookUpList list14 = new LookUpList() { Description = "KYB", GroupOf = "AssetItemTypes", Name = "Keyboard" };
            LookUpList list15 = new LookUpList() { Description = "VGC", GroupOf = "AssetItemTypes", Name = "Graphics Card" };
            LookUpList list16 = new LookUpList() { Description = "UPS", GroupOf = "AssetItemTypes", Name = "UPS" };
            LookUpList list17 = new LookUpList() { Description = "LCO", GroupOf = "AssetItemTypes", Name = "Laptop Unit" };
            LookUpList list18 = new LookUpList() { Description = "LAPADPT", GroupOf = "AssetItemTypes", Name = "Laptop Adapter" };
            LookUpList list19 = new LookUpList() { Description = "OS", GroupOf = "AssetItemTypes", Name = "Operating System" };
            LookUpList list20 = new LookUpList() { Description = "OFC", GroupOf = "AssetItemTypes", Name = "Office App System" };
            LookUpList list21 = new LookUpList() { Description = "EHDD", GroupOf = "AssetItemTypes", Name = "External Hard Drive" };
            LookUpList list22 = new LookUpList() { Description = "UFD", GroupOf = "AssetItemTypes", Name = "USB Flash Drive" };
            LookUpList list23 = new LookUpList() { Description = "EODD", GroupOf = "AssetItemTypes", Name = "External Optical Disk Drive" };
            LookUpList list24 = new LookUpList() { Description = "MFP", GroupOf = "AssetItemTypes", Name = "Multi-Function Printer & Scanner" };
            LookUpList list25 = new LookUpList() { Description = "P", GroupOf = "AssetItemTypes", Name = "Printer" };
            LookUpList list26 = new LookUpList() { Description = "SCR", GroupOf = "AssetItemTypes", Name = "Scanner" };
            LookUpList list27 = new LookUpList() { Description = "OTH", GroupOf = "AssetItemTypes", Name = "USB to Printer" };
            LookUpList list28 = new LookUpList() { Description = "OTH", GroupOf = "AssetItemTypes", Name = "USB to Scanner" };
            LookUpList list29 = new LookUpList() { Description = "ROUTR", GroupOf = "AssetItemTypes", Name = "Router" };
            LookUpList list30 = new LookUpList() { Description = "SWTCH", GroupOf = "AssetItemTypes", Name = "Switch" };
            LookUpList list31 = new LookUpList() { Description = "LPT", GroupOf = "AssetItemTypes", Name = "LPT Cable" };
            LookUpList list32 = new LookUpList() { Description = "KVM", GroupOf = "AssetItemTypes", Name = "KVM Switch" };
            LookUpList list33 = new LookUpList() { Description = "LAPBAT", GroupOf = "AssetItemTypes", Name = "Laptop Battery" };
            LookUpList list34 = new LookUpList() { Description = "D", GroupOf = "AssetItemTypes", Name = "Desktop" };

            LookUpList list37 = new LookUpList() { Description = "Dept", GroupOf = "ConsignmentType", Name = "Department" };
            LookUpList list36 = new LookUpList() { Description = "User", GroupOf = "ConsignmentType", Name = "User" };

            context.LookUpList.Add(list37);
            context.LookUpList.Add(list36);


            context.LookUpList.Add(list50);
            context.LookUpList.Add(list51);
            context.LookUpList.Add(list1);
            context.LookUpList.Add(list35);
            context.LookUpList.Add(list2);
            context.LookUpList.Add(list3);
            context.LookUpList.Add(list4);
            context.LookUpList.Add(list5);
            context.LookUpList.Add(list6);
            context.LookUpList.Add(list7);
            context.LookUpList.Add(list8);
            context.LookUpList.Add(list9);
            context.LookUpList.Add(list10);
            context.LookUpList.Add(list11);
            context.LookUpList.Add(list12);
            context.LookUpList.Add(list13);
            context.LookUpList.Add(list14);
            context.LookUpList.Add(list15);
            context.LookUpList.Add(list16);
            context.LookUpList.Add(list17);
            context.LookUpList.Add(list18);
            context.LookUpList.Add(list19);
            context.LookUpList.Add(list20);
            context.LookUpList.Add(list21);
            context.LookUpList.Add(list22);
            context.LookUpList.Add(list23);
            context.LookUpList.Add(list24);
            context.LookUpList.Add(list25);
            context.LookUpList.Add(list26);
            context.LookUpList.Add(list27);
            context.LookUpList.Add(list28);
            context.LookUpList.Add(list29);
            context.LookUpList.Add(list30);
            context.LookUpList.Add(list31);
            context.LookUpList.Add(list32);
            context.LookUpList.Add(list33);

            context.SaveChanges();



            LookUpList AssetStatus1 = new LookUpList() { Description = "Available", GroupOf = "AssetStatus", Name = "Available" };
            LookUpList AssetStatus2 = new LookUpList() { Description = "Defective", GroupOf = "AssetStatus", Name = "Defective" };
            LookUpList AssetStatus3 = new LookUpList() { Description = "OnUsed", GroupOf = "AssetStatus", Name = "OnUsed" };
            context.LookUpList.Add(AssetStatus1);
            context.LookUpList.Add(AssetStatus2);
            context.LookUpList.Add(AssetStatus3);



            context.SaveChanges();





            LookUpList vendor1 = new LookUpList() { Description = "Epson", GroupOf = "Vendor", Name = "Epson" };
            LookUpList vendor2 = new LookUpList() { Description = "Brother", GroupOf = "Vendor", Name = "Brother" };
            LookUpList vendor3 = new LookUpList() { Description = "HP", GroupOf = "Vendor", Name = "HP" };
            LookUpList vendor4 = new LookUpList() { Description = "Dell", GroupOf = "Vendor", Name = "Dell" };
            LookUpList vendor5 = new LookUpList() { Description = "Lenovo", GroupOf = "Vendor", Name = "Lenovo" };
            LookUpList vendor6 = new LookUpList() { Description = "Acer", GroupOf = "Vendor", Name = "Acer" };
            LookUpList vendor7 = new LookUpList() { Description = "Asus", GroupOf = "Vendor", Name = "Asus" };
            LookUpList vendor8 = new LookUpList() { Description = "Seagate", GroupOf = "Vendor", Name = "Seagate" };
            LookUpList vendor9 = new LookUpList() { Description = "Western Digital", GroupOf = "Vendor", Name = "Western Digital" };
            LookUpList vendor10 = new LookUpList() { Description = "Kingston", GroupOf = "Vendor", Name = "Kingston" };



            context.LookUpList.Add(vendor1);
            context.LookUpList.Add(vendor2);
            context.LookUpList.Add(vendor3);
            context.LookUpList.Add(vendor4);
            context.LookUpList.Add(vendor5);
            context.LookUpList.Add(vendor6);
            context.LookUpList.Add(vendor7);
            context.LookUpList.Add(vendor8);
            context.LookUpList.Add(vendor9);
            context.LookUpList.Add(vendor10);
            context.SaveChanges();


            LookUpList Dept1 = new LookUpList() { Description = "10", GroupOf = "DepartmentList", Name = "Parts Department", IsDeleted = false };
            //LookUpList Dept1 = new LookUpList() { Description = "10443", GroupOf = "DepartmentList", Name = "Transfered", IsDeleted = false };
            LookUpList Dept2 = new LookUpList() { Description = "20", GroupOf = "DepartmentList", Name = "Cebu Admin Department", IsDeleted = false };
            LookUpList Dept3 = new LookUpList() { Description = "21", GroupOf = "DepartmentList", Name = "Cebu Parts Department", IsDeleted = false };
            LookUpList Dept4 = new LookUpList() { Description = "25", GroupOf = "DepartmentList", Name = "Cebu Machine Department", IsDeleted = false };
            LookUpList Dept5 = new LookUpList() { Description = "27", GroupOf = "DepartmentList", Name = "Cebu Service Department", IsDeleted = false };
            LookUpList Dept6 = new LookUpList() { Description = "30", GroupOf = "DepartmentList", Name = "Davao Admin Department", IsDeleted = false };
            LookUpList Dept7 = new LookUpList() { Description = "31", GroupOf = "DepartmentList", Name = "Davao Parts Department", IsDeleted = false };
            LookUpList Dept8 = new LookUpList() { Description = "35", GroupOf = "DepartmentList", Name = "Davao Machine Department", IsDeleted = false };
            LookUpList Dept9 = new LookUpList() { Description = "37", GroupOf = "DepartmentList", Name = "Davao Service Department", IsDeleted = false };
            LookUpList Dept10 = new LookUpList() { Description = "40", GroupOf = "DepartmentList", Name = "Rental Department", IsDeleted = false };
            LookUpList Dept11 = new LookUpList() { Description = "41", GroupOf = "DepartmentList", Name = "Rental Operators Department", IsDeleted = false };
            LookUpList Dept12 = new LookUpList() { Description = "50", GroupOf = "DepartmentList", Name = "Machine Sales(VOLVO) Department", IsDeleted = false };
            LookUpList Dept13 = new LookUpList() { Description = "60", GroupOf = "DepartmentList", Name = "Machine Sales(Allied) Department", IsDeleted = false };
            LookUpList Dept14 = new LookUpList() { Description = "70", GroupOf = "DepartmentList", Name = "Surigao Admin Department", IsDeleted = false };
            LookUpList Dept15 = new LookUpList() { Description = "71", GroupOf = "DepartmentList", Name = "Surigao Parts Department", IsDeleted = false };
            LookUpList Dept16 = new LookUpList() { Description = "75", GroupOf = "DepartmentList", Name = "Surigao Machines Department", IsDeleted = false };
            LookUpList Dept17 = new LookUpList() { Description = "77", GroupOf = "DepartmentList", Name = "Surigao Service Department", IsDeleted = false };
            LookUpList Dept18 = new LookUpList() { Description = "80", GroupOf = "DepartmentList", Name = "Finance Department", IsDeleted = false };
            //LookUpList Dept19 = new LookUpList() { Description = "80", GroupOf = "DepartmentList", Name = "Finance Department", IsDeleted = false };
            LookUpList Dept20 = new LookUpList() { Description = "81", GroupOf = "DepartmentList", Name = "Admin Department", IsDeleted = false };
            LookUpList Dept21 = new LookUpList() { Description = "82", GroupOf = "DepartmentList", Name = "Material Department", IsDeleted = false };
            LookUpList Dept22 = new LookUpList() { Description = "83", GroupOf = "DepartmentList", Name = "OTP Department", IsDeleted = false };
            LookUpList Dept23 = new LookUpList() { Description = "84", GroupOf = "DepartmentList", Name = "Marketing Department", IsDeleted = false };
            LookUpList Dept24 = new LookUpList() { Description = "86", GroupOf = "DepartmentList", Name = "Computer System Department", IsDeleted = false };
            LookUpList Dept25 = new LookUpList() { Description = "87", GroupOf = "DepartmentList", Name = "Service Department", IsDeleted = false };
            LookUpList Dept26 = new LookUpList() { Description = "89", GroupOf = "DepartmentList", Name = "MAchine Sales Trucks Department", IsDeleted = false };
            LookUpList Dept27 = new LookUpList() { Description = "90", GroupOf = "DepartmentList", Name = "CDO Admin Department", IsDeleted = false };
            LookUpList Dept28 = new LookUpList() { Description = "91", GroupOf = "DepartmentList", Name = "CDO Parts Department", IsDeleted = false };
            LookUpList Dept29 = new LookUpList() { Description = "95", GroupOf = "DepartmentList", Name = "CDO Machines Department", IsDeleted = false };
            LookUpList Dept30 = new LookUpList() { Description = "97", GroupOf = "DepartmentList", Name = "CDO Service Department", IsDeleted = false };
            LookUpList Dept31 = new LookUpList() { Description = "IS", GroupOf = "DepartmentList", Name = "Isabela Department", IsDeleted = false };
            context.LookUpList.Add(Dept1);
            context.LookUpList.Add(Dept2);
            context.LookUpList.Add(Dept3);
            context.LookUpList.Add(Dept4);
            context.LookUpList.Add(Dept5);
            context.LookUpList.Add(Dept6);
            context.LookUpList.Add(Dept7);
            context.LookUpList.Add(Dept8);
            context.LookUpList.Add(Dept9);
            context.LookUpList.Add(Dept10);
            context.LookUpList.Add(Dept11);
            context.LookUpList.Add(Dept12);
            context.LookUpList.Add(Dept13);
            context.LookUpList.Add(Dept14);
            context.LookUpList.Add(Dept15);
            context.LookUpList.Add(Dept16);
            context.LookUpList.Add(Dept17);
            context.LookUpList.Add(Dept18);

            //context.LookUpList.Add(Dept19);
            context.LookUpList.Add(Dept20);
            context.LookUpList.Add(Dept21);
            context.LookUpList.Add(Dept22);
            context.LookUpList.Add(Dept23);
            context.LookUpList.Add(Dept24);
            context.LookUpList.Add(Dept25);
            context.LookUpList.Add(Dept26);
            context.LookUpList.Add(Dept27);
            context.LookUpList.Add(Dept28);
            context.LookUpList.Add(Dept29);
            context.LookUpList.Add(Dept30);
            context.LookUpList.Add(Dept31);


            //context.LookUpList.Add(Dept18);
            //context.LookUpList.Add(Dept18);
            //context.LookUpList.Add(Dept18);
            //context.LookUpList.Add(Dept18);

            //UserProfile myUser = new UserProfile() { CompanyId = 1, CompEmployeeNum = "2282", Department = "CSD", EmailAddress = "nbsa@civicmdsg.com.ph", Firstname = "Neil Bryan", Middlename = "Salarzon", Surname = "Abarabar" };
            //context.UserProfiles.Add(myUser);
            //LookUpList Asslist1 = new LookUpList() { Description = "Dell xx222", GroupOf = "AssetItemDesc", Name = "Dell xx222" };
            //LookUpList Asslist2 = new LookUpList() { Description = "Dell x323", GroupOf = "AssetItemDesc", Name = "Dell x323" };
            //LookUpList Asslist3 = new LookUpList() { Description = "intel x222", GroupOf = "AssetItemDesc", Name = "intel x222" };
            //LookUpList Asslist4 = new LookUpList() { Description = "intel x323", GroupOf = "AssetItemDesc", Name = "intel x323" };
            //LookUpList Asslist5 = new LookUpList() { Description = "amd x323", GroupOf = "AssetItemDesc", Name = "amd x323" };
            //LookUpList Asslist6 = new LookUpList() { Description = "amd x222", GroupOf = "AssetItemDesc", Name = "amd x222" };
            //context.LookUpList.Add(Asslist1);
            //context.LookUpList.Add(Asslist2);
            //context.LookUpList.Add(Asslist3);
            //context.LookUpList.Add(Asslist4);
            //context.LookUpList.Add(Asslist5);
            //context.LookUpList.Add(Asslist6);

            context.SaveChanges();

            Company CIVIC = new Company() { Code = "09", Name = "Civic Merchandising Inc.", Address = "77 Mindanao Ave. Bagong Pagasa", AssetTag = "CVC" };
            Company PQ = new Company() { Code = "01", Name = "PrimeQuest Transport Solutions Inc.", Address = "99 Mindanao Ave. brgy Tandang Sora", AssetTag = "PQS" };
            Company TPS = new Company() { Code = "02", Name = "Top Spot Heavy Equipt", Address = "77 Mindanao Ave. Bagong Pagasa", AssetTag = "TOP" };
            Company CMI = new Company() { Code = "05", Name = "CMI Equiptment", Address = "77 Mindanao Ave. Bagong Pagasa", AssetTag = "CMI" };

            context.Companies.Add(CIVIC);
            context.Companies.Add(PQ);
            context.Companies.Add(TPS);
            context.Companies.Add(CMI);


            context.SaveChanges();
            var role = roleManager.FindByName("Administrator");
            if (role == null)
            {
                role = new IdentityRole("Administrator");
                roleManager.Create(role);
            }

            var role2 = roleManager.FindByName("Common");
            if (role2 == null)
            {
                role2 = new IdentityRole("Common");
                roleManager.Create(role2);
            }



            var AdministratorUser = new ApplicationUser()
            {

                UserName = "Administrator",
                Email = "administrator@civicmdsg.com.ph",
                Firstname = "Neil Bryan",
                Middlename = "Salarzon",
                Surname = "Abarabar"

            };

            var user = UserManager.Create(AdministratorUser, "nbsa1234NBSA!@#$");
            UserManager.AddToRole(AdministratorUser.Id, "Administrator");


            UserCompany userCompay = new UserCompany() { CompanyId = 1, UserId = AdministratorUser.Id, IsDeleted = false };
            UserCompany userCompay1 = new UserCompany() { CompanyId = 2, UserId = AdministratorUser.Id, IsDeleted = false };

            UserCompany userCompay2 = new UserCompany() { CompanyId = 3, UserId = AdministratorUser.Id, IsDeleted = false };
            UserCompany userCompay3 = new UserCompany() { CompanyId = 4, UserId = AdministratorUser.Id, IsDeleted = false };

            context.UserCompanies.Add(userCompay);
            context.UserCompanies.Add(userCompay1);
            context.UserCompanies.Add(userCompay2);
            context.UserCompanies.Add(userCompay3);

            context.SaveChanges();

            Department PartsDepartment = new Department() { DepartmentCode = "10", Description = "Parts Department", CompanyId = 1 };
            context.Departments.Add(PartsDepartment);
            Department CebuAdminDepartment = new Department() { DepartmentCode = "20", Description = "Cebu Admin Department", CompanyId = 1 };
            context.Departments.Add(CebuAdminDepartment);
            Department CebuPartsDepartment = new Department() { DepartmentCode = "21", Description = "Cebu Parts Department", CompanyId = 1 }; context.Departments.Add(CebuPartsDepartment);
            Department CebuMachineDepartment = new Department() { DepartmentCode = "25", Description = "Cebu Machine Department", CompanyId = 1 }; context.Departments.Add(CebuMachineDepartment);
            Department CebuServiceDepartment = new Department() { DepartmentCode = "27", Description = "Cebu Service Department", CompanyId = 1 }; context.Departments.Add(CebuServiceDepartment);
            Department DavaoAdminDepartment = new Department() { DepartmentCode = "30", Description = "Davao Admin Department", CompanyId = 1 }; context.Departments.Add(DavaoAdminDepartment);
            Department DavaoPartsDepartment = new Department() { DepartmentCode = "31", Description = "Davao Parts Department", CompanyId = 1 }; context.Departments.Add(DavaoPartsDepartment);
            Department DavaoMachineDepartment = new Department() { DepartmentCode = "35", Description = "Davao Machine Department", CompanyId = 1 }; context.Departments.Add(DavaoMachineDepartment);
            Department DavaoServiceDepartment = new Department() { DepartmentCode = "37", Description = "Davao Service Department", CompanyId = 1 }; context.Departments.Add(DavaoServiceDepartment);
            Department RentalDepartment = new Department() { DepartmentCode = "40", Description = "Rental Department", CompanyId = 1 }; context.Departments.Add(RentalDepartment);
            Department RentalOperatorsDepartment = new Department() { DepartmentCode = "41", Description = "Rental Operators Department", CompanyId = 1 }; context.Departments.Add(RentalOperatorsDepartment);
            Department MachineVOLVOSalesDepartment = new Department() { DepartmentCode = "50", Description = "Machine VOLVO Sales Department", CompanyId = 1 }; context.Departments.Add(MachineVOLVOSalesDepartment);
            Department MachineAlliedSalesDepartment = new Department() { DepartmentCode = "60", Description = "Machine Allied Sales Department", CompanyId = 1 }; context.Departments.Add(MachineAlliedSalesDepartment);
            Department SurigaoAdminDepartment = new Department() { DepartmentCode = "70", Description = "Surigao Admin Department", CompanyId = 1 }; context.Departments.Add(SurigaoAdminDepartment);
            Department SurigaoPartsDepartment = new Department() { DepartmentCode = "71", Description = "Surigao Parts Department", CompanyId = 1 }; context.Departments.Add(SurigaoPartsDepartment);
            Department SurigaoMachinesDepartment = new Department() { DepartmentCode = "75", Description = "Surigao Machines Department", CompanyId = 1 }; context.Departments.Add(SurigaoMachinesDepartment);
            Department SurigaoServiceDepartment = new Department() { DepartmentCode = "77", Description = "Surigao Service Department", CompanyId = 1 }; context.Departments.Add(SurigaoServiceDepartment);
            Department FinanceDepartment = new Department() { DepartmentCode = "80", Description = "Finance Department", CompanyId = 1 }; context.Departments.Add(FinanceDepartment);
            Department AdminDepartment = new Department() { DepartmentCode = "81", Description = "Admin Department", CompanyId = 1 }; context.Departments.Add(AdminDepartment);
            Department MaterialDepartment = new Department() { DepartmentCode = "82", Description = "Material Department", CompanyId = 1 }; context.Departments.Add(MaterialDepartment);
            Department OTPDepartment = new Department() { DepartmentCode = "83", Description = "OTP Department", CompanyId = 1 }; context.Departments.Add(OTPDepartment);
            Department MarketingDepartment = new Department() { DepartmentCode = "84", Description = "Marketing Department", CompanyId = 1 }; context.Departments.Add(MarketingDepartment);
            Department ComputerSystemDepartment = new Department() { DepartmentCode = "86", Description = "Computer System Department", CompanyId = 1 }; context.Departments.Add(ComputerSystemDepartment);
            Department ServiceDepartment = new Department() { DepartmentCode = "87", Description = "Service Department", CompanyId = 1 }; context.Departments.Add(ServiceDepartment);
            Department MAchineSalesTrucksDepartment = new Department() { DepartmentCode = "89", Description = "MAchine Sales Trucks Department", CompanyId = 1 }; context.Departments.Add(MAchineSalesTrucksDepartment);
            Department CDOAdminDepartment = new Department() { DepartmentCode = "90", Description = "CDO Admin Department", CompanyId = 1 }; context.Departments.Add(CDOAdminDepartment);
            Department CDOPartsDepartment = new Department() { DepartmentCode = "91", Description = "CDO Parts Department", CompanyId = 1 }; context.Departments.Add(CDOPartsDepartment);
            Department CDOMachinesDepartment = new Department() { DepartmentCode = "95", Description = "CDO Machines Department", CompanyId = 1 }; context.Departments.Add(CDOMachinesDepartment);
            Department CDOServiceDepartment = new Department() { DepartmentCode = "97", Description = "CDO Service Department", CompanyId = 1 }; context.Departments.Add(CDOServiceDepartment);
            Department IsabelaDepartment = new Department() { DepartmentCode = "IS", Description = "Isabela Department", CompanyId = 1 }; context.Departments.Add(IsabelaDepartment);


            context.SaveChanges();

        }
    }
}
