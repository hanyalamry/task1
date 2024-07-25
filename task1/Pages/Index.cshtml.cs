using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using task1.entities;

namespace task1.Pages;

public class IndexModel : PageModel

{
    private readonly ILogger<IndexModel> _logger;
    private readonly task1.entities.AdventureWorks2019Context _context;

    public IndexModel(ILogger<IndexModel> logger,task1.entities.AdventureWorks2019Context context)
    {
        _logger = logger;
        _context=context;

    }

public List<empdept> filter { get; set; }
public List<tofemp> tofemps { get; set; }
public List<shifts> shift1 { get; set; }
    public async Task OnGetAsync ()
    {
    
  var employee= _context.Employees.Include(b=>b.BusinessEntity).Include(c=>c.EmployeeDepartmentHistories)
  .Select(b=>new{employeeid=b.BusinessEntityId,
  name=b.BusinessEntity.FirstName,gender=b.Gender,hiredate=b.HireDate,
  dept=b.EmployeeDepartmentHistories.Select(b=>new {
    empdeptcurrent=b.Department.Name
  }).FirstOrDefault()}).ToList();
  
  var alldeps=_context.Departments.Include(c=>c.EmployeeDepartmentHistories)
  .Select( b=> new{deptid=b.DepartmentId,deptname=b.Name,total=b.EmployeeDepartmentHistories.GroupBy(b=>b.BusinessEntityId).Select(b=>new{c=b.Key})}).ToList();



var allshift=_context.EmployeeDepartmentHistories.Include(e=>e.Shift).Select(b=>new{shiftid=b.ShiftId,shiftname=b.Shift.Name,stime=b.Shift.StartTime
,etime=b.Shift.EndTime,emp=b.BusinessEntity.EmployeeDepartmentHistories.Select(b=>new{empdeptcurrent=b.Department.Name}).FirstOrDefault(),emname=
b.BusinessEntity.BusinessEntity.BusinessEntity.Person.FirstName
}).ToList();


   filter= await (from emp in _context.Employees
    join person in _context.People
       on emp.BusinessEntityId equals person.BusinessEntityId
        join EmployeeDepartment in _context.EmployeeDepartmentHistories
      on person.BusinessEntityId equals EmployeeDepartment.BusinessEntityId
    join depts in _context.Departments
       on EmployeeDepartment.DepartmentId equals depts.DepartmentId
                    select new empdept
                    {
        id = emp.BusinessEntityId,
        fempname = person.FirstName,
        mempname = person.MiddleName,
        lempname = person.LastName,
        hiredate = emp.HireDate,
        title=emp.JobTitle,
        gender=emp.Gender,
        deptname=depts.Name
                    }).ToListAsync();


tofemps=await(from depts2 in _context.Departments 
join empdepts3 in _context.EmployeeDepartmentHistories on depts2.DepartmentId equals empdepts3.DepartmentId
 group new{depts2,empdepts3}  by new {depts2.Name,depts2.DepartmentId}
    into grp
    select new tofemp
    {
        total=grp.Count(),
        deptname=grp.Key.Name,
        deptid=grp.Key.DepartmentId
       
       
       //total=empdepts.ToArray()
                    }).ToListAsync();
    

    shift1=await(from shift in _context.Shifts 
join empdepts3 in _context.EmployeeDepartmentHistories on shift.ShiftId equals empdepts3.ShiftId
join emp in _context.People on empdepts3.BusinessEntityId equals emp.BusinessEntityId
join emp1 in _context.Employees on emp.BusinessEntityId equals emp1.BusinessEntityId
join depts2 in _context.Departments  on empdepts3.DepartmentId equals depts2.DepartmentId
 group new{shift,empdepts3,emp}  by new {shift.Name,shift.ShiftId,shift.StartTime,shift.EndTime,emp.FirstName
 ,emp1.JobTitle}
    into grp
    select new shifts
    {
       shiftid=grp.Key.ShiftId,
       shiftname=grp.Key.Name,
       statrttime=grp.Key.StartTime,
       endtime=grp.Key.EndTime,
       empname=grp.Key.FirstName,
       emptitle=grp.Key.JobTitle
       
       
                    }).ToListAsync();
    }
}
public class  empdept(){
  
  public int id { get; set; }
    public string fempname { get; set; }
     public string? mempname { get; set; }
      public string? lempname { get; set; }
    public string deptname { get; set; }
    public string? title { get; set; }
public string gender { get; set; }
public DateOnly hiredate { get; set; }
}

public class tofemp
{
public short deptid{get; set;}
public string deptname{set;get;}

public int total {set;get;}
}

public class shifts
{
public short shiftid{get; set;}
public string shiftname{set;get;}

public TimeOnly statrttime {set;get;}

public TimeOnly endtime {set;get;}
public string empname {set;get;}
public string emptitle {set;get;}

}