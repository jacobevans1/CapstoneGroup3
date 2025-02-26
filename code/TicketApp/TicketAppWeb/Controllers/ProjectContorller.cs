using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Numerics;

namespace TicketAppWeb.Controllers;

/// <summary>
/// The Controller responsible for displaying and managing projects
/// Jabesi Abwe 
/// 02/20/2025
/// </summary>
public class ProjectController : Controller
{
    private readonly TicketAppContext _context;

    public ProjectController(TicketAppContext context)
    {
        _context = context;
    }

    // GET: Projects
    public async Task<IActionResult> Index(QueryOptions<Project>
    options)
    {


        try {

            if (options.PageSize == 0) options.PageSize = 10;
            if (options.PageNumber == 0) options.PageNumber = 1;

            var projectsQuery = _context.Projects
            .Include(p => p.Lead)
            .Include(p => p.Groups)
            .AsQueryable();

            if (options.HasWhere) {

                projectsQuery = projectsQuery.Where(options.Where);
            }

            if (options.HasOrderBy) {

                projectsQuery = options.OrderByDirection == "asc"
                ? projectsQuery.OrderBy(options.OrderBy)
                : projectsQuery.OrderByDescending
                (options.OrderBy);
            }

            var totalCount = await projectsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount /
            options.PageSize);

            var paginatedProjects = await projectsQuery
            .Skip((options.PageNumber - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync();

            var viewModel = new ProjectViewModel
            {
                Projects = paginatedProjects,
                TotalPages = totalPages,
                //CurrentRoute = options
            };

            return View(viewModel);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "An error occurred while fetching projects.Please try again later.";
            return RedirectToAction("Index");
            }
            } 

    // Method to return group leads based on selected groups
    [HttpGet]
    public JsonResult GetGroupLeads(string groupIds)
    {
        if (string.IsNullOrEmpty(groupIds))
            return Json(new List<object>());

        var groupIdArray = groupIds.Split(",");
        var leads = _context.Groups
                            .Where(g => groupIdArray.Contains(g.Id))
                            .Select(g => new { id = g.ManagerId, fullName = g.Manager!.FullName })
                            .Distinct()
                            .ToList();

        return Json(leads);
    }

    // Add project action
    [HttpPost]
    public IActionResult Add(ProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableGroups = _context.Groups.ToList();
            model.AvailableGroupLeads = _context.Groups.Select(g => g.Manager).Distinct().ToList()!;
            return View("Index", model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var project = new Project
        {
            Id = Guid.NewGuid().ToString(),
            ProjectName = model.Project.ProjectName,
            LeadId = model.ProjectLeadId,
            CreatedById = userId,
            Groups = _context.Groups.Where(g => model.SelectedGroupIds.Contains(g.Id)).ToList(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Projects.Add(project);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    // Edit project action
    [HttpGet]
    public IActionResult Edit(string id)
    {
        var project = _context.Projects
            .Include(p => p.Groups)
            .FirstOrDefault(p => p.Id == id);

        if (project == null)
        {
            return NotFound();
        }

        var viewModel = new
        {
            project = new
            {
                id = project.Id,
                projectName = project.ProjectName,
                leadId = project.LeadId
            },
            availableGroups = _context.Groups.Select(g => new { id = g.Id, groupName = g.GroupName }).ToList(),
            selectedGroupIds = project.Groups.Select(g => g.Id).ToList()
        };

        return Json(viewModel);
    }

    // Edit project POST action

    [HttpPost]
    public IActionResult Edit(ProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableGroups = _context.Groups.ToList();

            model.AvailableGroupLeads = _context.Groups.Select(g => g.Manager).Distinct().ToList()!;

            return View(model);
        }

        var project = _context.Projects.Include(p => p.Groups).FirstOrDefault(p => p.Id == model.Project.Id);

        if (project == null)
        {
            return NotFound();
        }

        project.ProjectName = model.Project.ProjectName;

        project.LeadId = model.ProjectLeadId;

        project.Groups = _context.Groups.Where(g => model.SelectedGroupIds.Contains(g.Id)).ToList();

        _context.Projects.Update(project);

        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    // Delete project action
    [HttpDelete]
    public IActionResult Delete(string id)
    {
        var project = _context.Projects.Include(p => p.Groups).FirstOrDefault(p => p.Id == id);
        if (project == null)
        {
            return NotFound();
        }

        _context.Projects.Remove(project);
        _context.SaveChanges();

        return Ok();
    }
}
